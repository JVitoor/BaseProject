using UnityEngine;

public class PetalController : MonoBehaviour
{
    public int petalIndex;
    public FlowerController flowerController;
    public Renderer petalRenderer;

    public void SetColor(Color color)
    {
        petalRenderer.material.color = color;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            flowerController.OnPetalStepped(petalIndex);
            SetColor(Color.black);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SetColor(Color.white);
        }
    }
}