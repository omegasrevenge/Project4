using UnityEngine;
using System.Collections;

public class GUIObjectCancelButton : MonoBehaviour {
	void Start ()
	{
	    GetComponent<dfLabel>().Text = Localization.GetText("cancel");
	}

}
