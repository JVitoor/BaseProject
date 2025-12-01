using UnityEngine;
using TMPro;

public class TutorialController : MonoBehaviour
{
    public GameObject tutorialPanel;
    public TextMeshProUGUI tutorialText;

    public float timeToHide = 3f;

    private void Start()
    {
        tutorialPanel.SetActive(false);
    }

    public void ShowMessage(string msg, float duration = -1)
    {
        tutorialPanel.SetActive(true);
        tutorialText.text = msg;

        if (duration <= 0)
            duration = timeToHide;

        CancelInvoke(nameof(Hide));
        Invoke(nameof(Hide), duration);
    }

    void Hide()
    {
        tutorialPanel.SetActive(false);
    }
}