using AStar;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using UnityEditor.Android;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

namespace Load 
{
    /// <summary>
    /// ���й�����
    /// </summary>
    public class LoadManager:MonoBehaviour
    {
        //ˮƽ��
        List<OutLine> outLines;
        List<OutLine> tempOutLines;
        List<OutLine> backupOutLines;
        //ս������
        private int m_liveEnemy = 0;//�ж�������
        private int waveIndex = 0;
        private int enemyIndex = 0;
        // ս��������������
        public List<WaveData> waves;         
        //Ŀ�꼯��
        public List<PosData> Dests;
        //��Ϸ����������
        private GameManager gameManager;
        //������
        public static LoadManager Instance;
        //�װ�ߴ�
        [SerializeField] private Vector2 DeckSize;
        List<Enemy> enemies;
        public int n_enemies;
        private int LowestLineIdx;
        private OutLine LowestLine;
        //���ɼ��
        [SerializeField]private float spawnInterval;
        public void Awake()
        {
            gameManager = GameObject.FindObjectOfType<GameManager>();
            Instance = this;
        }
        public void Start()
        {
            enemies = new List<Enemy>();
            backupOutLines = new List<OutLine>();
            OutLine initLine = new OutLine(0, gameManager.m_mapSize.x, 0);
            outLines = new List<OutLine>();
            outLines.Clear();
            outLines.Add(initLine);
            this.LowestLine = initLine;
            this.LowestLineIdx = 0;
            ReadInfo();
            LayoutTest();
            waveIndex = 0;
            Vector3 vector3 = new Vector3(0, 0, 0);
            Quaternion targetRotation = Quaternion.LookRotation(vector3, Vector3.up);
            transform.rotation = targetRotation;
            StartCoroutine(SpawnEnemies());

        }
        //��LoadManager�е�waves��ȡ��������������Ϣ
        private void ReadInfo()
        {
            foreach (WaveData wave in waves)
            {
                foreach (GameObject item in wave.enemyPrefab)
                {
                    Enemy tmpEnemy = item.GetComponent<Enemy>();
                    enemies.Add(tmpEnemy);
                }
            }
        }

        /// <summary>
        /// �򵥲���
        /// </summary>
        public void LayoutTest()
        {
            ParaSet();
            Locating();
            Conclude();
        }
        /// <summary>
        /// ���ò���
        /// </summary>
        private void ParaSet()
        {
            //�װ�ߴ�
            DeckSize = gameManager.m_mapSize;
            //������Ŀ
            n_enemies = enemies.Count;
            //���Ŀ��
            Dests.Clear();
        }
        /// <summary>
        /// ��λ�㷨��������������װ壬���������������
        /// </summary>
        public void Locating()
        {
            List<Enemy> tmpEnemies = enemies;
            int i = 0;
            OutLine preLowestLine=null;
            while (tmpEnemies.Count>0)
            {   
                
                //ˮƽ�߼���lineList=[origin,end,high]������Ϊ��ʼ���꣬�յ����꣬�߼��߶�
                //�������ˮƽ�߼�������
                FindLowestLine();
                
                if (this.LowestLine == preLowestLine) 
                {
                    return;
                }
                //��¼��ǰ���ˮƽ��
                preLowestLine = this.LowestLine;
                //������ó���
                float availableWidth = GetLineWidth(this.LowestLineIdx);
                int candidateIdx = searchBySize(availableWidth, tmpEnemies);
                if (candidateIdx!=-1)
                {
                    Enemy pro = tmpEnemies[candidateIdx];
                    
                    float LowestLineWidth = GetLineWidth(LowestLineIdx);
                    //��¼������ͣ����
                    float centerX = LowestLine.origin + (pro.m_tankSize.x / 2);
                    float centerZ = LowestLine.height + (pro.m_tankSize.y / 2);
                    Vector3 singleResultPos = new Vector3(centerX, 0, centerZ);
                    //��¼Ŀ���
                    Dests.Add(new PosData(i++, singleResultPos, new Vector2(pro.m_tankSize.x, pro.m_tankSize.y)));

               

                    //Debug.LogFormat("pro.ID:{0},pro.TargetPos:{1},pro.AreaMessage:{2}",pro.m_dest.m_seq,pro.m_dest.m_point, new Vector2(pro.m_tankSize.x, pro.m_tankSize.y));
                    //Update the outlines
                    OutLine newLine1 = new OutLine(LowestLine.origin, LowestLine.origin + pro.m_tankSize.x, LowestLine.height + pro.m_tankSize.y);
                    OutLine newLine2 = new OutLine(LowestLine.origin + pro.m_tankSize.x, LowestLine.origin + LowestLineWidth, LowestLine.height);

                    outLines[LowestLineIdx] = newLine1;
                    if (LowestLineWidth > pro.m_tankSize.x)
                    {
                        if (LowestLineIdx + 1 == outLines.Count)
                        {
                            outLines.Add(newLine2);
                        }
                        else 
                        {
                            outLines.Insert(LowestLineIdx+1, newLine2);
                        }
                    }
                    
                    //�޳��Ѿ�������̹��
                    tmpEnemies.Remove(pro);
                }
                else
                {
                    EnhanceLine(LowestLineIdx);
                }
            }

        }//Locate
        private int searchBySize(float available_width,List<Enemy> tmpEnemies)
        {
            for (int i = 0; i < tmpEnemies.Count; i++)
            {
                Enemy enemy = tmpEnemies[i];
                if (enemy.m_tankSize.x <= available_width) 
                {
                    return i;
                }
            }
            return -1;
        }

       
        /// <summary>
        /// �ܽ᲼�гɹ�
        /// </summary>
        private void Conclude()
        {
            float maxHeight = CalMaxHeight();
            float maxWidth = CalMaxWidth();
            //����װ�������
            float usedArea = 0;
            //Debug.LogFormat("usedArea:{0}", usedArea);
            foreach (var item in Dests)
            {
                usedArea += item.Area;
            }
            //Debug.LogFormat("usedArea:{0}", usedArea);
            float ratio = (usedArea * 100) / (maxHeight * maxWidth);
            Debug.LogFormat("ratio:{0}%", ratio);
        }//conclude

