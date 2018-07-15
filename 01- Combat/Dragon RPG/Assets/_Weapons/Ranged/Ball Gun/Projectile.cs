using UnityEngine;
using RPG.Core; // TODO consider re-wiring

namespace RPG.Weapons
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float projectileSpeed;
        float destroyDelay = 0.01f;

        [SerializeField] GameObject shooter; // So can inspect when pause
        float damageCaused;

        public void SetDamage(float damage)
        {
            damageCaused = damage;
        }

        public void SetShooter(GameObject shooter)
        {
            this.shooter = shooter;
        }

        void OnCollisionEnter(Collision collision)
        {
            var layerCollidedWith = collision.gameObject.layer;
            if (shooter && layerCollidedWith != shooter.layer)
            {
                DamageIfDamageable(collision);
            }
        }

        private void DamageIfDamageable(Collision collision)
        {
            Component damagableComponent = collision.gameObject.GetComponent(typeof(IDamagable));

            if (damagableComponent)
                (damagableComponent as IDamagable).TakeDamage(damageCaused);

            Destroy(gameObject, destroyDelay);
        }

        internal float GetDefaultLaunchSpeed()
        {
            return projectileSpeed;
        }
    }
}
