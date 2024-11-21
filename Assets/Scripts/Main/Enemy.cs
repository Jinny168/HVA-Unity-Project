using UnityEngine;
using System.Collections;
using UnityEngine.Android;
using static UnityEngine.GraphicsBuffer;
using System;
using AStar;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using static Defender;
using System.Text;
using UnityEngine.UIElements;
using System.Linq;
using Load;
using System.Runtime.CompilerServices;

public class Enemy : MonoBehaviour
{


    public string m_tankType;//敌人的型号

    public List<PosData> m_datas;

    public System.Action<Enemy> onArrive;

    public Dest m_dest;//敌人的目标

    public struct Dest 
    {
        public int m_seq;//Load序号
        public Vector3 m_point;//Load目标点
    }


    public enum EnemyState
    {
        idle,
        manual,
        pathnode,
        astar,
        besizerastar,
        load,
        plan
    }
    /// <summary>
    /// 敌人的各项属性
    /// </summary>
    [Observe("ObserveStateChange")]
    public EnemyState m_currentState; //敌人当前的运动模式
    public void ObserveStateChange()
    {
        Enemy enemyScript = this.GetComponent<Enemy>();//获取脚本
        if (enemyScript.m_currentState.ToString().Contains("astar") ) //astar或besizerastar
        {
            targetGameObject_astar = GameObject.FindGameObjectWithTag("target");  //将一个物体设置为A*寻路目标，进行单体寻路
            SetTargetPosition(targetGameObject_astar.transform.position);
            pathVectorList = ReducePathVectorList(pathVectorList);
            pathVectorList = ReducePathVectorList(pathVectorList);
            if (enemyScript.m_currentState.ToString().CompareTo("besizerastar") == 0) 
            {
                InitPoint();
            }
        }
        else if (enemyScript.m_currentState.ToString().CompareTo("load") == 0)
        {
            SetTargetPosition(m_dest.m_point);
            pathVectorList = ReducePathVectorList(pathVectorList);
        }
        Debug.Log("坦克进入" + enemyScript.m_currentState.ToString() + "运动模式");
    }
    [Observe("ObserveIsSpawn")]
    public bool m_isSpawn=false;//敌人是否自动生成
    public void ObserveIsSpawn()
    {
        EnemySpawner enemySpawnerScript = GameObject.FindGameObjectWithTag("TankSpawner").GetComponent<EnemySpawner>();//获取脚本
        enemySpawnerScript.enabled = m_isSpawn;

        if (enemySpawnerScript.enabled == false) 
        { 
            Debug.Log("坦克自动生成器已关闭"); 
        }
        else 
        {
            Debug.Log("坦克自动生成器已开启");
        }
    }
    
    public PathNode m_currentNode; // 敌人的当前路点
    [HideInInspector]
    public int m_life = 15;  // 敌人的生命
    [HideInInspector]
    public int m_maxlife = 15; // 敌人的最大生命

    public float m_speed = 8;  // 敌人的移动速度
                               
    Vector3 m_targetDir;//要旋转的目标的朝向

    public float m_rotatespeed = 120;//敌人的转向速度
    public System.Action<Enemy> onDeath;  // 敌人的死亡事件
    public float m_radius = 2.0f;//敌人的路点接触半径
    protected Transform m_transform;//敌人的位姿
    protected Vector3 m_targetPosition;//敌人的索敌坐标
    public Vector2 m_tankSize;//敌人的尺寸
    internal float m_obj;//敌人的适应度

    private int currentPathIndex = 0;
    private List<Vector3> pathVectorList;
    private AStarManager pathFinding;
    private GameObject targetGameObject_astar;

    public LayerMask m_inputMask;//鼠标射线碰撞层


    
    Transform m_lifebarObj;  // 敌人的UI生命条GameObject
    UnityEngine.UI.Slider m_lifebar; //控制生命条显示的Slider

    bool isMove = false;

    // 模型Prefab
    protected GameObject m_model;

    //besizer曲线所用参数
    
    int m_targetIndex;//要移动到的路径点的下标

