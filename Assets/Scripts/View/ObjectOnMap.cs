using UnityEngine;

public abstract class ObjectOnMap:TouchObject
{
    public abstract void Execute();

    public abstract Vector2 GetScreenPosition();
}
