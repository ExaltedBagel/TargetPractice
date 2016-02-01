using UnityEngine;
using System.Collections;

public class CursorHandler : MonoBehaviour {

    public Texture2D customCursor;
    static private Texture2D cursorSkin;
    static public CursorMode cursorMode = CursorMode.Auto;
    static public Vector2 hotSpot = Vector2.zero;
    

    // Use this for initialization
    void Start () {
        cursorSkin = customCursor;
	}
    static public void cursorNo()
    {
        Cursor.SetCursor(cursorSkin, hotSpot, cursorMode);
    }
    static public void cursorStd()
    {
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }

}