    public LineRenderer lineRender;
    public static List<Vector3> lsPoint = new List<Vector3>();
    public int baseCount = 50;  //两个基础点之间的取点数量   值越大曲线就越平滑  但同时计算量也也越大
    


    // 静态函数 创建防守单位实例
    public static T Create<T>(Vector3 pos, Vector3 angle) where T : Enemy
    {   
        GameObject go = new GameObject("object");
        go.transform.position = pos;
        go.transform.eulerAngles = angle;
        T d = go.AddComponent<T>();
        d.Init();
        // 将自己所占格子的信息设为占用
        //TileObject.Instance.setDataFromPosition(d.transform.position.x, d.transform.position.z, (int)TileStatus.DEAD);

        return d;
    }

    // 初始化数值
    protected virtual void Init()
    {
        // 这里只是简单示范，在实际项目中，数值通常会从数据库或配置文件中读取
        m_speed = 8.0f;
        m_life = 20;
        m_maxlife = 20;

        if (m_currentNode == null) 
        { 
            PathNode[] pathNodes = PathNode.FindObjectsOfType<PathNode>();//设置路点
            foreach (var item in pathNodes)
            {
                if (item.name.Contains("start")) 
                { 
                    m_currentNode = item;
                }
            }
        }

        
        
        // 创建模型,这里的资源名称是写死的，实际的项目通常会从配置中读取
        CreateModel("T95");

        //StartCoroutine(Attack());// 执行攻击逻辑

    }
    // 创建模型
    protected virtual void CreateModel(string myname)
    {
        GameObject model = Resources.Load<GameObject>(myname);
        m_model = (GameObject)Instantiate(model, this.transform.position, this.transform.rotation, this.transform);
 
        //m_ani = m_model.GetComponent<Animator>();
    }



    void Awake()
    {
        m_inputMask = LayerMask.GetMask("ground");//鼠标射线碰撞层
        pathVectorList = new List<Vector3> { };                             //存储A*的寻路路点
        pathFinding = new AStarManager(SceneManager.GetActiveScene().name); //根据场景设置地图
        targetGameObject_astar = GameObject.FindGameObjectWithTag("target");  //将一个物体设置为A*寻路目标，进行单体寻路
        lineRender = this.gameObject.GetComponent<LineRenderer>();
    }


    void Start()
    {
        //更新队列，将其添加到队列，坦克阵亡后移出队列
        GameManager.Instance.m_EnemyList.Add(this);
        // 读取生命条prefab
        GameObject prefab = (GameObject)Resources.Load("Canvas3D");
        // 创建生命条
        m_lifebarObj = ((GameObject)Instantiate(prefab, Vector3.zero, Camera.main.transform.rotation, this.transform)).transform;
        m_lifebarObj.localPosition = new Vector3(0, 2.0f, 0);
        m_lifebarObj.localScale = new Vector3(0.02f, 0.02f, 0.02f);
        m_lifebar = m_lifebarObj.GetComponentInChildren<UnityEngine.UI.Slider>();
        // 更新生命条位置和角度
        StartCoroutine(UpdateLifebar());
        //获取坐标
        m_transform = this.transform;           //储存坦克自身坐标
        m_targetPosition = m_transform.position;//储存光标坐标
        if (m_currentState != EnemyState.load)
        {
            SetTargetPosition(targetGameObject_astar.transform.position);
            pathVectorList = ReducePathVectorList(pathVectorList);
            InitPoint();
        }
        else 
        {
            SetTargetPosition(m_dest.m_point);
            pathVectorList = ReducePathVectorList(pathVectorList);
        }
    }

    void Update()
    {
        switch (m_currentState)
        {
            case EnemyState.pathnode:
                Down();
                RotateTo_pathnode();
                MoveTo_pathnode();
                break;
            case EnemyState.manual:
                KeyInput();//检测键盘输入，根据键盘的上下左右移动，可注释掉
                Down();//物理下落，落至地面
                follow();//根据光标在屏幕的位置移动
                break;
            case EnemyState.astar:
                Down();
                MoveOp_astar();
                break;
            case EnemyState.besizerastar:
                Down();
                MoveOp_besizerastar();
                break;
            case EnemyState.load:
                Down();
                MoveOp_load();
                

                break;
            case EnemyState.idle:
                Down();
                //待定
                break;
        }
    }

