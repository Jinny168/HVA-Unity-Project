using AStar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AStar 
{
    /// <summary>
    /// ������
    /// </summary>
    public class Node
    {

        private Grid<Node> grid;
        // ���׽ڵ�
        public Node Father { get; set; }

        // F G H ֵ
        public float gCost;//������
        public float hCost;//�յ����
        public float fCost;//Ѱ·����
        public float F { get { return fCost = gCost + hCost; } set { fCost = value; } }
        public float G { get { return gCost; } set { gCost = value; } }
        public float H { get { return hCost; } set { hCost = value; } }

        // �ռ�����ֵ
        public float spaceX { get; set; }
        public float spaceY { get; set; }
        //��������ֵ
        public int gridX;
        public int gridY;

        //��������
        public NodeType nodeType;

        // �Ƿ����ϰ������ǽ��
        public bool IsWall { get; set; }

        // �õ����Ϸ���壨������Ҫ�ɲ��ÿ�ɾ����
        public GameObject gameObject;
        // �õ�Ŀռ�λ�ã�������Ҫ�ɲ��ÿ�ɾ����
        public Vector3 position;

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="x">��������Ϊ�����±꣬��������Ϊ��������</param>
        /// <param name="y">ͬx</param>
        /// <param name="go">��Ϸ����</param>
        /// <param name="parent">���ڵ�</param>
        /// <param name="position">�ռ�λ��</param>
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
        /// �����ϰ���
        /// </summary>
        /// <param name="_isWall">�Ƿ�Ϊǽ��</param>
        public void SetIsWall(bool _isWall)
        {
            this.IsWall = _isWall;
            grid.TriggerGridObjectChanged(gridX, gridY);
        }

        /// <summary>
        /// ����G��F ֵ���͸��׽ڵ�
        /// </summary>
        /// <param name="father">���ڵ�</param>
        /// <param name="g">������</param>
        public void UpdateFather(Node father, float g)
        {
            Father = father;
            G = g;
            F = G + H;
        }
        /// <summary>
        /// �ж��Ƿ���ĳ���ڵ�����ͬ�ڵ�
        /// </summary>
        /// <param name="p">�ڵ�p</param>
        /// <returns>��ͬ����true</returns>
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
