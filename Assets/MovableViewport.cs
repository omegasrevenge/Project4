using UnityEngine;

public class MovableViewport : MonoBehaviour
{
    public float phase;
    public bool pinned;
	void Start ()
	{
	    
	}
	
	// Update is called once per frame
	void LateUpdate ()
	{
        Debug.Log(camera.orthographicSize+" "+camera.fieldOfView);
	    camera.rect = new Rect(phase,0,1,1);
        float width = ((float)Screen.width/Screen.height)*camera.orthographicSize;
        Vector3 camoffset = new Vector3((pinned?-1f:1f)*(camera.rect.x * width), 0, 0);
		Matrix4x4 m  = Matrix4x4.TRS (camoffset, Quaternion.identity, new Vector3 (1,1,-1));
		camera.worldToCameraMatrix = m * transform.worldToLocalMatrix;
	}
}
