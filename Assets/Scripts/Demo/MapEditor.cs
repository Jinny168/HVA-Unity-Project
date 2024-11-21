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
        EditorGUILayout.LabelField("这是一个二维数组的配置文件，默认使用 int 类型");
        EditorGUILayout.LabelField("data : ");
        //EditorGUILayout.BeginVertical();
        //if (GUILayout.Button("清空数据"))
        //{
        //    grid.CleraData();
        //    EditorUtility.SetDirty(grid);
        //    AssetDatabase.SaveAssets();
        //}
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.TextField($"{map.MapHeight}行/{map.MapWidth}列", red);


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
        //if (GUILayout.Button("++添加尾行++"))
        //{
        //    grid.AddRow();
        //    EditorUtility.SetDirty(grid);
        //    AssetDatabase.SaveAssets();
        //}
        //GUILayout.Space(15);
        //if (GUILayout.Button("--删除尾行--"))
        //{
        //    grid.DeletedRow();
        //    EditorUtility.SetDirty(grid);
        //    AssetDatabase.SaveAssets();
        //}
        //EditorGUILayout.EndHorizontal();
        //EditorGUILayout.BeginHorizontal();

        //if (GUILayout.Button("++添加尾列++"))
        //{
        //    grid.AddColumn();
        //    EditorUtility.SetDirty(grid);
        //    AssetDatabase.SaveAssets();
        //}
        //GUILayout.Space(15);
        //if (GUILayout.Button("--删除尾列--"))
        //{
        //    grid.DeletedColumn();
        //    EditorUtility.SetDirty(grid);
        //    AssetDatabase.SaveAssets();
        //}
        //EditorGUILayout.EndHorizontal();
        //EditorGUILayout.EndVertical();
        //if (GUILayout.Button("保存/更新"))
        //{
        //    EditorUtility.SetDirty(grid);
        //    AssetDatabase.SaveAssets();
        //}
        serializedObject.ApplyModifiedProperties();
    }
}