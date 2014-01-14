using UnityEngine;

public class ViewController : MonoBehaviour
{
    private static ViewController _instance;
    private GameManager _gameManager;

    public static ViewController Singleton
    {
        get
        {
            if (_instance != null)
                return _instance;
            return null;
        }
    }

    public static ViewController Create()
    {
        if (_instance != null)
            return _instance;
        GameObject obj = new GameObject("controller_viewcontroller");
        obj.hideFlags = HideFlags.DontSave;
        obj.tag = GameManager.DontSaveTag;
        _instance = obj.AddComponent<ViewController>();
        return _instance;
    }

	void Awake ()
	{
        Init();
        if (_instance == null) 
            _instance = this;
        else if (_instance != this) 
            Destroy(gameObject);
	    Init();
	}

    private void Init()
    {
        _gameManager = GameManager.Singleton;
    }

    void Update () 
    {
	
	}


}
