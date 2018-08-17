using UnityEngine;

namespace RPG.Characters
{
    public class PowerAttackBehaviour : AbilityBehaviour
    {
        public void SetConfig(PowerAttackConfig configToSet)
        {
            this.config = configToSet;
        }

        public override void Use(GameObject target)
        {
            PlayAbilitySound();
            DealRadialDamage(target);
            PlayParticleEffect();
        }

        private void DealRadialDamage(GameObject target)
        {
            float damageToDeal = (config as PowerAttackConfig).GetExtraDamage();

            target.GetComponent<HealthSystem>().TakeDamage(damageToDeal);
        }
    }
}
