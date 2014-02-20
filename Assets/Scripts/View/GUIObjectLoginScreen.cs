using UnityEngine;

public class GUIObjectLoginScreen : MonoBehaviour 
{
    private const string Prefab = "GUI/panel_loginscreen";

    public static GameObject Create()
    {
        GameObject go = Instantiate(Resources.Load<GameObject>(Prefab)) as GameObject;
        return go;
    }
}
