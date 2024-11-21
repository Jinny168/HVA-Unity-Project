using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AStar;
using System;

public class Demo_Height : MonoBehaviour
{
    OutLine outLine1 = new OutLine(1, 2, 4);
    OutLine outLine2 = new OutLine(1, 3, 3);
    OutLine outLine3 = new OutLine(2, 4, 6);
    List<OutLine> outLines = new List<OutLine>();
    
    void Start()
    {
        outLines.Add(outLine1);
        outLines.Add(outLine2);
        outLines.Add(outLine3);
        outLines.Sort(SortByHeight);
        foreach (var line in outLines)
        {
            Debug.Log(line.height);
        }
    }
    public int SortByHeight(OutLine x, OutLine y)
    {
        if (x.height > y.height)
            return 1;
        else if (x.height == y.height)
            return 0;
        else
            return -1;
    }
}
