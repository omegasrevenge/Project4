using UnityEngine;

public class IntroScreen : MonoBehaviour
{

    private AudioSource _sound;

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


    void Awake()
    {
        _sound = GetComponent<AudioSource>();
    }

    public void PlaySound()
    {
        _sound.Play();
    }

    public void SwitchScene()
    {
        Application.LoadLevel("Main");
    }
}
