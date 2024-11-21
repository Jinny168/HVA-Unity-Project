using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TileObject))]
public class TileEditor : Editor {

    // 是否处于编辑模式
    protected bool editMode = false;

    // 受编辑器影响的tile角本
    protected TileObject tileObject;

    void OnEnable()
    {
        // 获得tile角本
        tileObject = (TileObject)target;
    }

    // 更改场景中的操作
    public void OnSceneGUI()
    {
        if (editMode)  // 如果在编辑模式
        {
            // 取消编辑器的选择功能
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            // 在编辑器中显示数据（画出辅助线）
            tileObject.debug = true;
            // 获取Input事件
            Event e = Event.current;
          
            // 如果是鼠标左键
            if ( e.button == 0 && (e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && !e.alt)
            {
                // 获取由鼠标位置产生的射线
                Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

                // 计算碰撞
                RaycastHit hitinfo;
                if (Physics.Raycast(ray, out hitinfo, 2000, tileObject.tileLayer))
                {
                    //float tx = hitinfo.point.x - tileObject.transform.position.x;
                    //float tz = hitinfo.point.z - tileObject.transform.position.z;

                    tileObject.setDataFromPosition(hitinfo.point.x, hitinfo.point.z, tileObject.dataID);

                }
            }
        }

        HandleUtility.Repaint();
    }

    // 自定义Inspector窗口的UI
    public override void OnInspectorGUI()
    {
        GUILayout.Label("Tile Editor"); // 显示编辑器名称
        editMode = EditorGUILayout.Toggle("Edit", editMode);  // 是否启用编辑模式
        tileObject.debug = EditorGUILayout.Toggle("Debug", tileObject.debug);  // 是否显示帮助信息
        //tileObject.dataID = EditorGUILayout.IntSlider("Data ID", tileObject.dataID, 0, 9); // 编辑id滑块

        string[] editDataStr = { "Road", "Wall", "Point" };
        tileObject.dataID = GUILayout.Toolbar(tileObject.dataID, editDataStr);
        //Debug.Log(tileObject.dataID);
        EditorGUILayout.Separator();  // 分隔符
        if (GUILayout.Button("Reset" ))   // 重置按钮
        {
            tileObject.Reset();  // 初始化
        }
        EditorGUILayout.Separator();  // 分隔符
        if (GUILayout.Button("Display"))   // 展示按钮
        {
            int[,] mapArr;
            mapArr=tileObject.getMapArray(); //填充二维数组，因为二维数组没法序列化
            TileObject.Instance.displayData(mapArr);  // 输出二维数组的信息
        }
        DrawDefaultInspector();
    }
}
