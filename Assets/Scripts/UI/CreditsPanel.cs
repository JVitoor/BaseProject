using UnityEngine;

public class CreditsPanel : MonoBehaviour
{
    public GameObject panel;

    public void Toggle()
    {
        panel.SetActive(!panel.activeSelf);
    }
}
