using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo_MaxValue : MonoBehaviour
{

    int a = int.MaxValue;
    int b = 0;
    int c = 1;
    int d;

    // Start is called before the first frame update
    void Start()
    {
        d = a + c;

        Debug.Log(d);
    }
}
