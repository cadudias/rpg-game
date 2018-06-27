using UnityEngine;

public class Player : MonoBehaviour, IDamagable {

    [SerializeField] float maxHealthPoints = 300f;

    float currentHealthPoints = 300f;

    public float healthAsPercentage { get { return currentHealthPoints / maxHealthPoints; } }

    public void TakeDamage(float damage)
    {
        currentHealthPoints = Mathf.Clamp((currentHealthPoints - damage), 0f, currentHealthPoints);
        if (currentHealthPoints <= 0)
        {
            Destroy(gameObject);
        }
    }
}