    private void CheckRot_load()
    { 
        transform.rotation = Quaternion.Euler(new Vector3(0,-180,0));
    }

    private void SetWall()
    {
        Vector3 endPos_load=pathVectorList[pathVectorList.Count-1];

        TileObject tileObject = TileObject.Instance;
        
        float left  = endPos_load.x - m_tankSize.x/2;
        float right = endPos_load.x + m_tankSize.x/2;
        float down = endPos_load.z - m_tankSize.y / 2;
        float up = endPos_load.z + m_tankSize.y / 2;
        
        int gridLeft= Mathf.FloorToInt((left-tileObject.originalPos.x)/tileObject.tileSize);
        int gridRight = Mathf.FloorToInt((right-tileObject.originalPos.x) / tileObject.tileSize);
        int gridDown = Mathf.FloorToInt((down-tileObject.originalPos.z) / tileObject.tileSize);
        int gridUp= Mathf.FloorToInt((up - tileObject.originalPos.z) / tileObject.tileSize);

        int gridWidth = gridRight-gridLeft;
        int gridHeight = gridUp-gridDown;

        for (int i = 0; i <gridWidth; i++) 
        {
            for (int j = 0; j <gridHeight; j++)
            {
                tileObject.data[(gridLeft + i) * tileObject.zTileCount + gridDown+j] = 1;   
            }
        }
    }

    private void MoveOp_load()
    {
        if (pathVectorList != null)
        {
            Vector3 targetPosition_astar = pathVectorList[currentPathIndex];
            Vector3 endPosition_astar = pathVectorList[pathVectorList.Count - 1];
            if (Vector3.Distance(transform.position, targetPosition_astar) >= 2f && targetPosition_astar != endPosition_astar)//如果坦克与目标距离较远
            {

                RotateTo_astar(targetPosition_astar);//先转向它

                Debug.DrawLine(transform.position, targetPosition_astar, Color.black);//再画线


                MoveTo_astar(targetPosition_astar);
                //OldMoveTo_astar(targetPosition_astar);
            }
            else if (targetPosition_astar == endPosition_astar) //如果坦克与最终目标接近
            {
                Debug.DrawLine(transform.position, targetPosition_astar, Color.black);//画线
                transform.position = Vector3.MoveTowards(transform.position, endPosition_astar, m_speed * Time.deltaTime * 10);
                if (Vector3.Distance(transform.position, endPosition_astar) <= 1.0f)
                {
                    m_targetDir = endPosition_astar - transform.position;
                }
                transform.forward = Vector3.Lerp(transform.forward, m_targetDir, m_rotatespeed * Time.deltaTime * 10);
                if (Vector3.Distance(transform.position, endPosition_astar) <= 0.1f)
                {
                    CheckRot_load();
                    transform.position = endPosition_astar;
                    SetWall();
                    m_currentState = EnemyState.idle;
                    onArrive(this);
                    Invoke("StopMoving", 0.1f);
                }
            }
            else
            {
                if (endPosition_astar != targetPosition_astar)
                {
                    //控制台打印坦克坐标
                    Debug.LogFormat("坦克坐标为{0}", targetPosition_astar);
                }
                currentPathIndex++;
                if (currentPathIndex >= pathVectorList.Count)
                {
                    StopMoving();
                }
            }

        }
    }

