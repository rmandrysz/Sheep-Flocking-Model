import matplotlib.pyplot as plt
import os
import numpy as np
import statistics as stat
from scipy.interpolate import make_interp_spline

dataSet = 11
here = os.path.dirname(os.path.abspath(__file__))
path = os.path.join(here, "Data" + str(dataSet) + ".txt")

print(path)
f = open(path)

data = [line for line in f.readlines()]

datapoints = []
grouped = {}
averages = []

# Read model data and group the values
for line in data:
    line = line.replace(",", ".")
    separated = line.split(sep="\t")

    velocity = float(separated[0])
    predatorDistance = float(separated[1])
    centerDistance = float(separated[2])

    datapoints.append(Datapoint(velocity, predatorDistance, centerDistance))
    key = float(str(predatorDistance).split('.')[0])
    value = centerDistance
    if key in grouped:
        grouped[key].append(value)
    else:
        grouped[key] = [value]

sortedGrouped = {key: grouped[key] for key in sorted(grouped.keys(), reverse=True)}


# Count Averages
for group in sortedGrouped.keys():
    average = stat.median(grouped[group])
    averages.append(average)


# Plot model data
xdata = [float(x) for x in sortedGrouped.keys()]

plt.plot(xdata, averages, 'c-', label="Distance from flock center")
plt.xlabel("Distance from predator")
plt.ylabel("Distance from the flock center")
plt.xlim(max(xdata), min(xdata))
# plt.title("Change in the number of healthy, sick and recovered agents over time\nwith probability of contraction p={} and recovery p={}".format(spreadChance, recoveryChance))
plt.legend()
plt.grid(True)
# plt.xticks(range(0, max(velocity) + 26, 25))
# plt.yticks(range(0, predatorDistance[0] + 1, 50))
plt.show()

f.close()
        