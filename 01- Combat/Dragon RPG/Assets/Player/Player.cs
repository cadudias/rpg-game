using UnityEngine;

public class Player : MonoBehaviour, IDamagable {

    [SerializeField] float maxHealthPoints = 100f;

    float currentHealthPoints = 100f;

    public float healthAsPercentage { get { return currentHealthPoints / maxHealthPoints; } }

    public void TakeDamage(float damage)
    {
        currentHealthPoints -= Mathf.Clamp(currentHealthPoints - damage, 0f, currentHealthPoints);
    }
}