    private void MoveOp_besizerastar()
    {

        
        //transform.position += transform.forward * moveSpeed * Time.deltaTime;
        //maxDistanceDelta最大的步长
        //Vector3.MoveTowards（起点坐标，终点坐标，最大步长），当距离终点较远时，每步走最大的步长的长度，返回要移动的位置
        //当靠近或抵达终点时，只会返回终点的坐标，不会越过
        transform.position = Vector3.MoveTowards(transform.position, lsPoint[m_targetIndex], m_speed * Time.deltaTime);
        //Rotate();
        if (Vector3.Distance(transform.position, lsPoint[m_targetIndex]) <= 1f)
        {
            m_targetIndex++;
            if (m_targetIndex >= lsPoint.Count)
            {
                this.enabled = false;
                return; //用return的原因是，更改enabled状态后，下一帧不会执行，这一帧会执行完
            }
            m_targetDir = lsPoint[m_targetIndex] - transform.position;
            transform.forward = Vector3.Lerp(transform.forward, m_targetDir, m_rotatespeed * Time.deltaTime);
            // transform.LookAt(way.Points[targetIndex]);
        }
        else if (Vector3.Distance(transform.position, targetGameObject_astar.transform.position) <= 1f)
        {
            CheckRot_besizerastar();
        }

    }
    void Rotate_besizerastar()
    {
        Vector3 dir = pathVectorList[m_targetIndex]- transform.position;
        float angle = Vector3.Angle(transform.forward, dir);

        angle = Mathf.Min(angle, m_rotatespeed);

        transform.Rotate(Vector3.Cross(transform.forward, dir) * angle);
    }

    private void MoveOp_astar()
    {
        if (pathVectorList != null)
        {
            Vector3 targetPosition_astar = pathVectorList[currentPathIndex];
            Vector3 endPosition_astar=pathVectorList[pathVectorList.Count-1];
            if (Vector3.Distance(transform.position, targetPosition_astar) >= 2f&&targetPosition_astar!=endPosition_astar)//如果坦克与目标距离较远
            {

                RotateTo_astar(targetPosition_astar);//先转向它

                Debug.DrawLine(transform.position, targetPosition_astar, Color.black);//再画线


                MoveTo_astar(targetPosition_astar);
                //OldMoveTo_astar(targetPosition_astar);
            }
            else if(targetPosition_astar==endPosition_astar) //如果坦克与最终目标接近
            {
                Debug.DrawLine(transform.position, targetPosition_astar, Color.black);//画线

                transform.position = Vector3.MoveTowards(transform.position, endPosition_astar, m_speed*Time.deltaTime);
                if (Vector3.Distance(transform.position, endPosition_astar) <= 2f)
                {
                    m_targetDir = endPosition_astar - transform.position;
                }
                transform.forward = Vector3.Lerp(transform.forward, m_targetDir, m_rotatespeed*Time.deltaTime*2);
            }
            else
            {
                if (endPosition_astar != targetPosition_astar)
                {
                    //控制台打印坦克坐标
                    Debug.LogFormat("坦克坐标为{0}", targetPosition_astar);
                }
                currentPathIndex++;
                if (currentPathIndex >= pathVectorList.Count)
                {
                    StopMoving();
                }
            }
            
        }
        //else//如果想持续追踪一个运动的目标《例如一个位置不固定的敌人》，就将这段代码的注释去掉
        //{
        //    StartCoroutine(FindTarget());//列表为空，就开启一个新的协程
        //}
    }
    /// <summary>
    /// 清空列表，并在控制台打印坦克坐标
    /// </summary>
    /// <param name="targetPosition_astar">目标坐标</param>
    private void StopMoving()
    {
        currentPathIndex = 0;
        pathVectorList = null;
    }

    IEnumerator FindTarget()
    {
        yield return new WaitForSeconds(1.5f);
        SetTargetPosition(targetGameObject_astar.transform.position);
    }

    public void SetTargetPosition(Vector3 targetPosition)
    {
        currentPathIndex = 0;

        Debug.LogFormat("坦克坐标为{0},目标坐标为{1}", transform.position, targetPosition);

        pathVectorList = pathFinding.FindPath(transform.position, targetPosition);
        if (pathVectorList != null && pathVectorList.Count > 1)
        {
            pathVectorList.RemoveAt(0);
        }
    }


