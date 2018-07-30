using UnityEngine;

// TODO consider re-wiring
using RPG.Core;
using RPG.Weapons;

namespace RPG.Characters
{
    public class Enemy : MonoBehaviour, IDamageable
    {
        public bool IsAttacking = false;

        [SerializeField] float maxHealthPoints = 100f;

        [SerializeField] float chaseRadius = 6f;

        [SerializeField] float attackRadius = 4f;
        [SerializeField] float damagePerShot = 10f;
        [SerializeField] float secondsBetweenShots = 0.5f;
        [SerializeField] GameObject projectileToUse;
        [SerializeField] GameObject projectileSocket;
        [SerializeField] Vector3 aimOffset = new Vector3(0, 1f, 0);

        //Projectile projectile;

        float currentHealthPoints;

        AICharacterControl aic = null;
        GameObject player = null;

        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            aic = GetComponent<AICharacterControl>();

            currentHealthPoints = maxHealthPoints;
        }

        void Update()
        {
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

            if (PlayerIsInAttackRange(distanceToPlayer) && !IsAttacking)
            {
                IsAttacking = true;

                InvokeRepeating("FireProjectile", 0f, secondsBetweenShots);
            }

            if (PlayerIsOutsideAttackRange(distanceToPlayer))
            {
                IsAttacking = false;
                CancelInvoke();
            }

            if (PlayerIsInChaseRange(distanceToPlayer))
            {
                aic.SetTarget(player.transform);
            }
            else
            {
                aic.SetTarget(transform);
            }
        }


        public float healthAsPercentage
        {
            get
            {
                return currentHealthPoints / maxHealthPoints;
            }
        }

        public void TakeDamage(float damage)
        {
            //if (projectile.shooter.gameObject.layer == 9)
            //{
            currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
            if (currentHealthPoints <= 0)
                Destroy(gameObject);
            //}
        }

        private bool PlayerIsOutsideAttackRange(float distanceToPlayer)
        {
            return distanceToPlayer > attackRadius;
        }

        // TODO separate character fire logic
        void FireProjectile()
        {
            GameObject newProjectile;
            Projectile projectileComponent;
            SpawnProjectile(out newProjectile, out projectileComponent);

            // get position to player and set the velocity of the Rigidbody inside the projectile game object
            // using the formula unitVectorToPlayer * projectileSpeed
            // we add the player position and add an offset to it so the projectile hit 1 point up in the y axys
            Vector3 unitVectorToPlayer = (player.transform.position + aimOffset - projectileSocket.transform.position).normalized;
            float projectileSpeed = projectileComponent.GetDefaultLaunchSpeed();
            newProjectile.GetComponent<Rigidbody>().velocity = unitVectorToPlayer * projectileSpeed;
        }

        private void SpawnProjectile(out GameObject newProjectile, out Projectile projectileComponent)
        {
            // Quaternion.identity = no rotation
            newProjectile = Instantiate(projectileToUse, projectileSocket.transform.position, Quaternion.identity);
            projectileComponent = newProjectile.GetComponent<Projectile>();
            projectileComponent.SetDamage(damagePerShot);
            projectileComponent.SetShooter(gameObject);
        }

        private bool PlayerIsInChaseRange(float distanceToPlayer)
        {
            return distanceToPlayer <= chaseRadius;
        }

        private bool PlayerIsInAttackRange(float distanceToPlayer)
        {
            return distanceToPlayer <= attackRadius;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRadius);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseRadius);
        }
    }
}
