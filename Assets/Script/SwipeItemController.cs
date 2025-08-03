using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SwipeItemController : MonoBehaviour
{
    public List<GameObject> itemList;
    public int selectedIndex = 0;
    public float swipeThreshold = 80f;
    public float animationDuration = 0.15f;
    public RectTransform swipeableArea;
    public bool IsCircular = true;
    public bool IsSwipeable = true;

    private Vector2 swipeStartPosition;
    public int currentItemIndex = 0;
    private int targetItemIndex = 0;
    private bool isSwiping = false;
    public bool IsAnimating = false;

    public bool HaveButtons = true;
    public Button LeftButton;
    public Button RightButton;
    public float Spacing = 50f;
    public bool HasTinderFunctionality = false;
    public bool IsEarnPopup = false;


    public delegate void MyActionDelegate(int value);

    private event MyActionDelegate OnItemChangedCallback;

    public void BindItemChangeCallback(MyActionDelegate callback)
    {
        OnItemChangedCallback += callback;   
    }
    
    public void UnbindItemChangeCallback(MyActionDelegate callback)
    {
        OnItemChangedCallback -= callback;   
    }
    
    private void InvokeItemChangeCallbacks(int value)
    {
        OnItemChangedCallback?.Invoke(value);
    }
    
    private void OnEnable()
    {
        isSwiping = false;
    }

    void Start()
    {
        //SetStarterIndex(selectedIndex);
        SetActiveAndUpdateItemsPositions();
        swipeableArea = this.gameObject.GetComponent<RectTransform>();
    }

    public void SetItemsInList(List<GameObject> list)
    {
        itemList = list;
        SetStarterIndex(0);
    }

    public void SetStarterIndex(int index)
    {
        SetAllUIPanelsState(true);
        selectedIndex = index;
        currentItemIndex = Mathf.Clamp(index, 0, itemList.Count - 1); // Handle out-of-bounds indices
        targetItemIndex = currentItemIndex;
        SetActiveAndUpdateItemsPositions();

        if (HaveButtons)
        {
            if (currentItemIndex == 0 && LeftButton != null && !IsCircular)
            {
                LeftButton.interactable = false;
            }
            else
            {
                LeftButton.interactable = true;
            }

            if (currentItemIndex == itemList.Count - 1 && RightButton != null && !IsCircular)
            {
                RightButton.interactable = false;
            }
            else
            {
                RightButton.interactable = true;
            }
        }
    }

    public int GetCurrentItemID()
    {
        return currentItemIndex;
    }

    // UI buttons callbacks
    public void OnNextPressed()
    {
        DisplayNextItem();
    }

    public void OnPrevPressed()
    {
        DisplayPrevItem();
    }

    void DisplayNextItem()
    {
        if (!IsAnimating)
        {
            if (IsCircular)
            {
                targetItemIndex = (currentItemIndex + 1) % itemList.Count;
            }
            else
            {
                if ((targetItemIndex + 1) >= itemList.Count)
                {
                    return;
                }
                else
                {
                    targetItemIndex += 1;
                    if (HaveButtons)
                    {
                        RightButton.interactable = true;
                        LeftButton.interactable = true;

                        if ((targetItemIndex + 1) >= itemList.Count)
                        {
                            RightButton.interactable = false;
                        }
                    }
                }
            }

            StartCoroutine(AnimateItem(1));
        }
    }

    void DisplayPrevItem()
    {
        if (!IsAnimating)
        {
            if (IsCircular)
            {
                targetItemIndex = (currentItemIndex - 1 + itemList.Count) % itemList.Count;
            }
            else
            {
                if ((targetItemIndex - 1) < 0)
                {
                    return;
                }
                else
                {
                    targetItemIndex -= 1;
                    if (HaveButtons)
                    {
                        LeftButton.interactable = true;
                        RightButton.interactable = true;

                        if (targetItemIndex <= 0)
                        {
                            LeftButton.interactable = false;
                        }
                    }
                }
            }

            StartCoroutine(AnimateItem(-1));
        }
    }

    void Update()
    {

        // USE IF not to swipe on one condition
        //if ()
        //{
        //    isSwiping = false;
        //    return;
        //}

        if (IsSwipeable)
        {
            if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
            {
                if (IsPointInsideSwipeableArea())
                {
                    if (!IsAnimating)
                    {
                        isSwiping = true;
                        Vector2 currentInputPosition = (Input.touchCount > 0)
                            ? Input.GetTouch(0).position
                            : (Vector2)Input.mousePosition;

                        swipeStartPosition = currentInputPosition;
                        targetItemIndex = currentItemIndex;
                    }
                }
            }

            if (isSwiping)
            {
                Vector2 currentInputPosition =
                    (Input.touchCount > 0) ? Input.GetTouch(0).position : (Vector2)Input.mousePosition;
                float swipeDelta = currentInputPosition.x - swipeStartPosition.x;
                if (Mathf.Abs(swipeDelta) > swipeThreshold)
                {
                    if (swipeDelta > 0)
                    {
                        DisplayPrevItem();
                    }
                    else
                    {
                        DisplayNextItem();
                    }

                    isSwiping = false;
                }
            }

            // Check for both mouse and touch input
            if (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
            {
                isSwiping = false;
            }
        }
        // Check for both mouse and touch input 
    }

    bool IsPointInsideSwipeableArea()
    {
        if (swipeableArea != null)
        {
            return RectTransformUtility.RectangleContainsScreenPoint(swipeableArea, GetInputPosition());
        }
        else
        {
            return true;
        }
    }

    Vector2 GetInputPosition()
    {
        if (Input.touchCount > 0)
        {
            return Input.GetTouch(0).position;
        }
        else
        {
            return new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
    }

    public float ScreenWidth = 2;

    IEnumerator AnimateItem(int direction)
    {
        IsAnimating = true;
        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / animationDuration);

            float newX = Mathf.Lerp(itemList[currentItemIndex].transform.localPosition.x,
                -direction * (Screen.width * ScreenWidth), t);
            itemList[currentItemIndex].transform.localPosition = new Vector3(newX, 0, 0);
            if (HasUIPanels)
            {
                AllUIPanels[0].localPosition = new Vector3(newX, 0, 0);
            }


            newX = Mathf.Lerp(itemList[targetItemIndex].transform.localPosition.x, 0, t);
            itemList[targetItemIndex].transform.localPosition = new Vector3(newX, 0, 0);
            if (HasUIPanels)
            {
                AllUIPanels[1].localPosition = new Vector3(newX, 0, 0);
            }

            yield return null;
        }

        currentItemIndex = targetItemIndex;
        SetActiveAndUpdateItemsPositions();
        //if (HasTinderFunctionality)
        //{
        //    MenuSceneUIController.Instance._panelsController._childEarnMoneyPanel.OnRepopulateItemInList();
        //}

        IsAnimating = false;
        InvokeItemChangeCallbacks(currentItemIndex);
    }

    private void SetActiveAndUpdateItemsPositions()
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            if (i == currentItemIndex)
            {
                itemList[i].SetActive(true);
                itemList[i].transform.localPosition = Vector3.zero;
                SetUIPanelPos(0, itemList[i].transform.localPosition);
            }
            else
            {
                itemList[i].SetActive(false);
                itemList[i].transform.localPosition = new Vector3((Screen.width * ScreenWidth), 0, 0);
            }
        }

        if (itemList.Count >= 3)
        {
            itemList[(currentItemIndex + 1) % itemList.Count].transform.localPosition =
                new Vector3((Screen.width * ScreenWidth * Spacing), 0, 0);
            itemList[(currentItemIndex + 1) % itemList.Count].SetActive(true);

            SetUIPanelPos(1, new Vector3((Screen.width * ScreenWidth * Spacing), 0, 0));

            itemList[(currentItemIndex - 1 + itemList.Count) % itemList.Count].transform.localPosition =
                new Vector3(-(Screen.width * ScreenWidth * Spacing), 0, 0);
            itemList[(currentItemIndex - 1 + itemList.Count) % itemList.Count].SetActive(true);

            //SetUIPanelPos(2, new Vector3(-(Screen.width * ScreenWidth * Spacing), 0, 0));
        }

        //if (HasTinderFunctionality)
        //{
        //    MenuSceneUIController.Instance._view._clickBlocker.gameObject.SetActive(false);
        //}
    }

    public bool HasUIPanels = false;
    public List<Transform> AllUIPanels;

    void SetUIPanelPos(int Index, Vector3 New_Pos)
    {
        if (HasUIPanels)
        {
            AllUIPanels[Index].transform.localPosition = New_Pos;
        }
    }

    void SetAllUIPanelsState(bool State)
    {
        for (int i = 0; i < AllUIPanels.Count; i++)
        {
            AllUIPanels[i].gameObject.SetActive(true);
        }
    }
}