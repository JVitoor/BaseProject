using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D cursorNormal;
    public Texture2D cursorClick;
    public Vector2 hotspot = Vector2.zero;

    void Start()
    {
        Cursor.SetCursor(cursorNormal, hotspot, CursorMode.Auto);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // quando CLICA
        {
            Cursor.SetCursor(cursorClick, hotspot, CursorMode.Auto);
        }

        if (Input.GetMouseButtonUp(0)) // quando SOLTA
        {
            Cursor.SetCursor(cursorNormal, hotspot, CursorMode.Auto);
        }
    }
}