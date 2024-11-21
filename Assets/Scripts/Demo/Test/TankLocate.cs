using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AStar;

namespace Load 
{
    /// <summary>
    /// 坦克定位算法
    /// </summary>
    public class TankLocate
    {
        List<OutLine> outLines=new List<OutLine>();
        public TankLocate()
        {
        }
        /// <summary>
        /// 初始化线集
        /// </summary>
        /// <param name="origin">线段起始点</param>
        /// <param name="end">线段终止点</param>
        /// <param name="height">线段高度</param>
        public void InitLineList(float origin, float end, float height)
        { 
            OutLine outline=new OutLine(origin, end, height);
            outLines.Add(outline);
        }
        /// <summary>
        /// 提升最低水平线
        /// </summary>
        public void EnhanceLine(int _Index)
        {
            int neighborIndex = 0;
            #region
            if (outLines.Count > 1)
            {
                //获取高度较低的相邻水平线索引，并更新水平线集
                if (_Index == 0)
                {
                    neighborIndex = 1;
                }
                else if (_Index + 1 == outLines.Count)
                {
                    neighborIndex = _Index - 1;
                }
                else
                {
                    //左边相邻水平线
                    OutLine leftNeighbor = outLines[_Index - 1];
                    //右边相邻水平线
                    OutLine rightNeighbor = outLines[_Index + 1];
                    //选择高度较低的相邻水平线，左右高度一致时，选择左边的相邻水平线
                    if (leftNeighbor.height < rightNeighbor.height)
                    {
                        neighborIndex = _Index - 1;
                    }
                    else if (leftNeighbor.height > rightNeighbor.height)
                    {
                        neighborIndex = _Index + 1;
                    }
                    //高度一致时
                    else
                    {
                        //选择靠左的水平线
                        if (leftNeighbor.origin < rightNeighbor.origin)
                        {
                            neighborIndex = _Index - 1;
                        }
                        else
                        {
                            neighborIndex = _Index + 1;
                        }
                    }

                }

            }//if
            #endregion
            //选择高度较低的相邻水平线
            OutLine oldLine = outLines[neighborIndex];
            //更新相邻水平线
            if (neighborIndex < _Index)
            {
                outLines[neighborIndex] = new OutLine(oldLine.origin, oldLine.end + GetLineWidth(_Index), oldLine.height);
            }
            else
            {
                outLines[neighborIndex] = new OutLine(oldLine.origin - GetLineWidth(_Index), oldLine.end, oldLine.height);
            }
            outLines.RemoveAt(_Index);
        }
        /// <summary>
        /// 根据下标获取线段长度
        /// </summary>
        /// <param name="_index">索引下标</param>
        /// <returns>线段长度</returns>
        public float GetLineWidth(int _index)
        {
            OutLine line = outLines[_index];
            return line.end - line.origin;
        }
        /// <summary>
        /// 根据下标更新水平线集
        /// </summary>
        /// <param name="_index">索引下标</param>
        /// <param name="_newLine">新水平线</param>
        public void UpdateLineList(int _index, OutLine _newLine)
        {
            outLines[_index] = _newLine;
        }
        /// <summary>
        /// 根据下标插入水平线集（默认插入到索引的后面）
        /// </summary>
        /// <param name="_index">索引下标</param>
        /// <param name="_newLine">新水平线</param>
        public void InsertLineList(int _index, OutLine _newLine)
        {
            outLines.Insert(_index, _newLine);
        }

        /// <summary>
        /// 找出最低的水平线（如果最低水平线不止一条则选取最左边的那条）
        /// </summary>
        public OutLine FindLowestLine()
        {
            List<OutLine> tempOutLines = outLines;
            List<OutLine> backupOutLines = new List<OutLine>();
            //一次处理
            //按高度对temp线集进行排序
            tempOutLines.Sort(SortByHeight);
            //记录最低高度
            float lowestHeight = tempOutLines[0].height;
            //筛选出拥有最低高度的线
            foreach (OutLine line in tempOutLines)
            {
                if (line.height == lowestHeight)
                {
                    backupOutLines.Add(line);
                }
            }
            //二次处理
            //按起始点对backup线集进行排序，优中选优
            if (backupOutLines.Count == 1)//仅有一条线，直接返回该线
            {
                OutLine outputLine = backupOutLines[0];
                return outputLine;
            }
            else if (backupOutLines.Count > 1)//不止一条
            {
                backupOutLines.Sort(SortByOrigin);
                OutLine outputLine = backupOutLines[0];//origin排序后返回第一条线
                return outputLine;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 自定义的比较器
        /// </summary>
        /// <param name="x">线1</param>
        /// <param name="y">线2</param>
        /// <returns>排序的依据，谁大谁小</returns>
        private int SortByOrigin(OutLine x, OutLine y)
        {
            if (x.origin > y.origin)
                return 1;
            else if (x.origin == y.origin)
                return 0;
            else
                return -1;
        }
        /// <summary>
        /// 自定义的比较器
        /// </summary>
        /// <param name="x">线1</param>
        /// <param name="y">线2</param>
        /// <returns>排序的依据，谁大谁小</returns>
        private int SortByHeight(OutLine x, OutLine y)
        {

            if (x.height > y.height)
                return 1;
            else if (x.height == y.height)
                return 0;
            else
                return -1;
        }

        /// <summary>
        /// 获取最低水平线的索引
        /// </summary>
        /// <param name="_outputLine">最低水平线</param>
        /// <returns></returns>
        public int GetLowestIndex(OutLine _outputLine)
        {
            for (int i = 0; i < outLines.Count; i++)
            {
                if (_outputLine == outLines[i])
                {
                    return i;
                }
            }
            return -1;
        }
        /// <summary>
        /// 清空水平线集合
        /// </summary>
        public void EmptyLineList()
        {
            outLines.Clear();
        }
        /// <summary>
        /// 计算最高水平线高度，即所用甲板最大长度
        /// </summary>
        /// <returns></returns>
        public float CalHighLine()
        {

            float maxHeight = 0;
            List<OutLine> tempOutLines = outLines;
            tempOutLines.Sort(SortByHeight);
            maxHeight = tempOutLines.Last().height;
            return maxHeight;
        }
    }//class
}
