using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(Map))]
public class DataGridEditor : Editor
{
    Map map;
    GUIStyle green = new GUIStyle(EditorStyles.textArea);
    GUIStyle red = new GUIStyle(EditorStyles.textArea);
    private void Awake()
    {
        map = (Map)target;
        green.normal.textColor = Color.green;
        red.normal.textColor = Color.red;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.LabelField("����һ����ά����������ļ���Ĭ��ʹ�� int ����");
        EditorGUILayout.LabelField("data : ");
        //EditorGUILayout.BeginVertical();
        //if (GUILayout.Button("�������"))
        //{
        //    grid.CleraData();
        //    EditorUtility.SetDirty(grid);
        //    AssetDatabase.SaveAssets();
        //}
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.TextField($"{map.MapHeight}��/{map.MapWidth}��", red);


        for (int j = 0; j < map.MapWidth; j++)
        {
            map.columnNames[j] = EditorGUILayout.TextField(map.columnNames[j].ToString(), green);
        }
        EditorGUILayout.EndHorizontal();
        for (int i = 0; i <map.MapHeight; i++)
        {
            EditorGUILayout.BeginHorizontal();
            map.rowNames[i] = EditorGUILayout.TextField(map.rowNames[i], green);
            for (int j = 0; j < map.MapWidth; j++)
            {
                map.mapArray[i, j] = int.Parse(EditorGUILayout.TextField(map.mapArray[i, j].ToString()));
            }
            EditorGUILayout.EndHorizontal();
        }


        //EditorGUILayout.BeginHorizontal();
        //if (GUILayout.Button("++���β��++"))
        //{
        //    grid.AddRow();
        //    EditorUtility.SetDirty(grid);
        //    AssetDatabase.SaveAssets();
        //}
        //GUILayout.Space(15);
        //if (GUILayout.Button("--ɾ��β��--"))
        //{
        //    grid.DeletedRow();
        //    EditorUtility.SetDirty(grid);
        //    AssetDatabase.SaveAssets();
        //}
        //EditorGUILayout.EndHorizontal();
        //EditorGUILayout.BeginHorizontal();

        //if (GUILayout.Button("++���β��++"))
        //{
        //    grid.AddColumn();
        //    EditorUtility.SetDirty(grid);
        //    AssetDatabase.SaveAssets();
        //}
        //GUILayout.Space(15);
        //if (GUILayout.Button("--ɾ��β��--"))
        //{
        //    grid.DeletedColumn();
        //    EditorUtility.SetDirty(grid);
        //    AssetDatabase.SaveAssets();
        //}
        //EditorGUILayout.EndHorizontal();
        //EditorGUILayout.EndVertical();
        //if (GUILayout.Button("����/����"))
        //{
        //    EditorUtility.SetDirty(grid);
        //    AssetDatabase.SaveAssets();
        //}
        serializedObject.ApplyModifiedProperties();
    }
}