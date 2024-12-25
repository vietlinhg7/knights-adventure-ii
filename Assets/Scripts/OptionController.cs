using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionController : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Start()
    {
        // Retrieve and set the initial volume levels from the AudioMixer
        float musicVolume = 0f;
        float sfxVolume = 0f;

        // Get current values from the AudioMixer for both music and SFX
        audioMixer.GetFloat("musicVolume", out musicVolume);
        audioMixer.GetFloat("sfxVolume", out sfxVolume);

        // Convert decibel values to linear scale for slider display
        musicSlider.value = Mathf.Pow(10, musicVolume / 20); // Linear scale
        sfxSlider.value = Mathf.Pow(10, sfxVolume / 20); // Linear scale
    }

    void Update()
    {
        // Continuously update the AudioMixer values based on slider changes
        audioMixer.SetFloat("musicVolume", Mathf.Log10(musicSlider.value)*20);
        audioMixer.SetFloat("sfxVolume", Mathf.Log10(sfxSlider.value) * 20);
    }

    public void Back()
    {
        // Load the "Menu" scene when the back button is pressed
        SceneManager.LoadScene("Menu");
    }
}
