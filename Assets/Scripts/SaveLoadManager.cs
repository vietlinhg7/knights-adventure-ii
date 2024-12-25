using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadManager : MonoBehaviour
{
    private string saveFilePath;

    private void Awake()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "savefile.json");
    }

    [System.Serializable]
    public class SaveData
    {
        public int sceneIndex;
        public float timeInvincible;
        public int wizardUpgrade;
        public float speed;
        public float jumpForce;
        public float dashingPower;
        public float dashingTime;
        public float dashingCooldown;
        public float attackRange;
        public float attackRange2;
        public float knockbackForce;
        public float knockbackUpwardForce;
        public float stunDuration;
        public float changeCooldown;
        public float attackTime;
        public float attackTime2;
        public float attackTime3;
        public int manaCost;
        public int maxHealth;
        public int maxMana;
        public int maxArrows;
        public int maxArmor;
        public int currentCoin;
        public int characterClass;
        public int health;
        public int mana;
        public int arrows;
        public int armor;
        public bool wrecked;
    }

    public void SavesData(GameObject character, int sceneIndex)
    {
        SaveData data = new SaveData
        {
            sceneIndex = sceneIndex,
            timeInvincible = character.GetComponent<KnightController>().timeInvincible,
            wizardUpgrade = character.GetComponent<KnightController>().wizardUpgrade,
            speed = character.GetComponent<KnightController>().speed,
            jumpForce = character.GetComponent<KnightController>().jumpForce,
            dashingPower = character.GetComponent<KnightController>().dashingPower,
            dashingTime = character.GetComponent<KnightController>().dashingTime,
            dashingCooldown = character.GetComponent<KnightController>().dashingCooldown,
            attackRange = character.GetComponent<KnightController>().attackRange,
            attackRange2 = character.GetComponent<KnightController>().attackRange2,
            knockbackForce = character.GetComponent<KnightController>().knockbackForce,
            knockbackUpwardForce = character.GetComponent<KnightController>().knockbackUpwardForce,
            stunDuration = character.GetComponent<KnightController>().stunDuration,
            changeCooldown = character.GetComponent<KnightController>().changeCooldown,
            attackTime = character.GetComponent<KnightController>().attackTime,
            attackTime2 = character.GetComponent<KnightController>().attackTime2,
            attackTime3 = character.GetComponent<KnightController>().attackTime3,
            manaCost = character.GetComponent<KnightController>().manaCost,
            maxHealth = character.GetComponent<KnightController>().maxHealth,
            maxMana = character.GetComponent<KnightController>().maxMana,
            maxArrows = character.GetComponent<KnightController>().maxArrows,
            maxArmor = character.GetComponent<KnightController>().maxArmor,
            currentCoin = character.GetComponent<KnightController>().currentCoin,
            characterClass = character.GetComponent<KnightController>().characterClass,
            health = character.GetComponent<KnightController>().health,
            arrows = character.GetComponent<KnightController>().arrows,
            armor = character.GetComponent<KnightController>().armor,
            wrecked = character.GetComponent<KnightController>().wrecked
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Game Saved: " + saveFilePath);
    }

    public SaveData LoadData()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            Debug.Log("Game Loaded");
            return data;
        }
        else
        {
            Debug.LogWarning("Save file not found!");
            return null;
        }
    }

    public void ApplyLoadedData(GameObject character, SaveData data)
    {
        if (data != null)
        {
            character.GetComponent<KnightController>().timeInvincible = data.timeInvincible;
            character.GetComponent<KnightController>().wizardUpgrade = data.wizardUpgrade;
            character.GetComponent<KnightController>().speed = data.speed;
            character.GetComponent<KnightController>().jumpForce = data.jumpForce;
            character.GetComponent<KnightController>().dashingPower = data.dashingPower;
            character.GetComponent<KnightController>().dashingTime = data.dashingTime;
            character.GetComponent<KnightController>().dashingCooldown = data.dashingCooldown;
            character.GetComponent<KnightController>().attackRange = data.attackRange;
            character.GetComponent<KnightController>().attackRange2 = data.attackRange2;
            character.GetComponent<KnightController>().knockbackForce = data.knockbackForce;
            character.GetComponent<KnightController>().knockbackUpwardForce = data.knockbackUpwardForce;
            character.GetComponent<KnightController>().stunDuration = data.stunDuration;
            character.GetComponent<KnightController>().changeCooldown = data.changeCooldown;
            character.GetComponent<KnightController>().attackTime = data.attackTime;
            character.GetComponent<KnightController>().attackTime2 = data.attackTime2;
            character.GetComponent<KnightController>().attackTime3 = data.attackTime3;
            character.GetComponent<KnightController>().manaCost = data.manaCost;
            character.GetComponent<KnightController>().maxHealth = data.maxHealth;
            character.GetComponent<KnightController>().maxMana = data.maxMana;
            character.GetComponent<KnightController>().maxArrows = data.maxArrows;
            character.GetComponent<KnightController>().maxArmor = data.maxArmor;
            character.GetComponent<KnightController>().currentCoin = data.currentCoin;
            character.GetComponent<KnightController>().characterClass = data.characterClass;
            character.GetComponent<KnightController>().health = data.health;
            character.GetComponent<KnightController>().arrows = data.arrows;
            character.GetComponent<KnightController>().armor = data.armor;
            character.GetComponent<KnightController>().wrecked = data.wrecked;
        }
    }

    public void DeleteData()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            Debug.Log("Save data deleted.");
        }
        else
        {
            Debug.LogWarning("No save data to delete.");
        }
    }
}
