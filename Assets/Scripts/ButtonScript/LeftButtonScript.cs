using UnityEngine;

public class LeftButtonScript : MonoBehaviour
{
    [SerializeField] private SlotGameManager GameManager;
    [SerializeField] private AnimationTest animationManager;
    public void OnObjectClicked()
    {
        GameManager.BuyEcology();
        
        // Ton action ici
    }
}
