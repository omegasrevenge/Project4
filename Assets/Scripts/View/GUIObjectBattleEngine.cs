using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class GUIObjectBattleEngine : MonoBehaviour
{
    public List<int> InputText;
    public float GUIDistance = 1.5f;
    public float DamageIndicatorMoveUpSpeed = 0.1f;

    private const string Prefab = "GUI/panel_battleui";
    private int _lastButton = -1;

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
    public dfProgressBar MonsterAHealthBg;
    public dfProgressBar MonsterBHealthBg;
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
	public dfButton ButtonExecute;
    public List<dfSprite> ComboIndicators;
    public List<dfButton> DriodSlots;
    public List<dfButton> Driods;
    public List<dfLabel> TxtIndicators;
    public List<dfSprite> DriodsHealth;
	public List<List<GameManager.ResourceElement>> DriodImprint;
    public List<List<dfPanel>> DriodImprintVisuals;

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

    public Creature.Slot[] Slots
    {
        get { return GameManager.Singleton.Player.CurCreature.slots; }
    }

    public int PresentDriodsCount
    {
        get
        {
            int value = 0;
            for (int i = 0; i < Driods.Count; i++)
            {
                if (GameManager.Singleton.Player.CurCreature.slots.Length > i)
                    value++;
            }
            int max = value;
            for (int i = 0; i < max; i++)
            {
				if (GameManager.Singleton.Player.CurCreature.slots[i].driodenElement == GameManager.ResourceElement.None)
                    value--;
            }
            return value;
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
		DriodImprint = new List<List<GameManager.ResourceElement>>();
        DriodImprintVisuals = new List<List<dfPanel>>();
        DriodsHealth = new List<dfSprite>();
        TxtIndicators = new List<dfLabel>();
        InputText = new List<int>();
        ComboIndicators = new List<dfSprite>();
        DriodSlots = new List<dfButton>();
        GGContainer = transform.FindChild("GGScreen").GetComponent<dfPanel>();
		ButtonExecute = transform.FindChild("ExecuteButton").GetComponent<dfButton>();
        DriodContainer = transform.FindChild("ButtonContainer").FindChild("BG_Driods").gameObject;
        for (int i = 0; i < 4; i++)
            DriodSlots.Add(DriodContainer.transform.FindChild("Slot_Driod" + (i + 1)).GetComponent<dfButton>());
        ComboIndicators.Add(DriodContainer.transform.FindChild("Combo_01").GetComponent<dfSprite>());
        ComboIndicators.Add(DriodContainer.transform.FindChild("Combo_02").GetComponent<dfSprite>());
        ComboIndicators.Add(DriodContainer.transform.FindChild("Combo_03").GetComponent<dfSprite>());
        ComboIndicators.Add(DriodContainer.transform.FindChild("Combo_12").GetComponent<dfSprite>());
        ComboIndicators.Add(DriodContainer.transform.FindChild("Combo_13").GetComponent<dfSprite>());
        ComboIndicators.Add(DriodContainer.transform.FindChild("Combo_23").GetComponent<dfSprite>());
        for (int i = 0; i < 4; i++)
            Driods.Add(DriodContainer.transform.FindChild("Driod" + (i + 1)).GetComponent<dfButton>());
        for (int i = 0; i < 4; i++)
            DriodsHealth.Add(DriodContainer.transform.FindChild("Health_Driod" + (i + 1)).GetComponent<dfSprite>());
        MonsterAContainer = transform.FindChild("MonsterAInfo").gameObject;
        MonsterBContainer = transform.FindChild("MonsterBInfo").gameObject;
        for (int i = 0; i < 7; i++)
            TxtIndicators.Add(transform.FindChild("HappenstanceIndicator"+(i+1)).GetComponent<dfLabel>());
        MonsterAElement = MonsterAContainer.transform.FindChild("MonsterElement").GetComponent<dfSprite>();
        MonsterBElement = MonsterBContainer.transform.FindChild("MonsterElement").GetComponent<dfSprite>();
        MonsterALevel = MonsterAContainer.transform.FindChild("BG_MonsterLevel").FindChild("MonsterLevel").gameObject;
        MonsterBLevel = MonsterBContainer.transform.FindChild("BG_MonsterLevel").FindChild("MonsterLevel").gameObject;
        MonsterAName = MonsterAContainer.transform.FindChild("MonsterName").gameObject;
        MonsterBName = MonsterBContainer.transform.FindChild("MonsterName").gameObject;
        MonsterAHealthBg = MonsterAContainer.transform.FindChild("MonsterHealth").GetComponent<dfProgressBar>();
        MonsterBHealthBg = MonsterBContainer.transform.FindChild("MonsterHealth").GetComponent<dfProgressBar>();
        MonsterAHealth = MonsterAHealthBg.transform.FindChild("MonsterHealthProgress").gameObject;
        MonsterBHealth = MonsterBHealthBg.transform.FindChild("MonsterHealthProgress").gameObject;
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
        for (int i = 0; i < Slots.Length; i++)
			DriodImprint.Add(new List<GameManager.ResourceElement>());
        for (int i = 0; i < 4; i++)
        {
            DriodImprintVisuals.Add(new List<dfPanel>());
            for (int j = 0; j < 10; j++)
                DriodImprintVisuals[i].Add(DriodSlots[i].transform.FindChild("Imprint" + (j + 1)).GetComponent<dfPanel>());
        }
		GGContainer.MouseMove += OnMouseMove;
		GGContainer.MouseDown += OnMouseMove;
		GGContainer.MouseUp += OnMouseUp;
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
        UpdateImprints();
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

    public void UpdateImprints()
    {

        for (int i = 0; i < Slots.Length; i++)
        {
            DriodImprint[i].Clear();
            for (int j = 0; j < Slots[i].fire; j++)
				DriodImprint[i].Add(GameManager.ResourceElement.Fire);
            for (int j = 0; j < Slots[i].energy; j++)
				DriodImprint[i].Add(GameManager.ResourceElement.Energy);
            for (int j = 0; j < Slots[i].nature; j++)
				DriodImprint[i].Add(GameManager.ResourceElement.Nature);
            for (int j = 0; j < Slots[i].water; j++)
				DriodImprint[i].Add(GameManager.ResourceElement.Water);
            for (int j = 0; j < Slots[i].storm; j++)
				DriodImprint[i].Add(GameManager.ResourceElement.Storm);
        }

        for (int i = 0; i < PresentDriodsCount; i++)
        {
            for (int j = 0; j < DriodImprint[i].Count; j++)
            {
                DriodImprintVisuals[i][j].Show();
                string element = "";
                string number;
                if (j == 9)
                    number = "10";
                else
                    number = "0" + (j + 1);
                switch (DriodImprint[i][j])
                {
				case GameManager.ResourceElement.Energy:
                        element = "energy";
                        break;
				case GameManager.ResourceElement.Fire:
                        element = "fire";
                        break;
				case GameManager.ResourceElement.Nature:
                        element = "life";
                        break;
				case GameManager.ResourceElement.Storm:
                        element = "storm";
                        break;
				case GameManager.ResourceElement.Water:
                        element = "water";
                        break;
                }
                DriodImprintVisuals[i][j].BackgroundSprite = "combat_slot_imprint_" + element + number;
            }
            for (int j = DriodImprint[i].Count; j < 10; j++)
                DriodImprintVisuals[i][j].Hide();
        }

        for (int i = Slots.Length; i < 4; i++)
        {
			for (int j = 0; j < 10; j++)
				DriodImprintVisuals[i][j].Hide();
        }
    }

    public void UpdateDriodHealth()
    {
        for (int i = Slots.Length; i < 4; i++)
            DriodsHealth[i].Hide();

        for (int i = 0; i < PresentDriodsCount; i++)
            DriodsHealth[i].FillAmount = 0.57f + Slots[i].driodenHealth * 0.36f;
    }

    public void UpdateMonsterElements()
    {
        switch (GameManager.Singleton.Player.CurCreature.BaseElement)
        {
		case GameManager.ResourceElement.None:
                MonsterAElement.Hide();
                break;

		case GameManager.ResourceElement.Energy:
                MonsterAElement.SpriteName = "spectre_stats_element_energy";
                break;

		case GameManager.ResourceElement.Fire:
                MonsterAElement.SpriteName = "spectre_stats_element_fire";
                break;

		case GameManager.ResourceElement.Nature:
                MonsterAElement.SpriteName = "spectre_stats_element_life";
                break;

		case GameManager.ResourceElement.Storm:
                MonsterAElement.SpriteName = "spectre_stats_element_storm";
                break;

		case GameManager.ResourceElement.Water:
                MonsterAElement.SpriteName = "spectre_stats_element_water";
                break;
        }
        switch (GameManager.Singleton.Player.CurFight.EnemyCreature.BaseElement)
        {
		case GameManager.ResourceElement.None:
                MonsterBElement.Hide();
                break;

		case GameManager.ResourceElement.Energy:
                MonsterBElement.SpriteName = "spectre_stats_element_energy";
                break;

		case GameManager.ResourceElement.Fire:
                MonsterBElement.SpriteName = "spectre_stats_element_fire";
                break;

		case GameManager.ResourceElement.Nature:
                MonsterBElement.SpriteName = "spectre_stats_element_life";
                break;

		case GameManager.ResourceElement.Storm:
                MonsterBElement.SpriteName = "spectre_stats_element_storm";
                break;

		case GameManager.ResourceElement.Water:
                MonsterBElement.SpriteName = "spectre_stats_element_water";
                break;
        }
    }

    public void UpdateButtons()
    {
        for (int i = 0; i < 4; i++)
        {
            Driods[i].Hide();
            DriodSlots[i].Hide();
            if (Slots.Length <= i) continue;
            DriodSlots[i].Show();
            Driods[i].Show();
            switch (Slots[i].driodenElement)
            {
			case GameManager.ResourceElement.None:
                    Driods[i].Hide();
                    break;

			case GameManager.ResourceElement.Energy:
                    Driods[i].BackgroundSprite = "combat_driod_energy";
                    break;

			case GameManager.ResourceElement.Fire:
                    Driods[i].BackgroundSprite = "combat_driod_fire";
                    break;

			case GameManager.ResourceElement.Nature:
                    Driods[i].BackgroundSprite = "combat_driod_life";
                    break;

			case GameManager.ResourceElement.Storm:
                    Driods[i].BackgroundSprite = "combat_driod_storm";
                    break;

			case GameManager.ResourceElement.Water:
                    Driods[i].BackgroundSprite = "combat_driod_water";
                    break;
            }
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
            MonsterACon.Hide();
            MonsterBCon.Hide();
            MonsterADot.Hide();
            MonsterBDot.Hide();
            MonsterAHot.Hide();
            MonsterBHot.Hide();
            MonsterABuff.Hide();
            MonsterBBuff.Hide();
        if (BattleEngine.Current.Result == null) return;
        if (BattleEngine.Current.Result.ConA)
            MonsterACon.Show();
        if (BattleEngine.Current.Result.ConB)
            MonsterBCon.Show();
        if (BattleEngine.Current.Result.DotA)
            MonsterADot.Show();
        if (BattleEngine.Current.Result.DotB)
            MonsterBDot.Show();
        if (BattleEngine.Current.Result.HotA)
            MonsterAHot.Show();
        if (BattleEngine.Current.Result.HotB)
            MonsterBHot.Show();
        if (BattleEngine.Current.Result.BuffA)
            MonsterABuff.Show();
        if (BattleEngine.Current.Result.BuffB)
            MonsterBBuff.Show();
    }

    public void UpdateHealth()
    {
		int friendlyHp = BattleEngine.Current.Result != null ? BattleEngine.Current.Result.MonsterAHP : GameManager.Singleton.Player.CurCreature.HP;
        int friendlyMaxHp = GameManager.Singleton.Player.CurCreature.HPMax;
		int enemyHp = BattleEngine.Current.Result != null ? BattleEngine.Current.Result.MonsterBHP : GameManager.Singleton.Player.CurFight.EnemyCreature.HP;
        int enemyMaxHp = GameManager.Singleton.Player.CurFight.EnemyCreature.HPMax;

        if (friendlyHp > 0)
        {
            MonsterAHealthText.GetComponent<dfLabel>().Text =
                friendlyHp + "/" + friendlyMaxHp;
            MonsterAHealth.GetComponent<dfProgressBar>().Value = 
                (float)friendlyHp / friendlyMaxHp;
        }
        else
        {
            MonsterAHealthText.GetComponent<dfLabel>().Text = "0/" + friendlyMaxHp;
            MonsterAHealth.GetComponent<dfProgressBar>().Value = 0;
        }

        if (enemyHp > 0)
        {
            MonsterBHealthText.GetComponent<dfLabel>().Text =
                enemyHp + "/" + enemyMaxHp;
            MonsterBHealth.GetComponent<dfProgressBar>().Value =
                (float)enemyHp / enemyMaxHp;
        }
        else
        {
            MonsterBHealthText.GetComponent<dfLabel>().Text = "0/" + enemyMaxHp;
            MonsterBHealth.GetComponent<dfProgressBar>().Value = 0;
        }

    }

    private void buttonKlick(int driod)
    {
        if (Slots.Length-1 < driod) return;

		int idx=InputText.IndexOf(driod);
		//if (InputText.Count > 0 && InputText.Count-1 == idx) //only remove last
		if (idx != -1) //remove from anywhere
        {
            InputText.Remove(driod);
            return;
        }

        if (InputText.Contains(driod)) return;

        InputText.Add(driod);
        //InputText is a List<int> filled with the indexes of the selected driods
        // CHECK FOR WHAT SOUND TO PLAY ON BUTTON KLICK HERE!
    }

    void OnMouseMove(dfControl ctrl, dfMouseEventArgs args)
    {
		int info = Driods.IndexOf(dfInputManager.ControlUnderMouse as dfButton);

#if UNITY_EDITOR
		if (args.Buttons==0) {info=-1;}
#endif
		//Debug.Log("mousemove " + info +" " + dfInputManager.ControlUnderMouse);


        if (info != _lastButton && info != -1)
            buttonKlick(info);
        _lastButton = info;
        args.Use();
    }
	void OnMouseUp(dfControl ctrl, dfMouseEventArgs args)
	{
		//Debug.Log("mouseup " + ctrl +" " + dfInputManager.ControlUnderMouse);

		if (dfInputManager.ControlUnderMouse==ButtonExecute as dfControl) {ExecuteClicked();}
	}

	public void ExecuteClicked()
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
