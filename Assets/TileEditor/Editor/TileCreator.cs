using UnityEngine;
using System.Collections;
using UnityEditor;

public class TileCreator : UnityEditor.EditorWindow {

    // 关卡模型名称
    string tileName = "tile1";
    // 每个四边形的顶点数量
    int verPerPoly = 6;
    // 每个多边形的大小
    int polySize = 1;
    // 模型x轴方向多边形数量
    int xCount = 2;
    // 模型z轴方向多边形数量
    int zCount = 2;

    [MenuItem("Tools/TileCreator")]
	static void Create () {

        EditorWindow.GetWindow(typeof(TileCreator));
	}

    void OnGUI()
    {
        // 显示窗口名称
        GUILayout.Label("Tile Creator", EditorStyles.boldLabel);
        // 模型名称
        tileName = EditorGUILayout.TextField("Tile Object Name", tileName);
        // 四边形大小
        polySize = EditorGUILayout.IntSlider("tile size", polySize, 1, 20);
        // x和z轴四边形数量
        xCount = EditorGUILayout.IntSlider("x tile count", xCount, 1, 100);
        zCount = EditorGUILayout.IntSlider("z tile count", zCount, 1, 100);

        if (GUILayout.Button("Create Tile Object"))
        {
            createMesh();
        }
    }


    void createMesh()
    {
        // 多边形（四边形）数量
        int polyCount = xCount * zCount;

        // 三角形编号 （ 一个三角形由3个顶点组成 一个四边形由2个三角形组成 ）
        int[] newTriangles = new int[ polyCount * verPerPoly ];

        // 顶点数量
        int verCount = (xCount + 1) * (zCount + 1);

        // 顶点
        Vector3[] vertices = new Vector3[verCount];
      
        // 模型normals
        Vector3[] normals = new Vector3[verCount];

        // uv
        Vector2[] newUV = new Vector2[verCount];

        // uv2
        Vector2[] newUV2 = new Vector2[verCount];

        // 创建顶点
        for (int i = 0; i < xCount + 1; i++)
        {
            for (int k = 0; k < zCount + 1; k++)
            {
                int index = i * (zCount + 1) + k;
                float nx = i * polySize;
                float nz = k * polySize;

                vertices[index] = new Vector3(nx, 0, nz);
                normals[index] = Vector3.up;
                newUV[index] = new Vector2(nx, nz);
                newUV2[index] = new Vector2(nx / (xCount * polySize), nz / (zCount * polySize));
            }
        }

        // 创建三角形 uv normal
        for (int i = 0; i < xCount; i++)
        {
            for (int k = 0; k < zCount; k++)
            {
                // 计算出当前四边形的四个顶点编号
                int vindex = i * (zCount + 1) + k;
                int v0 = vindex;
                int v1 = vindex + 1;
                int v2 = vindex + zCount + 2;
                int v3 = vindex;
                int v4 = vindex + zCount + 2;
                int v5 = vindex + zCount + 1;

                // 计算出当前的数组编号
                int index = i * zCount + k;

                // 为三角形指定顶点
                newTriangles[index * verPerPoly] = v0;
                newTriangles[index * verPerPoly + 1] = v1;
                newTriangles[index * verPerPoly + 2] = v2;
                newTriangles[index * verPerPoly + 3] = v3;
                newTriangles[index * verPerPoly + 4] = v4;
                newTriangles[index * verPerPoly + 5] = v5;

            }
        }

        // 创建mesh
        Mesh mesh = new Mesh();
        mesh.name = tileName;
        mesh.vertices = vertices;
        
        mesh.normals = normals;
        mesh.uv = newUV;
        mesh.uv2 = newUV2;
        mesh.subMeshCount = 1;

        //mesh.triangles = newTriangles;
        mesh.SetTriangles(newTriangles, 0);
        mesh.RecalculateBounds();

        AssetDatabase.CreateAsset(mesh, "Assets/TileEditor/Mesh/" + mesh.name + ".asset");
        AssetDatabase.SaveAssets();

        // 创建GameObject
        GameObject go = new GameObject(tileName, typeof(MeshRenderer), typeof(MeshFilter), typeof(MeshCollider));
        MeshRenderer r = go.GetComponent<MeshRenderer>();
        r.castShadows = false;
        MeshFilter mf = go.GetComponent<MeshFilter>();
        mf.mesh = mesh;
        MeshCollider mc = go.GetComponent<MeshCollider>();
        mc.sharedMesh = mesh;

        // 初始化tile
        TileObject tiledata = go.AddComponent<TileObject>();
        tiledata.tileSize = polySize;
        tiledata.xTileCount = xCount;
        tiledata.zTileCount = zCount;

        tiledata.data = new int[ xCount*zCount ];

        for (int w = 0; w < xCount; w++)
        {
            for (int h = 0; h < zCount; h++)
            {
                int index = w * zCount + h;
                tiledata.data[index] = new int();
                tiledata.data[index] = 0;
            }
        }
    }

}
