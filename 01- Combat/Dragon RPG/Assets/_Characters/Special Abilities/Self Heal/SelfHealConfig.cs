using System;
using UnityEngine;

namespace RPG.Characters
{
    [CreateAssetMenu(menuName = ("RPG/Special Ability/Self Heal"))]
    public class SelfHealConfig : AbilityConfig
    {
        [Header("Self Heal Specifics")]
        [SerializeField] float healAmount = 25f;
        
        public override void AttachComponent(GameObject gameObjectToAttachTo)
        {
            var behaviourComponent = gameObjectToAttachTo.AddComponent<SelfHealBehaviour>();
            behaviourComponent.SetConfig(this);
            behaviour = behaviourComponent; // this will be used it the SpecialAbility class
        }

        public float GetHealAmount()
        {
            return healAmount;
        }
    }
}