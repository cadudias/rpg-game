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
            //print(target.gameObject.name);
            PlayAbilitySound();
            DealRadialDamage(target);
            PlayParticleEffect();
            PlayAbilityAnimation();
        }

        private void DealRadialDamage(GameObject target)
        {
            float damageToDeal = (config as PowerAttackConfig).GetExtraDamage();
            print(target.gameObject.name);
            target.GetComponent<HealthSystem>().TakeDamage(damageToDeal);
        }
    }
}
