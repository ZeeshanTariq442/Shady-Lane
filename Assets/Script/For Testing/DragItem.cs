using System.Collections;
using UnityEngine;

public class DragItem : MonoBehaviour
{
    private Vector3 offset;
    private bool isDragging = false;

    public LayerMask slabLayer;
    public float overLapRadius = 0.3f;
    public ItemController controller;
    public float minDragDistance = 0.2f;
    public Vector3 manualOffset;

    private Vector3 dragStartPosition;
    private Collider2D currentHoveredSlab;
    private Vector3 originalSlabScale;
    private PolygonCollider2D itemCollider;
    private PolygonCollider2D[] overlapResults = new PolygonCollider2D[5];
    private bool applyManualOffset = false;
    private Coroutine checkItemStateCoroutine;
    private bool CheckItem;
    private Transform lastHit;
    private TutorialHandDrag TutorialHand;
    private void Start()
    {
        // Auto-assign
        if (itemCollider == null)
            itemCollider = gameObject.AddComponent<PolygonCollider2D>();

        itemCollider = gameObject.GetComponent<PolygonCollider2D>();

        TutorialHand = FindFirstObjectByType<TutorialHandDrag>();
    }

    public void SetDraggingFalse()
    {
        isDragging = false;
    }
    public void MouseDown()
    {
        offset = transform.position - GetMouseWorldPosition();
        isDragging = true;
        dragStartPosition = transform.localPosition;
        applyManualOffset = controller.isMoved ? false : true;
        if (PlayerPrefsHandler.GetInt(PlayerPrefsHandler.NewTutorialPrefKey, 0) == 0)
        {
            TutorialHand?.ChangeMsg(1);
        }
    }

    public void MouseDrag()
    {
        if (isDragging)
        {
            Vector3 mousePos = GetMouseWorldPosition() + offset;
            if (applyManualOffset)
            {
                transform.position = mousePos + manualOffset;
            }
            else
            {
                transform.position = mousePos;
            }

            // Check for overlap during dragging
            //   Collider2D hit = Physics2D.OverlapCircle(transform.position, overLapRadius, slabLayer);
            if (CheckOverlapWithSlab(out PolygonCollider2D hit))
            {
                if (hit != null && hit.CompareTag("slab"))
                {
                    if (hit.GetComponent<TileData>().isPlaced)
                    {
                        if(hit.transform.childCount >= 1 && controller.id != hit.transform.GetChild(0).GetComponent<ItemController>().id)
                        {
                            ResetPreviousSlab();
                            return;
                        }
                        else
                        {
                            float dragDistance = Vector3.Distance(hit.transform.localPosition, transform.localPosition);
                            if (dragDistance <= 0.3)
                                HighlightHoveredSlab(hit);
                        }
                        
                    }
                    if (currentHoveredSlab != hit)
                    {
                        HighlightHoveredSlab(hit);
                    }
                }
            }
            else
            {
                ResetPreviousSlab();
            }
        }
        if (PlayerPrefsHandler.GetInt(PlayerPrefsHandler.NewTutorialPrefKey, 0) == 0)
        {
            TutorialHand?.DisableTutorial(false);
        }

    }

    private void HighlightHoveredSlab(Collider2D hit)
    {
        if (PlayerPrefsHandler.GetInt(PlayerPrefsHandler.NewTutorialPrefKey, 0) == 0)
        {
            TutorialHand?.ChangeMsg(2);
        }
        applyManualOffset = true;
        ResetPreviousSlab();
        currentHoveredSlab = hit;
        originalSlabScale = hit.transform.localScale;
        hit.transform.localScale = originalSlabScale * 1.1f; // Slightly enlarge
    }
    private bool CheckOverlapWithSlab(out PolygonCollider2D slab)
    {
        slab = null;

        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(slabLayer);
        filter.useTriggers = true;

        int count = itemCollider.Overlap(filter, overlapResults);
        if (count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                if (overlapResults[i] != null && overlapResults[i].CompareTag("slab"))
                {
                    slab = overlapResults[i];
                    return true;
                }
            }
        }

        return false;
    }
    public void MouseUp()
    {
        isDragging = false;
        ResetPreviousSlab();
        CheckItem = false;
        if (CheckOverlapWithSlab(out PolygonCollider2D hit))
        {
            if (hit != null && hit.CompareTag("slab"))
            {
                lastHit = hit.transform;
                if (hit.transform.childCount > 1)
                {
                    controller?.OnItemClick();
                    return;
                }
                Debug.Log(" Top Upper IN");
                if (hit.GetComponent<TileData>().isPlaced)
                {
                    float dragDistance = Vector3.Distance(dragStartPosition, transform.localPosition);
                    if (dragDistance >= minDragDistance)
                    {
                        Debug.Log("Upper IN");
                        controller?.SnapToNearestPosition(hit.transform);
                    }else
                        controller?.SnapToNearestPosition(hit.transform);
                    return;
                }
                Debug.Log("Item placed on slab!");
                //transform.position = hit.transform.position;
                controller?.SnapToNearestPosition(hit.transform);
            }
        }
        else
        {
            Debug.Log("Not on slab, resetting...");
            float dragDistance = Vector3.Distance(dragStartPosition, transform.localPosition);

            // Only trigger if drag is minimal
            if (dragDistance >= minDragDistance)
            {
                Debug.Log("IN");
                controller?.OnItemClick();
            }
            if (PlayerPrefsHandler.GetInt(PlayerPrefsHandler.NewTutorialPrefKey, 0) == 0)
            {
                TutorialHand?.DisableTutorial(true);        
                TutorialHand?.ChangeMsg(0);        
            }
          
        }
    

    }

    private void ResetPreviousSlab()
    {
        if (currentHoveredSlab != null)
        {
            currentHoveredSlab.transform.localScale = originalSlabScale;
            currentHoveredSlab = null;
            if (PlayerPrefsHandler.GetInt(PlayerPrefsHandler.NewTutorialPrefKey, 0) == 0)
            {
                TutorialHand?.ChangeMsg(1);
            }
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 screenPos = Input.mousePosition;
        screenPos.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(screenPos);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, overLapRadius);
    }

 

    private IEnumerator CheckItemState()
    {
        while (controller != null) 
        {
            if (CheckItem)
            {
                if (!isDragging)
                {
                    yield return new WaitForSeconds(1f);
                    if (lastHit != null)
                    {
                        if (lastHit.childCount >= 1)
                            controller?.SnapToNearestPosition(lastHit.transform);
                        else 
                            controller?.OnItemClick();

                        CheckItem = false;
                    }
                   
                }
            }
            // Check item logic here
            yield return new WaitForSeconds(0.1f); 
        }
    }

    // Call this to start it
    public void StartCheckingItem()
    {
        if (checkItemStateCoroutine != null)
            StopCoroutine(checkItemStateCoroutine);

        checkItemStateCoroutine = StartCoroutine(CheckItemState());
    }

    // Call this to stop it
    public void StopCheckingItem()
    {
        if (checkItemStateCoroutine != null)
        {
            StopCoroutine(checkItemStateCoroutine);
            checkItemStateCoroutine = null;
        }
    }


    private void OnDisable()
    {
        StopCheckingItem();
    }
}
