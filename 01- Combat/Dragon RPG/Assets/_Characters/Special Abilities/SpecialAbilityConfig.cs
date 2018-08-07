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

    public abstract class SpecialAbility : ScriptableObject
    {
        //Header - group things by section
        [Header("Special Ability General")]
        [SerializeField] public float energyCost = 10f;
        [SerializeField] GameObject particlePrefab = null;

        // sub Config classes can set this value
        protected ISpecialAbility behaviour;

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
    }

    public interface ISpecialAbility
    {
        void Use(AbilityUseParams useParams);
    }
}