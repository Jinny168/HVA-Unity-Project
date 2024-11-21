using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Load;
using System;

public class TankSpawner : MonoBehaviour
{
    public static TankSpawner Instance;
    public int n_tanks;//tank的数量
    public List<WaveData> waves; // 战斗波数配置数组
    private int tankIndex = 0; //生成tank数组的下标
    private int waveIndex = 0; //战斗波数数组的下标
    LoadManager loadManager;
    int i = 0;
    private void Start()
    { 
       loadManager = LoadManager.Instance;
       waves = loadManager.waves;
       StartCoroutine(SpawnTanks()); // 开始生成敌人
    }

    IEnumerator SpawnTanks()
    {
        yield return new WaitForEndOfFrame();

        //GameManager.Instance.SetWave((waveIndex + 1));

        WaveData wave = waves[waveIndex];
        yield return new WaitForSeconds(wave.interval);
        while (tankIndex < wave.enemyPrefab.Count)
        {
            GameObject enmeyObj = (GameObject)Instantiate(wave.enemyPrefab[tankIndex], transform.position, Quaternion.identity); // 创建敌人
            Tank tank = enmeyObj.GetComponent<Tank>();  // 获得敌人的脚本
            tank.ID = i++;
            tankIndex++;  // 更新敌人数组下标
            yield return new WaitForSeconds(wave.interval); // 生成敌人时间间隔
        }

        tankIndex = 0; // 重置敌人数组下标
        waveIndex++;  // 更新战斗波数
        if (waveIndex < waves.Count) // 如果不是最后一波
        {
            StartCoroutine(SpawnTanks());
        }
        else
        {
            Debug.Log("well done.");// 生成完毕
        }
    }

    // 在编辑器中显示一个图标
    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "spawner.tif");
    }
}
