# HVA
## Introduction
**HVA**: Heuristic-based vehicle arrangement for ro-ro ships -- Unity3D simulation code for  vehicle arrangement

Unity3D simulation code to guide multiple agents to collaboratively plan their paths in a 3D grid world, as well as to test/visualize the heuristic policy on handcrafted scenarios.

Please try the demo of our designed HVA model! You can customize the grid size, add/remove obstacle, add agents and assign them goals automatically, and finally run the model and see the results.
## File list
**HVA\Assets\Scripts\Main**
- LoadManager.cs -D star -> Path optimization -> Cubic interpolation -> Velocity plan -> Solve optimization problem 
- Locate.cs -Generating parking spots based on bat search algorithms
- CameraController.cs -Switching camera view
- cameraDisplay.cs -Handling the display of vision
- EnemySpawner.cs -Vehicle Generator
- Grid.cs -Map Editor
- PathNode.cs -Managing Waypoints
- PosData.cs -Managing parking spot information
- OutLine.cs -Calculate the outline
## Requirements
- Unity Editor 2021.3.23f1c1
- MATLAB R2023a
## Authors
- Mingyuan Zhai
- Zhongyuan Jin
- Zelin Yan
- Zhengmin Gu
- Zhenni Li
- Dong Xiao
