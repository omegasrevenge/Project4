using UnityEngine;

public class IntroScreen : MonoBehaviour
{

    private AudioSource _sound;

    void OnApplicationFocus(bool focusStatus)
    {

        if (focusStatus)
        {
#if !UNITY_EDITOR
           // (new AndroidJavaClass("com.nerdiacs.nerdgpgplugin.NerdGPG")).CallStatic("HideNavigationBar");
#endif
        }
    }


    void Awake()
    {
        _sound = GetComponent<AudioSource>();
        AudioListener.volume = PlayerPrefs.GetFloat("Sound", 1f);
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
