using UnityEngine;
using System.Collections;

public class GUIObjectMarker : MonoBehaviour 
{
    private const string Prefab = "GUI/panel_marker";
    private const string AgentStr           = "panel_agent";
    private const string BaseStr            = "panel_base";
    private const string CooldownStr        = "panel_cooldown";
    private const string HealStr            = "panel_healingspot";
    private const string InterferenceStr    = "panel_interference";
    private const string ResourceStr        = "panel_resource";
    private const string SpectreStr         = "panel_spectre";
    private const float ReferenceHeight = 1280f;

    public static GUIObjectMarker Create(dfControl root, TouchObject[] objects)
    {
        dfControl cntrl = root.AddPrefab(Resources.Load<GameObject>(Prefab));
        Vector3 scale  = cntrl.transform.localScale;
        scale *= (Screen.height/ReferenceHeight);
        cntrl.transform.localScale = scale;
        GUIObjectMarker obj = cntrl.GetComponent<GUIObjectMarker>();

        return obj;
    }
}
