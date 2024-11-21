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


    public string m_tankType;//���˵��ͺ�

    public List<PosData> m_datas;

    public System.Action<Enemy> onArrive;

    public Dest m_dest;//���˵�Ŀ��

    public struct Dest 
    {
        public int m_seq;//Load���
        public Vector3 m_point;//LoadĿ���
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
    /// ���˵ĸ�������
    /// </summary>
    [Observe("ObserveStateChange")]
    public EnemyState m_currentState; //���˵�ǰ���˶�ģʽ
    public void ObserveStateChange()
    {
        Enemy enemyScript = this.GetComponent<Enemy>();//��ȡ�ű�
        if (enemyScript.m_currentState.ToString().Contains("astar") ) //astar��besizerastar
        {
            targetGameObject_astar = GameObject.FindGameObjectWithTag("target");  //��һ����������ΪA*Ѱ·Ŀ�꣬���е���Ѱ·
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
        Debug.Log("̹�˽���" + enemyScript.m_currentState.ToString() + "�˶�ģʽ");
    }
    [Observe("ObserveIsSpawn")]
    public bool m_isSpawn=false;//�����Ƿ��Զ�����
    public void ObserveIsSpawn()
    {
        EnemySpawner enemySpawnerScript = GameObject.FindGameObjectWithTag("TankSpawner").GetComponent<EnemySpawner>();//��ȡ�ű�
        enemySpawnerScript.enabled = m_isSpawn;

        if (enemySpawnerScript.enabled == false) 
        { 
            Debug.Log("̹���Զ��������ѹر�"); 
        }
        else 
        {
            Debug.Log("̹���Զ��������ѿ���");
        }
    }
    
    public PathNode m_currentNode; // ���˵ĵ�ǰ·��
    [HideInInspector]
    public int m_life = 15;  // ���˵�����
    [HideInInspector]
    public int m_maxlife = 15; // ���˵��������

    public float m_speed = 8;  // ���˵��ƶ��ٶ�
                               
    Vector3 m_targetDir;//Ҫ��ת��Ŀ��ĳ���

    public float m_rotatespeed = 120;//���˵�ת���ٶ�
    public System.Action<Enemy> onDeath;  // ���˵������¼�
    public float m_radius = 2.0f;//���˵�·��Ӵ��뾶
    protected Transform m_transform;//���˵�λ��
    protected Vector3 m_targetPosition;//���˵���������
    public Vector2 m_tankSize;//���˵ĳߴ�
    internal float m_obj;//���˵���Ӧ��

    private int currentPathIndex = 0;
    private List<Vector3> pathVectorList;
    private AStarManager pathFinding;
    private GameObject targetGameObject_astar;

    public LayerMask m_inputMask;//���������ײ��


    
    Transform m_lifebarObj;  // ���˵�UI������GameObject
    UnityEngine.UI.Slider m_lifebar; //������������ʾ��Slider

    bool isMove = false;

    // ģ��Prefab
    protected GameObject m_model;

    //besizer�������ò���
    
    int m_targetIndex;//Ҫ�ƶ�����·������±�

    public LineRenderer lineRender;
    public static List<Vector3> lsPoint = new List<Vector3>();
    public int baseCount = 50;  //����������֮���ȡ������   ֵԽ�����߾�Խƽ��  ��ͬʱ������ҲҲԽ��
    


    // ��̬���� �������ص�λʵ��
    public static T Create<T>(Vector3 pos, Vector3 angle) where T : Enemy
    {   
        GameObject go = new GameObject("object");
        go.transform.position = pos;
        go.transform.eulerAngles = angle;
        T d = go.AddComponent<T>();
        d.Init();
        // ���Լ���ռ���ӵ���Ϣ��Ϊռ��
        //TileObject.Instance.setDataFromPosition(d.transform.position.x, d.transform.position.z, (int)TileStatus.DEAD);

        return d;
    }

    // ��ʼ����ֵ
    protected virtual void Init()
    {
        // ����ֻ�Ǽ�ʾ������ʵ����Ŀ�У���ֵͨ��������ݿ�������ļ��ж�ȡ
        m_speed = 8.0f;
        m_life = 20;
        m_maxlife = 20;

        if (m_currentNode == null) 
        { 
            PathNode[] pathNodes = PathNode.FindObjectsOfType<PathNode>();//����·��
            foreach (var item in pathNodes)
            {
                if (item.name.Contains("start")) 
                { 
                    m_currentNode = item;
                }
            }
        }

        
        
        // ����ģ��,�������Դ������д���ģ�ʵ�ʵ���Ŀͨ����������ж�ȡ
        CreateModel("T95");

        //StartCoroutine(Attack());// ִ�й����߼�

    }
    // ����ģ��
    protected virtual void CreateModel(string myname)
    {
        GameObject model = Resources.Load<GameObject>(myname);
        m_model = (GameObject)Instantiate(model, this.transform.position, this.transform.rotation, this.transform);
 
        //m_ani = m_model.GetComponent<Animator>();
    }



    void Awake()
    {
        m_inputMask = LayerMask.GetMask("ground");//���������ײ��
        pathVectorList = new List<Vector3> { };                             //�洢A*��Ѱ··��
        pathFinding = new AStarManager(SceneManager.GetActiveScene().name); //���ݳ������õ�ͼ
        targetGameObject_astar = GameObject.FindGameObjectWithTag("target");  //��һ����������ΪA*Ѱ·Ŀ�꣬���е���Ѱ·
        lineRender = this.gameObject.GetComponent<LineRenderer>();
    }


    void Start()
    {
        //���¶��У�������ӵ����У�̹���������Ƴ�����
        GameManager.Instance.m_EnemyList.Add(this);
        // ��ȡ������prefab
        GameObject prefab = (GameObject)Resources.Load("Canvas3D");
        // ����������
        m_lifebarObj = ((GameObject)Instantiate(prefab, Vector3.zero, Camera.main.transform.rotation, this.transform)).transform;
        m_lifebarObj.localPosition = new Vector3(0, 2.0f, 0);
        m_lifebarObj.localScale = new Vector3(0.02f, 0.02f, 0.02f);
        m_lifebar = m_lifebarObj.GetComponentInChildren<UnityEngine.UI.Slider>();
        // ����������λ�úͽǶ�
        StartCoroutine(UpdateLifebar());
        //��ȡ����
        m_transform = this.transform;           //����̹����������
        m_targetPosition = m_transform.position;//����������
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
                KeyInput();//���������룬���ݼ��̵����������ƶ�����ע�͵�
                Down();//�������䣬��������
                follow();//���ݹ������Ļ��λ���ƶ�
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
                //����
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
            if (Vector3.Distance(transform.position, targetPosition_astar) >= 2f && targetPosition_astar != endPosition_astar)//���̹����Ŀ������Զ
            {

                RotateTo_astar(targetPosition_astar);//��ת����

                Debug.DrawLine(transform.position, targetPosition_astar, Color.black);//�ٻ���


                MoveTo_astar(targetPosition_astar);
                //OldMoveTo_astar(targetPosition_astar);
            }
            else if (targetPosition_astar == endPosition_astar) //���̹��������Ŀ��ӽ�
            {
                Debug.DrawLine(transform.position, targetPosition_astar, Color.black);//����
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
                    //����̨��ӡ̹������
                    Debug.LogFormat("̹������Ϊ{0}", targetPosition_astar);
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
        //maxDistanceDelta���Ĳ���
        //Vector3.MoveTowards��������꣬�յ����꣬��󲽳������������յ��Զʱ��ÿ�������Ĳ����ĳ��ȣ�����Ҫ�ƶ���λ��
        //��������ִ��յ�ʱ��ֻ�᷵���յ�����꣬����Խ��
        transform.position = Vector3.MoveTowards(transform.position, lsPoint[m_targetIndex], m_speed * Time.deltaTime);
        //Rotate();
        if (Vector3.Distance(transform.position, lsPoint[m_targetIndex]) <= 1f)
        {
            m_targetIndex++;
            if (m_targetIndex >= lsPoint.Count)
            {
                this.enabled = false;
                return; //��return��ԭ���ǣ�����enabled״̬����һ֡����ִ�У���һ֡��ִ����
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
            if (Vector3.Distance(transform.position, targetPosition_astar) >= 2f&&targetPosition_astar!=endPosition_astar)//���̹����Ŀ������Զ
            {

                RotateTo_astar(targetPosition_astar);//��ת����

                Debug.DrawLine(transform.position, targetPosition_astar, Color.black);//�ٻ���


                MoveTo_astar(targetPosition_astar);
                //OldMoveTo_astar(targetPosition_astar);
            }
            else if(targetPosition_astar==endPosition_astar) //���̹��������Ŀ��ӽ�
            {
                Debug.DrawLine(transform.position, targetPosition_astar, Color.black);//����

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
                    //����̨��ӡ̹������
                    Debug.LogFormat("̹������Ϊ{0}", targetPosition_astar);
                }
                currentPathIndex++;
                if (currentPathIndex >= pathVectorList.Count)
                {
                    StopMoving();
                }
            }
            
        }
        //else//��������׷��һ���˶���Ŀ�꡶����һ��λ�ò��̶��ĵ��ˡ����ͽ���δ����ע��ȥ��
        //{
        //    StartCoroutine(FindTarget());//�б�Ϊ�գ��Ϳ���һ���µ�Э��
        //}
    }
    /// <summary>
    /// ����б����ڿ���̨��ӡ̹������
    /// </summary>
    /// <param name="targetPosition_astar">Ŀ������</param>
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

        Debug.LogFormat("̹������Ϊ{0},Ŀ������Ϊ{1}", transform.position, targetPosition);

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
            // ��������Ļλ��
            Vector3 ms = Input.mousePosition;
            // ����Ļλ��תΪ����
            Ray ray = Camera.main.ScreenPointToRay(ms);
            // ������¼������ײ��Ϣ
            RaycastHit hitinfo;
            // ��������
            //LayerMask mask =new LayerMask();
            //mask.value = (int)Mathf.Pow(2.0f, (float)LayerMask.NameToLayer("plane"));
            bool iscast = Physics.Raycast(ray, out hitinfo, 1000, m_inputMask);
            if (iscast)
            {
                // �������Ŀ��,��¼������ײ��
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
        //�����ƶ�����
        float movev = 0;

        //ˮƽ�ƶ�����
        float moveh = 0;

        //���Ҽ�
        if (Input.GetKey(KeyCode.RightArrow))
        {
            isMove = false;
            movev += m_speed * Time.deltaTime;
        }

        // �����
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            isMove = false;
            movev -= m_speed * Time.deltaTime;
        }

        // ���¼�
        if (Input.GetKey(KeyCode.DownArrow))
        {
            isMove = false;
            moveh -= m_speed * Time.deltaTime;
        }

        // ���ϼ�
        if (Input.GetKey(KeyCode.UpArrow))
        {
            isMove = false;
            moveh += m_speed * Time.deltaTime;
        }

        //�ƶ�
        this.m_transform.Translate(new Vector3(movev, 0, moveh));
    }


    /// <summary>
    /// ģ�����������̹�����ڿ��У����������
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
    /// ת���ֶ�ģʽ��
    /// </summary>
    public void RotateTo_manual()
    {
        var position = m_targetPosition - transform.position;
        position.y = 0; // ��֤����תY��
        var targetRotation = Quaternion.LookRotation(position); // ���Ŀ����ת�Ƕ�
        float next = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetRotation.eulerAngles.y, //����м����ת�Ƕ�
            m_rotatespeed * Time.deltaTime);
        this.transform.eulerAngles = new Vector3(0, next, 0);
    }
    /// <summary>
    /// ת��·��ģʽ��
    /// </summary>
    public void RotateTo_pathnode()
    {
        var position = m_currentNode.transform.position - transform.position;
        position.y = 0; // ��֤����תY��
        var targetRotation = Quaternion.LookRotation(position); // ���Ŀ����ת�Ƕ�
        float next = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetRotation.eulerAngles.y, //����м����ת�Ƕ�
            m_rotatespeed * Time.deltaTime);
        this.transform.eulerAngles = new Vector3(0, next, 0);
    }

    /// <summary>
    /// ת��AStarģʽ��
    /// </summary>
    public void RotateTo_astar(Vector3 targetPosition_AStar)
    {
        var position = targetPosition_AStar - transform.position;
        position.y = 0; // ��֤����תY��
        var targetRotation = Quaternion.LookRotation(position); // ���Ŀ����ת�Ƕ�
        float next = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetRotation.eulerAngles.y, //����м����ת�Ƕ�
            m_rotatespeed * Time.deltaTime);
        this.transform.eulerAngles = new Vector3(0, next, 0);
    }

    /// <summary>
    /// ��Ŀ���ƶ����ֶ�ģʽ��
    /// </summary>
    public void MoveTo_manual()
    {
        Vector3 pos1 = this.transform.position;
        Vector3 pos2 = m_targetPosition;
        float dist = Vector2.Distance(new Vector2(pos1.x, pos1.z), new Vector2(pos2.x, pos2.z));
        if (dist < 2.0f)
        {
            GameManager.Instance.SetDamage(1); // �۳�һ���˺�ֵ
            isMove = false;
        }

        this.transform.Translate(new Vector3(0, 0, m_speed * Time.deltaTime));
        //m_bar.SetPosition(this.transform.position, 4.0f);
    }

    /// <summary>
    /// ��Ŀ���ƶ���·��ģʽ
    /// </summary>
    public void MoveTo_pathnode()
    {
        Vector3 pos1 = this.transform.position;
        Vector3 pos2 = m_currentNode.transform.position;
        float dist = Vector2.Distance(new Vector2(pos1.x, pos1.z), new Vector2(pos2.x, pos2.z));
        if (dist < 2.0f)
        {
            if (m_currentNode.m_next == null) // û��·�㣬˵���Ѿ������ҷ�����
            {
                CheckRot_pathnode();         //����̹�˵�ת��
                GameManager.Instance.SetDamage(1); // �۳�һ���˺�ֵ
                DestroyMe();  // ��������
            }
            else
                m_currentNode = m_currentNode.m_next;  // ���µ���һ��·��
        }

        this.transform.Translate(new Vector3(0, 0, m_speed * Time.deltaTime));
        //m_bar.SetPosition(this.transform.position, 4.0f);
    }

    /// <summary>
    /// ��Ŀ���ƶ���AStarģʽ
    /// </summary>
    public void MoveTo_astar(Vector3 targetPosition_astar)
    {
        Vector3 pos1 = this.transform.position;
        Vector3 pos2 = targetPosition_astar;

        float dist = Vector2.Distance(new Vector2(pos1.x, pos1.z), new Vector2(pos2.x, pos2.z));
        if (dist < 2.0f)
        {
            if (pathVectorList == null) // �б�Ϊ�գ�˵���Ѿ�����Ŀ���
            {
                GameManager.Instance.SetDamage(1); // �۳�һ���˺�ֵ
                DestroyMe();  // ��������
            }
        }

        this.transform.Translate(new Vector3(0, 0, m_speed * Time.deltaTime));
        //m_bar.SetPosition(this.transform.position, 4.0f);
    }

    private void CheckRot_pathnode()
    {

        float angleDifference = Quaternion.Angle(transform.rotation, m_currentNode.GetComponent<PathNode>().transform.rotation);

        float rotationSpeed = 60f; // ��ת�ٶ�
                                   // ʹ�ò�ֵ�����𽥽�������ת��Ŀ�귽��
        transform.rotation = Quaternion.Lerp(transform.rotation, m_currentNode.GetComponent<PathNode>().transform.rotation, rotationSpeed * Time.deltaTime);
    }
    private void CheckRot_besizerastar()
    {

        float angleDifference = Quaternion.Angle(transform.rotation, targetGameObject_astar.transform.rotation);

        float rotationSpeed = 60f; // ��ת�ٶ�
                                   // ʹ�ò�ֵ�����𽥽�������ת��Ŀ�귽��
        transform.rotation = Quaternion.Lerp(transform.rotation,targetGameObject_astar.transform.rotation, rotationSpeed * Time.deltaTime);
    }
    public void DestroyMe()
    {
        GameManager.Instance.m_EnemyList.Remove(this);
        onDeath(this);  // ������Ϣ
        Destroy(this.gameObject); // ע����ʵ����Ŀ��һ�㲻Ҫֱ�ӵ���Destroy
    }

    public void SetDamage(int damage)
    {
        m_life -= damage;
        if (m_life <= 0)
        {
            m_life = 0;
            // ÿ����һ����������һЩͭǮ
            GameManager.Instance.SetPoint(5);
            DestroyMe();
        }
    }


    IEnumerator UpdateLifebar()
    {
        // ������������ֵ
        m_lifebar.value = (float)m_life / (float)m_maxlife;
        // ���½Ƕȣ��������������
        m_lifebarObj.transform.eulerAngles = Camera.main.transform.eulerAngles;
        yield return 0; // û���κεȴ�
        StartCoroutine(UpdateLifebar());  // ѭ��ִ��
    }
    /// <summary>
    /// ����·�������ʵ�ֿ����ƶ�
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
                //Debug.Log("��������ͬ��");
                _ReducePathVectorList.RemoveAt(i + 1);
            }
            else
            {
                //Debug.Log("����������ͬ��");
                i++;
            }
        }
        return _ReducePathVectorList;
    }


    #region ���㱴�������ߵ���ϵ�
    //��ʼ��������еĵ����Ϣ
    void InitPoint()
    {
        //�������б�תΪ��������
        Vector3[] pointPos = pathVectorList.ToArray();
        m_targetIndex = 0;
        transform.LookAt(pointPos[m_targetIndex]);
        m_targetDir = pointPos[m_targetIndex] - transform.position;

        GetTrackPoint(pointPos);
    }

    /// <summary>
    /// �����趨�ڵ����ָ��������
    /// </summary>
    /// <param name="track">����ָ���ڵ����Ϣ</param>
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
    /// �������нڵ��Լ����Ƶ�����
    /// </summary>
    /// <param name="path">���нڵ�Ĵ洢����</param>
    /// <returns></returns>
    public Vector3[] PathControlPointGenerator(Vector3[] path)
    {
        Vector3[] suppliedPath;
        Vector3[] vector3s;

        suppliedPath = path;
        int offset = 2;
        vector3s = new Vector3[suppliedPath.Length + offset];
        Array.Copy(suppliedPath, 0, vector3s, 1, suppliedPath.Length);//Ԥ��index=0��index=length-1������
        vector3s[0] = vector3s[1] + (vector3s[1] - vector3s[2]);//�����ӳ�������index=0�����ֵ
        vector3s[vector3s.Length - 1] = vector3s[vector3s.Length - 2] + (vector3s[vector3s.Length - 2] - vector3s[vector3s.Length - 3]);//�����ӳ�������index=length�����ֵ
        if (vector3s[1] == vector3s[vector3s.Length - 2])//ԭpathΪ�ػ������
        {
            Vector3[] tmpLoopSpline = new Vector3[vector3s.Length];
            Array.Copy(vector3s, tmpLoopSpline, vector3s.Length);
            //����vector3����
            tmpLoopSpline[0] = tmpLoopSpline[tmpLoopSpline.Length - 3];//�ڶ����ػ���ǰ����Ǹ��������λ
            tmpLoopSpline[tmpLoopSpline.Length - 1] = tmpLoopSpline[2];//��һ���ػ��������Ǹ������ĩλ
            vector3s = new Vector3[tmpLoopSpline.Length];
            Array.Copy(tmpLoopSpline, vector3s, tmpLoopSpline.Length);
        }
        return (vector3s);
    }


    /// <summary>
    /// �������ߵ�������λ��
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



