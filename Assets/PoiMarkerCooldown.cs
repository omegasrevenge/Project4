using UnityEngine;

public class PoiMarkerCooldown : MonoBehaviour
{
    private const string ProgressStr = "sprite_progress";
    private const string Label = "label";
    private dfSprite _progress;
    private dfLabel _label;
    private dfControl _control;

    void Awake()
    {
        _control = GetComponent<dfControl>();
        _progress = transform.Find(ProgressStr).GetComponent<dfSprite>();
        _label = transform.Find(Label).GetComponent<dfLabel>();
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

    public void HideCooldown()
    {
        _control.Hide();
    }
}
