import matplotlib.pyplot as plt
import os
import numpy as np
import statistics as stat
from scipy.interpolate import make_interp_spline

dataSet = 33
here = os.path.dirname(os.path.abspath(__file__))
dataPath = os.path.join(here, "Files\Data" + str(dataSet) + ".txt")
referencePath = os.path.join(here, "Reference\Trial3Data.txt")

groupingSize = 1
startingPoint = 19

print(dataPath)
print(referencePath)
dataFile = open(dataPath)
referenceFile = open(referencePath)

lines = [line for line in dataFile.readlines()]
referenceLines = [line for line in referenceFile.readlines()]

grouped = {}
xdata = []
averages = []
referenceX = []
referenceY = []

xdataDivided = []
averagesDivided = []
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
    average = stat.median(grouped[group])
    averages.append(average)


# Plot model data
xdata = [float(x)*groupingSize for x in sortedGrouped.keys()]

averagesMin = min(averages[startingPoint:])
averagesDivided = [x/averagesMin for x in averages[startingPoint:]]
xdataPoint = 38.0
xdataDivided = [x/xdataPoint for x in xdata[startingPoint:]]

referenceMin = min(referenceY)
referenceYdivided = [x/referenceMin for x in referenceY]
referencePoint = 60.0
referenceXdivided = [x/referencePoint for x in referenceX]

plt.subplot(1, 2, 1)
plt.plot(xdata[startingPoint:], averages[startingPoint:], 'c-*', label="Distance from flock center")
plt.plot(referenceX, referenceY, 'g-+', label="reference")
plt.xlabel("Distance from predator")
plt.ylabel("Distance from the flock center")
plt.xlim(max([max(xdata), max(referenceX)]), min([min(xdata), min(referenceX)]))
plt.grid(True)
# plt.title("Change in the number of healthy, sick and recovered agents over time\nwith probability of contraction p={} and recovery p={}".format(spreadChance, recoveryChance))
plt.legend()

plt.subplot(1, 2, 2)
plt.plot(xdataDivided, averagesDivided, 'c-*')
plt.plot(referenceXdivided, referenceYdivided, 'g-+')
plt.xlim(max([max(xdataDivided), max(referenceXdivided)]), min([min(xdataDivided), min(referenceXdivided)]))
plt.grid(True)
# plt.xticks(range(0, max(velocity) + 26, 25))
# plt.yticks(range(0, predatorDistance[0] + 1, 50))
plt.show()

dataFile.close()
referenceFile.close()