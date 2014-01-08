using System;
using System.Collections;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
#endif

public class BakeFunction
{

#if UNITY_EDITOR

	[PostProcessScene( -1 )]
	public static void OnPostprocessScene()
	{
		// Scene post-processing code removed but the stub was
		// left in place because it will be used in the near
		// future 
	}

#endif

}
