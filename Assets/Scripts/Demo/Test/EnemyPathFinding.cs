using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AStar
{

    /// <summary>
    /// 索敌脚本
    /// </summary>
    public class EnemyPathFinding : MonoBehaviour
    {
        private int currentPathIndex;
        private List<Vector3> pathVectorList;
        private AStarManager pathFinding;
        public Vector3 targetPos;

        [SerializeField] private float moveSpeed;
        [SerializeField] private float rotateSpeed;
        System.Action<EnemyPathFinding> onArrive;

        void Awake()
        {
            pathVectorList = new List<Vector3> {};
            pathFinding=new AStarManager(SceneManager.GetActiveScene().name);

        }
        void Start()
        { 
            SetTargetPosition(targetPos);
        }
      

        void FixedUpdate()
        {
            HandleMovement();
        }
        public void SetTargetPosition(Vector3 targetPosition)
        {
            currentPathIndex = 0;
            pathVectorList = pathFinding.FindPath(transform.position, targetPosition);
            pathVectorList = ReducePathVectorList(pathVectorList);
            if (pathVectorList != null && pathVectorList.Count > 1)
            {
                pathVectorList.RemoveAt(0);
            }
        }
        private void HandleMovement()
        {
            Gravity();


            if (pathVectorList != null)
            {
                Vector3 targetPosition = pathVectorList[currentPathIndex];
                if (Vector3.Distance(transform.position, targetPosition) >= 0.1f)
                {
                    Debug.DrawLine(transform.position, targetPosition,Color.green);
                    RotateTo(targetPosition);
                    MoveTo(targetPosition);
                }
                else
                {
                    currentPathIndex++;
                    if (currentPathIndex >= pathVectorList.Count)
                    {
                        StopMoving();
                    }
                }
            }



        }

        private void StopMoving()
        {
            currentPathIndex = 0;
            pathVectorList = null;
        }

        public void Gravity()
        {
            float fallSpeed = 0;
            if (this.transform.position.y > 0.1f)
            {
                fallSpeed = 2.0f;
            }
            this.transform.Translate(new Vector3(0, -fallSpeed * Time.deltaTime, 0));
        }

        public void RotateTo(Vector3 targetPosition_AStar)
        {
            var position = targetPosition_AStar - transform.position;
            position.y = 0; // 保证仅旋转Y轴
            var targetRotation = Quaternion.LookRotation(position); // 获得目标旋转角度
            float next = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetRotation.eulerAngles.y, //获得中间的旋转角度
                rotateSpeed * Time.deltaTime);
            this.transform.eulerAngles = new Vector3(0, next, 0);
        }

        public void MoveTo(Vector3 targetPosition_astar)
        {
            Vector3 pos1 = this.transform.position;
            Vector3 pos2 = targetPosition_astar;

            float dist = Vector2.Distance(new Vector2(pos1.x, pos1.z), new Vector2(pos2.x, pos2.z));
            if (dist < 2.0f)
            {
                if (pathVectorList == null) // 列表为空，说明已经到达目标点
                {
                    
                    GameManager.Instance.SetDamage(1); // 扣除一点伤害值
                    
                }
            }
            this.transform.Translate(new Vector3(0, 0, moveSpeed * Time.deltaTime));
        }

        public List<Vector3> ReducePathVectorList(List<Vector3> pathVectorList)
        {
            List<Vector3> ReducePathVectorList = pathVectorList;
            int i = 0;
            while (ReducePathVectorList[i + 2] != pathVectorList.Last())
            {
                Vector3 fromVector3 = (ReducePathVectorList[i + 1] - ReducePathVectorList[i]);
                Vector3 toVector3 = (ReducePathVectorList[i + 2] - ReducePathVectorList[i + 1]);
                float angleBetween = Vector3.Angle(fromVector3, toVector3);
                if (angleBetween == 0 | angleBetween == 180)
                {
                    ReducePathVectorList.RemoveAt(i + 1);
                }
                else
                {
                    i++;
                }
            }
            return ReducePathVectorList;
        }

        //静态函数 创建坦克
        public static void Create(Vector3 targetPos, Vector3 spawnPos, System.Action<EnemyPathFinding> onArrive)
        {
            // 读取车辆模型
            GameObject prefab = Resources.Load<GameObject>("T95");
            GameObject go = (GameObject)Instantiate(prefab, spawnPos, Quaternion.identity);
            // 添加寻路组件
            EnemyPathFinding enemyModel = go.AddComponent<EnemyPathFinding>();
            // 设置寻路目标
            enemyModel.targetPos = targetPos;
            // 取得Action
            enemyModel.onArrive = onArrive;
        }

        
    }//EnemyPathFinding

    
}



