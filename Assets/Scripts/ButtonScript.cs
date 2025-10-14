using UnityEngine;
using TMPro;

public class ButtonScript : MonoBehaviour
{
    int LeftButtonPrice; 
    int RightButtonPrice;
    public GameObject LeftButton;
    public TextMeshProUGUI LeftButtonText;
    public TextMeshProUGUI RightButtonText;
    public GameObject RightButton;
    int mult = 1;
    int money;
    int energy;
    int energyboost = 10;

    public void LeftButtonClicked()
    {
        if(money >= LeftButtonPrice)
        {
            money -= LeftButtonPrice;
            mult++;
            LeftButtonPrice *= 2;
        }
    }

    public void RightButtonCLicked()
    {
        if (money >= RightButtonPrice)
        {
            money -= RightButtonPrice;
            energy += energyboost;
            RightButtonPrice *= 3;
        }
    }
}
