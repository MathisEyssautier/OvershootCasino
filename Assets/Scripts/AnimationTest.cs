using UnityEngine;

public class AnimationTest : MonoBehaviour
{
    [SerializeField] private Animator machineAnimator;
    [SerializeField] private Animator leverAnimator;
    [SerializeField] private Animator rightButtonAnimator;
    [SerializeField] private Animator leftButtonAnimator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            leverAnimator.Play("lever_pull");
            machineAnimator.Play("machine_shake");
        }

        if (Input.GetKey(KeyCode.Q)) {
            machineAnimator.Play("light_flicker");
        }
    }

    public void Lever()
    {
        leverAnimator.Play("lever_pull");
        machineAnimator.Play("machine_shake");
    }

    public void RightButton()
    {
        rightButtonAnimator.Play("Button_press");
    }
    public void LeftButton()
    {
        leftButtonAnimator.Play("buttonleft_press");
    }

}
