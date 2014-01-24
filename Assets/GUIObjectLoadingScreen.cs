using UnityEngine;

public class GUIObjectLoadingScreen : MonoBehaviour
{
    private const float RefHeight = 1280;
    private const string Prefab = "GUI/root_loadingscreen";
    private const string GUITextStr = "guitext";
    
    private GUIText _text;

    public string Text
    {
        get 
        { 
            if(_text != null) 
                return _text.text;
            return "";
        }
        set  { if(_text != null) _text.text = value; }
    }

    public static GUIObjectLoadingScreen Create(string text)
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>(Prefab)) as GameObject;
        if (obj)
        {
            GUIObjectLoadingScreen load = obj.GetComponent<GUIObjectLoadingScreen>();
            if (load)
            {
                load._text = load.transform.FindChild(GUITextStr).GetComponent<GUIText>();
                load._text.fontSize = Mathf.RoundToInt(load._text.fontSize * Screen.height / RefHeight);
                load.Text = text;
                return load;
            }
        }
        return null;
    }

    public void Show(string text)
    {
        gameObject.SetActive(true);
        Text = text;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
