using UnityEngine;

public class LeftButtonScript : MonoBehaviour
{
    [SerializeField] private SlotGameManager2 GameManager;
    [SerializeField] private AnimationTest animationManager;
    public void OnObjectClicked()
    {
        GameManager.BuyEcology();
        
        // Ton action ici
    }
}
