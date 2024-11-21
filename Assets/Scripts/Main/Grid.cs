using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AStar 
{

    /// <summary>
    /// ��ͼ��
    /// </summary>
    /// <typeparam name="T">����</typeparam>
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
        public T[,] gridArray;//����һ����ά���������洢�����ÿһ���ڵ㣬��СΪ���񳤶ȳ���������
        public float cellSize;
        private Vector3 originPosition;

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="_mapWidth">���ӿ�ռ���ٸ�</param>
        /// <param name="_mapHeight">���Ӹ�ռ���ٸ�</param>
        /// <param name="_cellSize">���θ��ӵı߳�</param>
        /// <param name="_originPosition">����ԭ�㣬Ĭ�ϣ�0��0��0��</param>
        /// <param name="_createGridObject">���ڴ���ÿ�����Ӷ����ί�к��������ί�к�������һ��Grid<T>���͵Ĳ����������������������ӵ�����������������������һ����������T�Ķ���</param>
        public Grid(int _mapWidth, int _mapHeight, float _cellSize,int[,] _mapArray, Vector3 _originPosition, Func<Grid<T>, int, int, NodeType, T> _createGridObject)
        {
            this.mapWidth = _mapWidth;//��ʾ���ӵ�����
            this.mapHeight = _mapHeight;//��ʾ���ӵ�����
            this.cellSize = _cellSize;//ÿ�����ӵĴ�С
            this.mapArray = _mapArray;//ÿ�����ӵ�����
            this.originPosition = _originPosition;//��ͼ��ԭ��λ�ã�ͨ�������½ǵ�λ��
            gridArray = new T[this.mapWidth, this.mapHeight];//������������
            for (int gridX = 0; gridX < mapWidth; gridX++)
            {
                for (int gridY = 0; gridY < mapHeight; gridY++)
                {
                    //NodeType type = UnityEngine.Random.Range(0, 100) < 20 ? NodeType.wall : NodeType.walk;//������ø�������

                    NodeType type = (NodeType)mapArray[gridX,gridY];

                    //Debug.Log(type.ToString());//���·������

                    gridArray[gridX, gridY] = _createGridObject(this, gridX, gridY, type);//��ά����洢ÿ�����ӵ���Ϣ
                                                                                          //����ÿ�����ӵ��±ߺ����
                    Debug.DrawLine(GetWorldPosition(gridX, gridY), GetWorldPosition(gridX, gridY + 1));
                    Debug.DrawLine(GetWorldPosition(gridX, gridY), GetWorldPosition(gridX + 1, gridY));
                }
            }
            //����������ͼ���ұߺ��ϱߣ��γ�һ������������
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
        /// ��ȡ��������ϵ����Ϊ����������ϵʹ����X���Z�ᣬ��Ҫ�ʵ�����
        /// </summary>
        /// <param name="gridX">������б�</param>
        /// <param name="gridY">������б�</param>
        /// <returns></returns>
        private Vector3 GetWorldPosition(int gridX, int gridY)
        {
            return new Vector3(gridX, 0, gridY) * cellSize + originPosition;//������������ϵ�µ�����
        }
        /// <summary>
        /// ������������ϵ�µ������ȡ��������ϵ�µ�����������
        /// </summary>
        /// <param name="_worldPosition">��������ϵ�µ�����</param>
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
        /// ��֪�����������������ֵ
        /// </summary>
        /// <param name="gridX">�������ڵ���</param>
        /// <param name="gridY">�������ڵ���</param>
        /// <param name="value">�����õĽڵ�ֵ</param>
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
        /// ��֪�����������������ֵ
        /// </summary>
        /// <param name="_worldPosition">��������</param>
        /// <param name="value">�����õ�ֵ</param>
        public void SetValue(Vector3 _worldPosition, T value)
        {
            int x, y;
            x = Mathf.FloorToInt(GetGridXY(_worldPosition).x);
            y = Mathf.FloorToInt(GetGridXY(_worldPosition).y);
            SetValue(x, y, value);
        }
        /// <summary>
        /// ��ȡ���������ֵ
        /// </summary>
        /// <param name="gridX">�������ڵ���</param>
        /// <param name="gridY">�������ڵ���</param>
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
        /// ��ȡ���������ֵ
        /// </summary>
        /// <param name="_worldPosition">��������</param>
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



