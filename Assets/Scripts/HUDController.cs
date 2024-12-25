using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [SerializeField] public Image[] avatars;
    public Slider changeBar;
    public Slider healthBar;
    public Slider armorBar;
    public Slider arrowBar;
    public Slider manaBar;
    public KnightController knightController;
    private float cooldownTimer = 0f; // Timer to track cooldown
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        knightController = player.GetComponent<KnightController>();
        healthBar.value = knightController.health / knightController.maxHealth;
        arrowBar.value = knightController.arrows / knightController.maxArrows;
        manaBar.value = knightController.mana / knightController.maxMana;
        armorBar.value = knightController.armor / knightController.maxArmor;
        changeClass(knightController.characterClass, 0f);
        

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
        
    }
}
