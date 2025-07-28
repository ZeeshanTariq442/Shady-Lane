using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stone : MonoBehaviour
{
    public waypoints currentWayPoint;
    int waypointPos;

    public int steps;

    bool isMoving;

    public static stone ins;
    private void Start()
    {
        ins = this;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isMoving)
        {
            if (!isMoving)
            {
                steps = Random.Range(1, 7);

                if (waypointPos + steps < currentWayPoint.childNodeList.Count)
                {
                    StartCoroutine(Move());
                }
                else
                {

                    print("list ended");
                }
            }
        }
    }
    public void movePlayer()
    {
        
        
    }
    IEnumerator Move()
     {
        if (isMoving)
        {
            yield break;
        }
        isMoving = true;
        while (steps > 0)
        {
            Vector3 nextPos = currentWayPoint.childNodeList[waypointPos + 1].position;
            while (moveToNextNode(nextPos))
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.1f);
            steps--;
            waypointPos++;
        }




        isMoving = false;
     }
    bool moveToNextNode(Vector3 goal)
    {
        return goal != (transform.position = Vector3.MoveTowards(transform.position, goal, 10f * Time.deltaTime));
        
    }
}
