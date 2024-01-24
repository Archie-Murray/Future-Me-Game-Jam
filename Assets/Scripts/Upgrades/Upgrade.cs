using UnityEngine;

namespace Upgrades {
    public class Upgrade : ScriptableObject {
        public Sprite Icon;
        public StatType Type;
        public float Amount;
        public string Description;
        [Range(0f, 1f)] public float FailChance;

        public void ApplyUpgrade(Stats playerStats) {
            playerStats.IncreaseStat(Type, Random.value > FailChance ? Amount : -Amount);
        }
    }
}