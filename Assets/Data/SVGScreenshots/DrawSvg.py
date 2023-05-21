import svg
import os

class Sheep:
    def __init__(self, values) -> None:
        self.posX = values[0]
        self.posY = values[1]
        self.dirX = values[2]
        self.dirY = values[3]

def calculateSheep(center, posx, posy):
    x = center[0] + posx
    y = center[1] - posy
    return svg.Ellipse(
                cx=x, cy=y,
                rx=10, ry=13,
                stroke="black",
                fill="transparent",
                stroke_width=1),\
            svg.Ellipse(
                cx=x, cy=y-10,
                rx=5, ry=7,
                stroke="black",
                fill="black",
                stroke_width=1)
                # transform=[svg.Rotate(-50, 75, 75)])

def drawSheep(data, sizeX, sizeY) -> svg.SVG:
    center = (float(sizeX/2), float(sizeY/2))
    return svg.SVG(
        width=sizeX,
        height=sizeY,
        elements=[calculateSheep(center, sheep.posX, sheep.posY) for sheep in data],
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
        separated = [item * 10 for item in separated]
        result.append(Sheep(separated))

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
    output = str(drawSheep(sheep, 1400.0, 800.0))
    saveToFile(screenNumber, output)
    # print(drawSheep(sheep, 1400.0, 800.0))