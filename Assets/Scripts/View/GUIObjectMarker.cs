using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

public class GUIObjectMarker : MonoBehaviour 
{
    private const string Prefab = "GUI/panel_marker";
    private const string AgentStr           = "POIs/panel_agent";
    private const string BaseStr            = "POIs/panel_base";
    private const string CooldownStr        = "panel_cooldown";
    private const string HealStr            = "POIs/panel_healingspot";
    private const string InterferenceStr    = "POIs/panel_interference";
    private const string ResourceStr        = "POIs/panel_resource";
    private const string SpectreStr         = "POIs/panel_spectre";
    private const float ReferenceHeight = 1280f;
    private const float ArrowHeight = 65f; //50f;
    private const float Padding = 11f;
    private const float PanelHeight = 128f;

    public dfSprite background;

    private dfControl _control;
    private dfControl _root;
    private Vector2 _pos;
    private Vector3 _scale;
    private ObjectOnMap[] _objectsOnMap;
    [SerializeField]
    private PoiMarkerCooldown[] _cooldowns;
    private bool _untouched;
    private bool _removed;

    public static GUIObjectMarker Create(dfControl root, ObjectOnMap[] objects)
    {
        dfControl cntrl = root.AddPrefab(Resources.Load<GameObject>(Prefab));
        GUIObjectMarker obj = cntrl.GetComponent<GUIObjectMarker>();
        obj.Init(root, cntrl, objects);
        return obj;
    }

    private void Init(dfControl root, dfControl cntrl, ObjectOnMap[] objects)
    {
        // VENGEA - NCE
        if (GameManager.Singleton.Player.CurrentFaction == Player.Faction.NCE)
            background.Color = GameManager.NCERed;

        SoundController.PlaySound(SoundController.SFXlocation + SoundController.Faction + SoundController.SoundFacChoose, SoundController.ChannelSFX);
        _scale = cntrl.transform.localScale;
        _scale *= (Screen.height / ReferenceHeight);
        Vector3 size = cntrl.Size;
        size.y = ArrowHeight + objects.Length * (Padding + PanelHeight) + Padding;
        cntrl.Size = size;
        _control = cntrl;
        _root = root;
        _root.Click += OnClickParent;
        TouchInput.Singleton.ClearAll += Remove;

        _pos = new Vector2(Padding, Padding);
        _cooldowns = new PoiMarkerCooldown[objects.Length];
        for (int i = 0; i < objects.Length; i++)
        {
            dfControl control = CreateMarkerPanel(objects[i], Remove);
            AddMarkerPanel(control);
            Transform trans = control.transform.Find(CooldownStr);
            Debug.Log(control.name);
            if (trans)
            {
                _cooldowns[i] = trans.GetComponent<PoiMarkerCooldown>();
            }         
        }
        _objectsOnMap = objects;

        transform.localScale = _scale;

        TouchInput.Enabled = false;
    }

    private void OnClickParent(dfControl control, dfMouseEventArgs mouseEvent)
    {
        if(mouseEvent.Used) return;
        mouseEvent.Use();
        SoundController.PlaySound(SoundController.SFXlocation + SoundController.Faction + SoundController.SoundFacClick, SoundController.ChannelSFX);
        Remove();
    }

    public void AddMarkerPanel(dfControl obj)
    {
        if (obj == null)
            return;
        obj.transform.parent = transform;
        obj.gameObject.layer = gameObject.layer;

        _control.AddControl(obj);
        obj.RelativePosition = _pos;
        _pos.y += PanelHeight + Padding;

        obj.BringToFront();

    }

    private static dfControl CreateMarkerPanel(ObjectOnMap obj, Action hideCalback)
    {
        if (obj is Resource)
        {
            dfControl control = CreateObjectOnMap(obj, ResourceStr, hideCalback);
            MarkerBiod marker = control.GetComponent<MarkerBiod>();
            marker.SetElementSymbol((obj as Resource).GetElement());
            return control;
        }
        if (obj is PlayerOnMap)
        {
            string path = AgentStr;
            if ((obj as PlayerOnMap).playerData.CurrentFaction == Player.Faction.NCE)
                path = InterferenceStr;
            return CreateObjectOnMap(obj, path, hideCalback);
        }
        if (obj is BaseOnMap)
            return CreateObjectOnMap(obj, BaseStr, hideCalback);
        if (obj is Spectre)
            return CreateObjectOnMap(obj, SpectreStr, hideCalback);
        if (obj is HealStation)
            return CreateObjectOnMap(obj, HealStr, hideCalback);



        return null;
    }

    private static dfControl CreateObjectOnMap(ObjectOnMap objectOnMap, string path, Action hideCallback)
    {
        GameObject go = (GameObject)Instantiate(Resources.Load<GameObject>(path));
        dfControl control = go.GetComponent<dfControl>();
        control.Click += (dfControl, args) =>
        {
            if (args.Used) return;
            args.Use();
            if (objectOnMap.GetCooldownProgress() > 0)
            {
                SoundController.PlaySound(SoundController.SFXlocation + SoundController.Faction + SoundController.SoundFacClick, SoundController.ChannelSFX);
                if (hideCallback != null)
                    hideCallback();
                return;
            }

            SoundController.PlaySound(SoundController.SFXlocation + SoundController.Faction + SoundController.SoundFacMapClick, SoundController.ChannelSFX);
            if (hideCallback != null)
                hideCallback();
            objectOnMap.Execute();
        };

        return control;
    }

    public void Update()
    {
        ObjectOnMap[] newList = _objectsOnMap.Where(o => o != null && o.Enabled).ToArray();
        if (newList.Length != _objectsOnMap.Length)
        {
            Remove();
            if(newList.Length > 0)
                Create(_root, newList);
            return;
        }



        Vector3 posOnMap = Vector3.zero;
        if (_objectsOnMap.Length > 0)
        {
            for (int i = 0; i < _objectsOnMap.Length; i++)
            {
                ObjectOnMap objOnMap = _objectsOnMap[i];
                posOnMap += objOnMap.transform.position;

                if(_cooldowns[i])
                    _cooldowns[i].SetCooldown(objOnMap.GetCooldownProgress(),objOnMap.GetCooldownString());
            }
            posOnMap /= _objectsOnMap.Length;
        }

        Vector2 pos = ViewController.Singleton.Camera3D.WorldToScreenPoint(posOnMap);
        pos.y = Screen.height - pos.y - (_control.Height)*_scale.y;
        pos.x -= (_control.Width/2f)*_scale.x; // + (ViewController.Singleton.Camera3D.rect.x) * Screen.width; //Correct but at the moment not necessary
        _control.RelativePosition = pos;
    }

    public void Remove()
    {
        TouchInput.Singleton.ClearAll -= Remove;
        if (_removed) return;
        _removed = true;
        _root.Click -= OnClickParent;
        StartCoroutine(CRemove());
    }

    private IEnumerator CRemove()
    {
        yield return new WaitForEndOfFrame();
        TouchInput.Enabled = true;
        Destroy(gameObject);
    }
}
