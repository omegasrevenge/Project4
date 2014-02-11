using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    public GameObject MonsterALevel;
    public GameObject MonsterBLevel;
    public GameObject MonsterAName;
    public GameObject MonsterBName;
    public GameObject MonsterAHealth;
    public GameObject MonsterBHealth;
    public GameObject MonsterAHealthText;
    public GameObject MonsterBHealthText;
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
    public dfSprite MonsterAElement;
    public dfSprite MonsterBElement;
    public List<dfSprite> ComboIndicators;
    public List<dfButton> DriodSlots;
    public List<dfButton> Driods;
    public List<dfLabel> TxtIndicators;
    public List<dfSprite> DriodsHealth; 

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

    public bool IndsArePlaying
    {
        get
        {
            return TxtIndicators.Any(txtIndicator => txtIndicator.GetComponent<IndicatorController>().IsPlaying);
        }
    }

    public struct IndicatorContent
    {
        public GameObject target;
        public string name;
        public int value;

        public IndicatorContent(GameObject t, string n, int v)
        {
            target = t;
            name = n;
            value = v;
        }
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
        DriodsHealth = new List<dfSprite>();
        TxtIndicators = new List<dfLabel>();
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
        Driods.Add(DriodContainer.transform.FindChild("Driod1").GetComponent<dfButton>());
        Driods.Add(DriodContainer.transform.FindChild("Driod2").GetComponent<dfButton>());
        Driods.Add(DriodContainer.transform.FindChild("Driod3").GetComponent<dfButton>());
        Driods.Add(DriodContainer.transform.FindChild("Driod4").GetComponent<dfButton>());
        DriodsHealth.Add(DriodContainer.transform.FindChild("Health_Driod1").GetComponent<dfSprite>());
        DriodsHealth.Add(DriodContainer.transform.FindChild("Health_Driod2").GetComponent<dfSprite>());
        DriodsHealth.Add(DriodContainer.transform.FindChild("Health_Driod3").GetComponent<dfSprite>());
        DriodsHealth.Add(DriodContainer.transform.FindChild("Health_Driod4").GetComponent<dfSprite>());
        MonsterAContainer = transform.FindChild("MonsterAInfo").gameObject;
        MonsterBContainer = transform.FindChild("MonsterBInfo").gameObject;
        TxtIndicators.Add(transform.FindChild("HappenstanceIndicator1").GetComponent<dfLabel>());
        TxtIndicators.Add(transform.FindChild("HappenstanceIndicator2").GetComponent<dfLabel>());
        TxtIndicators.Add(transform.FindChild("HappenstanceIndicator3").GetComponent<dfLabel>());
        TxtIndicators.Add(transform.FindChild("HappenstanceIndicator4").GetComponent<dfLabel>());
        TxtIndicators.Add(transform.FindChild("HappenstanceIndicator5").GetComponent<dfLabel>());
        TxtIndicators.Add(transform.FindChild("HappenstanceIndicator6").GetComponent<dfLabel>());
        MonsterAElement = MonsterAContainer.transform.FindChild("MonsterElement").GetComponent<dfSprite>();
        MonsterBElement = MonsterBContainer.transform.FindChild("MonsterElement").GetComponent<dfSprite>();
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
        UpdateMonsterElements();
        UpdateFaction();
        GGContainer.Hide();
        DeleteSelection();
        MonsterALevel.GetComponent<dfLabel>().Text = BattleEngine.Current.ServerInfo.MonsterALevel.ToString();
        MonsterBLevel.GetComponent<dfLabel>().Text = BattleEngine.Current.ServerInfo.MonsterBLevel.ToString();
        MonsterAName.GetComponent<dfLabel>().Text = BattleEngine.Current.ServerInfo.MonsterAName;
        MonsterBName.GetComponent<dfLabel>().Text = BattleEngine.Current.ServerInfo.MonsterBName;
		UpdateVisualsOnSkillHit ();
    }

    void Update()
    {
        if (BattleEngine.CurrentGameObject == null 
            || !BattleEngine.Current.Initialized 
            || !BattleEngine.Current.Fighting) 
            return;

        UpdateSelection();
    }

    public void ShowDamageIndicators(List<IndicatorContent> info)
    {
        if (info.Count > TxtIndicators.Count)
        {
            Debug.LogError("More than " + TxtIndicators.Count + " Happenstance Indicators? " + info.Count + " are too many.");
            return;
        }

        for (int i = 0; i < info.Count; i++)
        {
            string content = "";

            if (info[i].value > 0)
                content = info[i].value + " ";

            if (info[i].name.Length > 0)
                content += info[i].name + "!";

            TxtIndicators[i].Text = content;

            Vector3 startPos =
                (info[i].target.transform.position - BattleEngine.Current.Camera.transform.position).normalized * GUIDistance + BattleEngine.Current.Camera.transform.position;

            TxtIndicators[i].GetComponent<IndicatorController>().Play(startPos, 3f, new Vector3(0f, DamageIndicatorMoveUpSpeed * Time.deltaTime, 0f), 1f*i);
        }
    }

    public void UpdateVisualsOnSkillHit()
    {
        UpdateDriodHealth();
        UpdateHealth();
        UpdateStatusAilments();
        UpdateButtons();
    }

    public void UpdateDriodHealth()
    {
        for (int i = 0; i < DriodsHealth.Count; i++)
        {
            if (!Driods[i].IsVisible) continue;

            DriodsHealth[i].FillAmount = 0.57f + GameManager.Singleton.Player.CurCreature.slots[i].driodenHealth * 0.36f;
        }
    }

    public void UpdateMonsterElements()
    {
        switch (GameManager.Singleton.Player.CurCreature.BaseElement)
        {
            case BattleEngine.ResourceElement.None:
                MonsterAElement.Hide();
                break;

            case BattleEngine.ResourceElement.Energy:
                MonsterAElement.SpriteName = "spectre_stats_element_energy";
                break;

            case BattleEngine.ResourceElement.Fire:
                MonsterAElement.SpriteName = "spectre_stats_element_fire";
                break;

            case BattleEngine.ResourceElement.Nature:
                MonsterAElement.SpriteName = "spectre_stats_element_life";
                break;

            case BattleEngine.ResourceElement.Storm:
                MonsterAElement.SpriteName = "spectre_stats_element_storm";
                break;

            case BattleEngine.ResourceElement.Water:
                MonsterAElement.SpriteName = "spectre_stats_element_water";
                break;
        }
        switch (GameManager.Singleton.Player.CurFight.EnemyCreature.BaseElement)
        {
            case BattleEngine.ResourceElement.None:
                MonsterBElement.Hide();
                break;

            case BattleEngine.ResourceElement.Energy:
                MonsterBElement.SpriteName = "spectre_stats_element_energy";
                break;

            case BattleEngine.ResourceElement.Fire:
                MonsterBElement.SpriteName = "spectre_stats_element_fire";
                break;

            case BattleEngine.ResourceElement.Nature:
                MonsterBElement.SpriteName = "spectre_stats_element_life";
                break;

            case BattleEngine.ResourceElement.Storm:
                MonsterBElement.SpriteName = "spectre_stats_element_storm";
                break;

            case BattleEngine.ResourceElement.Water:
                MonsterBElement.SpriteName = "spectre_stats_element_water";
                break;
        }
    }

    public void UpdateButtons()
    {
        for (int i = 0; i < Driods.Count; i++)
        {
            if (GameManager.Singleton.Player.CurCreature.slots.Length > i)
            {
                Driods[i].Show();
                switch (GameManager.Singleton.Player.CurCreature.slots[i].driodenElement)
                {
                    case BattleEngine.ResourceElement.None:
                        Driods[i].Hide();
                        break;

                    case BattleEngine.ResourceElement.Energy:
                        Driods[i].BackgroundSprite = "combat_driod_energy";
                        break;

                    case BattleEngine.ResourceElement.Fire:
                        Driods[i].BackgroundSprite = "combat_driod_fire";
                        break;

                    case BattleEngine.ResourceElement.Nature:
                        Driods[i].BackgroundSprite = "combat_driod_life";
                        break;

                    case BattleEngine.ResourceElement.Storm:
                        Driods[i].BackgroundSprite = "combat_driod_storm";
                        break;

                    case BattleEngine.ResourceElement.Water:
                        Driods[i].BackgroundSprite = "combat_driod_water";
                        break;
                }
            }
            else Driods[i].Hide();
        }
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
        if (BattleEngine.Current.Result == null) return;
        if (BattleEngine.Current.Result.ConA)
            MonsterACon.Show();
        else MonsterACon.Hide();
        if (BattleEngine.Current.Result.ConB)
            MonsterBCon.Show();
        else MonsterBCon.Hide();
        if (BattleEngine.Current.Result.DotA)
            MonsterADot.Show();
        else MonsterADot.Hide();
        if (BattleEngine.Current.Result.DotB)
            MonsterBDot.Show();
        else MonsterBDot.Hide();
        if (BattleEngine.Current.Result.HotA)
            MonsterAHot.Show();
        else MonsterAHot.Hide();
        if (BattleEngine.Current.Result.HotB)
            MonsterBHot.Show();
        else MonsterBHot.Hide();
        if (BattleEngine.Current.Result.BuffA)
            MonsterABuff.Show();
        else MonsterABuff.Hide();
        if (BattleEngine.Current.Result.BuffB)
            MonsterBBuff.Show();
        else MonsterBBuff.Hide();
    }

    public void UpdateHealth()
    {
        int friendlyHP = GameManager.Singleton.Player.CurCreature.HP;
        int friendlyMaxHP = GameManager.Singleton.Player.CurCreature.HPMax;
        int enemyHP = GameManager.Singleton.Player.CurFight.EnemyCreature.HP;
        int enemyMaxHP = GameManager.Singleton.Player.CurFight.EnemyCreature.HPMax;

        if (friendlyHP > 0)
        {
            MonsterAHealthText.GetComponent<dfLabel>().Text =
                friendlyHP + "/" + friendlyMaxHP;
            MonsterAHealth.GetComponent<dfProgressBar>().Value = 
                (float)friendlyHP / friendlyMaxHP;
        }
        else
        {
            MonsterAHealthText.GetComponent<dfLabel>().Text = "0/" + friendlyMaxHP;
            MonsterAHealth.GetComponent<dfProgressBar>().Value = 0;
        }

        if (enemyHP > 0)
        {
            MonsterBHealthText.GetComponent<dfLabel>().Text =
                enemyHP + "/" + enemyMaxHP;
            MonsterBHealth.GetComponent<dfProgressBar>().Value =
                (float)enemyHP / enemyMaxHP;
        }
        else
        {
            MonsterBHealthText.GetComponent<dfLabel>().Text = "0/" + enemyMaxHP;
            MonsterBHealth.GetComponent<dfProgressBar>().Value = 0;
        }

    }

    private void buttonKlick(int driod)
    {
        if (GameManager.Singleton.Player.CurCreature.slots.Length-1 < driod) return;

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
        if (BattleEngine.Current.GetTurnState != BattleEngine.TurnState.Wait)
            return;
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
