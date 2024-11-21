using UnityEngine;

public class AirEnemy : Enemy {
    public float High = 3.0f;

    // ��ʼ����ֵ
    protected override void Init()
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
        CreateModel("Mi24");
        //StartCoroutine(Attack()); // ִ�й����߼�
    }

    void Update () {
        RotateTo_pathnode();
        MoveTo_pathnode();
        Fly();
	}

    public void Fly()
    {
        float flyspeed = 0;
        if (this.transform.position.y < High){
            flyspeed = 1.0f;
        }
        this.transform.Translate(new Vector3(0, flyspeed * Time.deltaTime,0));
    }
}
