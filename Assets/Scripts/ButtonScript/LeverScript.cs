using UnityEngine;

public class LeverScript : MonoBehaviour
{
    [SerializeField] private SlotGameManager GameManager;
    [SerializeField] private AnimationTest animationManager;
    public void OnObjectClicked()
    {
        GameManager.StartSpin();
        
        // Ton action ici
    }
}