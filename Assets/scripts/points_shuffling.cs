using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class points_shuffling : MonoBehaviour
{
    
    public GameObject[] tasks;
    public GameObject[] pos;
    public  int[] randomNumList= new int[28];


    public List<int> randomList = new List<int>();
    int MyNumber = 0;

    public static points_shuffling ins;

    // Start is called before the first frame update
    void Start()
    {
        ins = this;
       
        NewNumber();
        System.Array.Sort(randomNumList);
        tasksPlace();
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
  
  

    void NewNumber()
    {
       
            for (int j = 0; j < 14; j++)
            {
                MyNumber = Random.Range(3, 166);
                if (!randomList.Contains(MyNumber))
                {
                    randomList.Add(MyNumber);
                    randomNumList[j] = MyNumber;
                }
                else
                {
                MyNumber = Random.Range(3, 166);
                if (!randomList.Contains(MyNumber))
                {
                    randomList.Add(MyNumber);
                    randomNumList[j] = MyNumber;
                }
            }
           }
    
        
    }
    void tasksPlace()
    {
      
        for (int i = 0; i < 14; i++)
        {
            if (pos != null)
            {
                tasks[i].transform.position = pos[randomNumList[i]].transform.position;
            }
        }
    }
    
  
}
