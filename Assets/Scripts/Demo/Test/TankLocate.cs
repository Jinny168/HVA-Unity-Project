using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AStar;

namespace Load 
{
    /// <summary>
    /// ̹�˶�λ�㷨
    /// </summary>
    public class TankLocate
    {
        List<OutLine> outLines=new List<OutLine>();
        public TankLocate()
        {
        }
        /// <summary>
        /// ��ʼ���߼�
        /// </summary>
        /// <param name="origin">�߶���ʼ��</param>
        /// <param name="end">�߶���ֹ��</param>
        /// <param name="height">�߶θ߶�</param>
        public void InitLineList(float origin, float end, float height)
        { 
            OutLine outline=new OutLine(origin, end, height);
            outLines.Add(outline);
        }
        /// <summary>
        /// �������ˮƽ��
        /// </summary>
        public void EnhanceLine(int _Index)
        {
            int neighborIndex = 0;
            #region
            if (outLines.Count > 1)
            {
                //��ȡ�߶Ƚϵ͵�����ˮƽ��������������ˮƽ�߼�
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
                    //�������ˮƽ��
                    OutLine leftNeighbor = outLines[_Index - 1];
                    //�ұ�����ˮƽ��
                    OutLine rightNeighbor = outLines[_Index + 1];
                    //ѡ��߶Ƚϵ͵�����ˮƽ�ߣ����Ҹ߶�һ��ʱ��ѡ����ߵ�����ˮƽ��
                    if (leftNeighbor.height < rightNeighbor.height)
                    {
                        neighborIndex = _Index - 1;
                    }
                    else if (leftNeighbor.height > rightNeighbor.height)
                    {
                        neighborIndex = _Index + 1;
                    }
                    //�߶�һ��ʱ
                    else
                    {
                        //ѡ�����ˮƽ��
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
            //ѡ��߶Ƚϵ͵�����ˮƽ��
            OutLine oldLine = outLines[neighborIndex];
            //��������ˮƽ��
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
        /// �����±��ȡ�߶γ���
        /// </summary>
        /// <param name="_index">�����±�</param>
        /// <returns>�߶γ���</returns>
        public float GetLineWidth(int _index)
        {
            OutLine line = outLines[_index];
            return line.end - line.origin;
        }
        /// <summary>
        /// �����±����ˮƽ�߼�
        /// </summary>
        /// <param name="_index">�����±�</param>
        /// <param name="_newLine">��ˮƽ��</param>
        public void UpdateLineList(int _index, OutLine _newLine)
        {
            outLines[_index] = _newLine;
        }
        /// <summary>
        /// �����±����ˮƽ�߼���Ĭ�ϲ��뵽�����ĺ��棩
        /// </summary>
        /// <param name="_index">�����±�</param>
        /// <param name="_newLine">��ˮƽ��</param>
        public void InsertLineList(int _index, OutLine _newLine)
        {
            outLines.Insert(_index, _newLine);
        }

        /// <summary>
        /// �ҳ���͵�ˮƽ�ߣ�������ˮƽ�߲�ֹһ����ѡȡ����ߵ�������
        /// </summary>
        public OutLine FindLowestLine()
        {
            List<OutLine> tempOutLines = outLines;
            List<OutLine> backupOutLines = new List<OutLine>();
            //һ�δ���
            //���߶ȶ�temp�߼���������
            tempOutLines.Sort(SortByHeight);
            //��¼��͸߶�
            float lowestHeight = tempOutLines[0].height;
            //ɸѡ��ӵ����͸߶ȵ���
            foreach (OutLine line in tempOutLines)
            {
                if (line.height == lowestHeight)
                {
                    backupOutLines.Add(line);
                }
            }
            //���δ���
            //����ʼ���backup�߼�������������ѡ��
            if (backupOutLines.Count == 1)//����һ���ߣ�ֱ�ӷ��ظ���
            {
                OutLine outputLine = backupOutLines[0];
                return outputLine;
            }
            else if (backupOutLines.Count > 1)//��ֹһ��
            {
                backupOutLines.Sort(SortByOrigin);
                OutLine outputLine = backupOutLines[0];//origin����󷵻ص�һ����
                return outputLine;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// �Զ���ıȽ���
        /// </summary>
        /// <param name="x">��1</param>
        /// <param name="y">��2</param>
        /// <returns>��������ݣ�˭��˭С</returns>
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
        /// �Զ���ıȽ���
        /// </summary>
        /// <param name="x">��1</param>
        /// <param name="y">��2</param>
        /// <returns>��������ݣ�˭��˭С</returns>
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
        /// ��ȡ���ˮƽ�ߵ�����
        /// </summary>
        /// <param name="_outputLine">���ˮƽ��</param>
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
        /// ���ˮƽ�߼���
        /// </summary>
        public void EmptyLineList()
        {
            outLines.Clear();
        }
        /// <summary>
        /// �������ˮƽ�߸߶ȣ������üװ���󳤶�
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
