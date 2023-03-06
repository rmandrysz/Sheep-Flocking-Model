import matplotlib.pyplot as plt
import os
import numpy as np
import statistics as stat
import scipy as sp
from matplotlib.ticker import MultipleLocator

class Data:
    def __init__(self, filePath) -> None:
        self.dataFile = open(filePath)
        self.lines = [line for line in self.dataFile.readlines()]

        self.grouped = {}
        self.sortedGrouped = {}
        self.xdata = []
        self.averages = []
        self.standardError = []
        self.referenceX = []
        self.referenceY = []

        self.xdataDivided = []
        self.averagesDivided = []
        self.standardErrorDiv = []

    def __del__(self) -> None:
        print('DestructorCalled')
        self.dataFile.close()

    def readAndGroup(self, groupingSize, startingPoint):
        for line in self.lines:
            line = line.replace(",", ".")
            separated = line.split(sep="\t")

            velocity = float(separated[0])
            predatorDistance = float(separated[1])
            centerDistance = float(separated[2])

            key = float(str(predatorDistance).split('.')[0]) // groupingSize
            value = centerDistance

            if startingPoint != 0 and key > startingPoint:
                continue

            if key in self.grouped:
                self.grouped[key].append(value)
            else:
                self.grouped[key] = [value]

        self.sortedGrouped = {key: self.grouped[key] for key in sorted(self.grouped.keys(), reverse=True)}

    def calculateAverages(self):
        for group in self.sortedGrouped.keys():
            self.standardError.append(np.std(self.grouped[group]) / np.sqrt(np.size(self.grouped[group])))
            average = stat.mean(self.grouped[group])
            self.averages.append(average)

    def calculateXData(self):
        self.xdata = [float(x)*groupingSize for x in self.sortedGrouped.keys()]

    def calculateDivided(self, referencePoint):
        averagesMin = min(self.averages)
        self.averagesDivided = [x/averagesMin for x in self.averages]
        self.standardErrorDiv = [x/averagesMin for x in self.standardError]
        self.xdataDivided = [x/referencePoint for x in self.xdata]

    def preparePlotData(self, groupingSize, startingPoint, referencePoint):
        self.readAndGroup(groupingSize, startingPoint)
        self.calculateAverages()
        self.calculateXData()
        self.calculateDivided(referencePoint)




dataSet = "Official"
here = os.path.dirname(os.path.abspath(__file__))
dataPath = os.path.join(here, "Files\Data" + str(dataSet) + ".txt")

basicSet = "BasicModel"
basicPath = os.path.join(here, "Files\Data" + str(basicSet) + ".txt")

groupingSize = 1
startingPoint = 80
referencePoint = 45

print(dataPath)
print(basicPath)

modelData = Data(dataPath)
basicData = Data(basicPath)

modelData.preparePlotData(groupingSize=groupingSize, startingPoint=startingPoint, referencePoint=referencePoint)
basicData.preparePlotData(groupingSize=groupingSize, startingPoint=startingPoint, referencePoint=referencePoint)
# dataFile = open(dataPath)
# basicFile = open(basicPath)

# lines = [line for line in dataFile.readlines()]

# grouped = {}
# xdata = []
# averages = []
# standardError = []
# referenceX = []
# referenceY = []

# xdataDivided = []
# averagesDivided = []
# standardErrorDiv = []

# # Read model data and group the values
# for line in lines:
#     line = line.replace(",", ".")
#     separated = line.split(sep="\t")

#     velocity = float(separated[0])
#     predatorDistance = float(separated[1])
#     centerDistance = float(separated[2])

#     key = float(str(predatorDistance).split('.')[0]) // groupingSize
#     value = centerDistance

#     if startingPoint != 0 and key > startingPoint:
#         continue

#     if key in grouped:
#         grouped[key].append(value)
#     else:
#         grouped[key] = [value]

# sortedGrouped = {key: grouped[key] for key in sorted(grouped.keys(), reverse=True)}

# # Count Averages
# for group in sortedGrouped.keys():
#     standardError.append(np.std(grouped[group]) / np.sqrt(np.size(grouped[group])))
#     average = stat.mean(grouped[group])
#     averages.append(average)

# # Plot model data
# xdata = [float(x)*groupingSize for x in sortedGrouped.keys()]

plt.errorbar(modelData.xdataDivided, modelData.averagesDivided, yerr=modelData.standardErrorDiv, fmt='.', color='black', ecolor='gray', label="Wyniki dostosowanego modelu", mfc='none')
plt.errorbar(basicData.xdataDivided, basicData.averagesDivided, yerr=basicData.standardErrorDiv, fmt='h', color='darkred', ecolor='gray', label="Wyniki podstawowego modelu Boids", mfc='none')
plt.xlabel("Odległość drapieżnika od centrum stada")
plt.ylabel("Średnia odległość owiec od centrum stada")
plt.legend()
plt.xlim(max(modelData.xdataDivided) + 0.1, min(modelData.xdataDivided) - 0.1)
# plt.xticks(np.arange(max([max(xdata), max(referenceX)]), min([min(xdata), min(referenceX)]), step=groupingSize))
plt.xticks(np.arange(round(min(modelData.xdataDivided) - 0.1, 1), round(max(modelData.xdataDivided) + 0.1, 1), step=0.1))
plt.grid(True)
plt.axes().xaxis.set_minor_locator(MultipleLocator(1))
plt.savefig("FigMeanDistanceSim.svg", bbox_inches='tight', pad_inches=0)
# plt.title("Change in the number of healthy, sick and recovered agents over time\nwith probability of contraction p={} and recovery p={}".format(spreadChance, recoveryChance))
plt.show()