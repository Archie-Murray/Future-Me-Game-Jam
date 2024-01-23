using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Assests : Singleton<Assests>
{
    public ParticleSystem HitParticles;
    void Update() {
        HitParticles.playbackSpeed = 1;
    }

}
