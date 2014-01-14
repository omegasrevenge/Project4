using UnityEngine;

public class IntroScreen : MonoBehaviour 
{

    void OnApplicationFocus(bool focusStatus)
    {

        if (focusStatus)
        {
            Debug.Log("FOCUS CHANGED => HIDE NAVIGATION BAR!");
#if !UNITY_EDITOR
            (new AndroidJavaClass("com.nerdiacs.nerdgpgplugin.NerdGPG")).CallStatic("HideNavigationBar");
#endif
        }
    }

    public void SwitchScene()
    {
        Application.LoadLevel("Main");
    }
}
