import turtle

def drawFlower():

    window = turtle.Screen()
    window.bgcolor("pink")
    petals = 36
    rhombus = 45
    size = 60
    doof = turtle.Turtle()
    doof.shape("square")
    doof.color("yellow")
    doof.speed(0)
    doof.setheading(270)

    for i in range (0,petals):
        oblique = True
        for x in range (0,4):
            doof.forward(size)
            if (oblique):
                doof.right(90-rhombus)
                oblique = False
            else:
                doof.right(90+rhombus)
                oblique = True
        doof.right(360/petals)

    doof.forward(size*5)

    window.exitionclick()

    window.exitionclick()

drawFlower()
