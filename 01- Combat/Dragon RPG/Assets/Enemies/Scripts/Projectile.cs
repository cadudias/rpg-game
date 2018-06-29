using UnityEngine;

public class Projectile : MonoBehaviour {

    public float projectileSpeed; // note other classes can set it

    float damageCaused;

    public void SetDamage(float damage)
    {
        damageCaused = damage;
    }

    void OnTriggerEnter(Collider collider)
    {
        Component damagableComponent = collider.gameObject.GetComponent(typeof(IDamagable));
        
        if (damagableComponent)
            (damagableComponent as IDamagable).TakeDamage(damageCaused);
    }
}
