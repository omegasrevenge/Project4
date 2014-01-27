using System;
using UnityEngine;
using System.Collections;

public class SpectresIntro : SceneRoot3D
{
    private const string IrisSoundStr = "iris_03";
    private const string Prefab = "Scene/spectresintro";

    private AudioSource _audio;
    private bool _started = false;
    private Action _callback;
    private GUIObjectSpectresIntro _gui;

    public static SpectresIntro Create(Action callback)
    {
        GameObject obj = (GameObject) Instantiate(Resources.Load<GameObject>(Prefab));
        if (obj)
        {
            SpectresIntro intro = obj.GetComponent<SpectresIntro>();
            if (intro == null) return null;
            intro._callback = callback;
            return intro;
        }
        return null;
    }

    void Awake()
    {
        _audio = Localization.GetSound(IrisSoundStr);
        _audio.Play();
        if (_gui && _gui.Visualizer && _audio)
            _gui.Visualizer.Audio = _audio;
    }

    void Update()
    {
        if (!_started && _audio && _audio.isPlaying)
        {
            _started = true;
        }
        if (_started && _audio && !_audio.isPlaying && !Camera.gameObject.activeSelf)
        {
            OnIrisFinished();
        }
    }

    void OnIrisFinished()
    {
        Debug.Log("Und zum Finish finish");
        if (_callback != null)
            _callback();
        Destroy(gameObject);
        if (_gui)
        {
            Destroy(_gui.gameObject);
        }
    }

    public void AttachGUI(GUIObjectSpectresIntro gui)
    {
        _gui = gui;
        if (_gui.Visualizer && _audio)
            _gui.Visualizer.Audio = _audio;
    }
}
