using UnityEngine;

public class MovableViewport : MonoBehaviour
{
    public float phase;
    public bool pinned;
	
	// Update is called once per frame
	void LateUpdate ()
	{
	    camera.rect = new Rect(phase, 0, 1, 1);
	    if (camera.rect.x >= 1 || camera.rect.x <= -1)
	        return;
	    float translation;
        translation = (!pinned ^ camera.rect.x >= 0 ? 1f : -1f) * (-1f + (1f / (1f - Mathf.Abs(camera.rect.x))));
        Vector3 camoffset = new Vector3(translation, 0, 0);
	    Matrix4x4 m = Matrix4x4.TRS(camoffset, Quaternion.identity, new Vector3(1, 1, 1));
        camera.ResetProjectionMatrix();      
	    camera.projectionMatrix = m*camera.projectionMatrix;


	}

}

