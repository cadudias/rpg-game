using System;
using UnityEngine;

namespace RPG.Characters
{
    [CreateAssetMenu(menuName = ("RPG/Special Ability/Area Of Effect"))]
    public class AreaOfEffectConfig : SpecialAbility
    {
        [Header("Area Of Effect Specifics")]
        [SerializeField] float radius = 5f;
        [SerializeField] float damageToEachTarget = 10f;

        public override void AttachComponent(GameObject gameObjectToAttachTo)
        {
            var behaviourComponent = gameObjectToAttachTo.AddComponent<AreaOfEffectBehaviour>();
            behaviourComponent.SetConfig(this);
            behaviour = behaviourComponent;
        }

        public float GetRadius()
        {
            return radius;
        }

        internal float GetDamageToEachTarget()
        {
            return damageToEachTarget;
        }
    }
}
