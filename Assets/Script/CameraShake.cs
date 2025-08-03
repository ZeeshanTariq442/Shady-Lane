using System.Collections;
using UnityEngine;
public class CameraShake : MonoBehaviour
{

    private void OnEnable()
    {
        MyEventBus.SubscribeEvent(GameEventType.CameraShake, Shake);
    }
    private void OnDisable()
    {
        MyEventBus.UnSubscribeEvent(GameEventType.CameraShake, Shake);
    }
    private void Shake()
    {
        StartCoroutine(SmallCameraShake());
    }
    private IEnumerator SmallCameraShake(float duration = 0.4f, float magnitude = 0.04f)
    {
        yield return new WaitForSeconds(.5f);
        Vector3 originalPosition = transform.localPosition;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float offsetX = Random.Range(-magnitude, magnitude);
            float offsetY = Random.Range(-magnitude, magnitude);

            transform.localPosition = new Vector3(
                originalPosition.x + offsetX,
                originalPosition.y + offsetY,
                originalPosition.z
            );

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Reset camera position
        transform.localPosition = originalPosition;
    }
}


