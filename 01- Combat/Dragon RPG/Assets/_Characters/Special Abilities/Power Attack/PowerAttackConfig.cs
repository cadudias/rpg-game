using UnityEngine;

namespace RPG.Characters
{
    [CreateAssetMenu(menuName = ("RPG/Special Ability/Power Attack"))]
    public class PowerAttackConfig : SpecialAbility
    {
        [Header("Power Attack Specifics")]
        [SerializeField] float extraDamage = 10f;

        public float GetExtraDamage()
        {
            return extraDamage;
        }

        public override void AttachComponent(GameObject gameObjectToAttachTo)
        {
            var behaviourComponent = gameObjectToAttachTo.AddComponent<PowerAttackBehaviour>();
            behaviourComponent.SetConfig(this);
            behaviour = behaviourComponent;
        }
    }
}