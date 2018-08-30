using UnityEngine;

namespace RPG.Characters
{
    public class SelfHealBehaviour : AbilityBehaviour
    {        
        public void SetConfig(SelfHealConfig configToSet)
        {
            this.config = configToSet;
        }

        public override void Use(GameObject target)
        {
            PlayAbilitySound();
            
            var playerHealth = gameObject.GetComponent<HealthSystem>();
            playerHealth.Heal((config as SelfHealConfig).GetHealAmount());

            PlayParticleEffect();
            PlayAbilityAnimation();
        }
    }
}
