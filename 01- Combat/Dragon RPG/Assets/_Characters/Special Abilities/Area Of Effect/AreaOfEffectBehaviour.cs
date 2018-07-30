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
    }
}