using UnityEngine;

public class ScratchController : ActorControlls
{
    public Transform Origin;

    private float Delay = 1.5f;
    private float LifeTime = 1f;
    private float FadeTime = 0.1f;
    private float TravelDistance = 11f;
    private float YStartOffset = 20f;

    private float _speed;

    void Start()
    {
        SetChildrenActive(false);
        Origin = BattleEngine.Current.CurTarget.transform;
        transform.position = Origin.TransformPoint(new Vector3(1f, YStartOffset, 0.5f));
        transform.rotation = BattleEngine.Current.Camera.transform.rotation;
        _speed = LifeTime;
    }

    void Update()
    {
        if (Delay > 0f)
        {
            if (FadeTime == 0f)
                Debug.LogError("FadeTime MUSTN'T be 0. It has to fade for one entire frame at least, or the BattleEngine will never register CanShowDamage.");
            Delay -= Time.deltaTime;
            return;
        }

        SetChildrenActive(true);

        if (LifeTime > 0f)
        {
            transform.localPosition -= new Vector3(0f, TravelDistance * Time.deltaTime / LifeTime, 0f);
            LifeTime -= Time.deltaTime;
            if (_speed / (float)LifeTime > 2f) CanShowDamage = true;
            return;
        }

        if (FadeTime > 0f)
        {
            FadeTime -= Time.deltaTime;
            return;
        }

        BattleEngine.Current.Actor = null;
        Destroy(gameObject);
    }

    public void SetChildrenActive(bool value)
    {
        if (transform.GetChild(0) == null) return;
        if(transform.GetChild(0).gameObject.activeSelf == value) return;
        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(value);
    }
}
