using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GUIObjectBattleEngine : MonoBehaviour
{
    public List<int> InputText;

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
        MonsterAContainer = transform.FindChild("MonsterAInfo").gameObject;
        MonsterBContainer = transform.FindChild("MonsterBInfo").gameObject;
        IndicatorOne = transform.FindChild("HappenstanceIndicator1").gameObject;
        IndicatorTwo = transform.FindChild("HappenstanceIndicator2").gameObject;
        IndicatorThree = transform.FindChild("HappenstanceIndicator3").gameObject;
        IndicatorFour = transform.FindChild("HappenstanceIndicator4").gameObject;
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
		if (BattleEngine.Current.ServerInfo == null) Debug.LogError ("BattleEngine.Current.ServerInfo NULL");
        MonsterALevel.GetComponent<dfLabel>().Text = BattleEngine.Current.ServerInfo.MonsterALevel.ToString();
        MonsterBLevel.GetComponent<dfLabel>().Text = BattleEngine.Current.ServerInfo.MonsterBLevel.ToString();
        MonsterAName.GetComponent<dfLabel>().Text = BattleEngine.Current.ServerInfo.MonsterAName;
        MonsterBName.GetComponent<dfLabel>().Text = BattleEngine.Current.ServerInfo.MonsterBName;
    }

    void Update()
    {
        if (BattleEngine.CurrentGameObject == null || !BattleEngine.Current.Initialized) return;
        //MonsterAContainer.transform.position = BattleEngine.Current.Camera.WorldToViewportPoint(BattleEngine.Current.FriendlyCreature.transform.FindChild("GUIPos").transform.position);
        //MonsterBContainer.transform.position = BattleEngine.Current.Camera.WorldToViewportPoint(BattleEngine.Current.EnemyCreature.transform.FindChild("GUIPos").transform.position);
        MonsterAHealthText.GetComponent<dfLabel>().Text = BattleEngine.Current.FriendlyCreature.GetComponent<MonsterController>().Health + "/" + BattleEngine.Current.FriendlyCreature.GetComponent<MonsterStats>().HP;
        MonsterBHealthText.GetComponent<dfLabel>().Text = BattleEngine.Current.EnemyCreature.GetComponent<MonsterController>().Health + "/" + BattleEngine.Current.EnemyCreature.GetComponent<MonsterStats>().HP;
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
            IndicatorOne.GetComponent<dfTweenVector3>().StartValue =
                BattleEngine.Current.Camera.WorldToViewportPoint(BattleEngine.Current.FriendlyCreature.transform.position);
            IndicatorOne.GetComponent<dfTweenVector3>().EndValue =
	            BattleEngine.Current.Camera.WorldToViewportPoint(BattleEngine.Current.FriendlyCreature.transform.FindChild("GUIPos").position);
            IndicatorOne.GetComponent<dfTweenVector3>().Start();
	    }
        else 
        if (target == BattleEngine.Current.EnemyCreature)
        {
            if (!heal)
                IndicatorTwo.GetComponent<dfLabel>().Text =
                    damage + "DMG " + BattleEngine.Current.Result.SkillName + " & " + dot + "DoT ->" + BattleEngine.Current.Result.SkillName + "<-";
            else
                IndicatorTwo.GetComponent<dfLabel>().Text = damage + "HoT";
            IndicatorTwo.GetComponent<dfTweenVector3>().StartValue =
                BattleEngine.Current.Camera.WorldToViewportPoint(BattleEngine.Current.FriendlyCreature.transform.position);
            IndicatorTwo.GetComponent<dfTweenVector3>().EndValue =
                BattleEngine.Current.Camera.WorldToViewportPoint(BattleEngine.Current.FriendlyCreature.transform.FindChild("GUIPos").position);
            IndicatorTwo.GetComponent<dfTweenVector3>().Start();
	    }
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
        //var current = GameManager.Singleton.Player.CurCreature.slots;
        //for (int i = 0; i < current.Length; i++)
        //{
        //    string element = "";
        //    switch (current[i].driodenElement)
        //    {
        //        case BattleEngine.ResourceElement.Fire:
        //            element = "Fire";
        //            break;
        //
        //        case BattleEngine.ResourceElement.Nature:
        //            element = "Nature";
        //            break;
        //
        //        case BattleEngine.ResourceElement.Storm:
        //            element = "Storm";
        //            break;
        //
        //        case BattleEngine.ResourceElement.Energy:
        //            element = "Tech";
        //            break;
        //
        //        case BattleEngine.ResourceElement.Water:
        //            element = "Water";
        //            break;
        //    }
        //    if (GUI.Button(new Rect(0, Screen.height - 100 * (i + 1), 250, 100), (i + 1) + ". Driode. Element: " + element + ". HP: " + current[i].driodenHealth + "%."))
        //    {
        //
        //    }
        //}

        GUI.TextArea(new Rect(Screen.width - 100, 200, 100, 100), InputText.Aggregate("", (current1, number) => current1 + number));

        if (GameManager.Singleton.Player.CurFight.confused)
            GUI.Button(new Rect(0, 0, 100, 100), "CONFUSION!");
        if (GameManager.Singleton.Player.CurFight.defBoosted)
            GUI.Button(new Rect(100, 0, 100, 100), "DEF BOOSTED!");
    }
}
