using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AStar
{
    /// <summary>
    /// 绘制物体在Game窗口的轨迹
    /// </summary>
    public class AStarGUI : MonoBehaviour
    {
        public static AStarGUI Instance = null;
        [Header("配置")]
        // tile 碰撞层
        public LayerMask inputLayer;
        //网格大小
        public int tileSize=10;
        // x 轴方向tile数量
        public int xTileCount = 10;
        // z 轴方向tile数量
        public int zTileCount = 10;
        // 当前数据 id
        [HideInInspector]
        public int dataID = 0;
        [HideInInspector]
        public bool debug = false;// 是否显示数据信息


        public Vector2 mapSize=new Vector2(10,10);//地图宽高
        public Vector3 startPos;         //起点坐标
        public Vector3 endPos;          //终点坐标
        
        public Vector3 originalPos = Vector3.zero; //坐标原点


        //public GameObject tile;       //2d方块
        public GameObject[,] tiles;     //方块数组
        public Texture2D tilePic;
        Vector3 tileScale = new Vector3(1f, 1f, 1f);//网格放缩尺寸

        [Header("Read Only")]
        public string mapReadPath = "map";//地图CSV文件存放目录
        public string mapOutputName = "newmap";//输出的地图CSV文件存放目录


        public AStarManager aStarManager;

        public GameObject GridObject;//父物体



        public int[,] mapArray;//二维数组存储状态信息，其中格子的数值，0表示锁定,1表示通道，2表示始末
        [HideInInspector]
        public 

        /// <summary>
        /// 初始化
        /// </summary>
        void Awake()
        {
            mapArray=Map.Instance.mapArray;


            originalPos = Vector3.zero;
            font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            GridObject = GameObject.FindGameObjectWithTag("GridObject");
        }
        void Start()
        {
            aStarManager = new AStarManager(SceneManager.GetActiveScene().name);
        }
        

        public void DisplayNode()
        {
            aStarManager.FindPath(startPos,endPos);
            MarkValue();    //标记路点的值
            MarkKeyCells();//标记中间路点
            SaveMapToCsv();
        }

        /// <summary>
        /// 依据物理坐标获得相应tile的数值
        /// </summary>
        /// <param name="pox">x轴坐标</param>
        /// <param name="poz">z轴坐标</param>
        /// <returns></returns>
        public int getDataFromPosition(float pox, float poz)
        {
            //int index = (int)((pox - transform.position.x) / tileSize) * zTileCount + (int)((poz - transform.position.z) / tileSize);
            //if (index < 0 || index >= data.Length) return 0;
            //后续补充
            return 0;
        }
        /// <summary>
        /// 依据物理坐标设置相应tile的数值
        /// </summary>
        /// <param name="pox">x轴坐标</param>
        /// <param name="poz">z轴坐标</param>
        /// <param name="nodeType">该tile的数值</param>
        public void setDataFromPosition(float pox, float poz, NodeType nodeType)
        {
            //int index = (int)((pox - transform.position.x) / tileSize) * zTileCount + (int)((poz - transform.position.z) / tileSize);
            //if (index < 0 || index >= data.Length) return;
            //data[index] = number;

            //后续补充

        }
        /// <summary>
        /// 初始化地图数据
        /// </summary>
        public void ResetMapArray()
        {
            mapArray = new int[xTileCount,zTileCount];
        }

        // 在编辑模式显示帮助信息
        void OnDrawGizmos()
        {
            if (!debug)//不显示地图绘制界面
                return;
            if (mapArray == null)//地图mapArray为空
            {
                Debug.Log("Please reset data first");
                return;
            }

            Vector3 pos = transform.position;


            //补充后续绘图细节

            //for (int i = 0; i <= xTileCount; i++)  // 画Z方向轴辅助线
            //{
            //    Gizmos.color = new Color(0, 0, 1, 1);
            //    Gizmos.DrawLine(pos + new Vector3(tileSize * i, pos.y, 0),
            //        transform.TransformPoint(tileSize * i, pos.y, tileSize * zTileCount));

            //    for (int k = 0; k < zTileCount; k++)  // 高亮显示当前数值的格子
            //    {
            //        if ((i * zTileCount + k) < data.Length && data[i * zTileCount + k] == dataID)
            //        {
            //            Gizmos.color = new Color(1, 0, 0, 0.3f);
            //            Gizmos.DrawCube(new Vector3(pos.x + i * tileSize + tileSize * 0.5f,
            //                    pos.y, pos.z + k * tileSize + tileSize * 0.5f), new Vector3(tileSize, 0.2f, tileSize));
            //        }
            //    }
            //}

            //for (int k = 0; k <= zTileCount; k++) // 画X方向轴辅助线
            //{
            //    Gizmos.color = new Color(0, 0, 1, 1);
            //    Gizmos.DrawLine(pos + new Vector3(0, pos.y, tileSize * k),
            //        this.transform.TransformPoint(tileSize * xTileCount, pos.y, tileSize * k));
            //}


        }
        public void DrawMap()
        {

            //清空地图
            ClearChilds();

            //绘制网格地图
            for (int gridX = 0; gridX < mapSize.x; gridX++)
                for (int gridY = 0; gridY < mapSize.y; gridY++)
                {
                    Vector3 pos = new Vector3(gridX+0.5f,(float)0.1/tileSize,gridY+0.5f);

                    tiles[gridX, gridY] = new GameObject(gridX+ "-" + gridY);//名称由行号、列号构成
                    tiles[gridX, gridY].transform.SetParent(GridObject.transform);
                    tiles[gridX, gridY].transform.position = originalPos + pos * tileSize;

                    tiles[gridX, gridY].AddComponent<Image>();
                    tiles[gridX, gridY].GetComponent<Image>().sprite = Sprite.Create(tilePic, new Rect(0, 0, tilePic.width, tilePic.height),new Vector3(-90,0,0));//创建瓷砖贴图
                    tiles[gridX, gridY].transform.localScale = tileScale;//设置瓷砖贴图的大小
                    if (mapArray[gridX,gridY] == 1)
                    {
                        MarkPoint(gridX, gridY, Color.black);//默认瓷砖颜色为白色，若为墙体设置为黑色
                    }

                    //使用Selectable接收点击事件
                    tiles[gridX, gridY].AddComponent<Selectable>();
                    //使用物理碰撞体接收点击事件并不明智
                    //tiles[x, y].AddComponent<BoxCollider2D>().size = new Vector2(100,100);
                    //tiles[x, y].AddComponent<GraphicRaycaster>();
                }
        }
        /// <summary>
        /// 将起点或终点设置到错误的位置会报错
        /// </summary>
        public void ErrorCheck()
        {
            if (!aStarManager.isReachable(startPos))
            {
                Debug.LogError("起点或终点设置有误,请重新设置");
                Vector2 gridXY=aStarManager.Grid.GetGridXY(startPos);
                MarkPoint(gridXY, Color.red);
                setWhich = 0;
            }

            if (!aStarManager.isReachable(endPos))
            {
                Debug.LogError("起点或终点设置有误,请重新设置");
                Vector2 gridXY = aStarManager.Grid.GetGridXY(endPos);
                MarkPoint(gridXY, Color.red);
                setWhich = 0;
            }
        }
        /// <summary>
        /// 物理清除子物体
        /// </summary>
        public void ClearChilds()
        {
            Transform parent = GridObject.transform;
            int totalCount = parent.childCount-1;
            if (totalCount > 0)
            {
                for (int i = 0; i < totalCount; i++)
                {
                    if (parent.GetChild(i).name.Contains("-"))
                    {
                        DestroyImmediate(parent.GetChild(i).gameObject);
                    }
                }
            }
        }

        Font font;
        /// <summary>
        /// 显示该点的F\G\H值
        /// </summary>
        void MarkValue()
        {
            for (int gridX = 0; gridX < AStarManager.Instance.Grid.GetWidth(); gridX++)
                for (int gridY = 0; gridY < AStarManager.Instance.Grid.GetHeight(); gridY++)
                {
                    Node c = AStarManager.Instance.Grid.GetValue(gridX, gridY);//获取节点
                    if (c == null)
                    {
                        continue;
                    }
                    if (c.G !=int.MaxValue)
                    {
                        if (tiles[gridX, gridY].GetComponentInChildren<Text>() == null)
                        {
                            GameObject obj = new GameObject("Text");
                            obj.AddComponent<Text>();
                            obj.GetComponent<Text>().color = Color.black;
                            obj.GetComponent<Text>().font = font;
                            obj.transform.SetParent(tiles[gridX, gridY].transform);
                            obj.transform.localPosition = new Vector3(4, 0, -3);
                        }
                        Transform text = tiles[gridX, gridY].transform.GetChild(0);
                        text.GetComponent<Text>().text = c.F + "\n" + c.G + "\n" + c.H;
                        Debug.Log(text.GetComponent<Text>().text);
                    }
                }
        }

        public void MarkPoint(int gridX, int gridY, Color color)
        {
            tiles[gridX, gridY].GetComponent<Image>().color = color;
        }

        public void MarkPoint(Node c, Color color)
        {
            tiles[c.gridX, c.gridY].GetComponent<Image>().color = color;
        }

        public void MarkPoint(Vector3 p, Color color)
        {
            Vector2 gridXY=aStarManager.Grid.GetGridXY(p);
            tiles[(int)gridXY.x, (int)gridXY.y].GetComponent<Image>().color = color;
        }
        
        /// <summary>
        /// 绘制关键格子
        /// </summary>
        public void MarkKeyCells()
        {
            //绘制开放列表
            foreach (var i in aStarManager.openList)
            {
                MarkPoint(i, Color.cyan);//青色
            }
            //绘制关闭列表
            foreach (var i in aStarManager.closeList)
            {
                MarkPoint(i, Color.grey);//灰色
            }
            //绘制最小F点
            MarkPoint(aStarManager.GetNodeWithMinF(), Color.red);
            //绘制路径
            MarkPath();
            //绘制起点、终点
            MarkPoint(startPos, Color.yellow);
            MarkPoint(endPos, Color.yellow);
        }
        /// <summary>
        /// 绘制全图，含路、墙、点
        /// </summary>
        public void MarkBasicMap()
        {
            //绘制路与障碍
            for (int gridX = 0; gridX < aStarManager.Grid.GetWidth(); gridX++)
            {
                for (int gridY = 0; gridY < aStarManager.Grid.GetHeight() ; gridY++)
                {
                    if (mapArray == null) 
                    {
                        Debug.Log("空");
                    }
                    int temp1 = (int)MapType.wall;
                    int temp2 = mapArray[gridX, gridY];
                    if (temp1==temp2)//墙体为黑色
                    {
                        MarkPoint(gridX, gridY, Color.black);
                    }
                    else//通路为白色
                    {
                        MarkPoint(gridX, gridY, Color.white);
                    }
                }
            }
            //绘制起点终点
            MarkPoint(startPos, Color.yellow);
            MarkPoint(endPos, Color.yellow);
            ErrorCheck();
        }

        /// <summary>
        /// 将走过的路点标记为绿色
        /// </summary>
        public void MarkPath()
        {
            Node father = aStarManager.GetNodeWithMinF();
            while (father != null)
            {
                MarkPoint(father, Color.green);
                father = father.Father;
            }
        }

        /// <summary>
        /// 用于调整地图的位置
        /// </summary>
        void RefreshPos()
        {
            for (int gridX = 0; gridX < aStarManager.Grid.GetWidth(); gridX++)//数组第一维是各行(x)的头指针
                for (int gridY = 0; gridY < aStarManager.Grid.GetHeight(); gridY++)//数组第二维是各行的各个元素（列（y））
                {
                    Vector3 pos = new Vector3(gridX,(float)0.1/tileSize, gridY);
                    tiles[gridX, gridY].transform.position = originalPos + pos * tileSize;
                }
        }

        public void CreateMapByPraseCSV()
        {
            string text = System.IO.File.ReadAllText(Application.streamingAssetsPath + "/" + mapReadPath + ".csv");
            //TextAsset ta = Resources.Load<TextAsset>(mapReadPath);//不能有后缀名.csv
            //按行读取
            if (text == null) Debug.LogError("读取失败");
            string[] lines = text.Split('\n');

            tiles = null;

            for (int x = 0; x < lines.Length; x++)
            {
                if (string.IsNullOrEmpty(lines[x])) continue;
                //移除回车
                lines[x] = lines[x].Replace("\r", "");
                //按逗号解析
                string[] linePrased = lines[x].Split(',');

                //注：地图文档最后一行不能是空行
               
                mapSize = new Vector2(lines.Length, linePrased.Length);
                if (mapArray == null)
                {
                    mapArray = new int[lines.Length, linePrased.Length];  //行，列
                }
                if (tiles == null)
                {
                    tiles = new GameObject[lines.Length, linePrased.Length];
                }

                for (int y = 0; y < linePrased.Length; y++)
                {
                    //转换为整形
                    if (int.Parse(linePrased[y]) == 1)
                    {
                        aStarManager.Grid.GetValue(x, y).nodeType = NodeType.wall;
                    }
                    else
                    {
                        aStarManager.Grid.GetValue(x, y).nodeType = NodeType.walk;
                    }

                    mapArray[x, y] = int.Parse(linePrased[y]);
                }
            }
        }
        //注：表中每行最后一位无逗号
        public void CreateMapByGivenSize()
        {
            tiles = new GameObject[(int)mapSize.x, (int)mapSize.y];
        }

        public void SetRandomObstacle()
        {
            for (int gridX = 0; gridX < mapSize.x; gridX++)
            {
                for (int gridY = 0; gridY < mapSize.y; gridY++)
                {
                    Vector3 p = new Vector3(gridX,0, -gridY);
                    if (p == startPos || p == endPos)
                    {
                        continue;
                    }
                    #region
                    //不仅维护node,还要维护map
                    NodeType nodeType = (NodeType)(UnityEngine.Random.Range(0, 100) % 10 == 0 ? 1 : 0);     //障碍块出现率 0.1

                    aStarManager.Grid.GetValue(gridX, gridY).nodeType = nodeType;
                    mapArray[gridX, gridY] = (int)nodeType;
                    #endregion
                }
            }
        }

        public void SaveMapToCsv()
        {
            //读取地图数据到字符串
            StringBuilder stringBuilder = new StringBuilder();
            for (int x = 0; x < mapSize.x; x++)
            {
                for (int y = 0; y < mapSize.y; y++)
                {
                    if (y != mapSize.y - 1)
                        stringBuilder.Append(mapArray[x, y] + ",");
                    else
                        stringBuilder.Append(mapArray[x, y]);
                }
                if (x != mapSize.x - 1) stringBuilder.Append("\r\n");
            }

            //创建目录
            if (Directory.Exists(Application.streamingAssetsPath) == false)
            {
                Directory.CreateDirectory(Application.streamingAssetsPath);
            }

            //写入文件
            using (FileStream fileStream = new FileStream(Application.streamingAssetsPath + "\\" + mapOutputName + ".csv", FileMode.Create, FileAccess.Write))
            {
                using (TextWriter textWriter = new StreamWriter(fileStream, Encoding.UTF8))
                {
                    textWriter.Write(stringBuilder.ToString());
                }
            }
        }

        [HideInInspector]
        public int setWhich = 2;
       
        public AStarManager astar = AStarManager.Instance;
        public void SetStartAndEndPoint()
        {
            var obj = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
            Debug.Log(obj.transform.name);

            string[] pos = obj.transform.name.Split(' ');
            if (pos.Length != 2) return;

            if (setWhich >= 2) return;

            if (setWhich % 2 == 0)
            {
                startPos.x = int.Parse(pos[0]);
                startPos.z = int.Parse(pos[1]);

                astar.Grid.GetValue((int)startPos.x, (int)startPos.z).nodeType = NodeType.point;
                mapArray[(int)startPos.x, (int)startPos.z] =(int) MapType.point;
                MarkBasicMap();
            }
            else if (setWhich % 2 == 1)
            {
                endPos.x = int.Parse(pos[0]);
                endPos.z = int.Parse(pos[1]);

                astar.Grid.GetValue((int)endPos.x, (int)endPos.z).nodeType = NodeType.point;
                mapArray[(int)endPos.x, (int)endPos.z] = (int)MapType.point;
                MarkBasicMap();
            }
            setWhich++;
        }
        
        public void SetObstacle()
        {
            var obj = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
            Debug.Log(obj.transform.name);

            string[] pos = obj.transform.name.Split(' ');
            if (pos.Length != 2) return;

            int x = int.Parse(pos[0]);
            int y = int.Parse(pos[1]);
            
            if (astar.Grid.GetValue(x,y).nodeType== NodeType.wall)            {
                astar.Grid.GetValue(x, y).nodeType = NodeType.walk;
            }
            else if (astar.Grid.GetValue(x, y).nodeType == NodeType.walk)
            {
                astar.Grid.GetValue(x, y).nodeType = NodeType.wall;
            }
            if (mapArray[x, y] ==(int) MapType.walk)
            {

               mapArray[x, y] = (int)MapType.wall;
            }
            else if (mapArray[x,y]==(int)MapType.wall)
            {
                mapArray[x, y] = (int)MapType.walk;
            }

            MarkBasicMap();
        }
    }

    


}
