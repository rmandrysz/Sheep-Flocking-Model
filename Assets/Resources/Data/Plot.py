import matplotlib.pyplot as plt
import os
import numpy as np
import statistics as stat
import scipy as sp
from matplotlib.ticker import MultipleLocator

def getSimulationDataFilePath(dataFileNumber):
    here = os.path.dirname(os.path.abspath(__file__))

    return os.path.join(here, "Files\Data" + str(dataFileNumber) + ".txt")

def getReferenceDataFilePath():
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

def processReferenceData():
    lines = readLinesFromDataFile(getReferenceDataFilePath())

    data = extractDataFromLines(lines)
    xData = normalizeData(data = data.keys(), pointOfReference = 60.0)
    yData = normalizeData(data = data.values(), pointOfReference = min(data.values()))

    return xData, yData

def processSimulationData(dataFileNumber = 0):
    lines = readLinesFromDataFile(getSimulationDataFilePath(dataFileNumber))
    data = extractDataFromLines(lines)

    xData = normalizeData(data = data.keys(), pointOfReference = 45.0)
    yData = normalizeData(data = data.values(), pointOfReference = min(data.values()))

    return xData, yData

def plotReferenceData(xData, yData):
    plt.plot(xData, yData, '^', label="ExperimentalData", markerfacecolor='none', ms='6', color='seagreen')

def plotSimulationData(xData, yData):
    plt.plot(xData, yData, '.-', color='black', label="Simulation data", markerfacecolor='none')

def plot(simulationX, simulationY, referenceX, referenceY):
    plotSimulationData(simulationX, simulationY)
    plotReferenceData(referenceX, referenceY)

    plt.xlim(max(simulationX) + 0.1, min(simulationX) - 0.1)
    # plt.xticks(np.arange(round(min(simulationX) - 0.1, 1), round(max(simulationX) + 0.1, 1), step=0.1))
    plt.xlabel("Predator distance from the flock center")
    plt.ylabel("Mean agent distance from the flock center")
    plt.legend()
    plt.grid(True)
    # plt.savefig("FigMeanDistanceSim.png", bbox_inches='tight', pad_inches=0)
    plt.show()

if __name__ == '__main__':
    simulationX, simulationY = processSimulationData(1)
    referenceX, referenceY = processReferenceData()

    plot(simulationX, simulationY, referenceX, referenceY)