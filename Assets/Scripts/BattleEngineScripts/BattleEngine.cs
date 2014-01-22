using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BattleEngine : MonoBehaviour 
{
	//########## public #########
	public enum TurnState{Wait, Rotate, Execute, Hit}


	public ActorControlls Actor;
	public FightRoundResult.Player CurrentPlayer;
	public BattleInit ServerInfo;


	public string InputText = "";
	public int Turn = 0;
	public bool Fighting = true;


	public GameObject BattleCam;
	public GameObject MainCam;
	public GameObject FriendlyCreature;
	public GameObject EnemyCreature;
	public GameObject Arena;
	//########## public #########
	
	//########## static #########
	public static BattleEngine Current;
	public static GameObject   CurrentGameObject;
	//########## static #########
	
	//########## const #########
	public const string DefaultArena			 = "Battle/DefaultArena";
	public const string DefaultFriendlySpawnPos  = "FriendlySpawnPos";
	public const string DefaultEnemySpawnPos	 = "EnemySpawnPos";
	public const string DefaultMonster	 		 = "Battle/DefaultMonster";
	//########## const #########

	//########## private #########
	private TurnState _currentStatus = TurnState.Wait;
	private List<FightRoundResult> _results;
	private int _changeA;
	private int _changeB;
	private GameObject _actor;
	private List<GameObject> _damageIndicator;
	private float _counter = 0f;
	private GameObject _gg;
	private GameObject _monsterAName;
	private GameObject _monsterBName;
	private GameObject _monsterAHealth;
	private GameObject _monsterBHealth;
	private List<GameObject> _damageGUI;
	//########## private #########

	//########## getter #########
	public List<FightRoundResult> Results
	{
		get
		{
			return _results;
		}
	}

	public FightRoundResult Result
	{
		get
		{
			if(_results.Count < 1) return null;
			return _results[0];
		}
		set
		{
			if(!typeof(FightRoundResult).IsInstanceOfType(value)) return;
			if((_results.Count > 1 && _results[_results.Count-2].Turn >= _results[_results.Count-1].Turn)) return;
			if(value.Turn <= Turn) return;
			_results.Add(value);
		}
	}

	public TurnState GetTurnState
	{
		get
		{
			if((Actor == null || Actor.AnimationFinished) && !BattleCamInPosition(CurrentPlayer)) return TurnState.Rotate;
			if(_results.Count == 0 && (Actor == null || Actor.AnimationFinished)) return TurnState.Wait;
			if(Actor != null && Actor.CanShowDamage) return TurnState.Hit;
			return TurnState.Execute;
		}
	}

	public bool BattleCamInPosition(FightRoundResult.Player value)
	{
		if(value == FightRoundResult.Player.A && BattleCam.transform.parent.GetComponent<RotateBattleCam>().CameraInAPosition) return true;
		if(value == FightRoundResult.Player.B && BattleCam.transform.parent.GetComponent<RotateBattleCam>().CameraInBPosition) return true;
		return false;
	}
	//########## getter ##################################################################

	void Awake()
	{
		_results = new List<FightRoundResult>();
		_damageGUI = new List<GameObject>();
		_damageIndicator = new List<GameObject>();
	}

	public static void CreateBattle(BattleInit serverInfo) // <----------- this starts the battle
	{
		CurrentGameObject = new GameObject("BattleEngine");
		Current = CurrentGameObject.AddComponent<BattleEngine>();
		Current.Init(serverInfo);
		Current.BattleCam = Current.Arena.transform.FindChild("CameraPivot").FindChild("BattleCamera").gameObject;
		Current.BattleCam.SetActive(true);
		Current.MainCam = Camera.main.gameObject;
		Current.MainCam.SetActive(false);
		Current.CurrentPlayer = serverInfo.FirstTurnIsPlayer;
		Current.ServerInfo = serverInfo;
	}

	void Update()
	{
		positionGUI();

		if(!Fighting)
		{
			enforceEnd();
			return;
		}

		if(GetTurnState != _currentStatus)
		{
			_currentStatus = GetTurnState;
			switch(GetTurnState)
			{
			case TurnState.Wait:
				break;
			case TurnState.Rotate:
				rotateBattleCam();
				break;
			case TurnState.Execute:
				turnInit();
				break;
			case TurnState.Hit:
				executeSkill();
				break;
			}
		}
	}
	
	void OnGUI()
	{
		if (GUI.Button(new Rect(0, Screen.height-100, 200, 100), "Driode_1"))
		{
			InputText += "1";
		}
		if (GUI.Button(new Rect(200, Screen.height-100, 200, 100), "Driode_2"))
		{
			InputText += "2";
		}
		if (GUI.Button(new Rect(400, Screen.height-100, 200, 100), "Driode_3"))
		{
			InputText += "3";
		}
		if (GUI.Button(new Rect(600, Screen.height-100, 200, 100), "Driode_4"))
		{
			InputText += "4";
		}
		
		if (GUI.Button(new Rect(Screen.width-200, Screen.height/2-100, 200, 200), "Execute!"))
		{
			//sende an server info
			InputText = "";
            GameManager.Singleton.FightPlayerTurn(0,1,2,3);
		}
		
		GUI.TextArea(new Rect(Screen.width-100, Screen.height/2+100, 100, 100), InputText);
		
		if (GUI.Button(new Rect(Screen.width-200, Screen.height/2+100, 100, 100), "Delete Selection"))
		{
			InputText = "";
		}
	}

	private void positionGUI()
	{
		_damageGUI.RemoveAll(item => item == null);
		_damageIndicator.RemoveAll(item => item == null);
		_monsterAName.transform.position = BattleCam.GetComponent<Camera>().WorldToViewportPoint (FriendlyCreature.transform.FindChild("NamePos").transform.position); 
		_monsterBName.transform.position = BattleCam.GetComponent<Camera>().WorldToViewportPoint (EnemyCreature.transform.FindChild("NamePos").transform.position); 
		_monsterAHealth.GetComponent<GUIText>().text = FriendlyCreature.GetComponent<MonsterController>().Health.ToString()+"/"+FriendlyCreature.GetComponent<MonsterStats>().HP;
		_monsterBHealth.GetComponent<GUIText>().text = EnemyCreature.GetComponent<MonsterController>().Health.ToString()+"/"+EnemyCreature.GetComponent<MonsterStats>().HP;
		_monsterAHealth.transform.position = BattleCam.GetComponent<Camera>().WorldToViewportPoint (FriendlyCreature.GetComponent<MonsterController>().BgHealthbar.position); 
		_monsterBHealth.transform.position = BattleCam.GetComponent<Camera>().WorldToViewportPoint (EnemyCreature.GetComponent<MonsterController>().BgHealthbar.position); 
		if(_damageGUI.Count > 0 && _damageIndicator.Count == _damageGUI.Count)
		{
			for(int i = 0; i<_damageGUI.Count; i++)
			{
				_damageGUI[i].transform.position = BattleCam.GetComponent<Camera>().WorldToViewportPoint (_damageIndicator[i].transform.position); 
			}
		}
	}

	private void enforceEnd()
	{
		if(_gg == null) _gg = Create("Battle/GGScreen", 
		                             GameObject.Find("GGScreenPos").transform.position, 
		                             GameObject.Find("GGScreenPos").transform.rotation);
		_counter += Time.deltaTime;
		if(_counter >= 3f) { Destroy(_gg); DestroyBattle(); }
	}

	private void rotateBattleCam()
	{
		BattleCam.transform.parent.GetComponent<RotateBattleCam>().DoRotation = true;
	}

	private void turnInit()
	{
		Turn = Result.Turn;
		initSkill();
		_changeA = FriendlyCreature.GetComponent<MonsterController>().Health-Result.PlayerAHealth;
		_changeB = EnemyCreature   .GetComponent<MonsterController>().Health-Result.PlayerBHealth;
	}

	private void initSkill()
	{
		//actor -> monster
		//Execute casting animation
		//when done do
		//evalute which skill was used by Result.SkillID
		if(Result.SkillID == 1)
		{
			createSkillVisuals("Laser");
		}
	}

	private void createSkillVisuals(string name)
	{
		_actor = (GameObject)Instantiate(Resources.Load("Battle/"+name),Vector3.zero,Quaternion.identity);
		Actor = _actor.GetComponent<ActorControlls>();
		Actor.Owner = this;
		switch(CurrentPlayer)
		{
		case FightRoundResult.Player.A:
			_actor.transform.localPosition = FriendlyCreature.transform.position;
			_actor.transform.LookAt(EnemyCreature.transform);
			break;
		case FightRoundResult.Player.B:
			_actor.transform.localPosition = EnemyCreature.transform.position;
			_actor.transform.LookAt(FriendlyCreature.transform);
			break;
		}
	}

	private void executeSkill()
	{
		if(_changeA != 0)
		{
			_damageIndicator.Add(Create("Battle/Damage",Vector3.zero,Quaternion.identity));
			_damageIndicator[_damageIndicator.Count-1].transform.localPosition = FriendlyCreature.transform.position;
			_damageIndicator[_damageIndicator.Count-1].AddComponent<SelfDestruct>();
			_damageGUI.Add(new GameObject("GUI Damage Indicator Player A"));
			_damageGUI[_damageGUI.Count-1].AddComponent<GUIText>().text = Mathf.Abs(_changeA).ToString();
			_damageGUI[_damageGUI.Count-1].AddComponent<SelfDestruct>();
		}

		if(_changeB != 0)
		{
			_damageIndicator.Add(Create("Battle/Damage",Vector3.zero,Quaternion.identity));
			_damageIndicator[_damageIndicator.Count-1].transform.localPosition = EnemyCreature.transform.position;
			_damageIndicator[_damageIndicator.Count-1].AddComponent<SelfDestruct>();
			_damageGUI.Add(new GameObject("GUI Damage Indicator Player B"));
			_damageGUI[_damageGUI.Count-1].AddComponent<GUIText>().text = Mathf.Abs(_changeB).ToString();
			_damageGUI[_damageGUI.Count-1].AddComponent<SelfDestruct>();
		}

		FriendlyCreature.GetComponent<MonsterController>().Health = Result.PlayerAHealth;
		EnemyCreature.GetComponent<MonsterController>().Health    = Result.PlayerBHealth;
		CurrentPlayer = Result.PlayerTurn;
		_results.Remove(Result);
	}

	public void Init(BattleInit serverInfo)
	{
		//////////////////////////////          init Arena          //////////////////////////////////////////////////////////////////////////////////////////
		Arena = Create(DefaultArena, new Vector3(0f,1000f,1000f), Quaternion.identity);
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		/// 
		////////////////////////////// init Friendly Creature ////////////////////////////////////////////////////////////
		FriendlyCreature = Create(DefaultMonster, 
		                           Arena.transform.FindChild(DefaultFriendlySpawnPos).position, 
		                           Arena.transform.FindChild(DefaultFriendlySpawnPos).rotation);
		
		FriendlyCreature.GetComponent<MonsterController>().StartPosition = Arena.transform.FindChild(DefaultFriendlySpawnPos).position;
		FriendlyCreature.GetComponent<MonsterController>().Owner = this;
		FriendlyCreature.GetComponent<MonsterController>().Health = serverInfo.MonsterAHealth;
		FriendlyCreature.GetComponent<MonsterStats>().Init(serverInfo.MonsterAElement, serverInfo.MonsterAName, serverInfo.MonsterALevel, serverInfo.MonsterAMaxHealth);
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		/// 
		////////////////////////////// init Enemy Creature //////////////////////////////////////////////////////////////////////////////////////////
		EnemyCreature = Create(DefaultMonster, 
		                        Arena.transform.FindChild(DefaultEnemySpawnPos).position, 
		                        Arena.transform.FindChild(DefaultEnemySpawnPos).rotation);
		
		EnemyCreature.GetComponent<MonsterController>().StartPosition = Arena.transform.FindChild(DefaultEnemySpawnPos).position;
		EnemyCreature.GetComponent<MonsterController>().Owner = this;
		EnemyCreature.GetComponent<MonsterController>().Health = serverInfo.MonsterBHealth;
		EnemyCreature.GetComponent<MonsterStats>().Init(serverInfo.MonsterBElement, serverInfo.MonsterBName, serverInfo.MonsterBLevel, serverInfo.MonsterBMaxHealth);
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		/// 
		/// 
		_monsterAName = new GameObject("Monster A Name");
		_monsterBName = new GameObject("Monster B Name");
		_monsterAName.AddComponent<GUIText>().text = "Lv."+serverInfo.MonsterALevel+" "+serverInfo.MonsterAName;
		_monsterBName.AddComponent<GUIText>().text = "Lv."+serverInfo.MonsterBLevel+" "+serverInfo.MonsterBName;
		_monsterAHealth = new GameObject("Monster A Health");
		_monsterBHealth = new GameObject("Monster B Health");
		_monsterAHealth.AddComponent<GUIText>();
		_monsterBHealth.AddComponent<GUIText>();
	}

	public void DestroyBattle()
	{
		MainCam.SetActive(true);
		BattleCam.SetActive(false);
		Destroy(FriendlyCreature);
		Destroy(EnemyCreature);
		Destroy(Arena);
		Destroy(_monsterAName);
		Destroy(_monsterBName);
		Destroy(_monsterAHealth);
		Destroy(_monsterBHealth);
		Destroy(Current);
		Destroy(CurrentGameObject);
		GameManager.Singleton.SwitchGameMode(GameManager.GameMode.Map);
		Current = null;
		CurrentGameObject = null;
	}
	
	public GameObject Create(string name, Vector3 pos, Quaternion rot)
	{
		return (GameObject)Instantiate(Resources.Load(name), pos, rot);
	}
}
