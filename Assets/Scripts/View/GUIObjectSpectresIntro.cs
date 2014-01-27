using UnityEngine;
using System.Collections;

public class GUIObjectSpectresIntro : MonoBehaviour {
    private const string Prefab = "GUI/panel_spectresintro";


    public static GUIObjectSpectresIntro Create(dfControl root, string textKeyText)
    {
        dfControl cntrl = root.AddPrefab(Resources.Load<GameObject>(Prefab));
        cntrl.Size = cntrl.Parent.Size;
        cntrl.RelativePosition = Vector2.zero;
        GUIObjectSpectresIntro obj = cntrl.GetComponent<GUIObjectSpectresIntro>();
        GUIObjectTextPanel panel = cntrl.GetComponent<GUIObjectTextPanel>();
        panel.Text = textKeyText;
        return obj;
    }
}
