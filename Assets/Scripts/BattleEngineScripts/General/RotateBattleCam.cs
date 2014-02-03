using UnityEngine;
using System.Collections;

public class RotateBattleCam : MonoBehaviour 
{
	
	public enum To{A,B}
	
	[SerializeField]
	public float RotationSpeed = 90f;

	[HideInInspector]
	public bool DoRotation = false;

    private float positiveOffset = 2f;
    private float negativeOffset = 0.4f;
	
	public bool CameraInAPosition
	{
		get
		{
            return (Mathf.Abs(transform.localEulerAngles.y) >= -negativeOffset) && (Mathf.Abs(transform.localEulerAngles.y) <= positiveOffset);
		}
	}

	public bool CameraInBPosition
	{
		get
		{
            return (Mathf.Abs(transform.localEulerAngles.y) >= 180f - positiveOffset) && (Mathf.Abs(transform.localEulerAngles.y) <= 180f + negativeOffset);
		}
	}

	private To _currentDir;
	private bool _canStopRotation = false;

	void Update () 
	{
		if(!DoRotation) return;

		if(CameraInAPosition)
			_currentDir = To.B;

		if(CameraInBPosition)
			_currentDir = To.A;

		if(_currentDir == To.B)
			transform.localEulerAngles -= new Vector3(0, RotationSpeed * Time.deltaTime, 0);
		else
            transform.localEulerAngles += new Vector3(0, RotationSpeed * Time.deltaTime, 0);

		if(!CameraInAPosition && !CameraInBPosition)
			_canStopRotation = true;

		if(_canStopRotation && (CameraInAPosition || CameraInBPosition))
		{
			_canStopRotation = false;
			DoRotation = false;
		}
	}
}
