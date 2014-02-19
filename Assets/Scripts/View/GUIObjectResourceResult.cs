using System;
using System.Security.AccessControl;
using UnityEngine;
using System.Collections;

public class GUIObjectResourceResult : MonoBehaviour, IPopupContent
{
    private const string Prefab = "GUI/panel_rsc_result";
    private const string ButtonStr = "button_ok";
    private const string ScrollPanelStr = "scrollpanel";
    private const string Prefix = "element_";

    public event Action ClosePopup;
    public GameObject ResultTemplate;

    private dfControl _root;

    [SerializeField] private string _textkeyButton = "blindtext";

    [SerializeField] private dfButton _button;

    [SerializeField] public string Text;

    private dfScrollPanel _scrollpanel;

    public string Button
    {
        get { return _textkeyButton; }
        set
        {
            if (_button == null)
            {
                GameObject obj = transform.FindChild(ButtonStr).gameObject;
                if (obj)
                    _button = obj.GetComponent<dfButton>();
            }
            _textkeyButton = value;
            if (_button != null)
                _button.Text = Localization.GetText(value);
        }
    }

    public static GUIObjectResourceResult Create(dfControl root, string textKeyText, string textKeyButton, FarmResult result)
    {
        SoundController.PlaySound(SoundController.SoundChoose, SoundController.ChannelSFX);
        dfControl cntrl = root.AddPrefab(Resources.Load<GameObject>(Prefab));
        cntrl.Size = cntrl.Parent.Size;
        cntrl.RelativePosition = Vector2.zero;
        GUIObjectResourceResult obj = cntrl.GetComponent<GUIObjectResourceResult>();
        obj._root = root;
        obj.Button = textKeyButton;
        obj._scrollpanel = obj.transform.Find(ScrollPanelStr).GetComponent<dfScrollPanel>();

        foreach (FarmResult.Driod driod in result.GetDriods())
        {
            dfControl panelControl = obj._scrollpanel.AddPrefab(obj.ResultTemplate);
            GUIObjectTextPanel panel = panelControl.GetComponent<GUIObjectTextPanel>();
            dfSprite sprite = panelControl.transform.Find("sprite_rsc").GetComponent<dfSprite>();

            panel.Text = textKeyText + "#" + driod.Count + "#" + Localization.GetText(driod.Level.ToString()) + "#" + Localization.GetText(Resource.ResourceTypes[driod.Element + 1].ToLower());
            sprite.SpriteName = Prefix + Resource.ResourceTypes[driod.Element + 1].ToLower();

        }
        obj.ResultTemplate.SetActive(false);
        return obj;
    }
}