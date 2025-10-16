using UnityEngine;

public class LeverScript : MonoBehaviour
{
    [SerializeField] private SlotGameManager2 GameManager;
    [SerializeField] private AnimationTest animationManager;
    public void OnObjectClicked()
    {
        GameManager.StartSpin();
        
        // Ton action ici
    }

    public void OnMouseEnter()
    {
        GameManager.HoveringLever();
    }

    public void OnMouseExit()
    {
        GameManager.StopHovering();
    }
}