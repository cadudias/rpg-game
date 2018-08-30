using RPG.Core;
using UnityEngine;

namespace RPG.Characters
{
    public abstract class AbilityConfig : ScriptableObject
    {
        //Header - group things by section
        [Header("Special Ability General")]
        [SerializeField] public float energyCost = 10f;
        [SerializeField] GameObject particlePrefab;
        [SerializeField] AnimationClip abilityAnimation;

        // sub Config classes can set this value
        protected AbilityBehaviour behaviour;

        [SerializeField] AudioClip[] audioClips;

        public abstract AbilityBehaviour GetBehaviourComponent(GameObject objectToAttachTo);
        
        public void AttachAbilityTo(GameObject objectToAttachTo)
        {
            AbilityBehaviour behaviourComponent = GetBehaviourComponent(objectToAttachTo);
            behaviourComponent.SetConfig(this); // call from AbilityBehaviour class
            behaviour = behaviourComponent;
        }

        public AnimationClip GetAbilityAnimation()
        {
            return abilityAnimation;
        }

        public void Use(GameObject target)
        {
            behaviour.Use(target);
        }

        public float GetEnergyCost()
        {
            return energyCost;
        }

        public GameObject GetParticlePrefab()
        {
            return particlePrefab;
        }

        public AudioClip GetRandomAbilitySound()
        {
            return audioClips[Random.Range(0, audioClips.Length)];
        }
    }

    public interface ISpecialAbility
    {
        void Use(GameObject target);
    }
}