using Load;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Chrom <T>
{





    public T[] tankArray;
    public int n;
    public Chrom(List<Enemy> enemies,int n_tanks, Func<Chrom<T>,float,float,int,int,TankType,T> _createTankObject)
    {
        n = n_tanks;
        tankArray = new T[n];
        for (int i = 0; i < n_tanks; i++)
        {
            tankArray[i] = _createTankObject(this, enemies[i].m_tankSize.x, enemies[i].m_tankSize.y, i, i, (TankType)System.Enum.Parse(typeof(TankType), enemies[i].m_tankType));
        }
    }
    public void selfIntro()
    {
        //按面积对tank进行排序
        List<T> tempTanks = tankArray.ToList<T>();

        tempTanks.Sort(SortByArea);

        for (int i = 0; i < n; i++)
        {
            Tank tank = (Tank)Convert.ChangeType(tankArray[i],typeof(Tank));
            tank.ID = i;
            Debug.LogFormat("tank:width:{0},length:{1}", tank.Width, tank.Length);
        }
    }
    /// <summary>
    /// 比较器
    /// </summary>
    /// <param name="_x">tank1</param>
    /// <param name="_y">tank2</param>
    /// <returns>排序需要的参考值</returns>
    private int SortByArea(T _x, T _y)
    {
        Tank x = (Tank)Convert.ChangeType(_x, typeof(Tank));
        Tank y = (Tank)Convert.ChangeType(_x, typeof(Tank));
        if (x.Length * x.Width > y.Length * y.Width)
            return 1;
        else if (x.Length * x.Width == y.Length * y.Width)
            return 0;
        else
            return -1;
    }
}
