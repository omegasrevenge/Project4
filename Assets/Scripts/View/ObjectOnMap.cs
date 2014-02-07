using UnityEngine;

public abstract class ObjectOnMap:TouchObject
{
    private bool _inRange;

    public bool InRange
    {
        get { return _inRange; }
        set
        {
            if (_inRange == value) return;
            _inRange = value;
            if (value)
                EnterRange();
            else
                LeaveRange();
        }
    }

    public abstract void Execute();

    protected virtual void EnterRange()
    {
        Enabled = true;
    }
    protected virtual void LeaveRange()
    {
        Enabled = false;
    }

    public virtual float GetCooldownProgress()
    {
        return 0;
    }

    public virtual string GetCooldownString()
    {
        return "00:00:00";
    }
}
