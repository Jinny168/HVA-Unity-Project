using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AStar
{
    /// <summary>
    /// 格子枚举类
    /// </summary>
    public enum NodeType
    {
        walk,//可供行走的格子
        wall, //不可行走的障碍物
            point//起点与终点
    }//NodeType
}


