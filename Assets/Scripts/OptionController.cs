using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionController: MonoBehaviour
{
    void Update()
    {

    }

    public void Back()
    {
        SceneManager.LoadScene("Menu");
    }
}
