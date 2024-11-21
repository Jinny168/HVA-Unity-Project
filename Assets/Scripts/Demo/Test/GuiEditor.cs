using UnityEngine;
using UnityEditor;
namespace AStar 
{
    [CustomEditor(typeof(AStarGUI))]
    public class GuiEditor : Editor
    {

        // �Ƿ��ڱ༭ģʽ
        protected bool editMode = false;

        // �ܱ༭��Ӱ���tile�Ǳ�
        protected AStarGUI guiObject;

        void OnEnable()
        {
            // ���tile�Ǳ�
            guiObject = (AStarGUI)target;
        }

        // ���ĳ����еĲ���
        public void OnSceneGUI()
        {
            if (editMode)  // ����ڱ༭ģʽ
            {
                // ȡ���༭����ѡ����
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
                // �ڱ༭������ʾ���ݣ����������ߣ�
                guiObject.debug = true;
                // ��ȡInput�¼�
                Event e = Event.current;

                // �����������
                if (e.button == 0 && (e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && !e.alt)
                {
                    // ��ȡ�����λ�ò���������
                    Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

                    // ������ײ
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

        // �Զ���Inspector���ڵ�UI
        public override void OnInspectorGUI()
        {
            GUILayout.Label("Map Editor"); // ��ʾ�༭������
            editMode = EditorGUILayout.Toggle("Edit", editMode);  // �Ƿ����ñ༭ģʽ
            guiObject.debug = EditorGUILayout.Toggle("Debug", guiObject.debug);  // �Ƿ���ʾ������Ϣ
            //guiObject.dataID = EditorGUILayout.IntSlider("Data ID", guiObject.dataID, 0, 9); // �༭id����
            string[] editDataStr = { "Road", "Wall", "Point" };

            guiObject.dataID = GUILayout.Toolbar(guiObject.dataID, editDataStr);

            //Debug.Log(tileObject.dataID);

            EditorGUILayout.Separator();  // �ָ���

            if (GUILayout.Button("Reset"))   // ���ð�ť
            {
                guiObject.ResetMapArray();  // ��ʼ��
            }
            DrawDefaultInspector();
        }
    }


}
