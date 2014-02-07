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
    public List<GameObject> ComboIndicators;
    public List<dfButton> DriodSlots; 

    public bool Initialized
    {
        get
        {
            return MonsterAContainer != null;
        }
    }

    public static GUIObjectBattleEngine Create(dfControl root)
    {
        dfControl cntrl = root.AddPrefab(Resources.Load<GameObject>(Prefab));
        cntrl.Size = cntrl.Parent.Size;
        cntrl.RelativePosition = Vector2.zero;
        return cntrl.GetComponent<GUIObjectBattleEngine>();
    }

    public void Init()
    {
        InputText = new List<int>();
        ComboIndicators = new List<GameObject>();
        DriodSlots = new List<dfButton>();
        DriodContainer = transform.FindChild("ButtonContainer").FindChild("BG_Driods").gameObject;
        DriodSlots.Add(DriodContainer.transform.FindChild("Slot_Driod1").GetComponent<dfButton>());
        DriodSlots.Add(DriodContainer.transform.FindChild("Slot_Driod2").GetComponent<dfButton>());
        DriodSlots.Add(DriodContainer.transform.FindChild("Slot_Driod3").GetComponent<dfButton>());
        DriodSlots.Add(DriodContainer.transform.FindChild("Slot_Driod4").GetComponent<dfButton>());
        ComboIndicators.Add(DriodContainer.transform.FindChild("Combo_01").gameObject);
        ComboIndicators.Add(DriodContainer.transform.FindChild("Combo_02").gameObject);
        ComboIndicators.Add(DriodContainer.transform.FindChild("Combo_03").gameObject);
        ComboIndicators.Add(DriodContainer.transform.FindChild("Combo_12").gameObject);
        ComboIndicators.Add(DriodContainer.transform.FindChild("Combo_13").gameObject);
        ComboIndicators.Add(DriodContainer.transform.FindChild("Combo_23").gameObject);
        MonsterAContainer = transform.FindChild("MonsterAInfo").gameObject;
        MonsterBContainer = transform.FindChild("MonsterBInfo").gameObject;
        IndicatorOne =      transform.FindChild("HappenstanceIndicator1").gameObject;
        IndicatorTwo =      transform.FindChild("HappenstanceIndicator2").gameObject;
        IndicatorThree =    transform.FindChild("HappenstanceIndicator3").gameObject;
        IndicatorFour =     transform.FindChild("HappenstanceIndicator4").gameObject;
        MonsterAElement = MonsterAContainer.transform.FindChild("MonsterElement").gameObject;
        MonsterBElement = MonsterBContainer.transform.FindChild("MonsterElement").gameObject;
        MonsterALevel = MonsterAContainer.transform.FindChild("MonsterLevel").gameObject;
        MonsterBLevel = MonsterBContainer.transform.FindChild("MonsterLevel").gameObject;
        MonsterAName = MonsterAContainer.transform.FindChild("MonsterName").gameObject;
        MonsterBName = MonsterBContainer.transform.FindChild("MonsterName").gameObject;
        MonsterAHealth = MonsterAContainer.transform.FindChild("MonsterHealthProgress").gameObject;
        MonsterBHealth = MonsterBContainer.transform.FindChild("MonsterHealthProgress").gameObject;
        MonsterAHealthText = MonsterAHealth.transform.FindChild("MonsterHealthText").gameObject;
		MonsterBHealthText = MonsterBHealth.transform.FindChild("MonsterHealthText").gameObject;
        MonsterALevel.GetComponent<dfLabel>().Text = BattleEngine.Current.ServerInfo.MonsterALevel.ToString();
        MonsterBLevel.GetComponent<dfLabel>().Text = BattleEngine.Current.ServerInfo.MonsterBLevel.ToString();
        MonsterAName.GetComponent<dfLabel>().Text = BattleEngine.Current.ServerInfo.MonsterAName;
        MonsterBName.GetComponent<dfLabel>().Text = BattleEngine.Current.ServerInfo.MonsterBName;
        MonsterAHealthText.GetComponent<dfLabel>().Text = GameManager.Singleton.Player.CurCreature.HP + "/" + GameManager.Singleton.Player.CurCreature.HPMax;
        MonsterAHealth.GetComponent<dfProgressBar>().Value = (float)GameManager.Singleton.Player.CurCreature.HP / GameManager.Singleton.Player.CurCreature.HPMax;
        MonsterBHealthText.GetComponent<dfLabel>().Text = GameManager.Singleton.Player.CurFight.EnemyCreature.HP + "/" + GameManager.Singleton.Player.CurFight.EnemyCreature.HPMax;
        MonsterBHealth.GetComponent<dfProgressBar>().Value = (float)GameManager.Singleton.Player.CurFight.EnemyCreature.HP / GameManager.Singleton.Player.CurFight.EnemyCreature.HPMax;
    }

    void Update()
    {
        if (BattleEngine.CurrentGameObject == null || !BattleEngine.Current.Initialized || !BattleEngine.Current.Fighting) return;

        foreach (GameObject comboIndicator in ComboIndicators)
            comboIndicator.GetComponent<dfSprite>().Hide();
        for (int i = 0; i < DriodSlots.Count; i++)
        {
            if (InputText.Contains(i))
                DriodSlots[i].BackgroundSprite = "combat_slot_active_vengea";
            else
                DriodSlots[i].BackgroundSprite = "combat_slot";
        }

        if (InputText.Count < 2) return;

        for (int i = 0; i < InputText.Count-1; i++)
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
            if(ind == -1) continue;
            ComboIndicators[ind].GetComponent<dfSprite>().Show();
        }
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
        MonsterAHealthText.GetComponent<dfLabel>().Text = GameManager.Singleton.Player.CurCreature.HP + "/" + GameManager.Singleton.Player.CurCreature.HPMax;
        MonsterAHealth.GetComponent<dfProgressBar>().Value = (float)GameManager.Singleton.Player.CurCreature.HP / GameManager.Singleton.Player.CurCreature.HPMax;
        MonsterBHealthText.GetComponent<dfLabel>().Text = GameManager.Singleton.Player.CurFight.EnemyCreature.HP + "/" + GameManager.Singleton.Player.CurFight.EnemyCreature.HPMax;
        MonsterBHealth.GetComponent<dfProgressBar>().Value = (float)GameManager.Singleton.Player.CurFight.EnemyCreature.HP / GameManager.Singleton.Player.CurFight.EnemyCreature.HPMax;
	}

    public void FirstDriodKlicked()
    {
        InputText.Add(0);
    }

    public void SecondDriodKlicked()
    {
        InputText.Add(1);
    }

    public void ThirdDriodKlicked()
    {
        InputText.Add(2);
    }

    public void FourthDriodKlicked()
    {
        InputText.Add(3);
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
        InputText.Clear();
    }

    public void CatchKlicked()
    {
        GameManager.Singleton.CatchAttempt();
        InputText.Clear();
    }

    public void DeleteSelection()
    {
        InputText.Clear();
    }

    void OnGUI()
	{
		if (!BattleEngine.Current.Initialized) return;
		if (GameManager.Singleton.Player.CurFight == null) return;


        if (GameManager.Singleton.Player.CurFight.confused)
            GUI.Button(new Rect(0, 0, 100, 100), "CONFUSION!");
        if (GameManager.Singleton.Player.CurFight.defBoosted)
            GUI.Button(new Rect(100, 0, 100, 100), "DEF BOOSTED!");
    }
}
