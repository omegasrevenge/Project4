using UnityEngine;

public class LocationTestComp : MonoBehaviour
{

    public MapUtils.ProjectedPos ProjPos;
    public MapGrid Grid;
    void Awake()
    {
        GetComponent<SpriteRenderer>().sortingOrder = 1;
    }
    void Update()
    {
        Vector2 pos = Grid.GetPosition(ProjPos);
        transform.position = new Vector3(pos.x, 0.001f, pos.y);
    }
    void LateUpdate()
    {
        transform.rotation = Camera.main.transform.rotation;
    }
}
