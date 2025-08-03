using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpriteHorizontalLayout : MonoBehaviour
{
    public float spacing = .2f;
    public float maxSpriteSize = .8f;
    private Vector3 startPos;
    public bool isTile;
    public bool isBasket;
    private void Awake()
    {
        startPos = transform.position;
        if(!isBasket)
        transform.position = new Vector3(5, startPos.y, 0);

    }
    private IEnumerator MoveAnimation()
    {
        float speed = 15f;
        Invoke("PlayMoveSound", .2f);
        while (Vector3.Distance(transform.position, startPos) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, startPos, speed * Time.deltaTime);

            yield return null;
        }
        //    EventBus.RaiseEvent(EventType.ShakeBottle);
        transform.position = startPos;

    }

    public void ArrangeSpritesHorizontally()
    {
        List<SpriteRenderer> spriteRenderers;
        spriteRenderers = new List<SpriteRenderer>();


        foreach (var spriteRenderer in GetComponentsInChildren<SpriteRenderer>())
        {
            if (isTile)
            {
                spriteRenderers.Add(spriteRenderer);
            }
            else
            {
                if (spriteRenderer.transform.GetComponent<ItemController>() != null)
                {
                    spriteRenderers.Add(spriteRenderer);
                }
                if (spriteRenderer.transform.GetComponent<BasketHandler>() != null)
                {
                    spriteRenderers.Add(spriteRenderer);
                }
            }

        }

        if (spriteRenderers.Count == 0) 
            return;

        if (spriteRenderers.Count <= 3)
            spacing = 0.2f;
        else if (spriteRenderers.Count <= 5)
            spacing = 0.1f;
        else
            spacing = 0;

        float totalSpriteWidth = 0f;

        foreach (var spriteRenderer in spriteRenderers)
        {
            totalSpriteWidth += spriteRenderer.bounds.size.x;
        }

        Camera mainCamera = Camera.main;
        float screenHeight = mainCamera.orthographicSize * 2;
        float screenWidth = screenHeight * mainCamera.aspect;

        float totalSpace = 0;
        for (int i = 0; i < spriteRenderers.Count; i++)
        {
            totalSpace += spriteRenderers[i].bounds.size.x + spacing;
        }

        float startX =  - ( (totalSpace - spacing)/2);
        for (int i = 0; i < spriteRenderers.Count; i++)
        {
            float spriteWidth = spriteRenderers[i].bounds.size.x;

            spriteRenderers[i].transform.localPosition = new Vector3(startX + spriteWidth / 2, spriteRenderers[i].transform.localPosition.y, 0);
            startX += spriteWidth + spacing;
        }
        if (!isBasket)
            StartCoroutine(MoveAnimation());
    }

    private void PlayMoveSound() 
    {
        SoundManager.instance?.PlayOneShot(SoundManager.instance.move);
    }

}

