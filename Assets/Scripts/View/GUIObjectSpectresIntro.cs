using UnityEngine;

public class GUIObjectSpectresIntro : MonoBehaviour {
    private const string Prefab = "GUI/panel_spectresintro";
    private const string VisualizerStr = "panel_visualizer";
    public GUIObjectVisualizer Visualizer;

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

    void Awake()
    {
        GameObject obj = transform.FindChild(VisualizerStr).gameObject;
        if (obj)
            Visualizer = obj.GetComponent<GUIObjectVisualizer>();
    }
}
