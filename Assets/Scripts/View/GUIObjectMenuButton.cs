using UnityEngine;

public class GUIObjectMenuButton : MonoBehaviour
{
    private const string BGStr = "sprite_bg";
    private const string LabelStr = "label_name";

    protected dfControl _control;
    protected dfSprite _bg;
    protected dfLabel _label;
    protected virtual void Awake()
    {
        _control = GetComponent<dfControl>();
        _bg = transform.Find(BGStr).GetComponent<dfSprite>();
        _label = transform.Find(LabelStr).GetComponent<dfLabel>();
        _control.MouseDown += OnMouseDown;
        _control.MouseUp += OnMouseUp;
    }

    protected virtual void OnMouseUp(dfControl control, dfMouseEventArgs mouseEvent)
    {
        _bg.Hide();
    }

    protected virtual void OnMouseDown(dfControl control, dfMouseEventArgs mouseEvent)
    {
        _bg.Show();
    }
}
