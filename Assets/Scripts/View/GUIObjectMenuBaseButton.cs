using System;
using UnityEngine;

public class GUIObjectMenuBaseButton : GUIObjectMenuButton
{
    private const string CooldownStr = "label_cooldown";

    private bool _active = true;
    private float _respawnTime;
    private dfLabel _cooldownLabel;

    public Color32 ActiveColor;
    public Color32 InactiveColor;

    private bool Active
    {
        get { return _active; }
        set
        {
            if (value == _active)
                return;
            _active = value;
            if (_active) Activate();
            else Deactivate();
        }
    }

    protected override void Awake()
    {
        base.Awake();
        _cooldownLabel = transform.Find(CooldownStr).GetComponent<dfLabel>();
    }

    void Update()
    {
        if (_respawnTime == 0)
        {
            try
            {
                _respawnTime = (float)GameManager.Singleton.Techtree["Base"]["Cooldown"];
            }
            catch (Exception)
            {

                _respawnTime = 0;
                Active = false;
                return;
            }
            
        }
        int cooldown = (Mathf.FloorToInt(Mathf.Max(0,(float)(GameManager.Singleton.Player.BaseTime.AddHours(_respawnTime) -
                       GameManager.Singleton.GetServerTime()).TotalSeconds)));
        //Debug.Log(cooldown);
        Active = cooldown <= 0;
        if (!Active)
        {
            int hours = Mathf.FloorToInt(cooldown/3600);
            int mins = Mathf.FloorToInt((cooldown%3600)/60);
            _cooldownLabel.Text = string.Format(Localization.GetText("menu_basecooldown"),hours,mins);
        }
    }

    void Activate()
    {
        _label.Color = ActiveColor;
        _cooldownLabel.Hide();
    }

    void Deactivate()
    {
        _label.Color = InactiveColor;
        _cooldownLabel.Show();
    }

    protected override void OnMouseDown(dfControl control, dfMouseEventArgs mouseEvent)
    {
        if (Active)
            base.OnMouseDown(control, mouseEvent);
    }

    void OnClick(dfControl control, dfMouseEventArgs args)
    {
        if (args.Used || !_active) return;
        args.Use();
        GameManager.Singleton.SendBasePosition();
        Active = false;
    }

}

