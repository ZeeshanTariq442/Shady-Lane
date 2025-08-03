using UnityEngine;
using System.Collections;
public class JumpEffect : MonoBehaviour
{
    [Header("Jump Settings")]
    public float jumpHeight = 2f;
    public float jumpDuration = 0.5f;

    [Header("Shake Settings")]
    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 0.1f;

    private Vector3 originalPosition;
    private bool isJumping = false;

    void Start()
    {
        originalPosition = transform.position;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            originalPosition = transform.position;
            StartCoroutine(JumpAndShakeEffect());
        }
    }
    private void PlayEffect()
    {
        if (GetComponent<ItemController>().isMoved && !isJumping)
        {
            originalPosition = transform.position;
            StartCoroutine(JumpAndShakeEffect());
        }
    }
    private void OnEnable()
    {
        MyEventBus.SubscribeEvent(GameEventType.CompleteTurnEffect, PlayEffect);
        MyEventBus.SubscribeEvent(GameEventType.LevelFailed, () => isJumping = true);
    }
    private void OnDisable()
    {
        MyEventBus.UnSubscribeEvent(GameEventType.CompleteTurnEffect, PlayEffect);
        MyEventBus.UnSubscribeEvent(GameEventType.LevelFailed, () => isJumping = true);
    }

    #region ANIMATION
    private IEnumerator JumpAndShakeEffect()
    {
        isJumping = true;
        // Jump up
        float elapsedTime = 0;
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + Vector3.up * jumpHeight;

        while (elapsedTime < jumpDuration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, (elapsedTime / jumpDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Jump down
        elapsedTime = 0;
        while (elapsedTime < jumpDuration)
        {
            transform.position = Vector3.Lerp(targetPos, startPos, (elapsedTime / jumpDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPosition;

        // Shake effect
        yield return StartCoroutine(Shake(shakeDuration, shakeMagnitude));

        isJumping = false;
    }

    private IEnumerator Shake(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Vector3 randomPoint = originalPosition + (Vector3)(Random.insideUnitCircle * magnitude);
            transform.position = randomPoint;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPosition;
    }
    #endregion
}

