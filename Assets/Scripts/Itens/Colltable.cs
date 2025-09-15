using UnityEngine;

public class Colltable : MonoBehaviour
{
    #region Properties

    public int value = 1;

    #endregion

    #region Methods

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            player.HandleColletable(value);
            Destroy(gameObject);
        }
    }



   
        #endregion
    
}