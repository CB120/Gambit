using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantAudioPlayer : AudioPlayer
{
    [SerializeField] AudioType audioType;
    [SerializeField] AudioClip defaultSound;
    [SerializeField] float defaultVolume = 0.5f;
    
    private void Start() {
        AudioManager.InitialiseConstantAudioPlayer(audioType, this);
        AudioManager.PlaySound(defaultSound, audioType, defaultVolume);
    }
}
