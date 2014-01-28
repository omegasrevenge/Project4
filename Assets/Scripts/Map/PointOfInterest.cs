using UnityEngine;

public class PointOfInterest : MonoBehaviour
{

    public MapUtils.ProjectedPos ProjPos;
    public MapGrid Grid;

    void Update()
    {
        Vector2 pos = Grid.GetPosition(ProjPos);
        transform.localPosition = new Vector3(pos.x, 0.001f, pos.y);
    }
}
