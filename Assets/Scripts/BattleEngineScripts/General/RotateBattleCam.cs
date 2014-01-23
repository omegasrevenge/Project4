using UnityEngine;
using System.Collections;

public class RotateBattleCam : MonoBehaviour 
{
	
	public enum To{A,B}
	
	[SerializeField]
	public float RotationSpeed = 90f;

	[HideInInspector]
	public bool DoRotation = false;
	
	public bool CameraInAPosition
	{
		get
		{
			return (Mathf.Abs(transform.localEulerAngles.y) >= -0.4f) && (Mathf.Abs(transform.localEulerAngles.y) <= 2f);
		}
	}

	public bool CameraInBPosition
	{
		get
		{
			return (Mathf.Abs(transform.localEulerAngles.y) >= 178f) && (Mathf.Abs(transform.localEulerAngles.y) <= 180.4f);
		}
	}

	private To _currentDir;
	private bool _canStopRotation = false;

	void Update () 
	{
		if(!DoRotation) return;

		if(CameraInAPosition)
		{
			_currentDir = To.B;
		}

		if(CameraInBPosition)
		{
			_currentDir = To.A;
		}

		if(_currentDir == To.B)
		{
			transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 
			                                         transform.localEulerAngles.y-(RotationSpeed*Time.deltaTime), 
			                                         transform.localEulerAngles.z);
		}
		else
		{
			transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 
			                                         transform.localEulerAngles.y+(RotationSpeed*Time.deltaTime), 
			                                         transform.localEulerAngles.z);
		}

		if(!CameraInAPosition && !CameraInBPosition)
		{
			_canStopRotation = true;
		}

		if(_canStopRotation && (CameraInAPosition || CameraInBPosition))
		{
			_canStopRotation = false;
			DoRotation = false;
		}
	}
}
