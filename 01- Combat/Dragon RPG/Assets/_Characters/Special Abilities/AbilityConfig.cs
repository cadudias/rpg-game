using System;
using RPG.Core;
using UnityEngine;

namespace RPG.Characters
{
    public struct AbilityUseParams
    {
        public IDamageable target;
        public float baseDamage;

        public AbilityUseParams(IDamageable target, float baseDamage)
        {
            this.target = target;
            this.baseDamage = baseDamage;
        }
    }

    public abstract class AbilityConfig : ScriptableObject
    {
        //Header - group things by section
        [Header("Special Ability General")]
        [SerializeField] public float energyCost = 10f;
        [SerializeField] GameObject particlePrefab = null;

        // sub Config classes can set this value
        protected AbilityBehaviour behaviour;

        [SerializeField] AudioClip audioClip = null;

        // adds the component to the player at runtime
        abstract public void AttachComponent(GameObject gameObjectToAttachTo);

        public void Use(AbilityUseParams useParams)
        {
            behaviour.Use(useParams);
        }

        public float GetEnergyCost()
        {
            return energyCost;
        }

        public GameObject GetParticlePrefab()
        {
            return particlePrefab;
        }

        public AudioClip GetAudioClip()
        {
            return audioClip;
        }
    }

    public interface ISpecialAbility
    {
        void Use(AbilityUseParams useParams);
    }
}