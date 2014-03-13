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
            {
                _button.Text = Localization.GetText(value);
                if (GameManager.Singleton.Player.CurrentFaction == Player.Faction.NCE)
                    _button.TextColor = GameManager.Black;
            }
                
        }
    }

    public static GameObject Create(string textKeyText, string textKeyButton, FarmResult result)
    {
        SoundController.PlaySound(SoundController.SFXlocation+SoundController.Faction+SoundController.SoundFacChoose, SoundController.ChannelSFX);

        GameObject go = Instantiate(Resources.Load<GameObject>(Prefab)) as GameObject;
        GUIObjectResourceResult resourceContent = go.GetComponent<GUIObjectResourceResult>();
        resourceContent.Button = textKeyButton;

        resourceContent._scrollpanel = resourceContent.transform.Find(ScrollPanelStr).GetComponent<dfScrollPanel>();

        foreach (FarmResult.Driod driod in result.GetDriods())
        {
            dfControl panelControl = resourceContent._scrollpanel.AddPrefab(resourceContent.ResultTemplate);
            GUIObjectTextPanel panel = panelControl.GetComponent<GUIObjectTextPanel>();
            if (GameManager.Singleton.Player.CurrentFaction == Player.Faction.NCE)
                panel.transform.FindChild("label_text").GetComponent<dfLabel>().Color = GameManager.Black;
            dfSprite sprite = panelControl.transform.Find("sprite_rsc").GetComponent<dfSprite>();

            panel.Text = textKeyText + "#" + driod.Count + "#" + Localization.GetText(driod.Level.ToString()) + "#" + Localization.GetText(Resource.ResourceTypes[driod.Element + 1].ToLower());
            sprite.SpriteName = Prefix + Resource.ResourceTypes[driod.Element + 1].ToLower();

        }
        resourceContent.ResultTemplate.SetActive(false);
        return go;
    }
}