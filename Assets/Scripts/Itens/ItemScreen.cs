using UnityEngine;
using TMPro;

public class ItemScreen : MonoBehaviour
{
    #region Properties
    public TextMeshProUGUI texto;
    public float tempoDeExibicao = 5f;

    #endregion

    #region Methods
    public void Mostrar(string mensagem)
    {
        texto.text = mensagem;
        gameObject.SetActive(true);
        CancelInvoke();
        Invoke(nameof(Esconder), tempoDeExibicao);
    }

    private void Esconder()
    {
        gameObject.SetActive(false);
    }
    #endregion
}