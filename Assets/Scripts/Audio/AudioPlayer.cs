using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{

    AudioSource source;
    public void Play(AudioClip clip, float volume, AudioManager.AudioRuleSet rules) {
        source = GetComponent<AudioSource>();
        source.volume = volume;
        source.outputAudioMixerGroup = rules.MixerGroup;
        source.loop = rules.Looping;
        
        if (rules.Looping) {
            source.clip = clip;
            source.Play();
        } else {
            source.PlayOneShot(clip);
        }

        if (rules.DestroyOnFinish) {
            StartCoroutine(DestroyWhenDone());
        }
    }

    IEnumerator DestroyWhenDone () {
        while (source.isPlaying) {
            yield return null;
        }
        Destroy(gameObject);
    }
}
