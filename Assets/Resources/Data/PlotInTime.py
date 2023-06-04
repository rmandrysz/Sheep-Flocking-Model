import matplotlib.pyplot as plt
import os
import numpy as np
import statistics as stat
import scipy as sp
from mpl_toolkits.mplot3d import Axes3D
import matplotlib.pylab as pl
import matplotlib.patheffects as pe

dataSet = 19
here = os.path.dirname(os.path.abspath(__file__))
dataPath = os.path.join(here, "Files\Data" + str(dataSet) + ".txt")
referencePath = os.path.join(here, "Reference\Trial3Data.txt")

def sorting(listElement):
    return listElement[1]

groupingSize = 1
startingPoint = 80
sheepNumber = 46
groupingSize = 4

sheepData = [[] for i in range(sheepNumber)]
print(sheepData)

print(dataPath)
print(referencePath)
dataFile = open(dataPath)
referenceFile = open(referencePath)

lines = [line for line in dataFile.readlines()]
referenceLines = [line for line in referenceFile.readlines()]

sheepId = 0
dataPointCount = 0
# Read model data and group the values
for line in lines:
    line = line.replace(",", ".")
    separated = line.split(sep="\t")

    velocity = float(separated[0])
    predatorDistance = float(separated[1])
    centerDistance = float(separated[2])
    if dataPointCount % groupingSize == 0:
        sheepData[sheepId].append(centerDistance)
    else:
        sheepData[sheepId][dataPointCount//groupingSize] += centerDistance
        
    sheepId += 1

    if sheepId == sheepNumber:
        sheepId = 0
        dataPointCount += 1

    if dataPointCount == len(lines)//sheepNumber:
        break

for sheep in sheepData:
    sheep = [x / groupingSize for x in sheep]

dataPointCount = dataPointCount // groupingSize

sheepData.sort(key=sorting)

print(sheepId)
print(dataPointCount)

pl.figure(figsize=(20, 10))
ax = pl.subplot(projection='3d')

x   = np.linspace(0, dataPointCount, dataPointCount)

i = 0

x_scale=1
y_scale=2
z_scale=1

scale=np.diag([x_scale, y_scale, z_scale, 1.0])
scale=scale*(1.0/scale.max())
scale[3,3]=1.0

def short_proj():
  return np.dot(Axes3D.get_proj(ax), scale)

ax.get_proj=short_proj

for sheep in sheepData:
    color = ((80.0 - (80/sheepNumber * i))/255.0, (150.0 - (130.0/sheepNumber * i))/255.0, (150.0 - (130.0/sheepNumber * i))/255.0)
    z = np.ones(len(sheep)) * i
    ax.plot(x, z, sheep, linewidth=0.3, color=color, path_effects=[pe.Stroke(linewidth=0.4, foreground='black'), pe.Normal()])
    i += 1

# ax.plot(x, y1, z, color='r')
# ax.plot(x, y2, z, color='g')
# ax.plot(x, y3, z, color='b')

# ax.add_collection3d(pl.fill_between(x, 0.95*z, 1.05*z, color='r', alpha=0.3), zs=1, zdir='y')
# ax.add_collection3d(pl.fill_between(x, 0.90*z, 1.10*z, color='g', alpha=0.3), zs=2, zdir='y')
# ax.add_collection3d(pl.fill_between(x, 0.85*z, 1.15*z, color='b', alpha=0.3), zs=3, zdir='y')

ax.set_xlabel('Day')
ax.set_zlabel('Resistance (%)')
# ax.view_init(elev=20., azim=-35)

plt.show()

dataFile.close()
referenceFile.close()