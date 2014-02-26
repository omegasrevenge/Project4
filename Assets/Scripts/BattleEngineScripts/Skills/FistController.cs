using System.Collections.Generic;
using UnityEngine;

public class FistController : ActorControlls
{
    public GameObject FistObject;

    public Transform Origin;
    public Transform Target;

    public List<ParticleEmitter> Emitters;
    public List<ParticleSystem> Systems;

    private float _delay = 1.4f;
    private float _emitTime = 1f;
    private float _fadeTime = 3f;
    private float _zOffset = 3f;
    private float _yOffset = 0f;
    private bool _emitting = true;
    private float _reachTime;
    private Vector3 _destination;
    private bool _energy = false;
    public List<GameObject> Register; 

    void Start()
    {
        Register = new List<GameObject>();
        Origin = BattleEngine.Current.CurCaster.transform.FindChild(BattleEngine.DefaultSkillOrigin);
        Target = BattleEngine.Current.CurTarget.transform.FindChild(BattleEngine.DefaultSkillTarget);
        _destination = Target.TransformPoint(new Vector3(0f, _yOffset, _zOffset));

        Systems = new List<ParticleSystem>();
        Emitters = new List<ParticleEmitter>();

        foreach (var system in GetComponentsInChildren<ParticleSystem>())
            Systems.Add(system);
        foreach (var emitter in GetComponentsInChildren<ParticleEmitter>())
            Emitters.Add(emitter);
        foreach (var reg in GetComponentsInChildren<RegisterGameObjects>())
            Register.Add(reg.gameObject);

        SetEmit(false);
        _reachTime = _emitTime;
    }

    void Update()
    {
        if (_delay > 0f)
        {
            _delay -= Time.deltaTime;
            return;
        }

        if (_emitTime > 0f)
        {
            transform.position += (_destination - Origin.position) * Time.deltaTime / _reachTime;
            SetEmit(true);
            _emitTime -= Time.deltaTime;
            return;
        }

        CanShowDamage = true;
        SetEmit(false);

        if (_fadeTime > 0f)
        {
            _fadeTime -= Time.deltaTime;
            return;
        }

        BattleEngine.Current.Actor = null;
        Destroy(gameObject);
    }

    public void SetEmit(bool value)
    {
        if (_emitting == value) return;
        _emitting = value;
        FistObject.SetActive(value);
        foreach (GameObject reg in Register)
            reg.SetActive(value);
        foreach (ParticleEmitter emitter in Emitters)
            emitter.emit = value;
        foreach (ParticleSystem system in Systems)
            system.enableEmission = value;
    }
}
