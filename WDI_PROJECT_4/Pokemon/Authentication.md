#RAILS AUTHENTICATION

###0) create the database & user schema

- T: `rails db:create`
- T: `rails g scaffold User username first_name last_name email password_digest`
- Atom: check the migration file in case there are any spelling mistakes
- T: `rails db:migrate`


###1) install bcrypt

- A: Gemfile `gem 'bcrypt', '~> 3.1.7'`
- T: `bundle`

###2) add password and email fields to user schema (if user schema already exists)

- T: `rails g migration AddEmailAndPasswordDigestToUsers email password_digest` the password fiels MUST be password_digest (which is the way for the schema to store the encrypted password).
- T: `rails db:migrate`

###3) add `has_secure_password` in user model

- It gives two virtual properties, password and password_confirmation
- It adds a validation on the password field, but does not add a validation for password confirmation field, we would have to do that manually, with validates fields in the model. For example we can make sure that the email is a unique email: `validates :email, uniqueness: true`
- It hashes the password for us

###4) create authentication controller for register and login, NO SCAFFOLD

We need to remove the create method for the user, and instead use the register and login

- `rails g controller authentications register login`. In controllers, I now have:

```ruby

class AuthenticationsController < ApplicationController
  def register
  end

  def login
  end
end

```

- Make sure that they are routing properly, in routes.rb

```ruby
Rails.application.routes.draw do
  scope :api do
    resources :posts
    resources :users
    post "/register", to: "authentications#register"
    post "/login", to: "authentications#login"
  end
end
```

- Now, we need to create some strongparams for user fields check, inside the authentications controller for the instance when we register a user:
	- We create a private: everything under a private is a class scoped method meaning that it cannot be called on an instance

```ruby
class AuthenticationsController < ApplicationController
  def register
  end

  def login
  end

  private
    def user_params
      params.permit(:username, :email, :first_name, :last_name, :password, :password_confirmation)
    end
end

```

####*************** REGISTER

- a) we wanna create a new instance of a user
- b) we wanna save that instance of a user, if we manage to save it,
- c) return the user back from the response
- d) if the user cannot be saved, display the errors

```ruby
def register
    user = User.new(user_params) # a)
    if user.save # b)
      render json: user, status: :ok # c)
    else # d)
      render json: { errors: user.errors.full_messages }, status: :unprocessable_entity
    end
  end
```

- Now we can test in insomnia, by sending a POST request to /api/register

####****************** LOGIN

- a) find a user by the email value in the params
- b) check if there is a user and the password param value matches that of the user found in the database
- c) return the user that has been successfully logged in
- d) if the user is not found and/or the password doesn't match, render errors

```ruby
def login
    user = User.find_by_email(params[:email]) # a)
    if user && user.authenticate(params[:password]) # b)
      render json: user, status: :ok # c)
    else # d)
      render json: { errors: ["Invalid login credentials"]},
      status: 401
    end
  end
  
```

- Now we can test in insomnia, by sending a POST request to /api/login

###5) Create the JWT token

- A: in Gemfile add `gem 'jwt'`
- T: `bundle`
- T: we need to add the fucntionality to encode the token and decode the token. Inside lib directory: `touch lib/auth.rb`
- A: in config/application.rb, I need to inject the lib directory: `config.eager_load_paths << Rails.root.join('lib')`

To become:

```ruby
require_relative 'boot'

require "rails"
# Pick the frameworks you want:
require "active_model/railtie"
require "active_job/railtie"
require "active_record/railtie"
require "action_controller/railtie"
require "action_mailer/railtie"
require "action_view/railtie"
require "action_cable/engine"
# require "sprockets/railtie"
require "rails/test_unit/railtie"

# Require the gems listed in Gemfile, including any gems
# you've limited to :test, :development, or :production.
Bundler.require(*Rails.groups)

module TwitterClone
  class Application < Rails::Application
    # Initialize configuration defaults for originally generated Rails version.
    config.load_defaults 5.1

    # Settings in config/environments/* take precedence over those specified here.
    # Application configuration should go into files in config/initializers
    # -- all .rb files in that directory are automatically loaded.

    # Only loads a smaller set of middleware suitable for API only apps.
    # Middleware like session, flash, cookies can be added back manually.
    # Skip views, helpers and assets when generating a new resource.
    config.api_only = true
    config.eager_load_paths << Rails.root.join('lib')
  end
end

```

- A: in auth.rb I can create the class

```ruby
require 'jwt'

class Auth
  def self.encode

  end

  def self.decode

  end
end

```

- Define the secret, but we define it in the zshrc file on my computer, so it's not publically available.
- T: `atom ~/.zshrc`
- A: at the bottom of the zshrc file, I add this secret:

```
# ENV VARIABLES
export AUTH_SECRET="ofijerofijwxmncuefheuihwo023ij4rlsdk"
```
We can use the same key for all of our similar projects, we don't need to have a different one for each

- T: `source ~/.zshrc` to update the file
- A: in auth.rb I can add a method to require that secret key:

```ruby

def self.auth_secret
    ENV["AUTH_SECRET"]
  end
  
```

- write our encode & decode functions in auth.rb:

- a) algorithm to encrypt the token, it's for JWT to reference as the algorithm of choice when creating the token. Constant in capitals, it makes sure that the var cannot be changed
- b) leeway is the amt of time for how long the request needs to take before terminating the action. If it's set to 0, it's unlimited time
- c) allows to call the values in the decoded hash both by string and symbol syntax; both are used in the usage by gems/rails

