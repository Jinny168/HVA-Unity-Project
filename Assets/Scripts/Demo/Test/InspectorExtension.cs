using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AStar
{
    //通过特性指定按钮在哪个组件下
    [CustomEditor(typeof(AStarGUI))]
    [CanEditMultipleObjects]
    public class InspectorExtension : Editor
    {
        public override void OnInspectorGUI()
        {
            //这个是绘制原生的GUI
            base.OnInspectorGUI();

            GUILayout.Space(10f);

            if (GUILayout.Button("创建地图"))
            {
                AStarGUI drawMap = this.target as AStarGUI;
                drawMap.CreateMapByGivenSize();
                drawMap.astar = AStarManager.Instance;
                drawMap.DrawMap();
                drawMap.MarkBasicMap();
            }

            if (GUILayout.Button("加载地图"))
            {
                AStarGUI drawMap = this.target as AStarGUI;
                drawMap.CreateMapByPraseCSV();
                drawMap.astar = AStarManager.Instance;
                drawMap.DrawMap();
                drawMap.MarkBasicMap();
            }

            if (GUILayout.Button("保存地图"))
            {
                AStarGUI drawMap = this.target as AStarGUI;
                drawMap.SaveMapToCsv();
            }

            if (GUILayout.Button("清空地图"))
            {
                AStarGUI drawMap = this.target as AStarGUI;
               drawMap.ClearChilds();
                drawMap.MarkBasicMap();
            }

            GUILayout.Space(10f);

            if (GUILayout.Button("设置起点、终点"))
            {
                AStarGUI drawMap = this.target as AStarGUI;
                drawMap.setWhich = 0;
            }

            if (GUILayout.Button("设置障碍"))
            {
                AStarGUI drawMap = this.target as AStarGUI;
                drawMap.setWhich = -1;
            }

            if (GUILayout.Button("随机设置障碍"))
            {
                AStarGUI drawMap = this.target as AStarGUI;
                drawMap.SetRandomObstacle();
                drawMap.MarkBasicMap();
            }

            GUILayout.Space(10f);

            if (GUILayout.Button("执行"))
            {
                AStarGUI drawMap = this.target as AStarGUI;
                drawMap.DisplayNode();
            }
        }
    }
}