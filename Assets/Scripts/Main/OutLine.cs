using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AStar
{
    /// <summary>
    /// 水平线类
    /// </summary>
    public class OutLine
    {
        public float origin;//起始位置
        public float end;//终止位置
        public float height;//高度

        public OutLine(float v1, float v2, float v3)
        {
            this.origin = v1;
            this.end = v2;
            this.height = v3;
        }
        /// <summary>
        /// 打印最低水平线信息
        /// </summary>
        public void SelfIntro()
        {
            Debug.LogFormat("OutLine:origin:{0},end:{1},height:{2}", origin, end, height);
        }
        public bool IsEqual(OutLine other)
        {
            return (this.origin == other.origin) && (this.end == other.end) && (this.height == other.height);  
        }
    }
}
