using UnityEngine;

public class TileObject : MonoBehaviour {

    public static TileObject Instance = null;

    // tile 碰撞层
    public LayerMask tileLayer;
    // tile 大小
    public int tileSize = 1;
    // x 轴方向tile数量
    public int xTileCount = 5;
    // z 轴方向tile数量
    public int zTileCount = 3;
    //默认坐标原点
    public Vector3 originalPos = Vector3.zero;

    // 格子的数值，0表示锁定,1表示通道，2表示始末
    public int[] data;


    // 当前数据 id
    [HideInInspector]
    public int dataID = 0;
    [HideInInspector]
    // 是否显示数据信息
    public bool debug = false;
    void Awake()
    {
        Instance = this;
    }
    // 初始化地图数据
    public void Reset()
    {
        data = new int[xTileCount * zTileCount];
    }

    // 获得相应tile的数值
    public int getDataFromPosition(float pox, float poz)
    {
        int gridX = (int)((pox - transform.position.x) / tileSize);
        int gridY = (int)((poz - transform.position.z) / tileSize);
        int index = gridX * zTileCount + gridY;
        if (index < 0 || index >= data.Length) return 0;
        return data[index];
    }

    // 设置相应tile的数值
    public void setDataFromPosition( float pox, float poz, int number )
    {
        int gridX = (int)((pox - transform.position.x) / tileSize);
        int gridY = (int)((poz - transform.position.z) / tileSize);
        int index = gridX * zTileCount + gridY;
        if (index < 0 || index >= data.Length) return;
        data[index] = number;
    }

    /// <summary>
    /// 将完整的地图数据输出给Grid类供其创建map
    /// </summary>
    public int[,] getMapArray()
    {
        int[,] mapArray=new int[xTileCount,zTileCount];
        for (int i = 0; i < xTileCount; i++)
        {
            for (int j = 0; j < zTileCount; j++)
            {
                mapArray[i, j]=data[i*zTileCount+j];
            }
        }
        return mapArray;
    }
    /// <summary>
    /// 展示数据
    /// </summary>
    public void displayData(int[,] mapArr)
    {
        for (int i = 0; i < xTileCount; i++)
        {
            for (int j = 0; j < zTileCount; j++)
            {
                Debug.LogFormat("GridXY:({0},{1}),NodeType:"+mapArr[i, j].ToString(),i,j);
            }
        }
    }
    // 在编辑模式显示帮助信息
    void OnDrawGizmos()
    {
        if (!debug)
            return;
        if (data==null)
        {
            Debug.Log("Please reset data first");
            return;
        }

        Vector3 pos = transform.position;

        for (int i = 0; i <= xTileCount; i++)  // 画Z方向轴辅助线
        {
            Gizmos.color = new Color(0, 0, 1, 1);
            Gizmos.DrawLine(pos + new Vector3(tileSize * i, pos.y, 0),
                transform.TransformPoint(tileSize * i, pos.y, tileSize * zTileCount)); 
         
            for (int k = 0; k < zTileCount; k++)  // 高亮显示当前数值的格子
            {
                if ( (i * zTileCount + k) < data.Length && data[i * zTileCount + k] == dataID) 
                {
                    Gizmos.color = new Color(1, 0, 0, 0.3f);
                    Gizmos.DrawCube(new Vector3(pos.x + i * tileSize + tileSize * 0.5f,
                            pos.y, pos.z + k * tileSize + tileSize * 0.5f), new Vector3(tileSize, 0.2f, tileSize));
                }
            }
        }

        for (int k = 0; k <= zTileCount; k++) // 画X方向轴辅助线
        {
            Gizmos.color = new Color(0, 0, 1, 1);
            Gizmos.DrawLine(pos + new Vector3(0, pos.y, tileSize * k),
                this.transform.TransformPoint(tileSize * xTileCount, pos.y, tileSize * k));
        }
    }
}
