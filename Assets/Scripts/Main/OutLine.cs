using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AStar
{
    /// <summary>
    /// ˮƽ����
    /// </summary>
    public class OutLine
    {
        public float origin;//��ʼλ��
        public float end;//��ֹλ��
        public float height;//�߶�

        public OutLine(float v1, float v2, float v3)
        {
            this.origin = v1;
            this.end = v2;
            this.height = v3;
        }
        /// <summary>
        /// ��ӡ���ˮƽ����Ϣ
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
