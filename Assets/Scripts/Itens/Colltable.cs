using UnityEngine;

public class Colltable : MonoBehaviour
{
    #region Properties

    #endregion

    #region Methods

    public virtual void OnInteract()
    {
        Debug.Log("Interact with: " + gameObject.name);
        Destroy(gameObject);
    }
    /*private void OnTriggerEnter(Collider other)
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
    }*/


   
        #endregion
    
}