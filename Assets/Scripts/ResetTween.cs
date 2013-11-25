using UnityEngine;
using System.Collections;

public class ResetTween : MonoBehaviour
{
	public enum GuiType{dfLabel, dfRichTextLabel}

	public dfTweenVector3 tween;
	public string TweenGameObjectName;
	public GuiType GameObjectType;

    public void resetTween()
    {
		tween.Stop();
		tween.Reset();
		switch(GameObjectType)
		{
			case GuiType.dfLabel:
				GameObject.Find(TweenGameObjectName).GetComponent<dfLabel>().Position = tween.StartValue;
				break;
			case GuiType.dfRichTextLabel:
				GameObject.Find(TweenGameObjectName).GetComponent<dfRichTextLabel>().Position = tween.StartValue;
				break;
		}
    }
}
