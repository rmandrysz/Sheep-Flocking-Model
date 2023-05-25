import svg
import os
import math
import numpy as np

class Sheep:
    def __init__(self, posX, posY, angle) -> None:
        self.posX = posX
        self.posY = posY
        self.angle = angle

def calculateSheep(center, posx, posy, angle):
    x = center[0] + posx
    y = center[1] - posy
    return svg.Ellipse(
                cx=x, cy=y,
                rx=5.5, ry=7,
                stroke="black",
                fill="transparent",
                stroke_width=1,
                transform=[svg.Rotate(angle, x, y)]),\
            svg.Ellipse(
                cx=x, cy=y-6,
                rx=3, ry=5,
                stroke="black",
                fill="black",
                stroke_width=1,
                transform=[svg.Rotate(angle, x, y)])

# def calculateAngle(data):
#     result = math.atan2(data.dirY, data.dirX)
#     print(result, np.rad2deg(result))
#     # return np.rad2deg(result) - 90.0
#     return result

def draw(data, sizeX, sizeY) -> svg.SVG:
    center = (float(sizeX/2), float(sizeY/2))
    predatorData = data[-1]
    sheep = [calculateSheep(center, sheep.posX, sheep.posY, sheep.angle) for sheep in data[:-1]]
    predator = calculatePredator(center, predatorData.posX, predatorData.posY, predatorData.angle)
    return svg.SVG(
        width = sizeX,
        height = sizeY,
        elements = [*sheep, predator]
    )

def calculatePredator(center, posx, posy, angle) -> svg.SVG:
    offsetX = 8
    offsetY = 10
    posx = center[0] + posx
    posy = center[1] - posy
    lowerY = posy + offsetY
    higherY = posy - offsetY
    leftX = posx - offsetX
    rightX = posx + offsetX
    return svg.Polygon(
        points = [
            leftX, lowerY,
            rightX, lowerY,
            posx, higherY
        ],
        stroke = 'black',
        stroke_width = 2,
        fill = 'grey',
        transform = [svg.Rotate(angle, posx, posy)]
    )

def readFile(screenNumber):
    here = os.path.dirname(os.path.abspath(__file__))
    dataPath = os.path.join(here, "screen_" + str(screenNumber) + ".txt")

    dataFile = open(dataPath)

    lines = [line for line in dataFile.readlines()]
    result = []

    # Read model data and group the values
    for line in lines:
        line = line.replace(",", ".")
        separated = [float(item) for item in line.split(sep="\t")]
        result.append(Sheep(10*separated[0], 10*separated[1], -separated[2]))

    dataFile.close()
    return result

def saveToFile(screenNumber, input):
    here = os.path.dirname(os.path.abspath(__file__))
    filePath = os.path.join(here, "Screen" + str(screenNumber) + ".svg")
    with open(filePath, 'w') as f:
        f.write(input)

if __name__ == '__main__':
    screenNumber = 6
    sheep = readFile(screenNumber)
    output = str(draw(sheep, 1000.0, 600.0))
    saveToFile(screenNumber, output)
    # print(drawSheep(sheep, 1400.0, 800.0))