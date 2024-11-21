using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class People 
{

        public People( int age) {Age = age; }

        public int Age { get; set; }  //ÄêÁä

}

    
public class Test : MonoBehaviour
{
    
public int CompareTo(People left, People right)
    {
        int x;

        if (left.Age > right.Age)
            x = 1;
        else if (left.Age == right.Age)
            x = 0;
        else
            x = -1;

        return x;
    }



    // Start is called before the first frame update
    void Start()
    {
        List<People> peopleList = new List<People>();
        peopleList.Add(new People(22));
        peopleList.Add(new People(24));
        peopleList.Add(new People( 18));
        peopleList.Add(new People( 16));
        peopleList.Add(new People( 30));
        peopleList.Sort(CompareTo);
        foreach(var people in peopleList) 
        {
            Debug.Log(people.Age.ToString());
        }
    }
}
