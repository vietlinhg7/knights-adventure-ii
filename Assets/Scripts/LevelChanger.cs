using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D))]
public class LevelChanger : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    [SerializeField] private HUDController hudController;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == hudController.knightController.gameObject.name)
        {
            // Insert prompt here
            boxCollider.enabled = false;
            StartCoroutine(hudController.ShowBlackScreen(0.2f, 100, () =>
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }));
        }
    }
}
