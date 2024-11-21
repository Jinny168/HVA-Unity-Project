using UnityEngine;
using UnityEditor;
public class PathTool : ScriptableObject
{
    static PathNode m_parent=null;

    [MenuItem("PathTool/Create PathNode")]
    static void CreatePathNode()
    {
        // 创建一个新的路点
        GameObject go = new GameObject();
        go.AddComponent<PathNode>();
        go.name = "pathnode";
        // 设置tag
        go.tag = "pathnode";
        // 使该路点处于选择状态
        Selection.activeTransform = go.transform;
    }
    
    [MenuItem("PathTool/Set Parent %q")]
    static void SetParent()
    {
        if (!Selection.activeGameObject || Selection.GetTransforms(SelectionMode.Unfiltered).Length>1)
            return;
        if (Selection.activeGameObject.tag.CompareTo("pathnode") == 0)
        {       
            m_parent = Selection.activeGameObject.GetComponent<PathNode>();
            Debug.Log( "Set "+m_parent.name+" as parent.");
        }
    }

    [MenuItem("PathTool/Set Next %w")]
    static void SetNextChild()
    {
        if (!Selection.activeGameObject || m_parent==null || Selection.GetTransforms(SelectionMode.Unfiltered).Length>1)
            return;

        if (Selection.activeGameObject.tag.CompareTo("pathnode") == 0)
        {
            m_parent.SetNext(Selection.activeGameObject.GetComponent<PathNode>());
            m_parent = null;
           
            Debug.Log("Set " + Selection.activeGameObject.name + " as child.");
        }
    }
}
       
