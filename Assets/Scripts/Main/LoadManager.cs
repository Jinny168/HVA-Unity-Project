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
    /// 布列管理类
    /// </summary>
    public class LoadManager:MonoBehaviour
    {
        //水平线
        List<OutLine> outLines;
        List<OutLine> tempOutLines;
        List<OutLine> backupOutLines;
        //战斗波数
        private int m_liveEnemy = 0;//行动车辆数
        private int waveIndex = 0;
        private int enemyIndex = 0;
        // 战斗波数配置数组
        public List<WaveData> waves;         
        //目标集合
        public List<PosData> Dests;
        //游戏管理器单例
        private GameManager gameManager;
        //自身单例
        public static LoadManager Instance;
        //甲板尺寸
        [SerializeField] private Vector2 DeckSize;
        List<Enemy> enemies;
        public int n_enemies;
        private int LowestLineIdx;
        private OutLine LowestLine;
        //生成间隔
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
        //从LoadManager中的waves读取待派样车辆的信息
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
        /// 简单布列
        /// </summary>
        public void LayoutTest()
        {
            ParaSet();
            Locating();
            Conclude();
        }
        /// <summary>
        /// 设置参数
        /// </summary>
        private void ParaSet()
        {
            //甲板尺寸
            DeckSize = gameManager.m_mapSize;
            //车辆数目
            n_enemies = enemies.Count;
            //清除目标
            Dests.Clear();
        }
        /// <summary>
        /// 定位算法，将车辆填充至甲板，采用最左最下填充
        /// </summary>
        public void Locating()
        {
            List<Enemy> tmpEnemies = enemies;
            int i = 0;
            OutLine preLowestLine=null;
            while (tmpEnemies.Count>0)
            {   
                
                //水平线集，lineList=[origin,end,high]，依次为初始坐标，终点坐标，线集高度
                //计算最低水平线及其索引
                FindLowestLine();
                
                if (this.LowestLine == preLowestLine) 
                {
                    return;
                }
                //记录当前最低水平线
                preLowestLine = this.LowestLine;
                //计算可用长度
                float availableWidth = GetLineWidth(this.LowestLineIdx);
                int candidateIdx = searchBySize(availableWidth, tmpEnemies);
                if (candidateIdx!=-1)
                {
                    Enemy pro = tmpEnemies[candidateIdx];
                    
                    float LowestLineWidth = GetLineWidth(LowestLineIdx);
                    //记录车辆的停车点
                    float centerX = LowestLine.origin + (pro.m_tankSize.x / 2);
                    float centerZ = LowestLine.height + (pro.m_tankSize.y / 2);
                    Vector3 singleResultPos = new Vector3(centerX, 0, centerZ);
                    //记录目标点
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
                    
                    //剔除已经排样的坦克
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
        /// 总结布列成果
        /// </summary>
        private void Conclude()
        {
            float maxHeight = CalMaxHeight();
            float maxWidth = CalMaxWidth();
            //计算甲板利用率
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
            //计算最大排样高度
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
            //计算最大排样宽度
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
        /// 根据Seq获取敌人
        /// </summary>
        /// <param name="_Index">序号</param>
        /// <param name="_enemies">敌人列表</param>
        /// <returns></returns>
        private PosData GetEnemyDest(int enemyIndex)
        {
            return Dests[enemyIndex];
        }


        private static bool isSpawn=true;
        public IEnumerator SpawnEnemies()
        {

            yield return new WaitForEndOfFrame();//保证在start函数之后执行
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
                Debug.Log("over");//通知生成完毕
            }
            StartCoroutine(SpawnEnemies());
        }

        /// <summary>
        /// 提升最低水平线
        /// </summary>
        public void EnhanceLine(int _Index)
        {
            int neighborIndex = 0;
            if (outLines.Count > 1)
            {
                
                //获取高度较低的相邻水平线索引，并更新水平线集
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
                    //左边相邻水平线
                    OutLine leftNeighbor = outLines[_Index - 1];
                    //右边相邻水平线
                    OutLine rightNeighbor = outLines[_Index + 1];
                    //选择高度较低的相邻水平线，左右高度一致时，选择左边的相邻水平线
                    if (leftNeighbor.height < rightNeighbor.height)
                    {
                        neighborIndex = _Index - 1;
                    }
                    //高度一致时
                    else if (leftNeighbor.height == rightNeighbor.height)
                    {
                        //选择靠左的水平线
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
            //选择高度较低的相邻水平线
            OutLine oldLine = outLines[neighborIndex];
            //更新相邻水平线
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
        /// 根据下标获取线段长度
        /// </summary>
        /// <param name="_index">索引下标</param>
        /// <returns>线段长度</returns>
        public float GetLineWidth(int _index)
        {
            OutLine line = outLines[_index];
            return line.end - line.origin;
        }


        /// <summary>
        /// 找出最低的水平线（如果最低水平线不止一条则选取最左边的那条）
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
                    Debug.LogFormat("轮廓线：{0},{1},{2}", item.origin, item.end, item.height);
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
                //按高度对temp线集进行排序
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
                //记录最低高度
                float minHeight = tempOutLines[0].height;
                backupOutLines.Clear();
                //筛选出拥有最低高度的线
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
                        Debug.LogFormat("轮廓线：{0},{1},{2}", item.origin, item.end, item.height);
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
                    Debug.LogFormat("轮廓线：{0},{1},{2}", item.origin, item.end, item.height);
                }
                Debug.Log("--------");
            }

        }

        /// <summary>
        /// 自定义的比较器
        /// </summary>
        /// <param name="x">线1</param>
        /// <param name="y">线2</param>
        /// <returns>排序的依据，谁大谁小</returns>
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
        /// 自定义的比较器
        /// </summary>
        /// <param name="x">线1</param>
        /// <param name="y">线2</param>
        /// <returns>排序的依据，谁大谁小</returns>
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
        /// 清空水平线集合
        /// </summary>
        public void EmptyLineList()
        {
            outLines.Clear();
        }
        /// <summary>
        /// 计算最高水平线高度，即所用甲板最大长度
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
// 获得目标模型的边框
//arrowmodel.m_targetCenter = target.GetComponentInChildren<SkinnedMeshRenderer>().bounds;