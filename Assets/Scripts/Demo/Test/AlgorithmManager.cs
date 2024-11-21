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
    //遗传算法
    public class AlgorithmManager:MonoBehaviour
    {
        
        private List<Enemy> enemies;
        private int n_enemies;

        private LoadManager loadManager;
        //优化代数
        private int NIND = 0;       //种群大小

        private int gen = 0;        //当前迭代次数

        private int MAXGEN = 100;   //最大迭代次数

        private float Pc = 0.9f;    //交叉概率

        private float Pm = 0.3f;    //变异概率

        private float ggap = 0.9f;  //代沟

        //最优个体
        public Enemy bestIndividual;

        //最优个体列表
        public Enemy[] bestIndividualList;

        //最优适应度
        public float bestObj;

        //最优适应度列表
        public float[] bestObjList;

        //筛选子群
        public Enemy[] SelCh;

        public Enemy[,] iChrom;
        //编码
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
             * 测试m_chrom内容
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


                Debug.Log("我是分割线--------");
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
                throw new ArgumentException("源数组的长度超过了目标数组的宽度。");
            }

            for (int i = 0; i < sourceLength; i++)
            {
                destinationArray[rowIndex, i] = sourceArray[i];
            }
        }
        /// <summary>
        /// 迭代
        /// </summary>
        public void Iter()
        {
            //iChrom = InitPop();                     //初始化种群
            while (gen <= MAXGEN)
            {
                float[] fit=CalFit();               //返回适应度数组
                SelCh = Select();                   //选择操作
                SelCh = Crossover();                //交叉操作
                SelCh = Mutate();                   //变异操作
                iChrom = Reins();                   //重插入子代的新种群
                iChrom = AdjustChrom();             //将种群中不满足数量约束的个体进行约束处理
                Enemy cur_bestIndividual=CalBest(); //计算当前迭代中最优个体
                float cur_bestObj = cur_bestIndividual.m_obj;//当前迭代中最优适应度
                if (cur_bestObj >= bestObj) 
                {
                    bestObj = cur_bestObj;
                    bestIndividual = cur_bestIndividual;
                }
                bestObjList[gen] = bestObj;//记录每次迭代过程中的最优目标函数值
                Debug.LogFormat("第{0}次迭代的全局最优解为：", bestObj);
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
        //    //改为由布列管理器处获取信息
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
        /// 随机排列数组元素
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
