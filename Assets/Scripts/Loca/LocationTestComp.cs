using UnityEngine;

public class LocationTestComp : MonoBehaviour
{

    public MapUtils.ProjectedPos ProjPos;
    public MapGrid Grid;
	public GameObject MainCamera;

    void Awake()
    {
		MainCamera = GameObject.Find("Main Camera");
        //GetComponent<SpriteRenderer>().sortingOrder = 1;
    }
    void Update()
    {
        Vector2 pos = Grid.GetPosition(ProjPos);
        transform.position = new Vector3(pos.x, 0.001f, pos.y);
    }
    void LateUpdate()
    {
        //transform.rotation = Camera.main.transform.rotation;
        transform.GetChild(1).rotation = MainCamera.transform.rotation;
    }
    public void setText(string text)
    {
        GameObject textObj = (GameObject)Instantiate(Resources.Load("Text"));
        textObj.transform.parent = transform;
        textObj.transform.localPosition = new Vector3(0,0.5f,0);
        textObj.GetComponent<TextMesh>().text = text;
    }
}
