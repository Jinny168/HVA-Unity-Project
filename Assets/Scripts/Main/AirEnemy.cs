using UnityEngine;

public class AirEnemy : Enemy {
    public float High = 3.0f;

    // 初始化数值
    protected override void Init()
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
        CreateModel("Mi24");
        //StartCoroutine(Attack()); // 执行攻击逻辑
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
