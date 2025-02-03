 ## This document describes the impact particular parameters have on the end result, i.e the final graph.

1. Weights:
   | Parameter name | Default value | Tested value | Impact | Data file number for the test | Plot |
   | -------------- | ------------- | ------------ | ------ | ----------------------------- | ---- |
   | Flockmate avoidance weight | 0.2 | 0.6 | increasing flockmate avoidance weight makes the plot higher | 2 | ![image](Plots/FlockmateAvoidance.png) |
   | Flock centering weight | 0.8 | 0.4 | decreasing flock centering makes the plot lower and stretched it horizontally | 3 | ![image](Plots/FlockCentering.png) |
   | Velocity matching weight | 0.4 | 0.8 | increasing velocity matching makes the plot change in y value steeper | 4 | ![image](Plots/VelocityMatching.png) |
   | Obstacle avoidance weight | 0.2 | | | | |
   | Escape weight | 6 | 8 | increasing escape weight decreases the difference between minimum and maximum mean distance from the flock center - the graph gets 'lower' | 1 | ![image](Plots/EscapeWeight.png) |
   | Adjusted flockmate avoidance weight | 1.45 | | | | |
  
2. Basic agent movement:
   | Parameter name | Default value | Tested value | Impact | Data file number for the test | Plot |
   | -------------- | ------------- | ------------ | ------ | ----------------------------- | ---- |
   | Initial min speed | 0.7 | | | | |
   | Final min speed | 0.7 | | | | |
   | Initial Max speed | 8 | | | | |
   | Final max speed | 16 | | | | | 

3. Obstacle Avoidance
   | Parameter name | Default value | Tested value | Impact | Data file number for the test | Plot |
   | -------------- | ------------- | ------------ | ------ | ----------------------------- | ---- |
   | Obstacle Avoidance Softener | 15 | | | | |

4. Flockmate Avoidance
   | Parameter name | Default value | Tested value | Impact | Data file number for the test | Plot |
   | -------------- | ------------- | ------------ | ------ | ----------------------------- | ---- |
   | Flockmate avoidance radius | 6 | | | | |
   | Flockmate avoidance softener | 0.6 | | | | | 

5. Flocking
   | Parameter name | Default value | Tested value | Impact | Data file number for the test | Plot |
   | -------------- | ------------- | ------------ | ------ | ----------------------------- | ---- |
   | Flockmate detection radius | 40 | | | | |
   | Sight angle | 150 | | | | | 

6. Predator Adaptation
   | Parameter name | Default value | Tested value | Impact | Data file number for the test | Plot |
   | -------------- | ------------- | ------------ | ------ | ----------------------------- | ---- |
   | Flight zone radius | 24 | | | | |