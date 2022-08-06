using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetting : SingleTon<GameSetting>
{
    //private enum DPI { FHD, HDPlus, HD, }
    //private enum Controller { Keyboard, G29, }
    [SerializeField]
    private int currentDPI = 0;
    public int CurrentDPI { get { return currentDPI; } set { currentDPI = value; } }
    [SerializeField]
    private int currentController = 0;
    public int CurrentController { get { return currentController; } set { currentController = value; } }

    private bool isFullScreen = true; // Use to window setting
    public bool IsFull { get { return isFullScreen; } set { isFullScreen = value; } }
}
