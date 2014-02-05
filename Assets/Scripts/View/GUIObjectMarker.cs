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
    private const string CooldownStr        = "POIs/panel_cooldown";
    private const string HealStr            = "POIs/panel_healingspot";
    private const string InterferenceStr    = "POIs/panel_interference";
    private const string ResourceStr        = "POIs/panel_resource";
    private const string SpectreStr         = "POIs/panel_spectre";
    private const float ReferenceHeight = 1280f;
    private const float ArrowHeight = 50f;
    private const float Padding = 11f;
    private const float PanelHeight = 128f;


    private dfControl _control;
    private Vector2 _pos;
    private ObjectOnMap _first;

    public static GUIObjectMarker Create(dfControl root, ObjectOnMap[] objects)
    {
        dfControl cntrl = root.AddPrefab(Resources.Load<GameObject>(Prefab));
        Vector3 scale  = cntrl.transform.localScale;
        scale *= (Screen.height/ReferenceHeight);
        cntrl.transform.localScale = scale;
        Vector3 size = cntrl.Size;
        size.y = ArrowHeight + objects.Length*(Padding + PanelHeight) + Padding;
        cntrl.Size = size;
        GUIObjectMarker obj = cntrl.GetComponent<GUIObjectMarker>();
        obj._control = cntrl;
        obj._pos = new Vector2(Padding, Padding);

        foreach (ObjectOnMap objectOnMap in objects)
        {
            dfControl control = CreateMarkerPanel(objectOnMap);
            obj.AddMarkerPanel(control);
        }
        if (objects.Length > 0)
            obj._first = objects[0];

        return obj;
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

    private static dfControl CreateMarkerPanel(ObjectOnMap obj)
    {
        if (obj is Resource)
        {
            dfControl control = CreateObjectOnMap(obj, ResourceStr);
            MarkerBiod marker = control.GetComponent<MarkerBiod>();
            marker.SetElementSymbol((obj as Resource).GetElement());
            return control;
        }
        if (obj is PlayerOnMap)
        {
            string path = AgentStr;
            if ((obj as PlayerOnMap).playerData.CurrentFaction == Player.Faction.NCE)
                path = InterferenceStr;
            return CreateObjectOnMap(obj, path);
        }
        if (obj is BaseOnMap)
        {
            return CreateObjectOnMap(obj,BaseStr);
        }
        if (obj is Spectre)
        {
            return CreateObjectOnMap(obj, SpectreStr);
        }
        if (obj is HealStation)
        {
            return CreateObjectOnMap(obj, HealStr);
        }
        return null;
    }

    private static dfControl CreateObjectOnMap(ObjectOnMap objectOnMap, string path)
    {
        GameObject go = (GameObject)Instantiate(Resources.Load<GameObject>(path));
        dfControl control = go.GetComponent<dfControl>();
        control.Click += (dfControl, args) =>
        {
            if (args.Used) return;
            args.Use();
            objectOnMap.Execute();
        };

        return control;
    }

    public void Update()
    {
        if (Input.touchCount > 0)
        {
            //Destroy(gameObject);
            return;
        }

        Vector2 pos = _first.GetScreenPosition();
        pos.y = Screen.height - pos.y;
        _control.RelativePosition = pos;
    }
}
