using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AStar
{
    /// <summary>
    /// ����������Game���ڵĹ켣
    /// </summary>
    public class AStarGUI : MonoBehaviour
    {
        public static AStarGUI Instance = null;
        [Header("����")]
        // tile ��ײ��
        public LayerMask inputLayer;
        //�����С
        public int tileSize=10;
        // x �᷽��tile����
        public int xTileCount = 10;
        // z �᷽��tile����
        public int zTileCount = 10;
        // ��ǰ���� id
        [HideInInspector]
        public int dataID = 0;
        [HideInInspector]
        public bool debug = false;// �Ƿ���ʾ������Ϣ


        public Vector2 mapSize=new Vector2(10,10);//��ͼ���
        public Vector3 startPos;         //�������
        public Vector3 endPos;          //�յ�����
        
        public Vector3 originalPos = Vector3.zero; //����ԭ��


        //public GameObject tile;       //2d����
        public GameObject[,] tiles;     //��������
        public Texture2D tilePic;
        Vector3 tileScale = new Vector3(1f, 1f, 1f);//��������ߴ�

        [Header("Read Only")]
        public string mapReadPath = "map";//��ͼCSV�ļ����Ŀ¼
        public string mapOutputName = "newmap";//����ĵ�ͼCSV�ļ����Ŀ¼


        public AStarManager aStarManager;

        public GameObject GridObject;//������



        public int[,] mapArray;//��ά����洢״̬��Ϣ�����и��ӵ���ֵ��0��ʾ����,1��ʾͨ����2��ʾʼĩ
        [HideInInspector]
        public 

        /// <summary>
        /// ��ʼ��
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
            MarkValue();    //���·���ֵ
            MarkKeyCells();//����м�·��
            SaveMapToCsv();
        }

        /// <summary>
        /// ����������������Ӧtile����ֵ
        /// </summary>
        /// <param name="pox">x������</param>
        /// <param name="poz">z������</param>
        /// <returns></returns>
        public int getDataFromPosition(float pox, float poz)
        {
            //int index = (int)((pox - transform.position.x) / tileSize) * zTileCount + (int)((poz - transform.position.z) / tileSize);
            //if (index < 0 || index >= data.Length) return 0;
            //��������
            return 0;
        }
        /// <summary>
        /// ������������������Ӧtile����ֵ
        /// </summary>
        /// <param name="pox">x������</param>
        /// <param name="poz">z������</param>
        /// <param name="nodeType">��tile����ֵ</param>
        public void setDataFromPosition(float pox, float poz, NodeType nodeType)
        {
            //int index = (int)((pox - transform.position.x) / tileSize) * zTileCount + (int)((poz - transform.position.z) / tileSize);
            //if (index < 0 || index >= data.Length) return;
            //data[index] = number;

            //��������

        }
        /// <summary>
        /// ��ʼ����ͼ����
        /// </summary>
        public void ResetMapArray()
        {
            mapArray = new int[xTileCount,zTileCount];
        }

        // �ڱ༭ģʽ��ʾ������Ϣ
        void OnDrawGizmos()
        {
            if (!debug)//����ʾ��ͼ���ƽ���
                return;
            if (mapArray == null)//��ͼmapArrayΪ��
            {
                Debug.Log("Please reset data first");
                return;
            }

            Vector3 pos = transform.position;


            //���������ͼϸ��

            //for (int i = 0; i <= xTileCount; i++)  // ��Z�����Ḩ����
            //{
            //    Gizmos.color = new Color(0, 0, 1, 1);
            //    Gizmos.DrawLine(pos + new Vector3(tileSize * i, pos.y, 0),
            //        transform.TransformPoint(tileSize * i, pos.y, tileSize * zTileCount));

            //    for (int k = 0; k < zTileCount; k++)  // ������ʾ��ǰ��ֵ�ĸ���
            //    {
            //        if ((i * zTileCount + k) < data.Length && data[i * zTileCount + k] == dataID)
            //        {
            //            Gizmos.color = new Color(1, 0, 0, 0.3f);
            //            Gizmos.DrawCube(new Vector3(pos.x + i * tileSize + tileSize * 0.5f,
            //                    pos.y, pos.z + k * tileSize + tileSize * 0.5f), new Vector3(tileSize, 0.2f, tileSize));
            //        }
            //    }
            //}

            //for (int k = 0; k <= zTileCount; k++) // ��X�����Ḩ����
            //{
            //    Gizmos.color = new Color(0, 0, 1, 1);
            //    Gizmos.DrawLine(pos + new Vector3(0, pos.y, tileSize * k),
            //        this.transform.TransformPoint(tileSize * xTileCount, pos.y, tileSize * k));
            //}


        }
        public void DrawMap()
        {

            //��յ�ͼ
            ClearChilds();

            //���������ͼ
            for (int gridX = 0; gridX < mapSize.x; gridX++)
                for (int gridY = 0; gridY < mapSize.y; gridY++)
                {
                    Vector3 pos = new Vector3(gridX+0.5f,(float)0.1/tileSize,gridY+0.5f);

                    tiles[gridX, gridY] = new GameObject(gridX+ "-" + gridY);//�������кš��кŹ���
                    tiles[gridX, gridY].transform.SetParent(GridObject.transform);
                    tiles[gridX, gridY].transform.position = originalPos + pos * tileSize;

                    tiles[gridX, gridY].AddComponent<Image>();
                    tiles[gridX, gridY].GetComponent<Image>().sprite = Sprite.Create(tilePic, new Rect(0, 0, tilePic.width, tilePic.height),new Vector3(-90,0,0));//������ש��ͼ
                    tiles[gridX, gridY].transform.localScale = tileScale;//���ô�ש��ͼ�Ĵ�С
                    if (mapArray[gridX,gridY] == 1)
                    {
                        MarkPoint(gridX, gridY, Color.black);//Ĭ�ϴ�ש��ɫΪ��ɫ����Ϊǽ������Ϊ��ɫ
                    }

                    //ʹ��Selectable���յ���¼�
                    tiles[gridX, gridY].AddComponent<Selectable>();
                    //ʹ��������ײ����յ���¼���������
                    //tiles[x, y].AddComponent<BoxCollider2D>().size = new Vector2(100,100);
                    //tiles[x, y].AddComponent<GraphicRaycaster>();
                }
        }
        /// <summary>
        /// �������յ����õ������λ�ûᱨ��
        /// </summary>
        public void ErrorCheck()
        {
            if (!aStarManager.isReachable(startPos))
            {
                Debug.LogError("�����յ���������,����������");
                Vector2 gridXY=aStarManager.Grid.GetGridXY(startPos);
                MarkPoint(gridXY, Color.red);
                setWhich = 0;
            }

            if (!aStarManager.isReachable(endPos))
            {
                Debug.LogError("�����յ���������,����������");
                Vector2 gridXY = aStarManager.Grid.GetGridXY(endPos);
                MarkPoint(gridXY, Color.red);
                setWhich = 0;
            }
        }
        /// <summary>
        /// �������������
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
        /// ��ʾ�õ��F\G\Hֵ
        /// </summary>
        void MarkValue()
        {
            for (int gridX = 0; gridX < AStarManager.Instance.Grid.GetWidth(); gridX++)
                for (int gridY = 0; gridY < AStarManager.Instance.Grid.GetHeight(); gridY++)
                {
                    Node c = AStarManager.Instance.Grid.GetValue(gridX, gridY);//��ȡ�ڵ�
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
        /// ���ƹؼ�����
        /// </summary>
        public void MarkKeyCells()
        {
            //���ƿ����б�
            foreach (var i in aStarManager.openList)
            {
                MarkPoint(i, Color.cyan);//��ɫ
            }
            //���ƹر��б�
            foreach (var i in aStarManager.closeList)
            {
                MarkPoint(i, Color.grey);//��ɫ
            }
            //������СF��
            MarkPoint(aStarManager.GetNodeWithMinF(), Color.red);
            //����·��
            MarkPath();
            //������㡢�յ�
            MarkPoint(startPos, Color.yellow);
            MarkPoint(endPos, Color.yellow);
        }
        /// <summary>
        /// ����ȫͼ����·��ǽ����
        /// </summary>
        public void MarkBasicMap()
        {
            //����·���ϰ�
            for (int gridX = 0; gridX < aStarManager.Grid.GetWidth(); gridX++)
            {
                for (int gridY = 0; gridY < aStarManager.Grid.GetHeight() ; gridY++)
                {
                    if (mapArray == null) 
                    {
                        Debug.Log("��");
                    }
                    int temp1 = (int)MapType.wall;
                    int temp2 = mapArray[gridX, gridY];
                    if (temp1==temp2)//ǽ��Ϊ��ɫ
                    {
                        MarkPoint(gridX, gridY, Color.black);
                    }
                    else//ͨ·Ϊ��ɫ
                    {
                        MarkPoint(gridX, gridY, Color.white);
                    }
                }
            }
            //��������յ�
            MarkPoint(startPos, Color.yellow);
            MarkPoint(endPos, Color.yellow);
            ErrorCheck();
        }

        /// <summary>
        /// ���߹���·����Ϊ��ɫ
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
        /// ���ڵ�����ͼ��λ��
        /// </summary>
        void RefreshPos()
        {
            for (int gridX = 0; gridX < aStarManager.Grid.GetWidth(); gridX++)//�����һά�Ǹ���(x)��ͷָ��
                for (int gridY = 0; gridY < aStarManager.Grid.GetHeight(); gridY++)//����ڶ�ά�Ǹ��еĸ���Ԫ�أ��У�y����
                {
                    Vector3 pos = new Vector3(gridX,(float)0.1/tileSize, gridY);
                    tiles[gridX, gridY].transform.position = originalPos + pos * tileSize;
                }
        }

        public void CreateMapByPraseCSV()
        {
            string text = System.IO.File.ReadAllText(Application.streamingAssetsPath + "/" + mapReadPath + ".csv");
            //TextAsset ta = Resources.Load<TextAsset>(mapReadPath);//�����к�׺��.csv
            //���ж�ȡ
            if (text == null) Debug.LogError("��ȡʧ��");
            string[] lines = text.Split('\n');

            tiles = null;

            for (int x = 0; x < lines.Length; x++)
            {
                if (string.IsNullOrEmpty(lines[x])) continue;
                //�Ƴ��س�
                lines[x] = lines[x].Replace("\r", "");
                //�����Ž���
                string[] linePrased = lines[x].Split(',');

                //ע����ͼ�ĵ����һ�в����ǿ���
               
                mapSize = new Vector2(lines.Length, linePrased.Length);
                if (mapArray == null)
                {
                    mapArray = new int[lines.Length, linePrased.Length];  //�У���
                }
                if (tiles == null)
                {
                    tiles = new GameObject[lines.Length, linePrased.Length];
                }

                for (int y = 0; y < linePrased.Length; y++)
                {
                    //ת��Ϊ����
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
        //ע������ÿ�����һλ�޶���
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
                    //����ά��node,��Ҫά��map
                    NodeType nodeType = (NodeType)(UnityEngine.Random.Range(0, 100) % 10 == 0 ? 1 : 0);     //�ϰ�������� 0.1

                    aStarManager.Grid.GetValue(gridX, gridY).nodeType = nodeType;
                    mapArray[gridX, gridY] = (int)nodeType;
                    #endregion
                }
            }
        }

        public void SaveMapToCsv()
        {
            //��ȡ��ͼ���ݵ��ַ���
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

            //����Ŀ¼
            if (Directory.Exists(Application.streamingAssetsPath) == false)
            {
                Directory.CreateDirectory(Application.streamingAssetsPath);
            }

            //д���ļ�
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
