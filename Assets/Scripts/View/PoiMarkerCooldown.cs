using UnityEngine;

public class PoiMarkerCooldown : MonoBehaviour
{
    private const string ProgressStr = "sprite_progress";
    private const string OverlayStr = "sprite_overlay";
    private const string Label = "label";
    private dfSprite _progress;
    private dfSprite _overlay;
    private dfLabel _label;
    private dfControl _control;

    void Awake()
    {
        _control = GetComponent<dfControl>();
        _progress = transform.Find(ProgressStr).GetComponent<dfSprite>();
        _overlay = transform.Find(OverlayStr).GetComponent<dfSprite>();     
        _label = transform.Find(Label).GetComponent<dfLabel>();

        if (GameManager.Singleton.Player.CurrentFaction == Player.Faction.NCE)
        {
            _overlay.Color = GameManager.NCERed;
            _label.Color = GameManager.Black;
            _progress.Color = GameManager.Black;
        }   
    }

    public void SetCooldown(float progress, string time)
    {
        if (progress <= 0f)
        {
            _control.Hide();
            return;        
        }
        _control.Show();
        _progress.FillAmount = progress;

        _label.Text = time;
    }
}
