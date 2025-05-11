import matplotlib.pyplot as plt
import os
import numpy as np
import statistics as stat
import scipy as sp
from matplotlib.ticker import MultipleLocator

def generateSimulationDataFilePath(dataFileNumber):
    here = os.path.dirname(os.path.abspath(__file__))

    return os.path.join(here, "Files\Data" + str(dataFileNumber) + ".txt")

def genrateReferenceDataFilePath():
    here = os.path.dirname(os.path.abspath(__file__))

    return os.path.join(here, "Reference\Trial3Data.txt")

def readLinesFromDataFile(dataFilePath):
    file = open(dataFilePath)

    lines = file.readlines()
    file.close()

    return lines

def extractDataFromLines(lines):
    result = {}

    for line in lines:
        line = line.replace(",", ".")
        separated = line.split(sep="; ")

        predatorDistance = float(separated[0])
        centerDistance = float(separated[1])

        result[predatorDistance] = centerDistance
    
    return result

def normalizeData(data, pointOfReference):
    return [float(x)/pointOfReference for x in data]

def processDataFile(path, xDataPointOfReference):
    lines = readLinesFromDataFile(path)
    data = extractDataFromLines(lines)

    xData = normalizeData(data = data.keys(), pointOfReference = xDataPointOfReference)
    yData = normalizeData(data = data.values(), pointOfReference = min(data.values()))

    return xData, yData

def plotReferenceData(xData, yData):
    plt.plot(xData, yData, '^', label="Experimental data", markerfacecolor='none', ms='6', color='seagreen')

def plotSimulationData(xData, yData, plotColor = 'black', marker = '.', plotLabel='Simulation data'):
    plt.plot(xData, yData, marker, color=plotColor, label=plotLabel, markerfacecolor='none')

def preparePlot(simulationX):
    plt.xlim(max(simulationX) + 0.1, min(simulationX) - 0.1)
    # plt.xticks(np.arange(round(min(simulationX) - 0.1, 1), round(max(simulationX) + 0.1, 1), step=0.1))
    plt.xlabel("Predator distance from the flock center")
    plt.ylabel("Mean agent distance from the flock center")
    plt.legend()
    plt.grid(True)
    # plt.savefig("FigMeanDistanceSim.png", bbox_inches='tight', pad_inches=0)
    plt.show()

def plotNotNormalized(dataFilePath):
    lines = readLinesFromDataFile(dataFilePath)
    data = extractDataFromLines(lines)

    plotSimulationData(data.keys(), data.values())
    plt.show()

if __name__ == '__main__':
    newFileNumber = 12
    # oldFileNumber = newFileNumber - 1
    oldFileNumber = 0
    simulationXold, simulationYold = processDataFile(generateSimulationDataFilePath(oldFileNumber), 40.0)
    referenceX, referenceY = processDataFile(genrateReferenceDataFilePath(), 60.0)

    simulationXnew, simulationYnew = processDataFile(generateSimulationDataFilePath(newFileNumber), 40.0)
    plotSimulationData(simulationXold, simulationYold, plotColor='red', marker='x', plotLabel='Control sample')
    plotSimulationData(simulationXnew, simulationYnew, plotLabel = 'Test sample')
    plotReferenceData(referenceX, referenceY)

    preparePlot(simulationXnew)
    # plotNotNormalized(generateSimulationDataFilePath(newFileNumber))