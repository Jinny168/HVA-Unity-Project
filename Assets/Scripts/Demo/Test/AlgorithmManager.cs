using AStar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Load.LoadManager;
namespace Load 
{
    //�Ŵ��㷨
    public class AlgorithmManager:MonoBehaviour
    {
        
        private List<Enemy> enemies;
        private int n_enemies;

        private LoadManager loadManager;
        //�Ż�����
        private int NIND = 0;       //��Ⱥ��С

        private int gen = 0;        //��ǰ��������

        private int MAXGEN = 100;   //����������

        private float Pc = 0.9f;    //�������

        private float Pm = 0.3f;    //�������

        private float ggap = 0.9f;  //����

        //���Ÿ���
        public Enemy bestIndividual;

        //���Ÿ����б�
        public Enemy[] bestIndividualList;

        //������Ӧ��
        public float bestObj;

        //������Ӧ���б�
        public float[] bestObjList;

        //ɸѡ��Ⱥ
        public Enemy[] SelCh;

        public Enemy[,] iChrom;
        //����
        private int[] m_individual;
        private int[,] m_chrom;

        private void Start()
        {
            n_enemies = 10;
            NIND = 10;
            m_chrom = new int[NIND, n_enemies];
            m_individual = Enumerable.Range(1, n_enemies).ToArray();
            for (int i = 0; i < NIND; i++)
            {
                Shuffle(m_individual);
                FillRow(m_individual, m_chrom, i);
            }

            /*
             * ����m_chrom����
            for (int i = 0; i < m_chrom.GetLength(0); i++)
            {
                for (int j = 0; j < m_chrom.GetLength(1); j++)
                {
                    Debug.Log(m_chrom[i, j]);
                }
            }
            */

            List<Cnew> myList=new();
            List<List<Cnew>> cnews = new();
            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i < 5; i++)
                {
                    Cnew cnew = new Cnew(i);
                    myList.Add(cnew);
                }
                Shuffle(myList);

                cnews.Add(myList);
            }

            for (int j = 0; j < 3; j++)
            {
                List<Cnew> tmpList = cnews[j];
                
                for (int i = 0; i < 5; i++)
                {

                    Debug.Log(tmpList[i].num);
                }


                Debug.Log("���Ƿָ���--------");
            }


        }


        public class Cnew
        {
            public int num = 0;
            public Cnew(int _num)
            {
                this.num = _num;
            }
        }

        public class CCnew
        {


        }

        public static void FillRow<T>(T[] sourceArray, T[,] destinationArray, int rowIndex)
        {
            int sourceLength = sourceArray.Length;
            int destinationWidth = destinationArray.GetLength(1);

            if (sourceLength > destinationWidth)
            {
                throw new ArgumentException("Դ����ĳ��ȳ�����Ŀ������Ŀ�ȡ�");
            }

            for (int i = 0; i < sourceLength; i++)
            {
                destinationArray[rowIndex, i] = sourceArray[i];
            }
        }
        /// <summary>
        /// ����
        /// </summary>
        public void Iter()
        {
            //iChrom = InitPop();                     //��ʼ����Ⱥ
            while (gen <= MAXGEN)
            {
                float[] fit=CalFit();               //������Ӧ������
                SelCh = Select();                   //ѡ�����
                SelCh = Crossover();                //�������
                SelCh = Mutate();                   //�������
                iChrom = Reins();                   //�ز����Ӵ�������Ⱥ
                iChrom = AdjustChrom();             //����Ⱥ�в���������Լ���ĸ������Լ������
                Enemy cur_bestIndividual=CalBest(); //���㵱ǰ���������Ÿ���
                float cur_bestObj = cur_bestIndividual.m_obj;//��ǰ������������Ӧ��
                if (cur_bestObj >= bestObj) 
                {
                    bestObj = cur_bestObj;
                    bestIndividual = cur_bestIndividual;
                }
                bestObjList[gen] = bestObj;//��¼ÿ�ε��������е�����Ŀ�꺯��ֵ
                Debug.LogFormat("��{0}�ε�����ȫ�����Ž�Ϊ��", bestObj);
                gen++;
            }
            PlotObj();
        }

        private void PlotObj()
        {
            throw new NotImplementedException();
        }

        private Enemy CalBest()
        {
            throw new NotImplementedException();
        }

        private Enemy[,] AdjustChrom()
        {
            throw new NotImplementedException();
        }

        private Enemy[,] Reins()
        {
            throw new NotImplementedException();
        }

        private Enemy[] Mutate()
        {
            throw new NotImplementedException();
        }

        private Enemy[] Crossover()
        {
            throw new NotImplementedException();
        }

        private Enemy[] Select()
        {
            throw new NotImplementedException();
        }

        //private Enemy[,] InitPop()
        //{
        //    //��Ϊ�ɲ��й���������ȡ��Ϣ
        //    //enemies=new List<Enemy>();
        //    //enemies.

        //    //return enemies.ToArray();
        //}

        private float[] CalFit()
        {
            
            float[] fit = Locate();
            return fit;
        }

        private float[] Locate()
        {
            throw new NotImplementedException();
        }


        public static void Shuffle<T>(T[] array)
        {
            System.Random random = new System.Random((int)DateTime.Now.Ticks);
            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                T temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }
        }

        /// <summary>
        /// �����������Ԫ��
        /// </summary>
        /// <param name="myList"></param>
        /// <returns></returns>
        public static  List<T> Shuffle<T>(List<T> myList)where T:Cnew
        {
            System.Random ran = new System.Random();
            List<T> newList = new List<T>();
            int index;
            T temp ;
            for (int i = 0; i < myList.Count; i++)
            {

                index = ran.Next(0, myList.Count - 1);
                if (index != i)
                {
                    temp = myList[i];
                    myList[i] = myList[index];
                    myList[index] = temp;
                }
            }
            return myList;
        }




    }
}
