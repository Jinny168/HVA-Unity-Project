using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[AddComponentMenu("TankController")]
public class Manual : MonoBehaviour
{
    // �ٶ�
    public float m_speed = 20;  // ���˵��ƶ��ٶ�
    public float m_rotatespeed = 120;//���˵�ת���ٶ�
    public System.Action<Enemy> onDeath;  // ���˵������¼�
    public float m_radius = 2.0f;//���˵�·��Ӵ��뾶
                                 // ����

    public int m_life = 15;
    public int m_maxlife = 15; // ���˵��������
    // �ӵ�prefab
    public Transform m_rocket;
    protected Transform m_transform;

    public AudioClip m_shootClip;  // ����
    protected AudioSource m_audio;  // ����Դ
    public Transform m_explosionFX;  // ��ը��Ч
    protected Vector3 m_targetPos; // Ŀ��λ��

    public LayerMask m_inputMask; // ���������ײ��

    Transform m_lifebarObj;  // ���˵�UI������GameObject
    UnityEngine.UI.Slider m_lifebar; //������������ʾ��Slider

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

    void Move()
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
        // ʹ��Vector3�ṩ��MoveTowards��������ó�Ŀ���ƶ���λ��
        Vector3 pos = Vector3.MoveTowards(this.m_transform.position, m_targetPos, m_speed * Time.deltaTime);



        // ���µ�ǰλ��
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


    // ת��Ŀ��
    public void RotateTo()
    {
        var position = m_targetPos - transform.position;
        position.y = 0; // ��֤����תY��
        var targetRotation = Quaternion.LookRotation(position); // ���Ŀ����ת�Ƕ�
        float next = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetRotation.eulerAngles.y, //����м����ת�Ƕ�
            m_rotatespeed * Time.deltaTime);
        this.transform.eulerAngles = new Vector3(0, next, 0);
    }

    // ��Ŀ���ƶ�
    public void MoveTo()
    {
        Vector3 pos1 = this.transform.position;
        Vector3 pos2 = m_targetPos;
        float dist = Vector2.Distance(new Vector2(pos1.x, pos1.z), new Vector2(pos2.x, pos2.z));
        if (dist < 2.0f)
        {
            GameManager.Instance.SetDamage(1); // �۳�һ���˺�ֵ
            isMove = false;
        }

        this.transform.Translate(new Vector3(0, 0, m_speed * Time.deltaTime));
        //m_bar.SetPosition(this.transform.position, 4.0f);
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
}
