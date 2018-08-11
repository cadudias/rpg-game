using System.Collections;
using UnityEngine;

namespace RPG.Characters
{
    public abstract class AbilityBehaviour : MonoBehaviour
    {
        protected AbilityConfig config;
        const float PARTICLE_CLEAN_UP_DELAY = 20f;

        public abstract void Use(AbilityUseParams useParams);

        public void SetConfig(AbilityConfig configToSet)
        {
            config = configToSet;
        }

        protected void PlayAbilitySound()
        {
            var abilitySound = config.GetRandomAbilitySound(); // TODO change to random clip
            var audioSource = GetComponent<AudioSource>();
            audioSource.PlayOneShot(abilitySound);
        }

        protected void PlayParticleEffect()
        {
            // instantiate a particle system prefab attached to player
            // config.GetParticlePrefab() - get the particle prefab from the config
            // transform.position from the player because this behaviour is attached to the player
            var particlePrefab = config.GetParticlePrefab();
            var particleObject = Instantiate(
                particlePrefab,
                transform.position,
                particlePrefab.transform.rotation
            );
            // take the transform parent from the particle spawned and set it to ourselves, so the particle will always be nested to the gameObject that has the particle
            particleObject.transform.parent = transform; // set world space in prefab if required
            particleObject.GetComponent<ParticleSystem>().Play();
            DestroyParticleWhenFinished(particleObject);
        }

        /**
         * particlePrefab: particle we want to destroy
         * check if the particle we want to destroy is still playing, while is playing
         * we wait for that clean up delay to end, when is stops playing we
         * destroy the particle prefab and 
         * because the coroutine will want us to return something regardless of where we leave it at the end state we better wait for the Frame to end as well
         * **/
        IEnumerator DestroyParticleWhenFinished(GameObject particlePrefab)
        {
            while (particlePrefab.GetComponent<ParticleSystem>().isPlaying)
            {
                yield return new WaitForSeconds(PARTICLE_CLEAN_UP_DELAY);
            }
            Destroy(particlePrefab);
            yield return new WaitForEndOfFrame();
        }
    }
}
