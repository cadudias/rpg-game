using System;
using UnityEngine;

namespace RPG.Characters
{
    [CreateAssetMenu(menuName = ("RPG/Special Ability/Area Of Effect"))]
    public class AreaOfEffectConfig : AbilityConfig
    {
        [Header("Area Of Effect Specifics")]
        [SerializeField] float radius = 5f;
        [SerializeField] float damageToEachTarget = 10f;

        public override AbilityBehaviour GetBehaviourComponent(GameObject objectToAttachTo)
        {
            return objectToAttachTo.AddComponent<AreaOfEffectBehaviour>();
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
