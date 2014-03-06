using UnityEngine;

public class GUIObjectCreatureInfo : MonoBehaviour 
{
    private const string Prefab = "GUI/panel_creatureinfo";
    private const string BoxStr = "panel_box";
    private const string NameStr = "label_name";
    private const string BarStr = "sprite_lifebar_filled";
    private const string ElementStr = "sprite_element";
    private const string FactionStr = "sprite_faction";
    private const string CreatureStr = "sprite_creature";
    private const string ElementPrefix = "element_";
    private const string AvatarPrefix = "avatarbg_";
    private const string CreaturePrefix = "creatureavatar_";
    private static readonly string[] CreatureStrings = new []{"wolf", "giant"};

    private dfControl _control; //For Debugging
    private Creature _creature;
    private dfControl _box;
    private dfLabel _nameLabel;
    private dfSprite _elementSprite;
    private dfSprite _factionSprite;
    private dfSprite _creatureSprite;
    private dfSprite _lifebar;


    public static GUIObjectCreatureInfo Create(dfControl root, Creature creature)
    {
        dfControl cntrl = root.AddPrefab(Resources.Load<GameObject>(Prefab));
        cntrl.Size = cntrl.Parent.Size;
        cntrl.RelativePosition = Vector2.zero;
        cntrl.SendToBack();
        cntrl.SuspendLayout();
        GUIObjectCreatureInfo obj = cntrl.GetComponent<GUIObjectCreatureInfo>();
        obj.Init(creature);
        obj._control = cntrl; //For Debugging
        return obj;
    }

    public void Init(Creature creature)
    {
        Transform rootTransform = transform.Find(BoxStr);
        _box = rootTransform.GetComponent<dfControl>();
        _nameLabel = rootTransform.Find(NameStr).GetComponent<dfLabel>();
        _elementSprite = rootTransform.Find(ElementStr).GetComponent<dfSprite>();
        _factionSprite = rootTransform.Find(FactionStr).GetComponent<dfSprite>();
        _creatureSprite = rootTransform.Find(CreatureStr).GetComponent<dfSprite>();
        _lifebar = rootTransform.Find(BarStr).GetComponent<dfSprite>();
        SetCreature(creature);
    }

    public void SetCreature(Creature creature)
    {
        _creature = creature;
    }

    void Update()
    {
        UpdateCreature();
    }

    private void UpdateCreature()
    {
        if (_creature == null)  return;
        _box.Show();
        _nameLabel.Text = _creature.Name+", "+_creature.Level;
        _elementSprite.SpriteName = ElementPrefix + _creature.BaseElement.ToString().ToLower();
        _factionSprite.SpriteName = AvatarPrefix + GameManager.Singleton.Player.CurrentFaction.ToString().ToLower();
        _creatureSprite.SpriteName = CreaturePrefix + CreatureStrings[_creature.ModelID];
        _lifebar.FillAmount = _creature.HP/(float)_creature.HPMax;
    }
}
