using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waypoints : MonoBehaviour
{
    public Transform[] childObjects;
    public List<Transform> childNodeList = new List<Transform>();
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        fillnodes();
        for (int i = 0; i < childNodeList.Count; i++)
        {
            Vector3 currentpos = childNodeList[i].position;
            if (i > 0)
            {
                Vector3 previousPos = childNodeList[i - 1].position;
                Gizmos.DrawLine(previousPos, currentpos); 
            }
        }
    }
    void fillnodes()
    {
        childNodeList.Clear();
        childObjects = GetComponentsInChildren<Transform>();

        foreach(Transform child in childObjects)
        {
            if(child != this.transform)
            {
                childNodeList.Add(child);
            }
        }
    }
}
