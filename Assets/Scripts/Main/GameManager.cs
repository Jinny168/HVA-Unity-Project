using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI; 			// 注意UI控件命名空间的引用
using UnityEngine.Events; 			// 注意UI事件命名空间的引用
using UnityEngine.EventSystems;     // 注意UI事件命名空间的引用
using System.ComponentModel;
using AStar;
using Unity.Services.Analytics;
using Load;
using System;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;

    // 显示路点的debug开关
    public bool m_debug = true;
    // 路点
    public List<PathNode> m_PathNodes;

    // 敌人列表
    public List<Enemy> m_EnemyList = new List<Enemy>();

    //坦克列表
    public List<Tank> m_TankList = new List<Tank>();


    // 地面的碰撞Layer
    public LayerMask m_groundlayer;

    // 波数
    public int m_wave = 1;
    public int m_waveMax = 10;

    // 生命
    public int m_life = 10;

    // 铜钱数量
    public int m_point = 99;

    //地图信息
    public Vector3 m_mapSize;//对应地图宽度、地图高度、地图网格大小

    //坦克速度
    public int m_speed = 8;

    //坦克尺寸
    public Vector2 m_tankSize ;//对应坦克的宽度和长度

    //坦克坐标
    public Vector3 m_pos;


    // UI文字控件
    Text m_txt_wave;
    Text m_txt_life;
    Text m_txt_point;

    Text m_txt_mapSize;
    Text m_txt_speed;
    Text m_txt_tankSize;
    Text m_txt_pos;


    // UI重新游戏按钮控件
    Button m_but_try;       //刷新场景
    Button m_but_manual;    //手动操作
    Button m_but_pathnode;  //路点寻路
    Button m_but_astar;     //A*寻路
    Button m_but_besizer;   //besizer寻路
    Button m_but_load;      //单次装载
    Button m_but_plan;      //规划装载


    // 当前是否选中的创建防守单位的按钮
    bool m_isSelectedButton =false;

    //已有的游戏对象
    GameObject[] m_objects;//tank
    Enemy enemy1 = null;
    AirEnemy enemy2 = null;
    TileObject tileObject = null;


    void Awake()
    {
        Instance = this;
    }

	// Use this for initialization
	void Start () {

        // 创建UnityAction，在OnButCreateDefenderDown函数中响应按钮按下事件
        UnityAction<BaseEventData> downAction = new UnityAction<BaseEventData>(OnButCreateDefenderDown);
        // 创建UnityAction，在OnButCreateDefenderDown函数中响应按钮抬起事件
        UnityAction<BaseEventData> upAction = new UnityAction<BaseEventData>(OnButCreateDefenderUp);

        // 创建按钮按下事件Entry
        EventTrigger.Entry down = new EventTrigger.Entry();
        down.eventID = EventTriggerType.PointerDown;
        down.callback.AddListener(downAction);

        // 创建按钮抬起事件Entry
        EventTrigger.Entry up = new EventTrigger.Entry();
        up.eventID = EventTriggerType.PointerUp;
        up.callback.AddListener(upAction);
    
        // 查找所有子物体，根据名称获取UI控件
        foreach (Transform t in this.GetComponentsInChildren<Transform>())
        {
            if (t.name.CompareTo("mapsize") == 0)  //找到文字控件"地图信息"
            {
                m_txt_mapSize = t.GetComponent<Text>();
                tileObject = GameObject.FindGameObjectWithTag("GridObject").GetComponent<TileObject>();
                m_mapSize = new Vector3(tileObject.xTileCount, tileObject.zTileCount, tileObject.tileSize);
                SetMapSize(m_mapSize);
            }
            else if (t.name.CompareTo("tanksize") == 0) //找到文字控件"坦克尺寸"
            {
                m_txt_tankSize = t.GetComponent<Text>();
                m_txt_tankSize.text = string.Format("tanksize：<color=orange>{0}</color>", m_tankSize);
            }
            else if (t.name.CompareTo("tankspeed") == 0)  //找到文字控件"坦克速度"
            {
                m_txt_speed = t.GetComponent<Text>();
                m_txt_speed.text = string.Format("tankspeed：<color=orange>{0}</color>", m_speed);
            }
            else if (t.name.CompareTo("tankpos") == 0) //找到文字控件"坦克坐标"
            {
                m_txt_pos = t.GetComponent<Text>();
                m_txt_pos.text = string.Format("tankpos：<color=orange>{0}</color>", m_pos);
            }
            else if (t.name.CompareTo("wave") == 0)  //找到文字控件"波数"
            {
                m_txt_wave = t.GetComponent<Text>();
                SetWave(1);
            }
            else if (t.name.CompareTo("life") == 0)  //找到文字控件"生命"
            {
                m_txt_life = t.GetComponent<Text>();
                m_txt_life.text = string.Format("para2：<color=orange>{0}</color>", m_life);
            }
            else if (t.name.CompareTo("point") == 0) //找到文字控件"铜钱"
            {
                m_txt_point = t.GetComponent<Text>();
                m_txt_point.text = string.Format("para1：<color=orange>{0}</color>", m_point);
            }
            else if (t.name.CompareTo("but_try") == 0) //找到按钮控件"重新游戏"
            {
                m_but_try = t.GetComponent<Button>();
                // 添加按钮单击函数回调,重新游戏按钮
                m_but_try.onClick.AddListener(delegate ()
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                });
                // 默认隐藏重新游戏按钮
                m_but_try.gameObject.SetActive(false);
            }
            else if (t.name.Contains("but_tank")) //找到按钮控件"创建坦克单位"
            {
                // 给创建防守单位按钮添加EventTrigger，并添加前面定义的按钮事件
                EventTrigger trigger = t.gameObject.AddComponent<EventTrigger>();
                trigger.triggers = new List<EventTrigger.Entry>();
                trigger.triggers.Add(down);
                trigger.triggers.Add(up);
            }
            else if (t.name.Contains("but_aero")) //找到按钮控件"创建坦克单位"
            {
                // 给创建防守单位按钮添加EventTrigger，并添加前面定义的按钮事件
                EventTrigger trigger = t.gameObject.AddComponent<EventTrigger>();
                trigger.triggers = new List<EventTrigger.Entry>();
                trigger.triggers.Add(down);
                trigger.triggers.Add(up);
            }
            else if (t.name.CompareTo("but_mode1") == 0) //找到按钮控件"手动模式"
            {
                m_but_manual = t.GetComponent<Button>();
                m_but_manual.onClick.AddListener(delegate () {
                    GameObject.FindObjectOfType<Enemy>().m_currentState = Enemy.EnemyState.manual;
                });
            }
            else if (t.name.CompareTo("but_mode2") == 0) //找到按钮控件"路点模式"
            {
                m_but_pathnode = t.GetComponent<Button>();
                m_but_pathnode.onClick.AddListener(delegate () {
                    GameObject.FindObjectOfType<Enemy>().m_currentState = Enemy.EnemyState.pathnode;
                });
            }
            else if (t.name.CompareTo("but_mode3") == 0) //找到按钮控件"A*模式"
            {
                m_but_astar = t.GetComponent<Button>();
                m_but_astar.onClick.AddListener(delegate () {
                    GameObject.FindObjectOfType<Enemy>().m_currentState = Enemy.EnemyState.astar;
                });
            }
            else if (t.name.CompareTo("but_mode4") == 0) //找到按钮控件"besizer A*模式"
            {
                m_but_astar = t.GetComponent<Button>();
                m_but_astar.onClick.AddListener(delegate () {
                    GameObject.FindObjectOfType<Enemy>().m_currentState = Enemy.EnemyState.besizerastar;
                });
            }
            else if (t.name.CompareTo("but_mode5") == 0) //找到按钮控件"load模式"
            {
                m_but_astar = t.GetComponent<Button>();
                m_but_astar.onClick.AddListener(delegate () {
                    Enemy[] enemies = GameObject.FindObjectsOfType<Enemy>();
                    foreach (var item in enemies)
                    {
                        item.m_currentState = Enemy.EnemyState.load;
                    }
                });
            }
            else if (t.name.CompareTo("but_mode6") == 0) //找到按钮控件"idle模式"
            {
                m_but_astar = t.GetComponent<Button>();
                m_but_astar.onClick.AddListener(delegate () {
                    Enemy[] enemies = GameObject.FindObjectsOfType<Enemy>();
                    foreach (Enemy item in enemies)
                    {
                        item.m_currentState = Enemy.EnemyState.idle;
                    }
                });
            }
        }

        BuildPath();

    }

	// Update is called once per frame
	void Update () {

        // 如果选中创建士兵的按钮则取消摄像机操作
        if (m_isSelectedButton)
            return;

        // 鼠标或触屏操作，注意不同平台的Input代码不同
#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR 
        bool press = Input.touches.Length > 0 ? true : false;  // 手指是否触屏
        float mx = 0;
        float my = 0;
        if (press)
        {
            if ( Input.GetTouch(0).phase == TouchPhase.Moved)  // 获得手指移动距离
            {
                mx = Input.GetTouch(0).deltaPosition.x * 0.01f;
                my = Input.GetTouch(0).deltaPosition.y * 0.01f;
            }
        }
#else
        bool press = Input.GetMouseButton(0);
        // 获得鼠标移动距离
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");

        
#endif
        // 移动摄像机
        GameCamera.Inst.Control(press, mx, my);
    }
    
    /// <summary>
    /// 更新文字组件
    /// </summary>
    private void LateUpdate()
    {
        try
        {
            m_objects = GameObject.FindGameObjectsWithTag("Tank");
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        foreach (var item in m_objects)
        {
            if (item.TryGetComponent<Enemy>(out enemy1))
            {
                m_speed = (int)enemy1.m_speed;
                m_pos = enemy1.transform.position;
                m_tankSize = enemy1.m_tankSize;
                SetSpeed(m_speed);
                SetPos(m_pos);
                SetTankSize(m_tankSize);
            }
            else if (item.TryGetComponent<AirEnemy>(out enemy2))
            {
                m_speed = (int)enemy2.m_speed;
                m_pos = enemy2.transform.position;
                m_tankSize = enemy2.m_tankSize;
                SetSpeed(m_speed);
                SetPos(m_pos);
                SetTankSize(m_tankSize);
            }
        }
        
    }

    /// <summary>
    /// 更新文字控件"地图信息"
    /// </summary>
    /// <param name="_mapSize"></param>
    public void SetMapSize(Vector3 _mapSize)
    {
        m_mapSize = _mapSize;
        m_txt_mapSize.text = string.Format("map:<color=orange>{0}</color>", m_mapSize);

    }

    // 更新文字控件"坦克速度"
    public void SetSpeed(int _speed)
    {
        m_speed = _speed;
        m_txt_speed.text = string.Format("speed:<color=orange>{0}</color>", m_speed);

    }
    // 更新文字控件"坦克尺寸"
    public void SetTankSize(Vector2 _tankSize)
    {
        m_tankSize = _tankSize;
        m_txt_tankSize.text = string.Format("size:<color=yellow>{0}</color>", m_tankSize);

    }
    // 更新文字控件"坦克坐标"
    public void SetPos(Vector3 _pos)
    {
        m_pos = _pos;
        m_txt_pos.text = string.Format("pos:<color=yellow>{0}</color>", m_pos);
    }


    // 更新文字控件"波数"
    public void SetWave(int wave)
    {
        m_wave= wave;
        m_txt_wave.text = string.Format("para3:<color=yellow>{0}/{1}</color>", m_wave, m_waveMax);

    }

    // 更新文字控件"生命"
    public void SetDamage(int damage)
    {
        m_life -= damage;
        if (m_life <= 0) {
            m_life = 0;
            m_but_try.gameObject.SetActive(true); //显示重新游戏按钮
        }
        m_txt_life.text = string.Format("para2:<color=yellow>{0}</color>", m_life);
    }

    // 更新文字控件"铜钱"
    public bool SetPoint(int point)
    {
        if (m_point + point < 0) // ͭ如果铜钱数量不够
            return false;
        m_point += point;
        m_txt_point.text = string.Format("para1:<color=yellow>{0}</color>", m_point);
        return true;
    }


    // 按下"创建防守单位按钮"
    void OnButCreateDefenderDown(BaseEventData data)
    {
        m_isSelectedButton = true;
    }

    // 抬起 "创建防守单位按钮" 创建防守单位
    void OnButCreateDefenderUp( BaseEventData data )
    {
        // 创建射线
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitinfo;
        // 检测是否与地面相碰撞
        if (Physics.Raycast(ray, out hitinfo, 1000, m_groundlayer))
        {
            // 如果选中的是一个可用的格子
            if (TileObject.Instance.getDataFromPosition(hitinfo.point.x, hitinfo.point.z) != (int)NodeType.wall)
            {
                // 获得碰撞点位置
                Vector3 hitpos = new Vector3(hitinfo.point.x, 0, hitinfo.point.z);
                // 获得Grid Object坐位位置
                Vector3 gridPos = TileObject.Instance.transform.position;
                // 获得格子大小
                float tilesize = TileObject.Instance.tileSize;
                // 计算出所点击格子的中心位置
                hitpos.x = gridPos.x + (int)((hitpos.x - gridPos.x) / tilesize) * tilesize + tilesize * 0.5f;
                hitpos.z = gridPos.z + (int)((hitpos.z - gridPos.z) / tilesize) * tilesize + tilesize * 0.5f;

                // 获得选择的按钮GameObject，将简单通过按钮名字判断选择了哪个按钮 
                GameObject go = data.selectedObject;
                
                if (go.name.Contains("tank"))  //如果按钮名字包括“tank”
                {
                    if (SetPoint(-1)) // 减1个point，然后创建坦克单位
                        Enemy.Create<Enemy>(hitpos, new Vector3(0, 180, 0));
                }
                else if (go.name.Contains("aero"))// 如果按钮名字包括“aero”
                {
                    if (SetPoint(-2)) // 减2个point，然后创建飞机单位
                        Enemy.Create<AirEnemy>(hitpos, new Vector3(0, 180, 0));
                }
            }
        }
        m_isSelectedButton = false;
    }

    [ContextMenu("BuildPath")]
    void BuildPath()
    {
        m_PathNodes = new List<PathNode>();
        // 通过路点的Tag查找所有的路点
        GameObject[] objs = GameObject.FindGameObjectsWithTag("pathnode");
        for (int i = 0; i < objs.Length; i++)
        {
            m_PathNodes.Add( objs[i].GetComponent<PathNode>() );
        }
    }


    void OnDrawGizmos()
    {
        if (!m_debug || m_PathNodes == null)
            return;

        Gizmos.color = Color.red;  // 将路点连线的颜色设为红色
        foreach (PathNode node in m_PathNodes) // 遍历路点
        {
            if (node.m_next != null)
            {   // 在路点间画出连接线
                Gizmos.DrawLine(node.transform.position, node.m_next.transform.position);
            }
        }
    }

}
