using System;
using UnityEngine;
using System.Collections;

public class GUIObjectMonsterNameInput : MonoBehaviour
{
	public GameObject giant;
	public GameObject wolf;

    private const string Prefab = "GUI/panel_monsternameinput";
    private const string ButtonStr = "button_submit";
	private const string TextboxStr = "textbox_name";
	private const string ElementStr = "sprite_element";
	private const string CreatureStr = "sprite_creature";
	private const string ElementPrefix = "element_";
	private const string CreaturePrefix = "creatureavatar_";
	private static readonly string[] CreatureStrings = new[] { "wolf", "giant" };

	private Creature _creature;
	private dfSprite _elementSprite;
	private dfSprite _creatureSprite;

    [SerializeField]
    private string _textkeyDefault = "blindtext";
    [SerializeField]
    private string _textkeyButton = "blindtext";

    [SerializeField]
    private dfButton _button;

    [SerializeField] public string Text;

    public string Default
    {
        get { return _textkeyDefault; }
        set
        {
            _textkeyDefault = value;
            Text = Localization.GetText(value);
        }
    }

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

	public void ShowCreature()
	{
		_elementSprite = transform.Find(ElementStr).GetComponent<dfSprite>();
		_creatureSprite = transform.Find(CreatureStr).GetComponent<dfSprite>();

		if (_creature != null)
		{
			_elementSprite.SpriteName = ElementPrefix + _creature.BaseElement.ToString().ToLower();
			_creatureSprite.SpriteName = CreaturePrefix + CreatureStrings[_creature.ModelID];
		}
	}

    void Awake()
    {
        GameObject obj = transform.FindChild(TextboxStr).gameObject;

        if (obj)
        {
            dfTextbox box = obj.GetComponent<dfTextbox>();
            box.Click +=
                (control, @event) =>
                {
                    SoundController.PlaySound(SoundController.SoundClick, SoundController.ChannelSFX);
                };
        }

        if (_button == null)
        {
            obj = transform.FindChild(ButtonStr).gameObject;
            if (obj)
            {
                _button = obj.GetComponent<dfButton>();
            }
        }
        if (_button != null)
        {
            _button.Click +=
                (control, @event) =>
                {
                    SoundController.PlaySound(SoundController.SoundClick, SoundController.ChannelSFX);
                };
        }
    }

    public static GameObject Create(string textKeyTitle, string textKeyText, string textKeyButton, string textKeyUsername, Creature creature, Action<string> callback)
    {
        GameObject go = Instantiate(Resources.Load<GameObject>(Prefab)) as GameObject;
        GUIObjectMonsterNameInput input = go.GetComponent<GUIObjectMonsterNameInput>();
        input.Button = textKeyButton;
        input._creature = creature;
		input.ShowCreature();
		if(callback != null)
            input._button.Click += (control, @event) =>
            {
	            if (callback != null)
	            {
		            callback(input.Text);
	            }
            };
        input.Default = textKeyUsername;

        GUIObjectTextPanel panel = go.GetComponent<GUIObjectTextPanel>();
        panel.Title = textKeyTitle;
        panel.Text = textKeyText;

		

        return go;
    }
}
