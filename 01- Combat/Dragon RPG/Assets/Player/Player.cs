using System;
using UnityEngine;

public class Player : MonoBehaviour, IDamagable {

    [SerializeField] int enemyLayer = 9;
    [SerializeField] float damagePerShot = 30f;
    [SerializeField] float minTimesBetweenHits = .5f;
    [SerializeField] float maxAttackRange = 2f;
    
    GameObject currentTarget;
    public CameraRaycaster cameraRaycaster;
    
    float lastHitTime = 0;
    float currentHealthPoints;

    [SerializeField] float maxHealthPoints = 300f;
    public float healthAsPercentage { get { return currentHealthPoints / maxHealthPoints; } }

    void Start()
    {
        cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
        cameraRaycaster.notifyMouseClickObservers += OnMouseClick;

        currentHealthPoints = maxHealthPoints;
    }

    void OnMouseClick(RaycastHit raycastHit, int layerHit)
    {
        if (layerHit == enemyLayer)
        {
            var enemy = raycastHit.collider.gameObject;

            // check enemy is in range
            if ((enemy.transform.position - transform.position).magnitude > maxAttackRange)
                return;

            currentTarget = enemy;

            var enemyComponent = enemy.GetComponent<Enemy>();
            if (Time.time - lastHitTime > minTimesBetweenHits)
            {
                enemyComponent.TakeDamage(damagePerShot);
                lastHitTime = Time.time;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealthPoints = Mathf.Clamp((currentHealthPoints - damage), 0f, currentHealthPoints);
    }
}
