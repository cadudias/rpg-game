using System;
using UnityEngine;

namespace RPG.Characters
{
    public class PowerAttackBehaviour : MonoBehaviour, ISpecialAbility
    {
        PowerAttackConfig config = null;
        AudioSource audioSource = null;

        void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void SetConfig(PowerAttackConfig configToSet)
        {
            this.config = configToSet;
        }

        public void Use(AbilityUseParams useParams)
        {
            DealRadialDamage(useParams);
            audioSource.clip = config.GetAudioClip();
            audioSource.Play();
            PlayParticleEffect();
        }

        private void DealRadialDamage(AbilityUseParams useParams)
        {
            float damageToDeal = useParams.baseDamage + config.GetExtraDamage();
            useParams.target.TakeDamage(damageToDeal);
        }

        private void PlayParticleEffect()
        {
            // instantiate a particle system prefab attached to player
            // config.GetParticlePrefab() - get the particle prefab from the config
            // transform.position from the player because this behaviour is attached to the player
            var prefab = Instantiate(config.GetParticlePrefab(), transform.position, Quaternion.identity);
            Debug.Log(config.GetParticlePrefab());
            ParticleSystem myParticleSystem = prefab.GetComponent<ParticleSystem>();
            myParticleSystem.Play();
            Destroy(prefab, myParticleSystem.main.duration);
        }
    }
}
