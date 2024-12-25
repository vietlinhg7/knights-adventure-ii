using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button loadGame;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //can't find save file
        if (true) loadGame.interactable = false;
    }
    public void StartGame()
    {
        
    }
    public void LoadGame()
    {

    }
    public void Option()
    {

    }

    // Update is called once per frame
    public void Exit()
    {
        print("Exit");
        Application.Quit();
    }
}