    void follow()
    {
        if (Input.GetMouseButton(0))
        {
            // 获得鼠标屏幕位置
            Vector3 ms = Input.mousePosition;
            // 将屏幕位置转为射线
            Ray ray = Camera.main.ScreenPointToRay(ms);
            // 用来记录射线碰撞信息
            RaycastHit hitinfo;
            // 产生射线
            //LayerMask mask =new LayerMask();
            //mask.value = (int)Mathf.Pow(2.0f, (float)LayerMask.NameToLayer("plane"));
            bool iscast = Physics.Raycast(ray, out hitinfo, 1000, m_inputMask);
            if (iscast)
            {
                // 如果射中目标,记录射线碰撞点
                m_targetPosition = hitinfo.point;
                isMove = true;
            }
        }
        if (isMove == true)
        {
            RotateTo_manual();
            MoveTo_manual();
        }
    }
    private void KeyInput()
    {
        //纵向移动距离
        float movev = 0;

        //水平移动距离
        float moveh = 0;

        //按右键
        if (Input.GetKey(KeyCode.RightArrow))
        {
            isMove = false;
            movev += m_speed * Time.deltaTime;
        }

        // 按左键
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            isMove = false;
            movev -= m_speed * Time.deltaTime;
        }

        // 按下键
        if (Input.GetKey(KeyCode.DownArrow))
        {
            isMove = false;
            moveh -= m_speed * Time.deltaTime;
        }

        // 按上键
        if (Input.GetKey(KeyCode.UpArrow))
        {
            isMove = false;
            moveh += m_speed * Time.deltaTime;
        }

