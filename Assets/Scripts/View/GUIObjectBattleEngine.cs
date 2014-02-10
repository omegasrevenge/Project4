using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GUIObjectBattleEngine : MonoBehaviour
{
    public List<int> InputText;
    public float GUIDistance = 1.5f;
    public float DamageIndicatorMoveUpSpeed = 0.1f;

    private Camera Camera { get { return ViewController.Singleton.Camera3D; } }

    private const string Prefab = "GUI/panel_battleui";

    public GameObject MonsterAContainer;
    public GameObject MonsterBContainer;
    public GameObject MonsterAElement;
    public GameObject MonsterBElement;
    public GameObject MonsterALevel;
    public GameObject MonsterBLevel;
    public GameObject MonsterAName;
    public GameObject MonsterBName;
    public GameObject MonsterAHealth;
    public GameObject MonsterBHealth;
    public GameObject MonsterAHealthText;
    public GameObject MonsterBHealthText;
    public GameObject IndicatorOne;
    public GameObject IndicatorTwo;
    public GameObject IndicatorThree;
    public GameObject IndicatorFour;
    public GameObject DriodContainer;
    public dfPanel GGContainer;
    public dfSprite MonsterACon;
    public dfSprite MonsterBCon;
    public dfSprite MonsterADot;
    public dfSprite MonsterBDot;
    public dfSprite MonsterAHot;
    public dfSprite MonsterBHot;
    public dfSprite MonsterABuff;
    public dfSprite MonsterBBuff;
    public List<dfSprite> ComboIndicators;
    public List<dfButton> DriodSlots; 

    public bool Initialized
    {
        get
        {
            return MonsterAContainer != null;
        }
    }

    public Player.Faction Faction
    {
        get { return GameManager.Singleton.Player.CurrentFaction; }
    }

    public static GUIObjectBattleEngine Create(dfControl root)
    {
        dfControl cntrl = root.AddPrefab(Resources.Load<GameObject>(Prefab));
        cntrl.Size = cntrl.Parent.Size;
        cntrl.RelativePosition = Vector2.zero;
        return cntrl.GetComponent<GUIObjectBattleEngine>();
    }

    void Awake()
    {
        InputText = new List<int>();
        ComboIndicators = new List<dfSprite>();
        DriodSlots = new List<dfButton>();
        GGContainer = transform.FindChild("GGScreen").GetComponent<dfPanel>();
        DriodContainer = transform.FindChild("ButtonContainer").FindChild("BG_Driods").gameObject;
        DriodSlots.Add(DriodContainer.transform.FindChild("Slot_Driod1").GetComponent<dfButton>());
        DriodSlots.Add(DriodContainer.transform.FindChild("Slot_Driod2").GetComponent<dfButton>());
        DriodSlots.Add(DriodContainer.transform.FindChild("Slot_Driod3").GetComponent<dfButton>());
        DriodSlots.Add(DriodContainer.transform.FindChild("Slot_Driod4").GetComponent<dfButton>());
        ComboIndicators.Add(DriodContainer.transform.FindChild("Combo_01").GetComponent<dfSprite>());
        ComboIndicators.Add(DriodContainer.transform.FindChild("Combo_02").GetComponent<dfSprite>());
        ComboIndicators.Add(DriodContainer.transform.FindChild("Combo_03").GetComponent<dfSprite>());
        ComboIndicators.Add(DriodContainer.transform.FindChild("Combo_12").GetComponent<dfSprite>());
        ComboIndicators.Add(DriodContainer.transform.FindChild("Combo_13").GetComponent<dfSprite>());
        ComboIndicators.Add(DriodContainer.transform.FindChild("Combo_23").GetComponent<dfSprite>());
        MonsterAContainer = transform.FindChild("MonsterAInfo").gameObject;
        MonsterBContainer = transform.FindChild("MonsterBInfo").gameObject;
        IndicatorOne = transform.FindChild("HappenstanceIndicator1").gameObject;
        IndicatorTwo = transform.FindChild("HappenstanceIndicator2").gameObject;
        IndicatorThree = transform.FindChild("HappenstanceIndicator3").gameObject;
        IndicatorFour = transform.FindChild("HappenstanceIndicator4").gameObject;
        MonsterAElement = MonsterAContainer.transform.FindChild("MonsterElement").gameObject;
        MonsterBElement = MonsterBContainer.transform.FindChild("MonsterElement").gameObject;
        MonsterALevel = MonsterAContainer.transform.FindChild("BG_MonsterLevel").FindChild("MonsterLevel").gameObject;
        MonsterBLevel = MonsterBContainer.transform.FindChild("BG_MonsterLevel").FindChild("MonsterLevel").gameObject;
        MonsterAName = MonsterAContainer.transform.FindChild("MonsterName").gameObject;
        MonsterBName = MonsterBContainer.transform.FindChild("MonsterName").gameObject;
        MonsterAHealth = MonsterAContainer.transform.FindChild("MonsterHealthProgress").gameObject;
        MonsterBHealth = MonsterBContainer.transform.FindChild("MonsterHealthProgress").gameObject;
        MonsterAHealthText = MonsterAHealth.transform.FindChild("MonsterHealthText").gameObject;
        MonsterBHealthText = MonsterBHealth.transform.FindChild("MonsterHealthText").gameObject;
        MonsterACon = MonsterAContainer.transform.FindChild("ConIndicator").GetComponent<dfSprite>();
        MonsterBCon = MonsterBContainer.transform.FindChild("ConIndicator").GetComponent<dfSprite>();
        MonsterADot = MonsterAContainer.transform.FindChild("DotIndicator").GetComponent<dfSprite>();
        MonsterBDot = MonsterBContainer.transform.FindChild("DotIndicator").GetComponent<dfSprite>();
        MonsterAHot = MonsterAContainer.transform.FindChild("HotIndicator").GetComponent<dfSprite>();
        MonsterBHot = MonsterBContainer.transform.FindChild("HotIndicator").GetComponent<dfSprite>();
        MonsterABuff = MonsterAContainer.transform.FindChild("DefIndicator").GetComponent<dfSprite>();
        MonsterBBuff = MonsterBContainer.transform.FindChild("DefIndicator").GetComponent<dfSprite>();
    }

    public void Init()
    {
        UpdateFaction();
        GGContainer.Hide();
        DeleteSelection();
        MonsterALevel.GetComponent<dfLabel>().Text = BattleEngine.Current.ServerInfo.MonsterALevel.ToString();
        MonsterBLevel.GetComponent<dfLabel>().Text = BattleEngine.Current.ServerInfo.MonsterBLevel.ToString();
        MonsterAName.GetComponent<dfLabel>().Text = BattleEngine.Current.ServerInfo.MonsterAName;
        MonsterBName.GetComponent<dfLabel>().Text = BattleEngine.Current.ServerInfo.MonsterBName;
        UpdateHealth();
    }

    void Update()
    {
        if (BattleEngine.CurrentGameObject == null 
            || !BattleEngine.Current.Initialized 
            || !BattleEngine.Current.Fighting) 
            return;

        UpdateSelection();
    }

    public void KillBattle()
    {
        BattleEngine.Current.EndBattle();
        GameManager.Singleton.SwitchGameMode(GameManager.GameMode.Map);
    }

    public void UpdateSelection()
    {
        foreach (dfSprite comboIndicator in ComboIndicators)
            comboIndicator.Hide();
        for (int i = 0; i < DriodSlots.Count; i++)
        {
            if (InputText.Contains(i))
                DriodSlots[i].BackgroundSprite = Faction == Player.Faction.NCE ?
                    "combat_slot_active_nce" : "combat_slot_active_vengea";
            else
                DriodSlots[i].BackgroundSprite = "combat_slot";
        }

        if (InputText.Count < 2) return;

        for (int i = 0; i < InputText.Count - 1; i++)
        {
            int s1 = InputText[i];
            int s2 = InputText[i + 1];
            if (s1 > s2)
            {
                int a = s1;
                s1 = s2;
                s2 = a;
            }
            int ind = -1;
            if (s1 == 0 && s2 == 1) ind = 0;
            if (s1 == 0 && s2 == 2) ind = 1;
            if (s1 == 0 && s2 == 3) ind = 2;
            if (s1 == 1 && s2 == 2) ind = 3;
            if (s1 == 1 && s2 == 3) ind = 4;
            if (s1 == 2 && s2 == 3) ind = 5;
            if (ind == -1) continue;
            ComboIndicators[ind].GetComponent<dfSprite>().Show();
        }
    }

    public void UpdateFaction()
    {
        string faction = Faction == Player.Faction.NCE ? "nce" : "vengea";
        ComboIndicators[0].SpriteName = "connection_" + faction + "_1to2";
        ComboIndicators[1].SpriteName = "connection_" + faction + "_1to3";
        ComboIndicators[2].SpriteName = "connection_" + faction + "_1to4";
        ComboIndicators[3].SpriteName = "connection_" + faction + "_2to3";
        ComboIndicators[4].SpriteName = "connection_" + faction + "_2to4";
        ComboIndicators[5].SpriteName = "connection_" + faction + "_3to4";
        DriodContainer.GetComponent<dfPanel>().BackgroundSprite = "combat_panel_" + faction;
        DriodContainer.transform.parent.FindChild("Flee").GetComponent<dfButton>().BackgroundSprite = "combat_flee_" + faction;
        DriodContainer.transform.parent.FindChild("Catch").GetComponent<dfButton>().BackgroundSprite = "combat_catch_" + faction;
    }

    public void UpdateStatusAilments()
    {
        if (GameManager.Singleton.Player.CurFight.FighterAConfused)
            MonsterACon.Show();
        else MonsterACon.Hide();
        if (GameManager.Singleton.Player.CurFight.FighterBConfused)
            MonsterBCon.Show();
        else MonsterBCon.Hide();
        if (GameManager.Singleton.Player.CurFight.FighterABurning)
            MonsterADot.Show();
        else MonsterADot.Hide();
        if (GameManager.Singleton.Player.CurFight.FighterBBurning)
            MonsterBDot.Show();
        else MonsterBDot.Hide();
        if (GameManager.Singleton.Player.CurFight.FighterARegen)
            MonsterAHot.Show();
        else MonsterAHot.Hide();
        if (GameManager.Singleton.Player.CurFight.FighterBRegen)
            MonsterBHot.Show();
        else MonsterBHot.Hide();
        if (GameManager.Singleton.Player.CurFight.FighterABuffed)
            MonsterABuff.Show();
        else MonsterABuff.Hide();
        if (GameManager.Singleton.Player.CurFight.FighterBBuffed)
            MonsterBBuff.Show();
        else MonsterBBuff.Hide();
    }

    public void CreateDamageIndicator(GameObject target, int damage, int dot, bool heal = false)
	{
	    if (target == BattleEngine.Current.FriendlyCreature)
        {
            if (!heal)
                IndicatorOne.GetComponent<dfLabel>().Text =
                    damage + "DMG " + BattleEngine.Current.Result.SkillName + " & " + dot + "DoT ->" + BattleEngine.Current.Result.SkillName + "<-";
            else
                IndicatorOne.GetComponent<dfLabel>().Text = damage + "HoT";
            Vector3 startPos =
                (BattleEngine.Current.FriendlyCreature.transform.position -
                 BattleEngine.Current.Camera.transform.position).normalized*GUIDistance +
                BattleEngine.Current.Camera.transform.position;
            IndicatorOne.GetComponent<IndicatorController>().Play(startPos, 3f, new Vector3(0f, DamageIndicatorMoveUpSpeed*Time.deltaTime, 0f));
	    }
        else 
        if (target == BattleEngine.Current.EnemyCreature)
        {
            if (!heal)
                IndicatorTwo.GetComponent<dfLabel>().Text =
                    damage + "DMG " + BattleEngine.Current.Result.SkillName + " & " + dot + "DoT ->" + BattleEngine.Current.Result.SkillName + "<-";
            else
                IndicatorTwo.GetComponent<dfLabel>().Text = damage + "HoT";
            Vector3 startPos =
                (BattleEngine.Current.EnemyCreature.transform.position -
                 BattleEngine.Current.Camera.transform.position).normalized * GUIDistance +
                BattleEngine.Current.Camera.transform.position;
            IndicatorTwo.GetComponent<IndicatorController>().Play(startPos, 3f, new Vector3(0f, DamageIndicatorMoveUpSpeed*Time.deltaTime, 0f));
        }
        UpdateHealth();
        UpdateStatusAilments();
    }

    public void UpdateHealth()
    {
        MonsterAHealthText.GetComponent<dfLabel>().Text = GameManager.Singleton.Player.CurCreature.HP + "/" + GameManager.Singleton.Player.CurCreature.HPMax;
        MonsterAHealth.GetComponent<dfProgressBar>().Value = (float)GameManager.Singleton.Player.CurCreature.HP / GameManager.Singleton.Player.CurCreature.HPMax;
        MonsterBHealthText.GetComponent<dfLabel>().Text = GameManager.Singleton.Player.CurFight.EnemyCreature.HP + "/" + GameManager.Singleton.Player.CurFight.EnemyCreature.HPMax;
        MonsterBHealth.GetComponent<dfProgressBar>().Value = (float)GameManager.Singleton.Player.CurFight.EnemyCreature.HP / GameManager.Singleton.Player.CurFight.EnemyCreature.HPMax;
    }

    private void buttonKlick(int driod)
    {
        if (InputText.Count > 0 && InputText[InputText.Count - 1] == driod)
        {
            InputText.Remove(driod);
            return;
        }

        if (InputText.Contains(driod)) return;

        InputText.Add(driod);
        //InputText is a List<int> filled with the indexes of the selected driods
        // CHECK FOR WHAT SOUND TO PLAY ON BUTTON KLICK HERE!
    }

    public void FirstDriodKlicked()
    {
        buttonKlick(0);
    }

    public void SecondDriodKlicked()
    {
        buttonKlick(1);
    }

    public void ThirdDriodKlicked()
    {
        buttonKlick(2);
    }

    public void FourthDriodKlicked()
    {
        buttonKlick(3);
    }

    public void ExecuteKlicked()
    {
        switch (InputText.Count)
        {
            case 0:
                GameManager.Singleton.FightPlayerTurn(-1, -1, -1, -1);
                break;
            case 1:
                GameManager.Singleton.FightPlayerTurn(InputText[0], -1, -1, -1);
                break;
            case 2:
                GameManager.Singleton.FightPlayerTurn(InputText[0], InputText[1], -1, -1);
                break;
            case 3:
                GameManager.Singleton.FightPlayerTurn(InputText[0], InputText[1], InputText[2], -1);
                break;
            case 4:
                GameManager.Singleton.FightPlayerTurn(InputText[0], InputText[1], InputText[2], InputText[3]);
                break;
        }
        DeleteSelection();
    }

    public void EscapeKlicked()
    {
        GameManager.Singleton.EscapeAttempt();
        DeleteSelection();
    }

    public void CatchKlicked()
    {
        GameManager.Singleton.CatchAttempt();
        DeleteSelection();
    }

    public void DeleteSelection()
    {
        InputText.Clear();
    }
}
