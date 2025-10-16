using UnityEngine;

public class RightButtonScript : MonoBehaviour
{
    [SerializeField] private SlotGameManager GameManager;
    [SerializeField] private AnimationTest animationManager;
    public void OnObjectClicked()
    {
        GameManager.BuyIndustry();
        
        // Ton action ici
    }
}
