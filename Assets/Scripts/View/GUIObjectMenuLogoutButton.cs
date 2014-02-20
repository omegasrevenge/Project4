public class GUIObjectMenuLogoutButton : GUIObjectMenuButton 
{
    void OnClick(dfControl control, dfMouseEventArgs args)
    {
        GameManager.Singleton.Logout();
    }
}
