using UnityEngine;

public class GUIObjectLoginButton : MonoBehaviour 
{
    void OnClick(dfControl control, dfMouseEventArgs args)
    {
        GameManager.Singleton.GUIHideLoginScreen();
    }
}
