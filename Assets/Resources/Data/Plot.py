import matplotlib.pyplot as plt
import os
import numpy as np
import statistics as stat
import scipy as sp
from matplotlib.ticker import MultipleLocator

dataSet = 19
here = os.path.dirname(os.path.abspath(__file__))
dataPath = os.path.join(here, "Files\Data" + str(dataSet) + ".txt")
referencePath = os.path.join(here, "Reference\Trial3Data.txt")

groupingSize = 1
startingPoint = 80

print(dataPath)
print(referencePath)
dataFile = open(dataPath)
referenceFile = open(referencePath)

lines = [line for line in dataFile.readlines()]
referenceLines = [line for line in referenceFile.readlines()]

grouped = {}
xdata = []
averages = []
standardError = []
referenceX = []
referenceY = []

xdataDivided = []
averagesDivided = []
standardErrorDiv = []
referenceXdivided = []
referenceYdivided = []

# Read model data and group the values
for line in lines:
    line = line.replace(",", ".")
    separated = line.split(sep="\t")

    velocity = float(separated[0])
    predatorDistance = float(separated[1])
    centerDistance = float(separated[2])

    key = float(str(predatorDistance).split('.')[0]) // groupingSize
    value = centerDistance

    if startingPoint != 0 and key > startingPoint:
        continue

    if key in grouped:
        grouped[key].append(value)
    else:
        grouped[key] = [value]

sortedGrouped = {key: grouped[key] for key in sorted(grouped.keys(), reverse=True)}

# Read reference data
for line in referenceLines:
    line = line.replace(",", ".")
    separated = line.split(sep="; ")

    referenceX.append(float(separated[0]))
    referenceY.append(float(separated[1]))


# Count Averages
for group in sortedGrouped.keys():
    standardError.append(np.std(grouped[group]) / np.sqrt(np.size(grouped[group])))
    average = stat.mean(grouped[group])
    averages.append(average)

# Plot model data
xdata = [float(x)*groupingSize for x in sortedGrouped.keys()]

averagesMin = min(averages)
averagesDivided = [x/averagesMin for x in averages]
standardErrorDiv = [x/averagesMin for x in standardError]
xdataPoint = 45.0
xdataDivided = [x/xdataPoint for x in xdata]

referenceMin = min(referenceY)
referenceYdivided = [x/referenceMin for x in referenceY]
referencePoint = 60.0
referenceXdivided = [x/referencePoint for x in referenceX]

plt.errorbar(xdata, averages, yerr=standardError, fmt='.', color='black', ecolor='gray', label="Distance from flock center", mfc='none')
plt.xlabel("Odległość Drapieżnika od centroidu stada [Jednostki silnika Unity]")
plt.ylabel("Średnia odległość owiec od centroidu stada [Jednostki silnika Unity]")
plt.xlim(max(xdata) + max(groupingSize, 5), min(xdata) - max(groupingSize, 5))
# plt.xticks(np.arange(max([max(xdata), max(referenceX)]), min([min(xdata), min(referenceX)]), step=groupingSize))
plt.xticks(np.arange(min(xdata) - max(groupingSize, 5), max(xdata) + max(groupingSize, 5), step=max(groupingSize, 5)))
plt.grid(True)
plt.axes().xaxis.set_minor_locator(MultipleLocator(1))
plt.savefig("FigMeanDistanceSim.png", bbox_inches='tight', pad_inches=0)
# plt.title("Change in the number of healthy, sick and recovered agents over time\nwith probability of contraction p={} and recovery p={}".format(spreadChance, recoveryChance))
plt.show()

plt.errorbar(xdataDivided, averagesDivided, yerr=standardErrorDiv, fmt='.', ecolor='gray', label="Dane otrzymane w symulacji", mfc='none', color='black')
plt.plot(referenceXdivided, referenceYdivided, '^', label="Dane eksperymentalne", mfc='none', ms='6', color='seagreen')
plt.xlim(max(xdataDivided) + 0.1, min(xdataDivided) - 0.1)
plt.xticks(np.arange(round(min(xdataDivided) - 0.1, 1), round(max(xdataDivided) + 0.1, 1), step=0.1))
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