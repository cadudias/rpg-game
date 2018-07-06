using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class Enemy : MonoBehaviour, IDamagable
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

            InvokeRepeating("SpawnProjectile", 0f, secondsBetweenShots);
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
        currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
        if (currentHealthPoints <= 0)
            Destroy(gameObject);
    }

    private bool PlayerIsOutsideAttackRange(float distanceToPlayer)
    {
        return distanceToPlayer > attackRadius;
    }

    void SpawnProjectile()
    {
        // Quaternion.identity = no rotation
        GameObject newProjectile = Instantiate(projectileToUse, projectileSocket.transform.position, Quaternion.identity);
        Projectile projectileComponent = newProjectile.GetComponent<Projectile>();
        projectileComponent.SetDamage(damagePerShot);

        // get position to player and set the velocity of the Rigidbody inside the projectile game object
        // using the formula unitVectorToPlayer * projectileSpeed
        // we add the player position and add an offset to it so the projectile hit 1 point up in the y axys
        Vector3 unitVectorToPlayer = (player.transform.position + aimOffset - projectileSocket.transform.position).normalized;
        float projectileSpeed = projectileComponent.projectileSpeed;
        newProjectile.GetComponent<Rigidbody>().velocity = unitVectorToPlayer * projectileSpeed;
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
