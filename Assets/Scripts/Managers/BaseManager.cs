using UnityEngine;

public abstract class BaseManager : MonoBehaviour
{
    public virtual void Initialize()
    { }

    public virtual void OnGameStart()
    { }

    public virtual void OnGamePause()
    { }

    public virtual void OnGameResume()
    { }

    public virtual void OnGameOver()
    { }
}