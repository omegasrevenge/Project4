using UnityEngine;

public class GUIObjectMessage : MonoBehaviour 
{
    private dfControl _control;

    void Awake()
    {
        _control = GetComponent<dfPanel>();      
    }

    public void OnClick(dfControl control, dfMouseEventArgs @event)
    {
        if (@event.Used) return;
        @event.Use();

        SoundController.PlaySound(SoundController.SoundClick, SoundController.ChannelSFX);
        GetComponent<dfPanel>().Hide();
        GameManager.Singleton.GUIClickMessage();
    }
    public void Show()
    {
        _control.Show();
    }

    public void Hide()
    {
        _control.Hide();
    }
}
