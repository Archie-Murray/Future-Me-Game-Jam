using System;
using System.Collections.Generic;

using UnityEngine;
[Serializable] public enum SoundEffectType { NONE, UPGRADE, PLAYER_HIT, PLAYER_LIGHT, PLAYER_HEAVY, PLAYER_DEATH, ENEMY_ATTACK, ENEMY_HIT, ENEMY_DEATH }

[Serializable] public class SoundEffect {
    public AudioClip Clip;
    public SoundEffectType Type;
}