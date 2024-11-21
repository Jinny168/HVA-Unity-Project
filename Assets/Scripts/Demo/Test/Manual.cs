using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[AddComponentMenu("TankController")]
public class Manual : MonoBehaviour
{
    // 速度
    public float m_speed = 20;  // 敌人的移动速度
    public float m_rotatespeed = 120;//敌人的转向速度
    public System.Action<Enemy> onDeath;  // 敌人的死亡事件
    public float m_radius = 2.0f;//敌人的路点接触半径
                                 // 生命

    public int m_life = 15;
    public int m_maxlife = 15; // 敌人的最大生命
    // 子弹prefab
    public Transform m_rocket;
    protected Transform m_transform;

    public AudioClip m_shootClip;  // 声音
    protected AudioSource m_audio;  // 声音源
    public Transform m_explosionFX;  // 爆炸特效
    protected Vector3 m_targetPos; // 目标位置

    public LayerMask m_inputMask; // 鼠标射线碰撞层

    Transform m_lifebarObj;  // 敌人的UI生命条GameObject
    UnityEngine.UI.Slider m_lifebar; //控制生命条显示的Slider

    bool isMove = false;

    void Start()
    {
        m_transform = this.transform;
        m_targetPos = this.m_transform.position;
    }


    void Update()
    {
        KeyInput();
        Down();
        Move();
    }//Update

   

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

    void Move()
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
                m_targetPos = hitinfo.point;
                isMove = true;
            }
        }

        if (isMove == true)
        {
            RotateTo();
            MoveTo();
        }
    }

    private void OldMove()
    {
        // 使用Vector3提供的MoveTowards函数，获得朝目标移动的位置
        Vector3 pos = Vector3.MoveTowards(this.m_transform.position, m_targetPos, m_speed * Time.deltaTime);



        // 更新当前位置
        this.m_transform.position = pos;
    }

    public void Down()
    {
        float downspeed = 0;
        if (this.transform.position.y > 0.1f)
        {
            downspeed = 2.0f;
        }
        this.transform.Translate(new Vector3(0, -downspeed * Time.deltaTime, 0));
    }


    // 转向目标
    public void RotateTo()
    {
        var position = m_targetPos - transform.position;
        position.y = 0; // 保证仅旋转Y轴
        var targetRotation = Quaternion.LookRotation(position); // 获得目标旋转角度
        float next = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetRotation.eulerAngles.y, //获得中间的旋转角度
            m_rotatespeed * Time.deltaTime);
        this.transform.eulerAngles = new Vector3(0, next, 0);
    }

    // 向目标移动
    public void MoveTo()
    {
        Vector3 pos1 = this.transform.position;
        Vector3 pos2 = m_targetPos;
        float dist = Vector2.Distance(new Vector2(pos1.x, pos1.z), new Vector2(pos2.x, pos2.z));
        if (dist < 2.0f)
        {
            GameManager.Instance.SetDamage(1); // 扣除一点伤害值
            isMove = false;
        }

        this.transform.Translate(new Vector3(0, 0, m_speed * Time.deltaTime));
        //m_bar.SetPosition(this.transform.position, 4.0f);
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
}
