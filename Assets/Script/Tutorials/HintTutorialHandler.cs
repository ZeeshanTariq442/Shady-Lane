using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HintTutorialHandler : MonoBehaviour
{
    public GameObject Hand;
    public Text msg;
    public GameObject HintTutorialPanel;
    public Button HintTutorialButton;

    private string[] msgs = {
        "Tap to reveal your\n next move!",
        "Now tap on any slab\n tile to see the hint.",
        "Great!"
    };

    public void ShowTutorialPanel()
    {
        if (!HintTutorialShow()) return;
        HintTutorialPanel.SetActive(true);
        Hand.SetActive(true);
        AnimateHintText();
        msg.text = msgs[0];
        HintTutorialButton.onClick.AddListener(HandleHintButtonClicked);
        EnableHintModeOnAllItems();
    }
    private void HideTutorialPanel()
    {
        HintTutorialPanel.SetActive(false);
        HintTutorialButton.interactable = true;
       
    }

    private void HandleHintButtonClicked()
    {
        if (!HintTutorialShow()) return;
        HintTutorialButton.interactable = false;
        msg.text = msgs[1];
        Hand.SetActive(false);
        RegisterTileHintCallbacks();
    }

    private void AnimateHintText()
    {
        msg.transform.DOScale(1.1f, 1f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetUpdate(true);
    }

    private void EnableHintModeOnAllItems()
    {
        ItemController[] items = FindObjectsByType<ItemController>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (ItemController item in items)
        {
            item.HintTutorialPlay = true;
        }
    }
    private void DisableHintModeOnAllItems()
    {
        ItemController[] items = FindObjectsByType<ItemController>(FindObjectsSortMode.None);
        foreach (ItemController item in items)
        {
            item.HintTutorialPlay = false;
        }
    }

    private void RegisterTileHintCallbacks()
    {
        TileData[] tiles = FindObjectsByType<TileData>(FindObjectsSortMode.None);
        foreach (TileData tile in tiles)
        {
            tile.OnHintTutorialAction = () =>
            {
                msg.text = msgs[2];
                UsedHintTutorial();
                HintTutorialPanel.transform.GetChild(0)?.gameObject.SetActive(false);
                FindFirstObjectByType<LevelGenerate>().ChangeItemToDefaultLayer(-1);
                DisableHintModeOnAllItems();
                Invoke(nameof(HideTutorialPanel), 2f);
            };
        }
    }

    private bool HintTutorialShow()
    {
        return PlayerPrefsHandler.GetInt(PlayerPrefsHandler.HintTutorial, 0) == 0;
    }
    private void UsedHintTutorial()
    {
        PlayerPrefsHandler.SetInt(PlayerPrefsHandler.HintTutorial, 1);
    }
}

