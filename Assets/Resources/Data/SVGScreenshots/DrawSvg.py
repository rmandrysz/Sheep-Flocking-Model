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
                fill="white",
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

def draw(data, sizeX, sizeY, shouldDrawPredator) -> svg.SVG:
    center = (float(sizeX/2), float(sizeY/2))
    predatorData = data[-1]

    sheep = [calculateSheep(center, sheep.posX, sheep.posY, sheep.angle) for sheep in data[:-1]]

    background = svg.Rect(fill='white', 
                          stroke='transparent',
                          width=sizeX,
                          height=sizeY,
                          x = 150.0,
                          y = 150.0)
    result = [background]

    for s in sheep:
        result.append(s)

    if shouldDrawPredator:
        predator = calculatePredator(center, predatorData.posX, predatorData.posY, predatorData.angle)
        result.append(predator)
    else:
        lastSheep = [calculateSheep(center, predatorData.posX, predatorData.posY, predatorData.angle)]
        result.append(lastSheep)
    return svg.SVG(
        width = sizeX,
        height = sizeY,
        elements = result
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

def readFile(batchNumber, screenNumber):
    here = os.path.dirname(os.path.abspath(__file__))
    dataPath = str.format("{0}\Coordinates\screen_{1}_{2}.txt", here, batchNumber, screenNumber)
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

def saveToFile(batchNumber, screenNumber, input):
    here = os.path.dirname(os.path.abspath(__file__))
    filePath = str.format("{0}\SVG\screen_{1}_{2}.svg", here, batchNumber, screenNumber)
    with open(filePath, 'w') as f:
        f.write(input)

if __name__ == '__main__':
    screenshotsInBatch = 6
    batchesToDraw = 1
    shouldDrawPredator = True
    for batchCounter in range(batchesToDraw):
        for screenCounter in range(screenshotsInBatch):
            sheep = readFile(batchCounter, screenCounter)
            output = str(draw(sheep, 2000.0, 2000.0, shouldDrawPredator))
            saveToFile(batchCounter, screenCounter, output)
            # print(drawSheep(sheep, 1400.0, 800.0))