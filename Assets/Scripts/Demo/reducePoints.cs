using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class reducePoints : MonoBehaviour
{
    public List<Vector3> Listvector3;
    public List<Vector3> _Listvector3;
public LineRenderer lineRender;
    public static List<Vector3> lsPoint = new List<Vector3>();
    public int baseCount = 50;  //����������֮���ȡ������   ֵԽ�����߾�Խƽ��  ��ͬʱ������ҲҲԽ��
    public Transform sphere;   //Ŀ��С��
    float speed = 60;   //�˶��ٶ�
    float length = 0;   //С��ǰ���˶��켣����

    void Start()
    {        
        lineRender = gameObject.GetComponent<LineRenderer>();
        Listvector3.Add( new Vector3(0, 0, 0));
        Listvector3.Add(new Vector3(10, 0, 0));
        Listvector3.Add( new Vector3(20, 0, 0));
        Listvector3.Add(new Vector3(0, 30, 0));
        Listvector3.Add(new Vector3(0, 40, 0));
        Listvector3.Add(new Vector3(0, 0, 50));
        _Listvector3= ReducePathVectorList(Listvector3);
        foreach (Vector3 item in _Listvector3 )
        {
            Debug.Log(item);
        }
        InitPoint();

    }

    private void Update()
    {
        length += Time.deltaTime * speed;
        if (length >= lsPoint.Count - 1)
        {
            length = lsPoint.Count - 1;
        }
        this.transform.position = lsPoint[(int)(length)];
    }

    public List<Vector3> ReducePathVectorList(List<Vector3> _pathVectorList)
    {
        List<Vector3> _ReducePathVectorList = _pathVectorList;
        int i = 0;
        while (_ReducePathVectorList[i + 2] != _pathVectorList.Last())
        {
            Vector3 fromVector3 = (_ReducePathVectorList[i + 1] - _ReducePathVectorList[i]);
            Vector3 toVector3 = (_ReducePathVectorList[i + 2] - _ReducePathVectorList[i + 1]);
            float angleBetween = Vector3.Angle(fromVector3, toVector3);
            if (angleBetween == 0 | angleBetween == 180)
            {
                Debug.Log("��������ͬ��");
                _ReducePathVectorList.RemoveAt(i + 1);
            }
            else
            {
                Debug.Log("����������ͬ��");
                i++;
            }
        }
        return _ReducePathVectorList;
    }


    #region ���㱴�������ߵ���ϵ�
    //��ʼ��������еĵ����Ϣ
    void InitPoint()
    {
        //�������б�תΪ��������
        Vector3[] pointPos =_Listvector3.ToArray();
        GetTrackPoint(pointPos);
    }

    /// <summary>
    /// �����趨�ڵ����ָ��������
    /// </summary>
    /// <param name="track">����ָ���ڵ����Ϣ</param>
    public void GetTrackPoint(Vector3[] track)
    {
        Vector3[] vector3s = PathControlPointGenerator(track);
        int SmoothAmount = track.Length * baseCount;
        lineRender.positionCount = SmoothAmount;
        for (int i = 1; i < SmoothAmount; i++)
        {
            float pm = (float)i / SmoothAmount;
            Vector3 currPt = Interp(vector3s, pm);
            lineRender.SetPosition(i, currPt);
            lsPoint.Add(currPt);
        }
    }

    /// <summary>
    /// �������нڵ��Լ����Ƶ�����
    /// </summary>
    /// <param name="path">���нڵ�Ĵ洢����</param>
    /// <returns></returns>
    public Vector3[] PathControlPointGenerator(Vector3[] path)
    {
        Vector3[] suppliedPath;
        Vector3[] vector3s;

        suppliedPath = path;
        int offset = 2;
        vector3s = new Vector3[suppliedPath.Length + offset];
        Array.Copy(suppliedPath, 0, vector3s, 1, suppliedPath.Length);//Ԥ��index=0��index=length-1������
        vector3s[0] = vector3s[1] + (vector3s[1] - vector3s[2]);//�����ӳ�������index=0�����ֵ
        vector3s[vector3s.Length - 1] = vector3s[vector3s.Length - 2] + (vector3s[vector3s.Length - 2] - vector3s[vector3s.Length - 3]);//�����ӳ�������index=length�����ֵ
        if (vector3s[1] == vector3s[vector3s.Length - 2])//ԭpathΪ�ػ������
        {
            Vector3[] tmpLoopSpline = new Vector3[vector3s.Length];
            Array.Copy(vector3s, tmpLoopSpline, vector3s.Length);
            //����vector3����
            tmpLoopSpline[0] = tmpLoopSpline[tmpLoopSpline.Length - 3];//�ڶ����ػ���ǰ����Ǹ��������λ
            tmpLoopSpline[tmpLoopSpline.Length - 1] = tmpLoopSpline[2];//��һ���ػ��������Ǹ������ĩλ
            vector3s = new Vector3[tmpLoopSpline.Length];
            Array.Copy(tmpLoopSpline, vector3s, tmpLoopSpline.Length);
        }
        return (vector3s);
    }


    /// <summary>
    /// �������ߵ�������λ��
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public Vector3 Interp(Vector3[] pos, float t)
    {
        int length = pos.Length - 3;
        int currPt = Mathf.Min(Mathf.FloorToInt(t * length), length - 1);
        float u = t * (float)length - (float)currPt;
        Vector3 a = pos[currPt];
        Vector3 b = pos[currPt + 1];
        Vector3 c = pos[currPt + 2];
        Vector3 d = pos[currPt + 3];
        return .5f * (
           (-a + 3f * b - 3f * c + d) * (u * u * u)
           + (2f * a - 5f * b + 4f * c - d) * (u * u)
           + (-a + c) * u
           + 2f * b
       );
    }
    #endregion

}
