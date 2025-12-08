using UnityEngine;

public class AlavancaAnim : MonoBehaviour
{
    public Animator animator;
    public Animator animatorCamera;

    void Start()
    {
       
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            animator.SetTrigger("Active");
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            animatorCamera.SetTrigger("Active");
        }
    }
}
