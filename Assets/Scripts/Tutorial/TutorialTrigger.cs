using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public string message;
    public float duration = 3f;

    private void OnTriggerEnter(Collider other)
    {
        CharacterController cc = other.GetComponent<CharacterController>();
        if (cc != null)
        {
            TutorialController controller = FindObjectOfType<TutorialController>();
            controller.ShowMessage(message, duration);

            Destroy(gameObject);
        }
    }
}