        private float CalMaxHeight()
        {
            float maxHeight = 0;
            //������������߶�
            for (int i = 0; i < outLines.Count; i++)
            {
                if (outLines[i].height>= maxHeight)
                {
                    maxHeight = outLines[i].height;
                }
            }
            //Debug.LogFormat("maxHeight:{0}", maxHeight);
            return maxHeight;
        }
        private float CalMaxWidth()
        {
            float maxWidth = 0;
            //��������������
            for (int i = 0; i < outLines.Count; i++)
            {
                if (outLines[i].height > 0 && outLines[i].end>maxWidth)
                {
                    maxWidth = outLines[i].end;  
                }
            }
            //Debug.LogFormat("maxHeight:{0}", maxWidth);
            return maxWidth;
        }

        /// <summary>
        /// ����Seq��ȡ����
        /// </summary>
        /// <param name="_Index">���</param>
        /// <param name="_enemies">�����б�</param>
        /// <returns></returns>
        private PosData GetEnemyDest(int enemyIndex)
        {
            return Dests[enemyIndex];
        }


        private static bool isSpawn=true;
        public IEnumerator SpawnEnemies()
        {

            yield return new WaitForEndOfFrame();//��֤��start����֮��ִ��
            GameManager.Instance.SetWave(waveIndex+1);
            if (waveIndex >= waves.Count)
                yield break;
            WaveData wave = waves[waveIndex];
            yield return new WaitForSeconds(wave.interval);
            while (enemyIndex < wave.enemyPrefab.Count)
            {

                GameObject go = (GameObject)Instantiate(wave.enemyPrefab[enemyIndex], transform.position,Quaternion.identity );
                Enemy enemy = go.GetComponent<Enemy>();
                PosData pos = GetEnemyDest(enemyIndex);
                enemy.m_dest.m_point = pos.resultPos;
                enemy.m_dest.m_seq = enemyIndex;
                enemy.m_currentState = Enemy.EnemyState.load;
                m_liveEnemy++;

                enemy.onArrive = new Action<Enemy>((Enemy e) => { m_liveEnemy--; });
                enemyIndex++;
                yield return new WaitForSeconds(wave.interval);
            }
            while (m_liveEnemy > 0)
            {
                yield return 0;
            }
            waveIndex++;
            if (waveIndex < waves.Count)
            {
                StartCoroutine(SpawnEnemies());
            }
            else 
            {
                Debug.Log("over");//֪ͨ�������
            }
            StartCoroutine(SpawnEnemies());
        }

        /// <summary>
        /// �������ˮƽ��
        /// </summary>
        public void EnhanceLine(int _Index)
        {
            int neighborIndex = 0;
            if (outLines.Count > 1)
            {
                
                //��ȡ�߶Ƚϵ͵�����ˮƽ��������������ˮƽ�߼�
                if (_Index == 0)
                {
                    neighborIndex = 1;
                }
                else if (_Index + 1 == outLines.Count)
                {
                    neighborIndex = _Index - 1;
                }
                else
                {
                    //�������ˮƽ��
                    OutLine leftNeighbor = outLines[_Index - 1];
                    //�ұ�����ˮƽ��
                    OutLine rightNeighbor = outLines[_Index + 1];
                    //ѡ��߶Ƚϵ͵�����ˮƽ�ߣ����Ҹ߶�һ��ʱ��ѡ����ߵ�����ˮƽ��
                    if (leftNeighbor.height < rightNeighbor.height)
                    {
                        neighborIndex = _Index - 1;
                    }
                    //�߶�һ��ʱ
                    else if (leftNeighbor.height == rightNeighbor.height)
                    {
                        //ѡ�����ˮƽ��
                        if (leftNeighbor.origin < rightNeighbor.origin)
                        {
                            neighborIndex = _Index - 1;
                        }
                        else
                        {
                            neighborIndex = _Index + 1;
                        }
                    }
                    else 
                    {
                        neighborIndex = _Index + 1;
                    }

                }

            }//if
            //ѡ��߶Ƚϵ͵�����ˮƽ��
            OutLine oldLine = outLines[neighborIndex];
            //��������ˮƽ��
            if (neighborIndex < _Index)
            {
                outLines[neighborIndex] = new OutLine(oldLine.origin, oldLine.end + GetLineWidth(_Index), oldLine.height);
            }
            else
            {
                outLines[neighborIndex] = new OutLine(oldLine.origin - GetLineWidth(_Index), oldLine.end, oldLine.height);
            }
            outLines.RemoveAt(_Index);
        }
        /// <summary>
        /// �����±��ȡ�߶γ���
        /// </summary>
        /// <param name="_index">�����±�</param>
        /// <returns>�߶γ���</returns>
        public float GetLineWidth(int _index)
        {
            OutLine line = outLines[_index];
            return line.end - line.origin;
        }


