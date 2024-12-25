using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button loadGame;
    public Button startGame;
    public GameObject deleteSave;
    private SaveLoadManager saveLoadManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        saveLoadManager = FindFirstObjectByType<SaveLoadManager>();

        // Can't find save file
        if (saveLoadManager.LoadData() == null) loadGame.interactable = false;
    }
    public void StartGame()
    {
        // Can't find save file
        if (saveLoadManager.LoadData() == null)
            SceneManager.LoadScene("Tutorial");
        else
        {
            startGame.interactable = false;
            deleteSave.SetActive(true);
        }
    }
    public void LoadGame()
    {
        SceneManager.LoadScene(saveLoadManager.LoadData().sceneIndex); 

    }
    public void Option()
    {
        SceneManager.LoadScene("Option");
    }

    // Update is called once per frame
    public void Exit()
    {
        print("Exit");
        Application.Quit();
    }
    public void Yes()
    {
        saveLoadManager.DeleteData();
        SceneManager.LoadScene("Tutorial");
    }
    public void No()
    {
        startGame.interactable = true;
        deleteSave.SetActive(false);
    }
}
