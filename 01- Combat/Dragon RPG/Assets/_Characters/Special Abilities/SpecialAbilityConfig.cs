using UnityEngine;

namespace RPG.Characters
{
    public abstract class SpecialAbilityConfig : ScriptableObject
    {
        //Header - group things by section
        [Header("Special Ability General")]
        [SerializeField] public float energyCost = 10f;

        // sub Config classes can set this value
        protected ISpecialAbility behaviour;

        // adds the component to the player at runtime
        abstract public void AttachComponent(GameObject gameObjectToAttachTo);

        public void Use()
        {
            behaviour.Use();
        }
    }
}