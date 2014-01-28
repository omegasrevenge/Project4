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
        if (_gui && (_gui as GUIObjectSpectresIntro).Visualizer && _audio)
            (_gui as GUIObjectSpectresIntro).Visualizer.Audio = _audio;
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
        if (_callback != null)
            _callback();
        Destroy(gameObject);
    }

    public override void AttachGUI(Component gui)
    {
        base.AttachGUI(gui);
        if ((_gui as GUIObjectSpectresIntro).Visualizer && _audio)
            (_gui as GUIObjectSpectresIntro).Visualizer.Audio = _audio;
    }
}
