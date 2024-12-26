using System.Collections;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [SerializeField] public Image[] avatars;
    public Slider changeBar;
    public Slider healthBar;
    public Slider armorBar;
    public Slider arrowBar;
    public Slider manaBar;
    public TMP_Text coinText;
    public KnightController knightController;
    public Image blackScreen;
    private float cooldownTimer = 0f; // Timer to track cooldown
    public GameObject pauseScreen;
    public bool pause = false;
    public GameObject Frame;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pause = false;
        GameObject player = GameObject.FindWithTag("Player");
        knightController = player.GetComponent<KnightController>();
        changeClass(knightController.characterClass, 0f);
        coinText.text = knightController.currentCoin.ToString();
        blackScreen.gameObject.SetActive(true);
        StartCoroutine(HideBlackScreen(0.2f, 100));

    }
    public void changeClass(int chosen, float cooldown)
    {
        foreach (Image avatar in avatars)
        {
            avatar.enabled = false;
        }
        avatars[knightController.characterClass].enabled = true;
        if (cooldown > 0f)
        {
            StartCoroutine(CooldownCoroutine(cooldown));
        }
    }
    private IEnumerator CooldownCoroutine(float cooldown)
    {
        float elapsedTime = 0f;

        while (elapsedTime < cooldown)
        {
            elapsedTime += Time.deltaTime;

            // Update the changeBar slider value (assuming it's normalized between 0 and 1)
            changeBar.value = 1 - elapsedTime / cooldown;

            yield return null; // Wait for the next frame
        }
        // Reset the changeBar if needed
        changeBar.value = 0;
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.value = (float)knightController.health / knightController.maxHealth;
        arrowBar.value = (float)knightController.arrows / knightController.maxArrows;
        manaBar.value = (float)knightController.mana / knightController.maxMana;
        coinText.text = knightController.currentCoin.ToString();
        if (knightController.characterClass == 0) armorBar.value = (float)knightController.armor / knightController.maxArmor;
        else armorBar.value = 0f;
    }

    public IEnumerator ShowBlackScreen(float duration = 1f, int step = 10, System.Action onDone = null)
    {
        blackScreen.gameObject.SetActive(true);
        knightController.enabled = false;
        float alpha = 0;
        for (int i = 0; i < step; i++)
        {
            alpha += 1f / step;
            blackScreen.color = new Color(0, 0, 0, alpha);
            yield return new WaitForSeconds(duration / step);
        }
        knightController.enabled = true;

        if (onDone != null)
        {
            onDone();
        }
    }

    public IEnumerator HideBlackScreen(float duration = 1f, int step = 10)
    {
        knightController.enabled = false;
        float alpha = 1;
        for (int i = 0; i < step; i++)
        {
            alpha -= 1f / step;
            blackScreen.color = new Color(0, 0, 0, alpha);
            yield return new WaitForSeconds(duration / step);
        }
        blackScreen.gameObject.SetActive(false);
        knightController.enabled = true;
    }

    public void Pause()
    {
        pause = true;
        pauseScreen.SetActive(true);
    }
    public void Continue()
    {
        pause = false;
        pauseScreen.SetActive(false);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Option()
    {
        Frame.SetActive(true);
    }
    
    public void Menu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void Exit()
    {
        print("Exit");
        Application.Quit();
    }
}
