using UnityEngine;
[System.Serializable]
public class PosData
{    
    public int Seq = 0;
    public float Area = 0;
    public Vector3 resultPos;
    public Vector2 AreaMessage;
    public EnemyType enemyType;
    public PosData(int _Seq, Vector3 _resultPos, Vector2 _AreaMessage)
    {
        Seq = _Seq;
        resultPos = _resultPos;
        AreaMessage = _AreaMessage;
        Area = _AreaMessage.x * _AreaMessage.y;
    }
    public PosData(int _Seq,Vector3 _resultPos,Vector2 _AreaMessage,EnemyType _enemyType) 
    {
        Seq = _Seq;
        resultPos = _resultPos;
        AreaMessage = _AreaMessage;
        enemyType = _enemyType;
        Area = _AreaMessage.x * _AreaMessage.y;
    }    
}
