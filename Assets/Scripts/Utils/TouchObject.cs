using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TouchObject : MonoBehaviour
{
    private bool _enabled = true;
    [SerializeField]
    private List<TouchObject> _disabledBy;

    public delegate void OnTapAction(TouchInput.Touch2D touch2D);
    public OnTapAction Tap;

    public bool Enabled
    {
        get { return _enabled && !_disabledBy.Any(); }
        set { _enabled = value; }
    }

    public bool Solo
    {
        get { return TouchInput.SoloTouchObject == this; }
        set
        {
            if (value)
            {
                TouchInput.SoloTouchObject = this;
                return;
            }
            TouchInput.SoloTouchInput = TouchInput.SoloTouchObject != this;
        }               
    }

    virtual public void OnTouchStart(TouchInput.Touch2D touch2D)
    {
        
    }

    virtual public void OnTouchMove(TouchInput.Touch2D touch2D)
    {

    }

    virtual public void OnTouchEnd(TouchInput.Touch2D touch2D)
    {
        if (touch2D.Current[0] == this && !TouchInput.Rotating)
        {
            OnTap(touch2D);
            Debug.Log("##########################################################");
            if (Tap != null)
                Tap(touch2D);
        }
    }

    virtual public void OnTap(TouchInput.Touch2D touch2D)
    {

    }

    public Touch[] Touches
    {
        get { return TouchInput.GetTouches(this); }
    }

    public Touch[] InitTouches
    {
        get { return TouchInput.GetInitTouches(this); }
    }

    public Touch[] EndTouches
    {
        get { return TouchInput.GetEndTouches(this); }
    }

    protected void Start()
    {
        _disabledBy = new List<TouchObject>();
        TouchInput.Register(this);
    }

    protected void OnDestroy()
    {
        TouchInput.Unregister(this);
    }

    public void DisableBy(TouchObject obj)
    {
        if (_disabledBy.Contains(obj))
            return;
        _disabledBy.Add(obj);
    }

    public void EnableBy(TouchObject obj)
    {
        _disabledBy.Remove(obj);
    }

}
