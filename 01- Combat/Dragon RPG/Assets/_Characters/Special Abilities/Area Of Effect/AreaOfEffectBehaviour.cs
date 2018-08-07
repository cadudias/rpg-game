using System;
using RPG.Core;
using UnityEngine;

namespace RPG.Characters
{
    public class AreaOfEffectBehaviour : MonoBehaviour, ISpecialAbility
    {
        AreaOfEffectConfig config;

        public void SetConfig(AreaOfEffectConfig configToSet)
        {
            this.config = configToSet;
        }

        public void Use(AbilityUseParams useParams)
        {
            DealRadialDamage(useParams);
            PlayParticleEffect();
        }       

        private void DealRadialDamage(AbilityUseParams useParams)
        {
            //  Vector3.up - we dont care were it's moving it
            // this is designed to fire a ball but we're creating a STATIC SPHERE
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, config.GetRadius(), Vector3.up, config.GetRadius());
            // Cast a sphere wrapping character controller 10 meters forward
            // to see if it is about to hit anything.
            foreach (var hit in hits)
            {
                var damageable = hit.collider.gameObject.GetComponent<IDamageable>();

                if (damageable != null)
                {
                    float damageToDeal = useParams.baseDamage + config.GetDamageToEachTarget();
                    damageable.TakeDamage(damageToDeal);
                }
            }
        }

        private void PlayParticleEffect()
        {
            // instantiate a particle system prefab attached to player
            // config.GetParticlePrefab() - get the particle prefab from the config
            // transform.position from the player because this behaviour is attached to the player
            var prefab = Instantiate(config.GetParticlePrefab(), transform.position, Quaternion.identity);
            ParticleSystem myParticleSystem = prefab.GetComponent<ParticleSystem>();
            myParticleSystem.Play();
            Destroy(prefab, myParticleSystem.main.duration);
        }
    }
}