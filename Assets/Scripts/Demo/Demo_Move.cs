using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Demo_Move : MonoBehaviour
{
    public GameObject cube1;                           //����һ������cube1�����������ϳ��֣��������cube(1)�����ȥ��                                                                                  cube1�ʹ�����cube(1)��
    Vector3 cube_pos;
    Quaternion cube_rot;
    void Start()
    {
        cube1=GameObject.FindGameObjectWithTag("cube");
        cube_pos= cube1.transform.position;
    }



    void Update()
    {   
        float step = 120f * Time.deltaTime;
        transform.rotation = Quaternion.Lerp(transform.rotation, cube1.transform.rotation , 0.05f);//Quaternion.Euler(0, 0, 90)

       transform.position = Vector3.MoveTowards(transform.position, cube_pos, step);
    }
}
