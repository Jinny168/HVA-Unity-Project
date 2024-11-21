using UnityEngine;
using UnityEditor;
namespace AStar 
{
    [CustomEditor(typeof(AStarGUI))]
    public class GuiEditor : Editor
    {

        // 是否处于编辑模式
        protected bool editMode = false;

        // 受编辑器影响的tile角本
        protected AStarGUI guiObject;

        void OnEnable()
        {
            // 获得tile角本
            guiObject = (AStarGUI)target;
        }

        // 更改场景中的操作
        public void OnSceneGUI()
        {
            if (editMode)  // 如果在编辑模式
            {
                // 取消编辑器的选择功能
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
                // 在编辑器中显示数据（画出辅助线）
                guiObject.debug = true;
                // 获取Input事件
                Event e = Event.current;

                // 如果是鼠标左键
                if (e.button == 0 && (e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && !e.alt)
                {
                    // 获取由鼠标位置产生的射线
                    Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

                    // 计算碰撞
                    RaycastHit hitinfo;
                    if (Physics.Raycast(ray, out hitinfo, 2000, guiObject.inputLayer))
                    {
                        //float tx = hitinfo.point.x - tileObject.transform.position.x;
                        //float tz = hitinfo.point.z - tileObject.transform.position.z;

                        guiObject.setDataFromPosition(hitinfo.point.x, hitinfo.point.z, NodeType.wall);

                    }
                }
            }

            HandleUtility.Repaint();
        }

        // 自定义Inspector窗口的UI
        public override void OnInspectorGUI()
        {
            GUILayout.Label("Map Editor"); // 显示编辑器名称
            editMode = EditorGUILayout.Toggle("Edit", editMode);  // 是否启用编辑模式
            guiObject.debug = EditorGUILayout.Toggle("Debug", guiObject.debug);  // 是否显示帮助信息
            //guiObject.dataID = EditorGUILayout.IntSlider("Data ID", guiObject.dataID, 0, 9); // 编辑id滑块
            string[] editDataStr = { "Road", "Wall", "Point" };

            guiObject.dataID = GUILayout.Toolbar(guiObject.dataID, editDataStr);

            //Debug.Log(tileObject.dataID);

            EditorGUILayout.Separator();  // 分隔符

            if (GUILayout.Button("Reset"))   // 重置按钮
            {
                guiObject.ResetMapArray();  // 初始化
            }
            DrawDefaultInspector();
        }
    }


}
