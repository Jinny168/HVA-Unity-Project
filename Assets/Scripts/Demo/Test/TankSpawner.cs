using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Load;
using System;

public class TankSpawner : MonoBehaviour
{
    public static TankSpawner Instance;
    public int n_tanks;//tank������
    public List<WaveData> waves; // ս��������������
    private int tankIndex = 0; //����tank������±�
    private int waveIndex = 0; //ս������������±�
    LoadManager loadManager;
    int i = 0;
    private void Start()
    { 
       loadManager = LoadManager.Instance;
       waves = loadManager.waves;
       StartCoroutine(SpawnTanks()); // ��ʼ���ɵ���
    }

    IEnumerator SpawnTanks()
    {
        yield return new WaitForEndOfFrame();

        //GameManager.Instance.SetWave((waveIndex + 1));

        WaveData wave = waves[waveIndex];
        yield return new WaitForSeconds(wave.interval);
        while (tankIndex < wave.enemyPrefab.Count)
        {
            GameObject enmeyObj = (GameObject)Instantiate(wave.enemyPrefab[tankIndex], transform.position, Quaternion.identity); // ��������
            Tank tank = enmeyObj.GetComponent<Tank>();  // ��õ��˵Ľű�
            tank.ID = i++;
            tankIndex++;  // ���µ��������±�
            yield return new WaitForSeconds(wave.interval); // ���ɵ���ʱ����
        }

        tankIndex = 0; // ���õ��������±�
        waveIndex++;  // ����ս������
        if (waveIndex < waves.Count) // ����������һ��
        {
            StartCoroutine(SpawnTanks());
        }
        else
        {
            Debug.Log("well done.");// �������
        }
    }

    // �ڱ༭������ʾһ��ͼ��
    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "spawner.tif");
    }
}
