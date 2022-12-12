using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCenter : MonoBehaviour
{
    // Start is called before the first frame update
    Texture2D yourCursor ;  // Your cursor texture
    int cursorSizeX = 20;  // Your cursor size x
    int cursorSizeY = 20;  // Your cursor size y
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        OnGUI();
    }

    //void Start()
    //{
    //    Screen.lockCursor = false;
    //    Screen.showCursor = false;

    //}
    void OnGUI()
    {
        //GUI.DrawTexture(new Rect(50, 50, Screen.width / 2, Screen.height / 2), yourCursor);
        GUI.DrawTexture(new Rect(Screen.width / 2, Screen.height / 2, cursorSizeX, cursorSizeY), yourCursor);

    }
}





