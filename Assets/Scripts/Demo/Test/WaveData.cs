using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class WaveData {
    public int wave = 0;
    public List<GameObject> enemyPrefab;  
    public int level = 1;  // 敌人的等级
    public float interval = 3; // 每3秒创建一个敌人
    public WaveData()
    {
        interval = 3;
    }
}
