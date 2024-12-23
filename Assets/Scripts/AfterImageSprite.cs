using UnityEngine;
using System.Collections.Generic;

public class AfterImageSprite : MonoBehaviour
{
    [SerializeField]
    private float activeTime = 0.1f;
    private float timeActivated;
    private float alpha;
    [SerializeField]
    private float alphaSet = 0.8f;
    private float alphaMultiplier = 0.85f;

    public Transform source;

    [SerializeField]
    private SpriteRenderer SR;
    private SpriteRenderer sourceSR;

    private Color color;

    private void OnEnable()
    {
        
        source = GameObject.FindGameObjectWithTag("ObjectPooler").transform.parent.gameObject.transform;
        SR = GetComponent<SpriteRenderer>();
        sourceSR = source.GetComponent<SpriteRenderer>();

        alpha = alphaSet;
        SR.sprite = sourceSR.sprite;
        transform.position = source.position;
        timeActivated = Time.time;
        
    }
    private void Update()
    {
        // Gradually reduce alpha over time
        alpha -= alphaMultiplier * Time.deltaTime;
        color.a = Mathf.Clamp(alpha, 0f, 1f); // Ensure alpha is clamped between 0 and 1
        SR.color = color;

        // Deactivate after active time has passed
        if (Time.time >= timeActivated + activeTime)
        {
            gameObject.SetActive(false); // Use pooling for optimization
        }
    }
}
