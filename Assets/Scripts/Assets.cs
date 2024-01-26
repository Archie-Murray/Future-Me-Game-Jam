using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Assets : Singleton<Assets> {
    public GameObject HitParticles;
    public AudioMixer AudioMixer;
    public AudioMixerGroup SFX;
    public AudioMixerGroup BGM;
}
