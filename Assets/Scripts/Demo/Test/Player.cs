using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    //导航代理
    NavMeshAgent agent;
    //画线工具
    public LineRenderer line;
    //目标位置
    Vector3 target;
    //平面层级
    int PlaneLayer = 1 << 9;

    // Start is called before the first frame update
    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 300, PlaneLayer))
            {
                if (hit.collider.tag == "tile")
                {
                    target = hit.point;
                    agent.SetDestination(target);
                    Vector3[] posArr = GetPath();
                    line.positionCount = posArr.Length;
                    for (int i = 0; i < posArr.Length; i++)
                    {
                        line.SetPosition(i, posArr[i]);
                    }
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, target) <= 0.1f)
        {
            line.positionCount = 0;
        }
        else
        {
            Vector3[] posArr = GetPath();
            line.positionCount = posArr.Length;
            for (int i = 0; i < posArr.Length; i++)
            {
                line.SetPosition(i, posArr[i]);
            }
        }
    }

    Vector3[] GetPath()
    {
        //创建一个导航路径对象
        NavMeshPath path = new NavMeshPath();
        //给定了一个寻路的终点，agent组件会计算出一个最终的导航路线，
        //但是不会真的进行寻路，而是把计算出的路径存储在path对象中
        //if (!agent.isActiveAndEnabled)
        //{
        //    
        //}
        if (TestNavigation()) 
        {
            Debug.Log("OK");        
        }

        agent.enabled=true;
        agent.CalculatePath(target, path);

        //path.corners拐点，不包含起点和终点
        Vector3[] posArr = new Vector3[path.corners.Length + 2];
        posArr[0] = transform.position;
        posArr[posArr.Length - 1] = target;
        for (int i = 0; i < path.corners.Length; i++)
        {
            posArr[i + 1] = path.corners[i];
        }
        return posArr;
    }
    public bool TestNavigation()
    {
        if (agent.isOnNavMesh)
        {
            NavMeshHit navigationHit;
            if (NavMesh.SamplePosition(target, out navigationHit, 15, agent.areaMask))
                return agent.SetDestination(navigationHit.position);
            return false;
        }
        else
        {
            return agent.Warp(new Vector3(0,0,0));
        }
    }
}

