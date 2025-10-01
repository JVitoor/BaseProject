using UnityEngine;

public class AnimPlayer : MonoBehaviour
{
    public Animator animator;
    public KeyCode KeyCode;

    void Start()
    {
        
    }

    
    void Update()
    {
        PlayAnimation();
    }

    public void PlayAnimation()
    {
        if (Input.GetKeyDown(KeyCode))
        {
            animator.SetTrigger("Jump");
        }
    }
}
