using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AStar
{
    //ͨ������ָ����ť���ĸ������
    [CustomEditor(typeof(AStarGUI))]
    [CanEditMultipleObjects]
    public class InspectorExtension : Editor
    {
        public override void OnInspectorGUI()
        {
            //����ǻ���ԭ����GUI
            base.OnInspectorGUI();

            GUILayout.Space(10f);

            if (GUILayout.Button("������ͼ"))
            {
                AStarGUI drawMap = this.target as AStarGUI;
                drawMap.CreateMapByGivenSize();
                drawMap.astar = AStarManager.Instance;
                drawMap.DrawMap();
                drawMap.MarkBasicMap();
            }

            if (GUILayout.Button("���ص�ͼ"))
            {
                AStarGUI drawMap = this.target as AStarGUI;
                drawMap.CreateMapByPraseCSV();
                drawMap.astar = AStarManager.Instance;
                drawMap.DrawMap();
                drawMap.MarkBasicMap();
            }

            if (GUILayout.Button("�����ͼ"))
            {
                AStarGUI drawMap = this.target as AStarGUI;
                drawMap.SaveMapToCsv();
            }

            if (GUILayout.Button("��յ�ͼ"))
            {
                AStarGUI drawMap = this.target as AStarGUI;
               drawMap.ClearChilds();
                drawMap.MarkBasicMap();
            }

            GUILayout.Space(10f);

            if (GUILayout.Button("������㡢�յ�"))
            {
                AStarGUI drawMap = this.target as AStarGUI;
                drawMap.setWhich = 0;
            }

            if (GUILayout.Button("�����ϰ�"))
            {
                AStarGUI drawMap = this.target as AStarGUI;
                drawMap.setWhich = -1;
            }

            if (GUILayout.Button("��������ϰ�"))
            {
                AStarGUI drawMap = this.target as AStarGUI;
                drawMap.SetRandomObstacle();
                drawMap.MarkBasicMap();
            }

            GUILayout.Space(10f);

            if (GUILayout.Button("ִ��"))
            {
                AStarGUI drawMap = this.target as AStarGUI;
                drawMap.DisplayNode();
            }
        }
    }
}