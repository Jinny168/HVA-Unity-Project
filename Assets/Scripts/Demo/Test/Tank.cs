using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UIElements;
using static Defender;
using static Load.LoadManager;
namespace Load 
{
    //敌人个体
    public class Tank:MonoBehaviour
    {
        //封装的一些属性
        public int ID;//编号，第一辆布列车辆的编号为0
        public int Seq;//进场序号
        public float Width;//车宽
        public float Length;//车长
        public TankType m_Type;//车型
        public Vector3 TargetPos;//车辆的目标点坐标
        // 模型Prefab
        protected GameObject m_model;
        
        // 创建模型
        protected  void CreateModel(string myname)
        {
            GameObject model = Resources.Load<GameObject>(myname);
            m_model = (GameObject)Instantiate(model, this.transform.position, this.transform.rotation, this.transform);
        }

        // 静态函数 创建防守单位实例
        public static T Create<T>(Vector3 pos, Vector3 angle) where T : Tank
        {
            GameObject go = new GameObject("tank");
            go.transform.position = pos;
            go.transform.eulerAngles = angle;
            T d = go.AddComponent<T>();
            d.Init();
            // 将自己所占格子的信息设为占用
            //TileObject.Instance.setDataFromPosition(d.transform.position.x, d.transform.position.z, (int)TileStatus.DEAD);
            return d;
        }


        private void Init()
        {

            int i=Seq;
            //TargetPos=LoadManager.Dests[i].resultPos;
            CreateModel("");
            StartCoroutine(PathFind());
        }

        private string PathFind()
        {
            throw new NotImplementedException();
        }

        public Tank() 
        {

            Width = -1;
            Length = -1;
            m_Type = TankType.Unknown;
            Seq = -1;
            ID = -1;
            TargetPos = Vector3.zero;
        }
        public void SetSeq(int value)
        {
            Seq=value;
        }
        public int GetSeq() 
        {
            return Seq;
        }
        public void SetTankType(TankType value)
        {
            m_Type = value;
        }
        public TankType GetTankType() 
        {
            return m_Type;
        }
 
        /// <summary>
        /// 打印坦克信息
        /// </summary>
        public void SelfIntro()
        {
            Debug.LogFormat("tank:width:{0},length:{1},type:{2},id:{3}", Width, Length, m_Type, ID);
        }

        
        /// <summary>
        /// 根据ID获取坦克
        /// </summary>
        /// <param name="_ID">ID</param>
        /// <param name="_tanks">坦克列表</param>
        /// <returns>Tank</returns>
        public static Tank GetTankByID(int _ID, List<Tank> _tanks)
{
            foreach (Tank item in _tanks)
            {
                if (item.ID == _ID)
                {
                    return item;
                }
            }
            return null;
        }
        public float GetArea()
        {
            return Width * Length;
        }

        private void Update()
        {
            FindEnemy();
            RotateTo();
            Attack();
        }
        // 攻击范围
        public float m_attackArea = 2.0f;
        // 攻击力
        public int m_power = 1;
        // 攻击时间间隔
        public float m_attackInterval = 2.0f;
        // 目标敌人
        protected Enemy m_targetEnemy;
        // 是否已经面向敌人
        protected bool m_isFaceEnemy;
        public void RotateTo()
        {
            if (m_targetEnemy == null)
                return;

            var targetdir = m_targetEnemy.transform.position - transform.position;
            targetdir.y = 0; // 保证仅旋转Y轴
                             // 获取旋转方向
            Vector3 rot_delta = Vector3.RotateTowards(this.transform.forward, targetdir, 20.0f * Time.deltaTime, 0.0F);
            Quaternion targetrotation = Quaternion.LookRotation(rot_delta);

            // 计算当前方向与目标之间的角度
            float angle = Vector3.Angle(targetdir, transform.forward);
            // 如果已经面向敌人
            if (angle < 1.0f)
            {
                m_isFaceEnemy = true;
            }
            else
                m_isFaceEnemy = false;

            transform.rotation = targetrotation;
        }

        // 查找目标敌人
        void FindEnemy()
        {
            if (m_targetEnemy != null)
                return;
            m_targetEnemy = null;
            int minlife = 0; // 最低的生命值
            foreach (Enemy enemy in GameManager.Instance.m_EnemyList) // 遍历敌人
            {
                if (enemy.m_life == 0)
                    continue;
                Vector3 pos1 = this.transform.position; pos1.y = 0;
                Vector3 pos2 = enemy.transform.position; pos2.y = 0;
                // 计算与敌人的距离
                float dist = Vector3.Distance(pos1, pos2);
                // 如果距离超过攻击范围
                if (dist > m_attackArea)
                    continue;
                // 查找生命值最低的敌人
                if (minlife == 0 || minlife > enemy.m_life)
                {
                    m_targetEnemy = enemy;
                    minlife = enemy.m_life;
                }
            }
        }
        // 攻击逻辑
        protected virtual IEnumerator Attack()
        {
            while (m_targetEnemy == null || !m_isFaceEnemy) // 如果没有目标一直等待
                yield return 0;
            //m_ani.CrossFade("attack", 0.1f); // 播放攻击动画

            //while (!m_ani.GetCurrentAnimatorStateInfo(0).IsName("attack")) // 等待进入攻击动画
            //    yield return 0;
            //float ani_lenght = m_ani.GetCurrentAnimatorStateInfo(0).length; // 获得攻击动画时间长度
            //yield return new WaitForSeconds(ani_lenght * 0.5f); // 等待完成攻击动作
            //if (m_targetEnemy != null)
            //    m_targetEnemy.SetDamage(m_power); // 攻击
            //yield return new WaitForSeconds(ani_lenght * 0.5f); // 等待播放剩余的攻击动画
            //m_ani.CrossFade("idle", 0.1f); // 播放待机动画

            yield return new WaitForSeconds(m_attackInterval); // 间隔一定时间

            StartCoroutine(Attack()); // 下一轮攻击
        }



    }
}
