using RPG.Core;
using UnityEngine;

namespace RPG.Characters
{
    public class AreaOfEffectBehaviour : AbilityBehaviour
    {
        public void SetConfig(AreaOfEffectConfig configToSet)
        {
            this.config = configToSet;
        }

        public override void Use(GameObject target)
        {
            PlayAbilitySound();
            DealRadialDamage();
            PlayParticleEffect();
        }       

        private void DealRadialDamage()
        {
            //  Vector3.up - we dont care were it's moving it
            // this is designed to fire a ball but we're creating a STATIC SPHERE
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, (config as AreaOfEffectConfig).GetRadius(), Vector3.up, (config as AreaOfEffectConfig).GetRadius());
            // Cast a sphere wrapping character controller 10 meters forward
            // to see if it is about to hit anything.
            foreach (var hit in hits)
            {
                var damageable = hit.collider.gameObject.GetComponent<HealthSystem>();
                bool hitPlayer = hit.collider.gameObject.GetComponent<PlayerMovement>();
                if (damageable != null && !hitPlayer)
                {
                    float damageToDeal = (config as AreaOfEffectConfig).GetDamageToEachTarget();
                    damageable.TakeDamage(damageToDeal);
                }
            }
        }
    }
}