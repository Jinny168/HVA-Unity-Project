using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AStar 
{

    /// <summary>
    /// 地图类
    /// </summary>
    /// <typeparam name="T">泛型</typeparam>
    public class Grid<T>
    {
        public event EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;
        public class OnGridValueChangedEventArgs : EventArgs
        {
            public int gridx;
            public int gridy;
        }
        public int mapWidth;
        public int mapHeight;
        public int[,] mapArray;
        public T[,] gridArray;//创建一个二维数组用来存储网格的每一个节点，大小为网格长度乘以网格宽度
        public float cellSize;
        private Vector3 originPosition;

        /// <summary>
        /// 创建网格
        /// </summary>
        /// <param name="_mapWidth">格子宽占多少格</param>
        /// <param name="_mapHeight">格子高占多少格</param>
        /// <param name="_cellSize">方形格子的边长</param>
        /// <param name="_originPosition">坐标原点，默认（0，0，0）</param>
        /// <param name="_createGridObject">用于创建每个格子对象的委托函数。这个委托函数接受一个Grid<T>类型的参数和两个整数参数（格子的行索引和列索引），返回一个泛型类型T的对象。</param>
        public Grid(int _mapWidth, int _mapHeight, float _cellSize,int[,] _mapArray, Vector3 _originPosition, Func<Grid<T>, int, int, NodeType, T> _createGridObject)
        {
            this.mapWidth = _mapWidth;//表示格子的列数
            this.mapHeight = _mapHeight;//表示格子的行数
            this.cellSize = _cellSize;//每个格子的大小
            this.mapArray = _mapArray;//每个格子的类型
            this.originPosition = _originPosition;//地图的原点位置，通常是左下角的位置
            gridArray = new T[this.mapWidth, this.mapHeight];//构造网格数组
            for (int gridX = 0; gridX < mapWidth; gridX++)
            {
                for (int gridY = 0; gridY < mapHeight; gridY++)
                {
                    //NodeType type = UnityEngine.Random.Range(0, 100) < 20 ? NodeType.wall : NodeType.walk;//随机设置格子类型

                    NodeType type = (NodeType)mapArray[gridX,gridY];

                    //Debug.Log(type.ToString());//输出路点类型

                    gridArray[gridX, gridY] = _createGridObject(this, gridX, gridY, type);//二维数组存储每个格子的信息
                                                                                          //绘制每个格子的下边和左边
                    Debug.DrawLine(GetWorldPosition(gridX, gridY), GetWorldPosition(gridX, gridY + 1));
                    Debug.DrawLine(GetWorldPosition(gridX, gridY), GetWorldPosition(gridX + 1, gridY));
                }
            }
            //绘制整个地图的右边和上边，形成一个完整的网格
            Debug.DrawLine(GetWorldPosition(mapWidth, 0), GetWorldPosition(mapWidth, mapHeight));
            Debug.DrawLine(GetWorldPosition(0, mapHeight), GetWorldPosition(mapWidth, mapHeight));
        }
        public int GetWidth()
        {
            return this.mapWidth;
        }
        public int GetHeight()
        {
            return this.mapHeight;
        }
        public float GetCellSize()
        {
            return this.cellSize;
        }
        public T[,] GetGridArray()
        {
            return this.gridArray;
        }
        public Vector3 GetOriginPosition()
        {
            return this.originPosition;
        }
        /// <summary>
        /// 获取世界坐标系，因为建立的坐标系使用了X轴和Z轴，需要适当调整
        /// </summary>
        /// <param name="gridX">网格的列标</param>
        /// <param name="gridY">网格的行标</param>
        /// <returns></returns>
        private Vector3 GetWorldPosition(int gridX, int gridY)
        {
            return new Vector3(gridX, 0, gridY) * cellSize + originPosition;//返回世界坐标系下的坐标
        }
        /// <summary>
        /// 根据世界坐标系下的坐标获取网格坐标系下的列数和行数
        /// </summary>
        /// <param name="_worldPosition">世界坐标系下的坐标</param>
        /// <returns></returns>
        public Vector2 GetGridXY(Vector3 _worldPosition)
        {

            int gridX = Mathf.FloorToInt((_worldPosition.x - originPosition.x) / cellSize);
            int gridY = Mathf.FloorToInt((_worldPosition.z - originPosition.z) / cellSize);
            return new Vector2(gridX, gridY);
        }
        public Vector2 GetGridXY(Vector2 _worldPosition)
        {
            int gridX = Mathf.FloorToInt((_worldPosition.x - originPosition.x) / cellSize);
            int gridY = Mathf.FloorToInt((_worldPosition.y - originPosition.y) / cellSize);
            return new Vector2(gridX, gridY);
        }
        /// <summary>
        /// 已知网格坐标设置网格的值
        /// </summary>
        /// <param name="gridX">方格所在的列</param>
        /// <param name="gridY">方格所在的行</param>
        /// <param name="value">待设置的节点值</param>
        public void SetValue(int gridX, int gridY, T value)
        {
            if (gridX >= 0 && gridY >= 0 && gridX < mapWidth && gridY < mapHeight)
            {
                gridArray[gridX, gridY] = value;
                OnGridValueChanged?.Invoke(this, new OnGridValueChangedEventArgs { gridx = gridX, gridy = gridY });
            }
        }
        public void TriggerGridObjectChanged(int gridX, int gridY)
        {
            OnGridValueChanged?.Invoke(this, new OnGridValueChangedEventArgs { gridx = gridX, gridy = gridY });
        }
        /// <summary>
        /// 已知世界坐标设置网格的值
        /// </summary>
        /// <param name="_worldPosition">世界坐标</param>
        /// <param name="value">待设置的值</param>
        public void SetValue(Vector3 _worldPosition, T value)
        {
            int x, y;
            x = Mathf.FloorToInt(GetGridXY(_worldPosition).x);
            y = Mathf.FloorToInt(GetGridXY(_worldPosition).y);
            SetValue(x, y, value);
        }
        /// <summary>
        /// 获取所在网格的值
        /// </summary>
        /// <param name="gridX">网格所在的列</param>
        /// <param name="gridY">网格所在的行</param>
        /// <returns></returns>
        public T GetValue(int gridX, int gridY)
        {
            if (gridX >= 0 && gridY >= 0 && gridX < mapWidth && gridY < mapHeight)
            {
                return gridArray[gridX, gridY];
            }
            else
            {
                return default;
            }
        }
        /// <summary>
        /// 获取所在网格的值
        /// </summary>
        /// <param name="_worldPosition">世界坐标</param>
        /// <returns></returns>
        public T GetValue(Vector3 _worldPosition)
        {
            int x, y;
            x = Mathf.FloorToInt(GetGridXY(_worldPosition).x);
            y = Mathf.FloorToInt(GetGridXY(_worldPosition).y);
            return GetValue(x, y);
        }
    }//Grid

}



