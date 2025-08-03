using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CoinsAnimation : MonoBehaviour
{
    public GameObject coinIcon;
    public GameObject coinText;
    private Text coins;
    private int coinsCount;

    private void Start()
    {
        coins = coinText.GetComponent<Text>();
        coinsCount = int.Parse(coins.text);
    }

    public void OnCompleteAnimation()
    {    
        coinText.GetComponent<DOTweenAnimation>().DORestartById("1");
        coinIcon.GetComponent<DOTweenAnimation>().DORestartById("2");
    }
    public void AddCoin()
    {
        coinsCount += 10;
        coins.text = coinsCount.ToString();
        Debug.Log(coins.text);
    }
}
