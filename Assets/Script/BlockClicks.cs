using UnityEngine;
using System.Collections;

public class BlockClicks : MonoBehaviour
{
    ItemController[] controllers;

    public void OnEnable()
    {
        controllers = FindObjectsByType<ItemController>(FindObjectsSortMode.None);
    }

  

}
