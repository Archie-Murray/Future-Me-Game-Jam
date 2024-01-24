using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Assests : Singleton<Assests>
{
    public ParticleSystem HitParticles;
    void LateUpdate() {
        var main = HitParticles.main;
        main.simulationSpeed = 0;

    }
    void damgage() {
        var main = HitParticles.main;
        main.simulationSpeed = 1;
    }

}
