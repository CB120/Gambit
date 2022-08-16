using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum AudioType { MasterDebug, SoundDebug, Music, Ambience, UnitSounds, Attack, MenuUI, GameUI }


public class AudioManager : MonoBehaviour
{

    // A set of rules for each type of audio
    [System.Serializable]
    public struct AudioRuleSet {
        // The AudioType this rule set controls
        public AudioType Type;
        // The mixer group to output the sound from.
        public AudioMixerGroup MixerGroup;
        // (Optional) Use a different audioplayer prefab than the default
        public GameObject OverrideAudioPlayer;
        // Destroy the voice on finish - should nearly always be true.
        public bool DestroyOnFinish;
        // (Optional) Provide a constant audio source to play from
        // Ideal for constant or looping sources such as ambient noise or basic music
        // Usually these sources should be ConstantAudioPlayers, instead of standard AudioPlayers
        public AudioPlayer ConstantAudioPlayer;
        // Loop repeatedly. Note there is currently no way to stop a sound so be wary using this.
        public bool Looping;
    }

    
    static AudioManager Singleton;

    [SerializeField] AudioMixer mixer;
    
    // Default volume to use for Plays called with no volume specified.
    [SerializeField] float defaultVolume = 0.7f;
    
    // Default rules for sounds, to be used when no other rules are found.
    [SerializeField] AudioRuleSet defaultRules;
    // A list of rule sets, ensure there is only ONE set for each type.
    [SerializeField] List<AudioRuleSet> audioRules = new List<AudioRuleSet>();

    // Prefab with an AudioSource.
    [SerializeField] GameObject audioPlayer;

    

    private void Awake() {
        if (Singleton) Destroy(Singleton);
        Singleton = this;
    }

    private void Start() {
        UpdateAudioMixer();
    }

    public static void PlaySound(AudioClip clip, AudioType type) {
        PlaySound(clip, type, Singleton.defaultVolume);
    }
    public static void PlaySound(AudioClip clip, AudioType type, float volume) {
        AudioRuleSet rules = Singleton.FindRules(type);
        
        if (rules.ConstantAudioPlayer) {
            // For a single voice, just play the provided source
            rules.ConstantAudioPlayer.Play(clip, volume, rules);
        } else {
            AudioPlayer audioPlayer = CreateAudioPlayer();
            if (audioPlayer == null) Debug.LogWarning("Audio player prefab does not have an AudioPlayer.");
            audioPlayer.Play(clip, volume, rules);
        }
    }

    static AudioPlayer CreateAudioPlayer() {
        AudioPlayer newAudioPlayer = Instantiate(Singleton.audioPlayer, Singleton.transform).GetComponent<AudioPlayer>();
        return newAudioPlayer;
    }

    AudioRuleSet FindRules (AudioType type) {
        foreach (AudioRuleSet rules in audioRules) {
            if (rules.Type == type) return rules;
        }
        Debug.LogWarning("No audio rules found for type " + type + ", using default rules");
        return defaultRules;
    }

    public static void InitialiseConstantAudioPlayer(AudioType type, AudioPlayer audioPlayer) {
        AudioRuleSet rules = Singleton.FindRules(type);
        rules.ConstantAudioPlayer = audioPlayer;
    }

    public void UpdateAudioMixer () { 
        mixer.SetFloat("MusicVolume", GetVolumePlayerPref("MusicVolume"));
        mixer.SetFloat("SoundVolume", GetVolumePlayerPref("SFXVolume"));
        mixer.SetFloat("AmbienceVolume", GetVolumePlayerPref("AmbienceVolume"));
    }

    static float GetVolumePlayerPref(string pref) {
        return Mathf.Log10(PlayerPrefs.GetFloat(pref, 1)) * 20;
    }

}