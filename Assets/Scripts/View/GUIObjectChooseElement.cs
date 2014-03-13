using System;
using System.Collections.Generic;
using UnityEngine;

public class GUIObjectChooseElement : MonoBehaviour
{
	private const string Prefab = "GUI/panel_chooseelement";

    public List<dfButton> Buttons = new List<dfButton>();
	
    void Awake()
    {
	    foreach (dfButton button in Buttons)
	    {
			if (button)
			{
				button.Click +=
					(control, @event) =>
					{
						SoundController.PlaySound(SoundController.SoundFacClick, SoundController.ChannelSFX);
					};
			} 
	    }
    }

    public static GameObject Create(string textKeyTitle, string textKeyText, Action<int> callback)
    {
        GameObject go = Instantiate(Resources.Load<GameObject>(Prefab)) as GameObject;
		GUIObjectChooseElement input = go.GetComponent<GUIObjectChooseElement>();
		foreach (dfButton button in input.Buttons)
	    {
		    if(callback != null)
				button.Click += (control, @event) =>
				{
					if (!@event.Used)
					{
						@event.Use();
						callback(input.Buttons.IndexOf((dfButton)control));
					}
				};
	    }

        GUIObjectTextPanel panel = go.GetComponent<GUIObjectTextPanel>();
        panel.Title = textKeyTitle;
        panel.Text = textKeyText;

        return go;
    }
}
