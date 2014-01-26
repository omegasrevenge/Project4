using UnityEngine;
using System.Collections;

public class SoundController : MonoBehaviour
{
    private const float LoadTimeOut = 2f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    private IEnumerator StreamSound(string source)
    {

        string url = Application.streamingAssetsPath + "/Sounds/" + source + ".mp3";
        WWW file = new WWW(url);
        yield return file;
        AudioClip audio = file.GetAudioClip(false, true);
        float time = Time.time;
        while (!audio.isReadyToPlay && (Time.time-time)<=LoadTimeOut)
        {
            yield return new WaitForEndOfFrame();
        }
        _audio.clip = audio;
        _audio.Play();
    }
}
