using UnityEngine;
using System.Collections;

public class Radar : MonoBehaviour {

    public void PlaySound()
    {
        SoundController.PlaySound(SoundController.SFXlocation+SoundController.Faction+SoundController.SoundFacLocate, SoundController.ChannelSFX);
    }
}
