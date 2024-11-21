using AStar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AStar 
{
    /// <summary>
    /// 格子类
    /// </summary>
    public class Node
    {

        private Grid<Node> grid;
        // 父亲节点
        public Node Father { get; set; }

        // F G H 值
        public float gCost;//起点距离
        public float hCost;//终点距离
        public float fCost;//寻路距离
        public float F { get { return fCost = gCost + hCost; } set { fCost = value; } }
        public float G { get { return gCost; } set { gCost = value; } }
        public float H { get { return hCost; } set { hCost = value; } }

        // 空间坐标值
        public float spaceX { get; set; }
        public float spaceY { get; set; }
        //网格坐标值
        public int gridX;
        public int gridY;

        //格子类型
        public NodeType nodeType;

        // 是否是障碍物（例如墙）
        public bool IsWall { get; set; }

        // 该点的游戏物体（根据需要可不用可删除）
        public GameObject gameObject;
        // 该点的空间位置（根据需要可不用可删除）
        public Vector3 position;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="x">整型输入为数组下标，流型输入为世界坐标</param>
        /// <param name="y">同x</param>
        /// <param name="go">游戏物体</param>
        /// <param name="parent">父节点</param>
        /// <param name="position">空间位置</param>
        public Node(Grid<Node> _grid, float x, float y, NodeType type, GameObject go = null, Node father = null, Vector3 position = default)
        {
            grid = _grid;
            spaceX = x;
            spaceY = y;
            gameObject = go;
            this.position = position;
            Father = father;
            nodeType = type;
            if (nodeType == NodeType.walk)
                IsWall = false;
            else
                IsWall = true;
        }
        public Node(Grid<Node> _grid, int gridx, int gridy, NodeType type, GameObject go = null, Node father = null, Vector3 position = default)
        {
            grid = _grid;
            gridX = gridx;
            gridY = gridy;
            gameObject = go;
            this.position = position;
            Father = father;
            nodeType = type;
            if (nodeType == NodeType.walk)
                IsWall = false;
            else
                IsWall = true;
        }
        public Node(Grid<Node> _grid, int gridx, int gridy, NodeType type)
        {
            this.grid = _grid;
            this.gridX = gridx;
            this.gridY = gridy;
            nodeType = type;
            if (nodeType == NodeType.walk)
                IsWall = false;
            else
                IsWall = true;
        }
        public Node(Grid<Node> _grid, float spacex, float spacey, NodeType type)
        {
            this.grid = _grid;
            this.spaceX = spacex;
            this.spaceY = spacey;
            nodeType = type;
            if (nodeType == NodeType.walk)
                IsWall = false;
            else
                IsWall = true;
        }

        /// <summary>
        /// 设置障碍物
        /// </summary>
        /// <param name="_isWall">是否为墙体</param>
        public void SetIsWall(bool _isWall)
        {
            this.IsWall = _isWall;
            grid.TriggerGridObjectChanged(gridX, gridY);
        }

        /// <summary>
        /// 更新G，F 值，和父亲节点
        /// </summary>
        /// <param name="father">父节点</param>
        /// <param name="g">起点距离</param>
        public void UpdateFather(Node father, float g)
        {
            Father = father;
            G = g;
            F = G + H;
        }
        /// <summary>
        /// 判断是否与某个节点是相同节点
        /// </summary>
        /// <param name="p">节点p</param>
        /// <returns>相同返回true</returns>
        public bool Equal(Node p)
        {
            if (p.gridX == this.gridX && p.gridY == this.gridY)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }//Node

}
