using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class Enemy : MonoBehaviour, IDamagable
{
    [SerializeField] float maxHealthPoints = 100f;
    [SerializeField] float triggerRadius = 5f;
    float currentHealthPoints = 100f;

    AICharacterControl aic = null;
    GameObject player = null;

    public float healthAsPercentage { get { return currentHealthPoints / maxHealthPoints; } }

    public void TakeDamage(float damage)
    {
        currentHealthPoints -= Mathf.Clamp(currentHealthPoints - damage, 0f, currentHealthPoints);
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        aic = GetComponent<AICharacterControl>();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

        if (distanceToPlayer <= triggerRadius)
        {
            aic.SetTarget(player.transform);
        }
    }
}
