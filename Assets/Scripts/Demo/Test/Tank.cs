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
    //���˸���
    public class Tank:MonoBehaviour
    {
        //��װ��һЩ����
        public int ID;//��ţ���һ�����г����ı��Ϊ0
        public int Seq;//�������
        public float Width;//����
        public float Length;//����
        public TankType m_Type;//����
        public Vector3 TargetPos;//������Ŀ�������
        // ģ��Prefab
        protected GameObject m_model;
        
        // ����ģ��
        protected  void CreateModel(string myname)
        {
            GameObject model = Resources.Load<GameObject>(myname);
            m_model = (GameObject)Instantiate(model, this.transform.position, this.transform.rotation, this.transform);
        }

        // ��̬���� �������ص�λʵ��
        public static T Create<T>(Vector3 pos, Vector3 angle) where T : Tank
        {
            GameObject go = new GameObject("tank");
            go.transform.position = pos;
            go.transform.eulerAngles = angle;
            T d = go.AddComponent<T>();
            d.Init();
            // ���Լ���ռ���ӵ���Ϣ��Ϊռ��
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
        /// ��ӡ̹����Ϣ
        /// </summary>
        public void SelfIntro()
        {
            Debug.LogFormat("tank:width:{0},length:{1},type:{2},id:{3}", Width, Length, m_Type, ID);
        }

        
        /// <summary>
        /// ����ID��ȡ̹��
        /// </summary>
        /// <param name="_ID">ID</param>
        /// <param name="_tanks">̹���б�</param>
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
        // ������Χ
        public float m_attackArea = 2.0f;
        // ������
        public int m_power = 1;
        // ����ʱ����
        public float m_attackInterval = 2.0f;
        // Ŀ�����
        protected Enemy m_targetEnemy;
        // �Ƿ��Ѿ��������
        protected bool m_isFaceEnemy;
        public void RotateTo()
        {
            if (m_targetEnemy == null)
                return;

            var targetdir = m_targetEnemy.transform.position - transform.position;
            targetdir.y = 0; // ��֤����תY��
                             // ��ȡ��ת����
            Vector3 rot_delta = Vector3.RotateTowards(this.transform.forward, targetdir, 20.0f * Time.deltaTime, 0.0F);
            Quaternion targetrotation = Quaternion.LookRotation(rot_delta);

            // ���㵱ǰ������Ŀ��֮��ĽǶ�
            float angle = Vector3.Angle(targetdir, transform.forward);
            // ����Ѿ��������
            if (angle < 1.0f)
            {
                m_isFaceEnemy = true;
            }
            else
                m_isFaceEnemy = false;

            transform.rotation = targetrotation;
        }

        // ����Ŀ�����
        void FindEnemy()
        {
            if (m_targetEnemy != null)
                return;
            m_targetEnemy = null;
            int minlife = 0; // ��͵�����ֵ
            foreach (Enemy enemy in GameManager.Instance.m_EnemyList) // ��������
            {
                if (enemy.m_life == 0)
                    continue;
                Vector3 pos1 = this.transform.position; pos1.y = 0;
                Vector3 pos2 = enemy.transform.position; pos2.y = 0;
                // ��������˵ľ���
                float dist = Vector3.Distance(pos1, pos2);
                // ������볬��������Χ
                if (dist > m_attackArea)
                    continue;
                // ��������ֵ��͵ĵ���
                if (minlife == 0 || minlife > enemy.m_life)
                {
                    m_targetEnemy = enemy;
                    minlife = enemy.m_life;
                }
            }
        }
        // �����߼�
        protected virtual IEnumerator Attack()
        {
            while (m_targetEnemy == null || !m_isFaceEnemy) // ���û��Ŀ��һֱ�ȴ�
                yield return 0;
            //m_ani.CrossFade("attack", 0.1f); // ���Ź�������

            //while (!m_ani.GetCurrentAnimatorStateInfo(0).IsName("attack")) // �ȴ����빥������
            //    yield return 0;
            //float ani_lenght = m_ani.GetCurrentAnimatorStateInfo(0).length; // ��ù�������ʱ�䳤��
            //yield return new WaitForSeconds(ani_lenght * 0.5f); // �ȴ���ɹ�������
            //if (m_targetEnemy != null)
            //    m_targetEnemy.SetDamage(m_power); // ����
            //yield return new WaitForSeconds(ani_lenght * 0.5f); // �ȴ�����ʣ��Ĺ�������
            //m_ani.CrossFade("idle", 0.1f); // ���Ŵ�������

            yield return new WaitForSeconds(m_attackInterval); // ���һ��ʱ��

            StartCoroutine(Attack()); // ��һ�ֹ���
        }



    }
}