        //移动
        this.m_transform.Translate(new Vector3(movev, 0, moveh));
    }


    /// <summary>
    /// 模拟重力，如果坦克悬在空中，就让它落地
    /// </summary>
    public void Down()
    {
        float downspeed = 0;
        if (this.transform.position.y > 0.1f)
        {
            downspeed = 2.0f;
        }
        this.transform.Translate(new Vector3(0, -downspeed * Time.deltaTime, 0));
    }

    /// <summary>
    /// 转向，手动模式下
    /// </summary>
    public void RotateTo_manual()
    {
        var position = m_targetPosition - transform.position;
        position.y = 0; // 保证仅旋转Y轴
        var targetRotation = Quaternion.LookRotation(position); // 获得目标旋转角度
        float next = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetRotation.eulerAngles.y, //获得中间的旋转角度
            m_rotatespeed * Time.deltaTime);
        this.transform.eulerAngles = new Vector3(0, next, 0);
    }
    /// <summary>
    /// 转向，路点模式下
    /// </summary>
    public void RotateTo_pathnode()
    {
        var position = m_currentNode.transform.position - transform.position;
        position.y = 0; // 保证仅旋转Y轴
        var targetRotation = Quaternion.LookRotation(position); // 获得目标旋转角度
        float next = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetRotation.eulerAngles.y, //获得中间的旋转角度
            m_rotatespeed * Time.deltaTime);
        this.transform.eulerAngles = new Vector3(0, next, 0);
    }

    /// <summary>
    /// 转向，AStar模式下
    /// </summary>
    public void RotateTo_astar(Vector3 targetPosition_AStar)
    {
        var position = targetPosition_AStar - transform.position;
        position.y = 0; // 保证仅旋转Y轴
        var targetRotation = Quaternion.LookRotation(position); // 获得目标旋转角度
        float next = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetRotation.eulerAngles.y, //获得中间的旋转角度
            m_rotatespeed * Time.deltaTime);
        this.transform.eulerAngles = new Vector3(0, next, 0);
    }

    /// <summary>
    /// 向目标移动，手动模式下
    /// </summary>
    public void MoveTo_manual()
    {
        Vector3 pos1 = this.transform.position;
        Vector3 pos2 = m_targetPosition;
        float dist = Vector2.Distance(new Vector2(pos1.x, pos1.z), new Vector2(pos2.x, pos2.z));
        if (dist < 2.0f)
        {
            GameManager.Instance.SetDamage(1); // 扣除一点伤害值
            isMove = false;
        }

        this.transform.Translate(new Vector3(0, 0, m_speed * Time.deltaTime));
        //m_bar.SetPosition(this.transform.position, 4.0f);
    }

    /// <summary>
    /// 向目标移动，路点模式
    /// </summary>
    public void MoveTo_pathnode()
    {
        Vector3 pos1 = this.transform.position;
        Vector3 pos2 = m_currentNode.transform.position;
        float dist = Vector2.Distance(new Vector2(pos1.x, pos1.z), new Vector2(pos2.x, pos2.z));
        if (dist < 2.0f)
        {
            if (m_currentNode.m_next == null) // 没有路点，说明已经到达我方基地
            {
                CheckRot_pathnode();         //调整坦克的转向
                GameManager.Instance.SetDamage(1); // 扣除一点伤害值
                DestroyMe();  // 销毁自身
            }
            else
                m_currentNode = m_currentNode.m_next;  // 更新到下一个路点
        }

        this.transform.Translate(new Vector3(0, 0, m_speed * Time.deltaTime));
        //m_bar.SetPosition(this.transform.position, 4.0f);
    }

    /// <summary>
    /// 向目标移动，AStar模式
    /// </summary>
    public void MoveTo_astar(Vector3 targetPosition_astar)
    {
        Vector3 pos1 = this.transform.position;
        Vector3 pos2 = targetPosition_astar;

        float dist = Vector2.Distance(new Vector2(pos1.x, pos1.z), new Vector2(pos2.x, pos2.z));
        if (dist < 2.0f)
        {
            if (pathVectorList == null) // 列表为空，说明已经到达目标点
            {
                GameManager.Instance.SetDamage(1); // 扣除一点伤害值
                DestroyMe();  // 销毁自身
            }
        }

        this.transform.Translate(new Vector3(0, 0, m_speed * Time.deltaTime));
        //m_bar.SetPosition(this.transform.position, 4.0f);
    }

    private void CheckRot_pathnode()
    {

        float angleDifference = Quaternion.Angle(transform.rotation, m_currentNode.GetComponent<PathNode>().transform.rotation);

        float rotationSpeed = 60f; // 旋转速度
                                   // 使用插值方法逐渐将物体旋转到目标方向
        transform.rotation = Quaternion.Lerp(transform.rotation, m_currentNode.GetComponent<PathNode>().transform.rotation, rotationSpeed * Time.deltaTime);
    }
    private void CheckRot_besizerastar()
    {

        float angleDifference = Quaternion.Angle(transform.rotation, targetGameObject_astar.transform.rotation);

        float rotationSpeed = 60f; // 旋转速度
                                   // 使用插值方法逐渐将物体旋转到目标方向
        transform.rotation = Quaternion.Lerp(transform.rotation,targetGameObject_astar.transform.rotation, rotationSpeed * Time.deltaTime);
    }
    public void DestroyMe()
    {
        GameManager.Instance.m_EnemyList.Remove(this);
        onDeath(this);  // 发布消息
        Destroy(this.gameObject); // 注意在实际项目中一般不要直接调用Destroy
    }

    public void SetDamage(int damage)
    {
        m_life -= damage;
        if (m_life <= 0)
        {
            m_life = 0;
            // 每消灭一个敌人增加一些铜钱
            GameManager.Instance.SetPoint(5);
            DestroyMe();
        }
    }


    IEnumerator UpdateLifebar()
    {
        // 更新生命条的值
        m_lifebar.value = (float)m_life / (float)m_maxlife;
        // 更新角度，如终面向摄像机
        m_lifebarObj.transform.eulerAngles = Camera.main.transform.eulerAngles;
        yield return 0; // 没有任何等待
        StartCoroutine(UpdateLifebar());  // 循环执行
    }
    /// <summary>
    /// 精简路点个数，实现快速移动
    /// </summary>
    public List<Vector3> ReducePathVectorList(List<Vector3> _pathVectorList)
    {
        List<Vector3> _ReducePathVectorList = _pathVectorList;
        int i = 0;
        while (_ReducePathVectorList[i + 2] != _pathVectorList.Last())
        {
            Vector3 fromVector3 = (_ReducePathVectorList[i + 1] - _ReducePathVectorList[i]);
            Vector3 toVector3 = (_ReducePathVectorList[i + 2] - _ReducePathVectorList[i + 1]);
            float angleBetween = Vector3.Angle(fromVector3, toVector3);
            if (angleBetween == 0 | angleBetween == 180)
            {
                //Debug.Log("两个向量同向");
                _ReducePathVectorList.RemoveAt(i + 1);
            }
            else
            {
                //Debug.Log("两个向量不同向");
                i++;
            }
        }
        return _ReducePathVectorList;
    }


    #region 计算贝塞尔曲线的拟合点
    //初始化算出所有的点的信息
    void InitPoint()
    {
        //将向量列表转为向量数组
        Vector3[] pointPos = pathVectorList.ToArray();
        m_targetIndex = 0;
        transform.LookAt(pointPos[m_targetIndex]);
        m_targetDir = pointPos[m_targetIndex] - transform.position;

        GetTrackPoint(pointPos);
    }

    /// <summary>
    /// 根据设定节点绘制指定的曲线
    /// </summary>
    /// <param name="track">所有指定节点的信息</param>
    public void GetTrackPoint(Vector3[] track)
    {
        Vector3[] vector3s = PathControlPointGenerator(track);
        int SmoothAmount = track.Length * baseCount;
        lineRender.positionCount = SmoothAmount;
        for (int i = 1; i < SmoothAmount; i++)
        {
            float pm = (float)i / SmoothAmount;
            Vector3 currPt = Interp(vector3s, pm);
            lineRender.SetPosition(i, currPt);
            lsPoint.Add(currPt);
        }
    }

    /// <summary>
    /// 计算所有节点以及控制点坐标
    /// </summary>
    /// <param name="path">所有节点的存储数组</param>
    /// <returns></returns>
    public Vector3[] PathControlPointGenerator(Vector3[] path)
    {
        Vector3[] suppliedPath;
        Vector3[] vector3s;

        suppliedPath = path;
        int offset = 2;
        vector3s = new Vector3[suppliedPath.Length + offset];
        Array.Copy(suppliedPath, 0, vector3s, 1, suppliedPath.Length);//预留index=0和index=length-1两个点
        vector3s[0] = vector3s[1] + (vector3s[1] - vector3s[2]);//反向延长，设置index=0点的数值
        vector3s[vector3s.Length - 1] = vector3s[vector3s.Length - 2] + (vector3s[vector3s.Length - 2] - vector3s[vector3s.Length - 3]);//正向延长，设置index=length点的数值
        if (vector3s[1] == vector3s[vector3s.Length - 2])//原path为回环的情况
        {
            Vector3[] tmpLoopSpline = new Vector3[vector3s.Length];
            Array.Copy(vector3s, tmpLoopSpline, vector3s.Length);
            //修正vector3数组
            tmpLoopSpline[0] = tmpLoopSpline[tmpLoopSpline.Length - 3];//第二个回环点前面的那个点放在首位
            tmpLoopSpline[tmpLoopSpline.Length - 1] = tmpLoopSpline[2];//第一个回环点后面的那个点放在末位
            vector3s = new Vector3[tmpLoopSpline.Length];
            Array.Copy(tmpLoopSpline, vector3s, tmpLoopSpline.Length);
        }
        return (vector3s);
    }


    /// <summary>
    /// 计算曲线的任意点的位置
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public Vector3 Interp(Vector3[] pos, float t)
    {
        int length = pos.Length - 3;
        int currPt = Mathf.Min(Mathf.FloorToInt(t * length), length - 1);
        float u = t * (float)length - (float)currPt;
        Vector3 a = pos[currPt];
        Vector3 b = pos[currPt + 1];
        Vector3 c = pos[currPt + 2];
        Vector3 d = pos[currPt + 3];
        return .5f * (
           (-a + 3f * b - 3f * c + d) * (u * u * u)
           + (2f * a - 5f * b + 4f * c - d) * (u * u)
           + (-a + c) * u
           + 2f * b
       );
    }
    #endregion

    
}



