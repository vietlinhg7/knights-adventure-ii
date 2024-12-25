using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button loadGame;
    public Button startGame;
    public GameObject deleteSave;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Can't find save file
        if (true) loadGame.interactable = false;
    }
    public void StartGame()
    {
        // Can't find save file
        if (false)
            SceneManager.LoadScene("Jungle");
        else
        {
            startGame.interactable = false;
            deleteSave.SetActive(true);
        }
    }
    public void LoadGame()
    {

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
        SceneManager.LoadScene("Jungle");
    }
    public void No()
    {
        startGame.interactable = true;
        deleteSave.SetActive(false);
    }
}