        /// <summary>
        /// �ҳ���͵�ˮƽ�ߣ�������ˮƽ�߲�ֹһ����ѡȡ����ߵ�������
        /// </summary>
        public void FindLowestLine()
        {
            tempOutLines = this.outLines;
            if (tempOutLines.Count == 1)
            {
                this.LowestLineIdx = 0;
                this.LowestLine = tempOutLines[0];
                foreach (var item in outLines)
                {
                    Debug.LogFormat("�����ߣ�{0},{1},{2}", item.origin, item.end, item.height);
                    //DateTime dt1=DateTime.Now;
                    //while ((DateTime.Now-dt1).TotalMilliseconds<100) 
                    //{
                    //    continue;
                    //};
                }
                Debug.Log("--------");
                return;
            }
            else 
            {
                //���߶ȶ�temp�߼���������
                tempOutLines.Sort(delegate (OutLine a, OutLine b) {
                    if (a.height > b.height)
                    {
                        return 1;
                    }
                    else
                    {
                        return -1;
                    }
                });
                //��¼��͸߶�
                float minHeight = tempOutLines[0].height;
                backupOutLines.Clear();
                //ɸѡ��ӵ����͸߶ȵ���
                foreach (OutLine line in tempOutLines)
                {
                    if (line.height == minHeight)
                    {
                        backupOutLines.Add(line);
                    }
                }
                if (backupOutLines.Count == 1)
                {
                    for (int i = 0; i < this.outLines.Count; i++)
                    {
                        OutLine item = this.outLines[i];
                        if (item.height == minHeight)
                        {
                            this.LowestLineIdx = i;
                            this.LowestLine = item;
                        }
                    }
                    foreach (var item in outLines)
                    {
                        Debug.LogFormat("�����ߣ�{0},{1},{2}", item.origin, item.end, item.height);
                    }
                    Debug.Log("--------");
                    return;
                }
                else 
                { 
                    backupOutLines.Sort(delegate(OutLine a, OutLine b){
                        if (a.origin > b.origin)
                        {
                            return 1;
                        }
                        else 
                        {
                            return -1;
                        }
                    });
                    float minOrigin=backupOutLines[0].origin;
                    for (int i = 0; i < this.outLines.Count; i++)
                    {
                        OutLine item = this.outLines[i];
                        if ((item.origin == minOrigin )&&(item.height==minHeight))
                        {
                            this.LowestLineIdx = i;
                            this.LowestLine = item;
                            
                        }
                    }
                }
                foreach (var item in outLines)
                {
                    Debug.LogFormat("�����ߣ�{0},{1},{2}", item.origin, item.end, item.height);
                }
                Debug.Log("--------");
            }

        }

        /// <summary>
        /// �Զ���ıȽ���
        /// </summary>
        /// <param name="x">��1</param>
        /// <param name="y">��2</param>
        /// <returns>��������ݣ�˭��˭С</returns>
        private int SortByOrigin(OutLine x, OutLine y)
        {
            if (x.origin > y.origin)
                return 1;
            else if (x.origin == y.origin)
                return 0;
            else
                return -1;
        }
        /// <summary>
        /// �Զ���ıȽ���
        /// </summary>
        /// <param name="x">��1</param>
        /// <param name="y">��2</param>
        /// <returns>��������ݣ�˭��˭С</returns>
        private int SortByHeight(OutLine x, OutLine y)
        {

            if (x.height > y.height)
                return 1;
            else if (x.height == y.height)
                return 0;
            else
                return -1;
        }

        /// <summary>
        /// ���ˮƽ�߼���
        /// </summary>
        public void EmptyLineList()
        {
            outLines.Clear();
        }
        /// <summary>
        /// �������ˮƽ�߸߶ȣ������üװ���󳤶�
        /// </summary>
        /// <returns></returns>
        public float CalHighLine()
        {

            float maxHeight = 0;
            List<OutLine> tempOutLines = outLines;
            tempOutLines.Sort(SortByHeight);
            maxHeight = tempOutLines.Last().height;
            return maxHeight;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, "spawner.tif");

        }
    }


}
// ���Ŀ��ģ�͵ı߿�
//arrowmodel.m_targetCenter = target.GetComponentInChildren<SkinnedMeshRenderer>().bounds;