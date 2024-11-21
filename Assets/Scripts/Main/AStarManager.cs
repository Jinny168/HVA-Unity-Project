using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AStar 
{
    /// <summary>
    /// A星封装
    /// </summary>
    public class AStarManager
    {
        public static AStarManager Instance;


        //#region
        ////二维数组
        //public Map mi=Map.Instance;//0路1墙2点，维护这个表格，方便AStarGUI绘图
        //#endregion


        //直线移动和对角移动的代价
        private const int MOVE_STRAIGHT_COST = 10;
        private const int MOVE_DIAGONAL_COST = 14;
        public Grid<Node> Grid { get; set; }
        public Node GetNode(int gridX, int gridY)
        {
            return Grid.GetValue(gridX, gridY);
        }
        //开启列表
        internal List<Node> openList = new List<Node>();
        //关闭列表
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
                    //读取tileObject界面设置的关键信息
                    TileObject tileObject = GameObject.FindGameObjectWithTag("GridObject").GetComponent<TileObject>();
                    mapWidth = tileObject.xTileCount;
                    mapHeight = tileObject.zTileCount;
                    gridSize = tileObject.tileSize;
                    Vector3 originalPos = tileObject.originalPos;
                    int[,] mapArray = tileObject.getMapArray();

                    Debug.LogFormat("地图宽度为{0},地图高度为{1},地图单元格尺寸为{2}",mapWidth,mapHeight,gridSize);
                    //tileObject.displayData(mapArray);

                    Grid = new Grid<Node>(mapWidth, mapHeight, gridSize,mapArray,originalPos,
                        (Grid<Node> g, int gridx, int gridy, NodeType type) => new Node(g, gridx, gridy, type));

                    Enemy enemy = GameObject.FindObjectOfType<Enemy>();
                    radius = enemy.m_radius;
                    //Grid = new Grid<Node>((int)AstarGUI.instance.mapSize.x, 
                    //    (int)AstarGUI.instance.mapSize.y, (int)AstarGUI.instance.cellSize, AstarGUI.instance.originalPos,
                    //    (Grid<Node> g, int gridx, int gridy, NodeType type) => new Node(g, gridx, gridy, type));
                    //(Grid<Node> g, int x, int y) => new Node(g, x, y)是一个匿名委托函数，
                    //它接受一个Grid<Node>类型的参数g，以及两个整数参数x和y，并返回一个新的Node对象。
                    //这个委托函数用于创建每个格子对象。
                    break;
            }
        }
        /// <summary>
        /// 寻路的方法
        /// </summary>
        /// <param name="startPos">起点坐标</param>
        /// <param name="endPos">终点坐标</param>
        /// <returns></returns>
        public List<Vector3> FindPath(Vector3 startPos, Vector3 endPos)
        {
            if (!isReachable(startPos) || !isReachable(endPos))
            {
                Debug.LogError("起点和终点不能设在障碍块上");
                return null;
            }

            //初始点和终点的网格坐标
            Vector2 startPos2d = Grid.GetGridXY(startPos);
            Vector2 endPos2d = Grid.GetGridXY(endPos);
            int startGridX=Mathf.FloorToInt(startPos2d.x);
            int startGridY=Mathf.FloorToInt(startPos2d.y);
            int endGridX=Mathf.FloorToInt(endPos2d.x);
            int endGridY = Mathf.FloorToInt(endPos2d.y);
            
            //#region
            ////维护地图表
            //mi.mapArray[startGridX, startGridY] = 2;
            //mi.mapArray[endGridX, endGridY] = 2;
            //#endregion

            List<Node> path = FindPath(startGridX, startGridY,endGridX, endGridY);

            if (path == null)
            {
                Debug.Log("未找到路径");
                return null;
            }
            else
            {
                List<Vector3> worldPath = new List<Vector3> { };
                foreach (Node node in path)
                {
                    //nodeCenter:路点中心
                    Vector3 nodeCenter = Grid.GetOriginPosition() + new Vector3(node.gridX, 0, node.gridY) * Grid.GetCellSize() + new Vector3(1, 0, 1) * Grid.GetCellSize() * .5f;
                    

                    //#region
                    ////维护地图表
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
            Node startNode = Grid.GetValue(startGridX, startGridY);//定义起始节点，起始节点将作为Open表中的第一个元素
            Node endNode = Grid.GetValue(endGridX, endGridY);


            openList = new List<Node> { startNode };
            closeList = new List<Node>();

            //初始化所有节点，让每个节点的gCost设为无穷大，前一节点设为空值
            for (int gridX = 0; gridX < Grid.GetWidth(); gridX++)
            {
                for (int gridY = 0; gridY < Grid.GetHeight(); gridY++)
                {
                    Node node = Grid.GetValue(gridX, gridY);
                    node.gCost = int.MaxValue;
                    node.Father = null;
                }
            }
            //初始化起始节点
            startNode.gCost = 0;
            startNode.hCost = CaculateDistanceCost(startNode, endNode);

            //开始寻找节点
            while (openList.Count > 0)
            {

                //从开表中选出一个开销最小的作为当前节点
                openList.Sort(SortOpenList);//使用自定义的比较器排序
                //SortList();//冒泡排序，同上，任选一种使用
                //openList=openList.OrderBy(c => c.F).ToList();//使用匿名函数作为比较器排序,同上，任选一种使用
                Node currentNode = openList[0];

                if (currentNode == endNode)//当前节点为终止节点，说明寻路成功
                {
                    openList.Remove(currentNode);
                    closeList.Add(currentNode);
                    return CaculatePath(currentNode);//一路找爹，爹的爹的爹的爹，形成一条完整路径
                }
                else//当前节点不为终止节点，查找邻居节点
                {
                    openList.Remove(currentNode);
                    closeList.Add(currentNode);
                    foreach (Node neighbourNode in GetNeighbourList(currentNode))
                    {
                        if (closeList.Contains(neighbourNode))//邻居节点位于闭表中
                        {
                            continue;
                        }
                        if (neighbourNode.IsWall)//邻居节点位于墙体
                        {
                            closeList.Add(neighbourNode);
                            continue;
                        }

                        float tentativeGCost = currentNode.gCost + CaculateDistanceCost(currentNode, neighbourNode);//实验性花销

                        if (tentativeGCost < neighbourNode.gCost)//如果邻居节点的实验性开销比较小，说明这个邻居节点可以被安排到开表，作为待选的节点使用
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
        /// 检查该点是否靠近障碍物
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
        /// 获取当前节点的邻居节点列表
        /// </summary>
        /// <param name="_currentNode">当前节点</param>
        /// <returns></returns>
        private List<Node> GetNeighbourList(Node _currentNode)
        {
            List<Node> neighbourList = new List<Node> { };
            if ((_currentNode.gridX - 1) >= 0)
            {
                neighbourList.Add(GetNode(_currentNode.gridX - 1, _currentNode.gridY));//左邻居
                if ((_currentNode.gridY - 1) >= 0)
                {
                    neighbourList.Add(GetNode(_currentNode.gridX - 1, _currentNode.gridY - 1));//左下邻居
                }
                if ((_currentNode.gridY + 1) < Grid.GetHeight())
                {
                    neighbourList.Add(GetNode(_currentNode.gridX - 1, _currentNode.gridY + 1));//左上邻居
                }
            }
            if ((_currentNode.gridX + 1) < Grid.GetWidth())
            {
                neighbourList.Add(GetNode(_currentNode.gridX + 1, _currentNode.gridY));//右邻居
                if ((_currentNode.gridY - 1) >= 0)
                {
                    neighbourList.Add(GetNode(_currentNode.gridX + 1, _currentNode.gridY - 1));//右下邻居
                }
                if ((_currentNode.gridY + 1) < Grid.GetHeight())
                {
                    neighbourList.Add(GetNode(_currentNode.gridX + 1, _currentNode.gridY + 1));//右上邻居
                }
            }
            if ((_currentNode.gridY - 1) >= 0)
            {
                neighbourList.Add(GetNode(_currentNode.gridX, _currentNode.gridY - 1));//下邻居
            }
            if ((_currentNode.gridY + 1) < Grid.GetHeight())
            {
                neighbourList.Add(GetNode(_currentNode.gridX, _currentNode.gridY + 1));//上邻居
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
        private int CaculateDistanceCost(Node a, Node b, int DistanceMetric = 0)//默认使用折叠距离
        {
            int distanceX = Mathf.Abs(a.gridX - b.gridX);
            int distanceY = Mathf.Abs(a.gridY - b.gridY);
            int remaining = Mathf.Abs(distanceX - distanceY);
            switch (DistanceMetric)
            {
                case 0://计算折叠距离
                    return MOVE_DIAGONAL_COST * Mathf.Min(distanceX, distanceY) + MOVE_STRAIGHT_COST * remaining;//斜线惩罚=14，直线惩罚=10
                case 1://计算曼哈顿距离
                    return distanceX + distanceY;
                case 2://计算直线距离
                    return (int)Mathf.Sqrt(distanceX ^ 2 + distanceY ^ 2);
                default:
                    throw new SystemException("指定的参数度量超过范畴，可指定0、1、2");
            }

        }
        /// <summary>
        /// 简单的冒泡排序
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
        /// 自定义的Comparer
        /// </summary>
        /// <param name="a">节点a</param>
        /// <param name="b">节点b</param>
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
        /// 判断某个格子是否为路
        /// </summary>
        /// <param name="x">行</param>
        /// <param name="y">列</param>
        /// <returns></returns>
        public bool isReachable(int gridX, int gridY)
        {
            //超越地图边界
            if (gridX < 0 || gridY < 0 || gridX > Grid.GetWidth() || gridY > Grid.GetHeight())
            {
                return false;
            }
            else if (Grid.GetValue(gridX, gridY).IsWall)            //判断是否是障碍物
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
            //超越地图边界
            if (n.gridX < 0 || n.gridY < 0 || n.gridX > Grid.GetWidth() || n.gridY > Grid.GetHeight())
            {
                return false;
            }
            else if (n.IsWall)            //判断是否是障碍物
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
            //超越地图边界
            if (n.x < 0 || n.y < 0 || n.x > Grid.GetWidth() || n.y > Grid.GetHeight())
            {
                return false;
            }
            else if (Grid.GetValue((int)n.x, (int)n.y).IsWall)            //判断是否是障碍物
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
            //超越地图边界
            if (n.x < 0 || n.y < 0 || n.x > Grid.GetWidth() || n.y > Grid.GetHeight())
            {
                return false;
            }
            else if (Grid.GetValue((int)n.x, (int)n.y).IsWall)            //判断是否是障碍物
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
            //从开表中选出一个开销最小的作为当前节点
            openList.Sort(SortOpenList);//使用自定义的比较器排序
                                        //SortList();//冒泡排序，同上，任选一种使用
                                        //openList=openList.OrderBy(c => c.F).ToList();//使用匿名函数作为比较器排序,同上，任选一种使用
            Node currentNode = openList[0];
            return currentNode;
        }
    }//AStarManager


}

