using System.Collections;
using UnityEngine;


public class Plate : MonoBehaviour
{
    private void OnEnable()
    {
        MyEventBus.SubscribeEvent(GameEventType.MoveBasket, MoveAndDestroy);
    }
    private void OnDisable()
    {
        MyEventBus.UnSubscribeEvent(GameEventType.MoveBasket, MoveAndDestroy);
    }

    private void MoveAndDestroy()
    {
        StartCoroutine(MoveObject(transform, new Vector3(-10, transform.position.y, 0)));

    }

    private IEnumerator MoveObject(Transform targetObject, Vector3 targetPosition)
    {
        float speed = 5f;
        while (Vector3.Distance(targetObject.position, targetPosition) > 0.01f)
        {
            targetObject.position = Vector3.Lerp(targetObject.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }
        targetObject.position = targetPosition;
        Destroy(gameObject);
    }
}


