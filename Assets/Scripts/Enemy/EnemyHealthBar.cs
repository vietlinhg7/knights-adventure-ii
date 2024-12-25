using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private SpriteRenderer remainingHealth;

    private void Start()
    {
        remainingHealth.transform.localScale = Vector3.one;
        remainingHealth.transform.localPosition = Vector3.zero;
    }

    public void UpdateHealth(float percentage)
    {
        if (percentage == 0)
        {
            remainingHealth.enabled = false;
        }
        remainingHealth.transform.localScale = new Vector3(percentage, 1, 1);
        remainingHealth.transform.localPosition = new Vector3(-(1 - percentage) / 2, 0, 0);
        Debug.Log(percentage + "|" + remainingHealth.transform.localScale.ToString());
    }
}
