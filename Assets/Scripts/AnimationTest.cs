using UnityEngine;

public class AnimationTest : MonoBehaviour
{
    [SerializeField] private Animator animator;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            animator.Play("lever_pull");
        }

        if (Input.GetKey(KeyCode.B))
        {
            animator.speed = 0.6f;
        }
    }
}
