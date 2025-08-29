using UnityEngine;

public class Colltable : MonoBehaviour
{
    #region Properties
    public int valor = 1; 
    public ItemScreen itemScreen; // referência ao script da tela

    #endregion

    #region Methods
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            if (other.CompareTag("Player"))
            {
                if (itemScreen != null)
                    itemScreen.Mostrar("+" + valor);

                // destruir o item coletado
                Destroy(gameObject);
            }
        }
    }
    #endregion
}