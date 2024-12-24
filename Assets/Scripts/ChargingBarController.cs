using UnityEngine;
using UnityEngine.UI;

public class ChargingBarController : MonoBehaviour
{
    public Slider chargingBar; // Reference to the slider

    void Update()
    {
    }

    public void SetChargeLevel(float chargeTime, float maxChargeTime)
    {
        if (chargingBar != null)
        {
            chargingBar.value = chargeTime / maxChargeTime; // Update slider value
        }
    }

    public void ShowBar(bool show)
    {
        chargingBar.gameObject.SetActive(show); // Toggle bar visibility
    }
    public void SetBarColor(Color color1, Color color2)
    {
        Transform background = chargingBar.transform.GetChild(0);
        Transform fillArea = chargingBar.transform.GetChild(1);
        background.GetComponent<Image>().color = color1;
        fillArea.GetChild(0).GetComponent<Image>().color = color2;
    }
}
