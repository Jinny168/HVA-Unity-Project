using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Demo_Move : MonoBehaviour
{
    public GameObject cube1;                           //创建一个物体cube1，这个在面板上出现，把上面的cube(1)拖入进去，                                                                                  cube1就代表了cube(1)；
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