```ruby

require 'jwt'

class Auth
  ALGORITHM = 'HS256' # a)

  def self.encode(payload, expiry_in_minutes=60*24*30)
    payload[:expiry] = expiry_in_minutes.minutes.from_now.to_i
    JWT.encode(payload, auth_secret, ALGORITHM)
  end

  def self.decode(token, leeway=0) # b)
    decoded = JWT.decode(token, auth_secret, true, { leeway: leeway, algorithm: ALGORITHM })

    HashWithIndifferentAccess.new(decoded[0]) # c)
  end

  def self.auth_secret
    ENV["AUTH_SECRET"]
  end
end


```

- A: Back in our authentications controller, add token to register and login functions:

- a) id: user.id is my payload data I wanna encode, so here is where I define what I wanna encode inside the payload
- b) so this way I'm ensuring that all relationships I built are gonna be accessible still as the response of login together with the user

```ruby

class AuthenticationsController < ApplicationController
  def register
    user = User.new(user_params)
    if user.save
      token = Auth.encode({ id: user.id })
      render json: { token: token, user: UserSerializer.new(user) }, status: :ok
    else
      render json: { errors: user.errors.full_messages }, status: :unprocessable_entity
    end
  end

  def login
    user = User.find_by_email(params[:email])
    if user && user.authenticate(params[:password])
      token = Auth.encode({ id: user.id }) # a)
      render json: { token: token, user: UserSerializer.new(user) }, status: :ok # b)
    else
      render json: { errors: ["Invalid login credentials"] }, status: 401
    end
  end

  private
    def user_params
      params.permit(:username, :email, :first_name, :last_name, :password, :password_confirmation)
    end
end

```

Now if we test again logging in / registering in insomnia, we can see that there is a token parsed through.

###6) Make sure that we do that encode/decode token in every route I go through

Before we defined the decode and decode methods, now we're actually calling them.

- A: in my application_controller.rb I can write that functionality, because all the other controllers and models inherit from this class and every request to my API to any of my routes, will go through the applications controller first 

####fist I set a private, a series of functions to be run before we run everything else:

- a) check if there is a token being parsed through as part of the header; the ||= is for if there is no token, try and find another one, if there's one already, keep the same value
- b) if you find it, take it, split it, and just take the token bit
- c) if the value of token gives me back a truthy value, then I wanna use that token to decode it otherwise, populate it with the new value
- d) this is gonna return true if there is a token, if the token is decoded and whether the payload contains an id key

```ruby

class ApplicationController < ActionController::API

  private

    def id_found? # d)
      token && decoded_token && decoded_token[:id]
    end

    # ability to decode and encode token
    def decoded_token
      @decoded_token ||= Auth.decode(token) if token # c)
    end

    def token
      @token ||= if request.headers['Authorization'].present? # a)
        request.headers['Authorization'].split.last # b)
      end
    end
end


```

####then we define my main functions

- a) if there user is already logged in, use the data that is already there, otherwise createa brand new value
- b) if something goes wrong at some point, I can return an error, but I want it to be a very specific one, I wanna get nil value --> this is called a rescue
- c) if any type of error occurs, return nil. not the actual error
- d) if someone is logged in, this will return true, else false. I use !! to turn the objec tinto boolean value
- e) this will be the error set as the response when a request is made with an INVALID token
- f) when any request is made to the API, execute authenticate_user method to see if there is a current user or not before anything is run in the controller, which will then chain down all the way to private

```ruby

class ApplicationController < ActionController::API

  before_action :authenticate_user # f)

  def authenticate_user
    # e)
    render json: { errors: ["You must be logged in to view this content"]}, status: 401 unless user_signed_in?
  end

  def user_signed_in?
    !!current_user # d)
  end

  def current_user
    @current_user ||= User.find(decoded_token[:id]) if id_found? # a)
  rescue # b)
    nil # c)
  end
  
  private

    def id_found?
      token && decoded_token && decoded_token[:id]
    end

    # ability to decode and encode token
    def decoded_token
      @decoded_token ||= Auth.decode(token) if token
    end

    def token
      @token ||= if request.headers['Authorization'].present?
        request.headers['Authorization'].split.last
      end
    end
end

```

So now, if I don't set my header in insomnia and I make for example a GET request to /posts, it will give me a you must be logged in to view this content message.


####Avoid blocking the routes register and login, so whitelist these routes 

A: in authentications_controller add a skip_before_action at the top of the function:

- a) don't run the authenticate_user method before requests made to the AuthenticationsController

```ruby
skip_before_action :authenticate_user # a)
```

To become:

```ruby

class AuthenticationsController < ApplicationController
  skip_before_action :authenticate_user

  def register
    user = User.new(user_params)
    if user.save
      token = Auth.encode({ id: user.id }) 
      render json: { token: token, user: UserSerializer.new(user) }, status: :ok
    else
      render json: { errors: user.errors.full_messages }, status: :unprocessable_entity
    end
  end

  def login
    user = User.find_by_email(params[:email])
    if user && user.authenticate(params[:password])
      token = Auth.encode({ id: user.id }) # id: user.is is my payload data I wanna encode
      render json: { token: token, user: UserSerializer.new(user) }, status: :ok
    else
      render json: { errors: ["Invalid login credentials"] }, status: 401
    end
  end

  private
    def user_params
      params.permit(:username, :email, :first_name, :last_name, :password, :password_confirmation)
    end
end


```

