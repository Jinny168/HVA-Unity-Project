using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AStar 
{
    /// <summary>
    /// A�Ƿ�װ
    /// </summary>
    public class AStarManager
    {
        public static AStarManager Instance;


        //#region
        ////��ά����
        //public Map mi=Map.Instance;//0·1ǽ2�㣬ά�������񣬷���AStarGUI��ͼ
        //#endregion


        //ֱ���ƶ��ͶԽ��ƶ��Ĵ���
        private const int MOVE_STRAIGHT_COST = 10;
        private const int MOVE_DIAGONAL_COST = 14;
        public Grid<Node> Grid { get; set; }
        public Node GetNode(int gridX, int gridY)
        {
            return Grid.GetValue(gridX, gridY);
        }
        //�����б�
        internal List<Node> openList = new List<Node>();
        //�ر��б�
        internal List<Node> closeList = new List<Node>();
        float radius;
        private int mapWidth;
        private int mapHeight;
        private int gridSize;

        public AStarManager(string sceneName)
        {
            Instance = this;

            switch (sceneName)
            {
                case "Demo":
                    //��ȡtileObject�������õĹؼ���Ϣ
                    TileObject tileObject = GameObject.FindGameObjectWithTag("GridObject").GetComponent<TileObject>();
                    mapWidth = tileObject.xTileCount;
                    mapHeight = tileObject.zTileCount;
                    gridSize = tileObject.tileSize;
                    Vector3 originalPos = tileObject.originalPos;
                    int[,] mapArray = tileObject.getMapArray();

                    Debug.LogFormat("��ͼ���Ϊ{0},��ͼ�߶�Ϊ{1},��ͼ��Ԫ��ߴ�Ϊ{2}",mapWidth,mapHeight,gridSize);
                    //tileObject.displayData(mapArray);

                    Grid = new Grid<Node>(mapWidth, mapHeight, gridSize,mapArray,originalPos,
                        (Grid<Node> g, int gridx, int gridy, NodeType type) => new Node(g, gridx, gridy, type));

                    Enemy enemy = GameObject.FindObjectOfType<Enemy>();
                    radius = enemy.m_radius;
                    //Grid = new Grid<Node>((int)AstarGUI.instance.mapSize.x, 
                    //    (int)AstarGUI.instance.mapSize.y, (int)AstarGUI.instance.cellSize, AstarGUI.instance.originalPos,
                    //    (Grid<Node> g, int gridx, int gridy, NodeType type) => new Node(g, gridx, gridy, type));
                    //(Grid<Node> g, int x, int y) => new Node(g, x, y)��һ������ί�к�����
                    //������һ��Grid<Node>���͵Ĳ���g���Լ�������������x��y��������һ���µ�Node����
                    //���ί�к������ڴ���ÿ�����Ӷ���
                    break;
            }
        }
        /// <summary>
        /// Ѱ·�ķ���
        /// </summary>
        /// <param name="startPos">�������</param>
        /// <param name="endPos">�յ�����</param>
        /// <returns></returns>
        public List<Vector3> FindPath(Vector3 startPos, Vector3 endPos)
        {
            if (!isReachable(startPos) || !isReachable(endPos))
            {
                Debug.LogError("�����յ㲻�������ϰ�����");
                return null;
            }

            //��ʼ����յ����������
            Vector2 startPos2d = Grid.GetGridXY(startPos);
            Vector2 endPos2d = Grid.GetGridXY(endPos);
            int startGridX=Mathf.FloorToInt(startPos2d.x);
            int startGridY=Mathf.FloorToInt(startPos2d.y);
            int endGridX=Mathf.FloorToInt(endPos2d.x);
            int endGridY = Mathf.FloorToInt(endPos2d.y);
            
            //#region
            ////ά����ͼ��
            //mi.mapArray[startGridX, startGridY] = 2;
            //mi.mapArray[endGridX, endGridY] = 2;
            //#endregion

            List<Node> path = FindPath(startGridX, startGridY,endGridX, endGridY);

            if (path == null)
            {
                Debug.Log("δ�ҵ�·��");
                return null;
            }
            else
            {
                List<Vector3> worldPath = new List<Vector3> { };
                foreach (Node node in path)
                {
                    //nodeCenter:·������
                    Vector3 nodeCenter = Grid.GetOriginPosition() + new Vector3(node.gridX, 0, node.gridY) * Grid.GetCellSize() + new Vector3(1, 0, 1) * Grid.GetCellSize() * .5f;
                    

                    //#region
                    ////ά����ͼ��
                    //Vector2 vector2nodeCenter = Grid.GetGridXY(nodeCenter);
                    //int gridXnodeCenter = Mathf.FloorToInt(vector2nodeCenter.x);
                    //int gridYnodeCenter = Mathf.FloorToInt(vector2nodeCenter.y);
                    //Node currentNode=Grid.GetValue(gridXnodeCenter, gridYnodeCenter);
                    //Node startNode = Grid.GetValue(startGridX, startGridY);
                    //Node endNode = Grid.GetValue(endGridX, endGridY);
                    //if (currentNode != startNode&& currentNode != endNode)
                    //{
                    //    mi.mapArray[gridXnodeCenter, gridYnodeCenter] = (int)MapType.wall;
                    //}
                    //#endregion

                    worldPath.Add(nodeCenter);
                }
                return worldPath;
            }
        }

        public List<Node> FindPath(int startGridX, int startGridY, int endGridX, int endGridY)
        {
            Node startNode = Grid.GetValue(startGridX, startGridY);//������ʼ�ڵ㣬��ʼ�ڵ㽫��ΪOpen���еĵ�һ��Ԫ��
            Node endNode = Grid.GetValue(endGridX, endGridY);


            openList = new List<Node> { startNode };
            closeList = new List<Node>();

            //��ʼ�����нڵ㣬��ÿ���ڵ��gCost��Ϊ�����ǰһ�ڵ���Ϊ��ֵ
            for (int gridX = 0; gridX < Grid.GetWidth(); gridX++)
            {
                for (int gridY = 0; gridY < Grid.GetHeight(); gridY++)
                {
                    Node node = Grid.GetValue(gridX, gridY);
                    node.gCost = int.MaxValue;
                    node.Father = null;
                }
            }
            //��ʼ����ʼ�ڵ�
            startNode.gCost = 0;
            startNode.hCost = CaculateDistanceCost(startNode, endNode);

            //��ʼѰ�ҽڵ�
            while (openList.Count > 0)
            {

                //�ӿ�����ѡ��һ��������С����Ϊ��ǰ�ڵ�
                openList.Sort(SortOpenList);//ʹ���Զ���ıȽ�������
                //SortList();//ð������ͬ�ϣ���ѡһ��ʹ��
                //openList=openList.OrderBy(c => c.F).ToList();//ʹ������������Ϊ�Ƚ�������,ͬ�ϣ���ѡһ��ʹ��
                Node currentNode = openList[0];

                if (currentNode == endNode)//��ǰ�ڵ�Ϊ��ֹ�ڵ㣬˵��Ѱ·�ɹ�
                {
                    openList.Remove(currentNode);
                    closeList.Add(currentNode);
                    return CaculatePath(currentNode);//һ·�ҵ������ĵ��ĵ��ĵ����γ�һ������·��
                }
                else//��ǰ�ڵ㲻Ϊ��ֹ�ڵ㣬�����ھӽڵ�
                {
                    openList.Remove(currentNode);
                    closeList.Add(currentNode);
                    foreach (Node neighbourNode in GetNeighbourList(currentNode))
                    {
                        if (closeList.Contains(neighbourNode))//�ھӽڵ�λ�ڱձ���
                        {
                            continue;
                        }
                        if (neighbourNode.IsWall)//�ھӽڵ�λ��ǽ��
                        {
                            closeList.Add(neighbourNode);
                            continue;
                        }

                        float tentativeGCost = currentNode.gCost + CaculateDistanceCost(currentNode, neighbourNode);//ʵ���Ի���

                        if (tentativeGCost < neighbourNode.gCost)//����ھӽڵ��ʵ���Կ����Ƚ�С��˵������ھӽڵ���Ա����ŵ�������Ϊ��ѡ�Ľڵ�ʹ��
                        {
                            neighbourNode.Father = currentNode;
                            neighbourNode.gCost = tentativeGCost;
                            neighbourNode.hCost = CaculateDistanceCost(neighbourNode, endNode);
                            if (!openList.Contains(neighbourNode)&&!IsCloseToWall(neighbourNode))
                            {
                                openList.Add(neighbourNode);
                            }
                        }
                    }
                }
            }//while
            return null;
        }
        /// <summary>
        /// ���õ��Ƿ񿿽��ϰ���
        /// </summary>
        private bool IsCloseToWall(Node _neighbourNode)
        {
            int RefX = _neighbourNode.gridX;
            int RefY = _neighbourNode.gridY;
            for (int i = Mathf.Max(0,RefX-Mathf.CeilToInt(radius)); i <Mathf.Min(mapWidth,RefX+Mathf.CeilToInt(radius)); i++)
            {
                for (int j = Mathf.Max(0,RefY - Mathf.CeilToInt(radius)); j < Mathf.Min(mapHeight,RefY +Mathf.CeilToInt(radius)); j++)
                {
                    if (GetNode(i, j).IsWall) 
                    {
                        //Debug.Log("I'm a wall neighbour.");
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// ��ȡ��ǰ�ڵ���ھӽڵ��б�
        /// </summary>
        /// <param name="_currentNode">��ǰ�ڵ�</param>
        /// <returns></returns>
        private List<Node> GetNeighbourList(Node _currentNode)
        {
            List<Node> neighbourList = new List<Node> { };
            if ((_currentNode.gridX - 1) >= 0)
            {
                neighbourList.Add(GetNode(_currentNode.gridX - 1, _currentNode.gridY));//���ھ�
                if ((_currentNode.gridY - 1) >= 0)
                {
                    neighbourList.Add(GetNode(_currentNode.gridX - 1, _currentNode.gridY - 1));//�����ھ�
                }
                if ((_currentNode.gridY + 1) < Grid.GetHeight())
                {
                    neighbourList.Add(GetNode(_currentNode.gridX - 1, _currentNode.gridY + 1));//�����ھ�
                }
            }
            if ((_currentNode.gridX + 1) < Grid.GetWidth())
            {
                neighbourList.Add(GetNode(_currentNode.gridX + 1, _currentNode.gridY));//���ھ�
                if ((_currentNode.gridY - 1) >= 0)
                {
                    neighbourList.Add(GetNode(_currentNode.gridX + 1, _currentNode.gridY - 1));//�����ھ�
                }
                if ((_currentNode.gridY + 1) < Grid.GetHeight())
                {
                    neighbourList.Add(GetNode(_currentNode.gridX + 1, _currentNode.gridY + 1));//�����ھ�
                }
            }
            if ((_currentNode.gridY - 1) >= 0)
            {
                neighbourList.Add(GetNode(_currentNode.gridX, _currentNode.gridY - 1));//���ھ�
            }
            if ((_currentNode.gridY + 1) < Grid.GetHeight())
            {
                neighbourList.Add(GetNode(_currentNode.gridX, _currentNode.gridY + 1));//���ھ�
            }
            return neighbourList;
        }
        private List<Node> CaculatePath(Node node)
        {
            List<Node> path = new List<Node>();
            path.Add(node);
            Node currentNode = node;
            while (currentNode.Father != null)
            {
                path.Add(currentNode.Father);
                currentNode = currentNode.Father;
            }
            path.Reverse();
            return path;
        }
        private int CaculateDistanceCost(Node a, Node b, int DistanceMetric = 0)//Ĭ��ʹ���۵�����
        {
            int distanceX = Mathf.Abs(a.gridX - b.gridX);
            int distanceY = Mathf.Abs(a.gridY - b.gridY);
            int remaining = Mathf.Abs(distanceX - distanceY);
            switch (DistanceMetric)
            {
                case 0://�����۵�����
                    return MOVE_DIAGONAL_COST * Mathf.Min(distanceX, distanceY) + MOVE_STRAIGHT_COST * remaining;//б�߳ͷ�=14��ֱ�߳ͷ�=10
                case 1://���������پ���
                    return distanceX + distanceY;
                case 2://����ֱ�߾���
                    return (int)Mathf.Sqrt(distanceX ^ 2 + distanceY ^ 2);
                default:
                    throw new SystemException("ָ���Ĳ��������������룬��ָ��0��1��2");
            }

        }
        /// <summary>
        /// �򵥵�ð������
        /// </summary>
        private void SortList()
        {
            for (int i = 0; i < this.openList.Count - 1; i++)
            {
                int lowestIndex = i;
                for (int j = this.openList.Count - 1; j < i; j--)
                {
                    if (this.openList[j].fCost < this.openList[lowestIndex].fCost)
                    {
                        lowestIndex = j;
                    }
                }
                Node tempNode = this.openList[lowestIndex];
                openList[lowestIndex] = this.openList[i];
                this.openList[i] = tempNode;
            }
        }

        /// <summary>
        /// �Զ����Comparer
        /// </summary>
        /// <param name="a">�ڵ�a</param>
        /// <param name="b">�ڵ�b</param>
        /// <returns></returns>
        private int SortOpenList(Node a, Node b)
        {
            if (a.F > b.F)
                return 1;
            else if (a.F == b.F)
                return 0;
            else
                return -1;
        }



        /// <summary>
        /// �ж�ĳ�������Ƿ�Ϊ·
        /// </summary>
        /// <param name="x">��</param>
        /// <param name="y">��</param>
        /// <returns></returns>
        public bool isReachable(int gridX, int gridY)
        {
            //��Խ��ͼ�߽�
            if (gridX < 0 || gridY < 0 || gridX > Grid.GetWidth() || gridY > Grid.GetHeight())
            {
                return false;
            }
            else if (Grid.GetValue(gridX, gridY).IsWall)            //�ж��Ƿ����ϰ���
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool isReachable(Node n)
        {
            //��Խ��ͼ�߽�
            if (n.gridX < 0 || n.gridY < 0 || n.gridX > Grid.GetWidth() || n.gridY > Grid.GetHeight())
            {
                return false;
            }
            else if (n.IsWall)            //�ж��Ƿ����ϰ���
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool isReachable(Vector2 _n)
        {
            Vector2 n = Grid.GetGridXY(_n);
            //��Խ��ͼ�߽�
            if (n.x < 0 || n.y < 0 || n.x > Grid.GetWidth() || n.y > Grid.GetHeight())
            {
                return false;
            }
            else if (Grid.GetValue((int)n.x, (int)n.y).IsWall)            //�ж��Ƿ����ϰ���
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool isReachable(Vector3 _n)
        {
            Vector2 n = Grid.GetGridXY(_n);
            //��Խ��ͼ�߽�
            if (n.x < 0 || n.y < 0 || n.x > Grid.GetWidth() || n.y > Grid.GetHeight())
            {
                return false;
            }
            else if (Grid.GetValue((int)n.x, (int)n.y).IsWall)            //�ж��Ƿ����ϰ���
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public Node GetNodeWithMinF()
        {
            //�ӿ�����ѡ��һ��������С����Ϊ��ǰ�ڵ�
            openList.Sort(SortOpenList);//ʹ���Զ���ıȽ�������
                                        //SortList();//ð������ͬ�ϣ���ѡһ��ʹ��
                                        //openList=openList.OrderBy(c => c.F).ToList();//ʹ������������Ϊ�Ƚ�������,ͬ�ϣ���ѡһ��ʹ��
            Node currentNode = openList[0];
            return currentNode;
        }
    }//AStarManager


}

