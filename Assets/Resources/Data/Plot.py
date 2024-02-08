import matplotlib.pyplot as plt
import os
import numpy as np
import statistics as stat
import scipy as sp
from matplotlib.ticker import MultipleLocator

#region common data utils
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
#endregion

#region reference data processing
def processReferenceData():
    lines = readLinesFromDataFile(getReferenceDataFilePath())

    data = extractDataFromLines(lines)
    xData = normalizeData(data = data.keys(), pointOfReference = 60.0)
    yData = normalizeData(data = data.values(), pointOfReference = min(data.values()))

    return xData, yData
#endregion

#region simulation data processing
def splitSimulationDataIntoGroups(data, startingPoint = 80):
    result = {}

    for key, value in data.items():
        predatorDistance = float(str(key).split('.')[0])

        centerDistance = float(value)

        if startingPoint != 0 and predatorDistance > startingPoint:
            continue

        if predatorDistance in result:
            result[predatorDistance].append(centerDistance)
        else:
            result[predatorDistance] = [centerDistance]

    return result

def sortSimulationData(data):
    return {key: data[key] for key in sorted(data.keys(), reverse=True)}

def calculateMeanGroupValue(data):
    return {key: stat.mean(data[key]) for key in data.keys()}

def processSimulationData(dataFileNumber = 0):
    lines = readLinesFromDataFile(getSimulationDataFilePath(dataFileNumber))
    extracted = extractDataFromLines(lines)
    data = splitSimulationDataIntoGroups(extracted)
    data = sortSimulationData(data)
    meanAgentDistance = calculateMeanGroupValue(data)

    xData = normalizeData(data = meanAgentDistance.keys(), pointOfReference = 45.0)
    yData = normalizeData(data = meanAgentDistance.values(), pointOfReference = min(meanAgentDistance.values()))

    return xData, yData
#endregion

#region plotting formulas
def plotReferenceData(xData, yData):
    plt.plot(xData, yData, '^', label="ExperimentalData", markerfacecolor='none', ms='6', color='seagreen')

def plotSimulationData(xData, yData):
    plt.plot(xData, yData, '.', color='black', label="Simulation data", markerfacecolor='none')

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
#endregion

if __name__ == '__main__':
    simulationX, simulationY = processSimulationData(0)
    referenceX, referenceY = processReferenceData()

    plot(simulationX, simulationY, referenceX, referenceY)