import matplotlib.pyplot as plt
import os
import numpy as np
import statistics as stat
import scipy as sp
from matplotlib.ticker import MultipleLocator

dataFileNumber = 0
here = os.path.dirname(os.path.abspath(__file__))
dataFilePath = os.path.join(here, "Files\Data" + str(dataFileNumber) + ".txt")
referenceDataFilePath = os.path.join(here, "Reference\Trial3Data.txt")

startingPoint = 80

print(dataFilePath)
print(referenceDataFilePath)
dataFile = open(dataFilePath)
referenceFile = open(referenceDataFilePath)

lines = [line for line in dataFile.readlines()]
referenceLines = [line for line in referenceFile.readlines()]

splitAgentDistance = {}
plotXData = []
averageAgentDistances = []
standardError = []
referenceX = []
referenceY = []

xNormalizedToReference = []
averagesNormalizedToReference = []
standardErrorDiv = []
referenceXNormalized = []
referenceYNormalized = []

# Read model data and group the values
for line in lines:
    line = line.replace(",", ".")
    separated = line.split(sep="\t")

    predatorDistance = float(separated[0])
    centerDistance = float(separated[1])

    key = float(str(predatorDistance).split('.')[0])
    value = centerDistance

    if startingPoint != 0 and key > startingPoint:
        continue

    if key in splitAgentDistance:
        splitAgentDistance[key].append(value)
    else:
        splitAgentDistance[key] = [value]

sortedSplitAgentDistance = {key: splitAgentDistance[key] for key in sorted(splitAgentDistance.keys(), reverse=True)}

# Read reference data
for line in referenceLines:
    line = line.replace(",", ".")
    separated = line.split(sep="; ")

    referenceX.append(float(separated[0]))
    referenceY.append(float(separated[1]))


# Count Averages
for group in sortedSplitAgentDistance.keys():
    standardError.append(np.std(splitAgentDistance[group]) / np.sqrt(np.size(splitAgentDistance[group])))
    averageAgentDistance = stat.mean(splitAgentDistance[group])
    averageAgentDistances.append(averageAgentDistance)

# Plot model data
plotXData = [float(x) for x in sortedSplitAgentDistance.keys()]

averagesMin = min(averageAgentDistances)
averagesNormalizedToReference = [x/averagesMin for x in averageAgentDistances]
xPointOfReference = 45.0
xNormalizedToReference = [x/xPointOfReference for x in plotXData]

referenceMin = min(referenceY)
referenceYNormalized = [x/referenceMin for x in referenceY]
referencePoint = 60.0
referenceXNormalized = [x/referencePoint for x in referenceX]

plt.errorbar(plotXData, averageAgentDistances, fmt='.', color='black', ecolor='gray', label="Distance from flock center", mfc='none')
plt.xlabel("Predator distance to flock center [Jednostki silnika Unity]")
plt.ylabel("Average predator distance from the flock center [Jednostki silnika Unity]")
plt.xlim(max(plotXData) + 5, min(plotXData) - 5)
plt.xticks(np.arange(min(plotXData) - 5, max(plotXData) + 5, step=5))
plt.grid(True)
plt.savefig("FigMeanDistanceSim.png", bbox_inches='tight', pad_inches=0)

plt.plot(referenceXNormalized, referenceYNormalized, '^', label="Dane eksperymentalne", mfc='none', ms='6', color='seagreen')
plt.xlim(max(xNormalizedToReference) + 0.1, min(xNormalizedToReference) - 0.1)
plt.xticks(np.arange(round(min(xNormalizedToReference) - 0.1, 1), round(max(xNormalizedToReference) + 0.1, 1), step=0.1))
plt.xlabel("Odległość Drapieżnika od centroidu stada")
plt.ylabel("Średnia odległość owiec od centroidu stada")
plt.legend()
plt.grid(True)
plt.savefig("FigMeanDistanceSim.png", bbox_inches='tight', pad_inches=0)
# plt.xticks(range(0, max(velocity) + 26, 25))
# plt.yticks(range(0, predatorDistance[0] + 1, 50))
plt.show()

dataFile.close()
referenceFile.close()