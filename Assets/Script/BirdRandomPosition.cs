using DG.Tweening;
using UnityEngine;

public class BirdRandomPosition : MonoBehaviour
{

    public float minYPosition;
    public float mixYPosition;
    public Animator animator;
    public DOTweenAnimation moveAnimation;

    private Vector3 initialPosition;
    private void Start()
    {
        animator.speed = Random.Range(0.7f,1);
        initialPosition = transform.position;
    }
    public void SetRandomPosition()
    {
        float speed = Random.Range(0.7f, 1);
        moveAnimation.duration = speed * 30;
        animator.speed = speed;
        transform.position = new Vector2(initialPosition.x,Random.Range(minYPosition, mixYPosition));
        float scale = Random.Range(0.6f, 1);
        transform.localScale = new Vector2(scale, scale);
    }
}
