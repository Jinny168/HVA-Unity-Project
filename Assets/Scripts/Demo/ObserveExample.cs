using UnityEngine;

public class ObserveExample : MonoBehaviour
{
    [Observe("Callback")]
    public string
        hoge;

    [Observe("Callback2")]
    public Test
        test;

    public enum Test
    {
        Hoge,
        Fuga
    }

    public void Callback()
    {

        Debug.Log("call");
        var i = this.GetComponent<ObserveExample>();
        Debug.Log(i.hoge.ToString());
    
    }

    private void Callback2()
    {
        Debug.Log("call2");
        var i = this.GetComponent<ObserveExample>();
        Debug.Log(i.test.ToString());
    }
}