#Rails Schema Relationships

There is never the situation to have embedded relationships. Only referenced(which is the user thing)

First thing before we start to build the rails app we need to plan how our resources are gonna be structured

#PLAN

What resources are we gonna have?

##Encounter Methods

**Pokemon API** 

id:integer 

name:string 

order:integer 

names: list name

We then see that is makes sense for the 



##Pokemon Version Group


Schema Relationship between the Pokemon and Version. There are many versions the pokemon can belong to. 

#CREATE THE APP

1) 

T: rails new world -d postgresql

2) 

T: cd pokemon

3) 

T: rm -rf .git

4) 

T: scaffold. Usually I would start with the parent
	
`rails g scaffold World title:string image:text`

5) 

Atom: now I check the migration file that was created when we ran command to see if everything that we wanted has been added and we can edit it if there are any problems!

6) 

T: rails db:create db:migrate

7) 

Atom: now I can check schema but!

8) 

T: Only at this point I run rails s

 




