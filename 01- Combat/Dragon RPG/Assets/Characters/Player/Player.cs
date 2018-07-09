using System;
using UnityEngine;

public class Player : MonoBehaviour, IDamagable {

    [SerializeField] int enemyLayer = 9;
    [SerializeField] float damagePerShot = 30f;
    [SerializeField] float minTimesBetweenHits = .5f;
    [SerializeField] float maxAttackRange = 2f;
    [SerializeField] Weapon weaponInUse;
    
    GameObject currentTarget;
    public CameraRaycaster cameraRaycaster;
    
    float lastHitTime = 0;
    float currentHealthPoints;

    [SerializeField] float maxHealthPoints = 300f;
    public float healthAsPercentage { get { return currentHealthPoints / maxHealthPoints; } }

    void Start()
    {
        RegisterForMouseClick();
        currentHealthPoints = maxHealthPoints;
        PutWeaponInHand();
    }

    private void PutWeaponInHand()
    {
        // get default weapon
        var weaponPrefab = weaponInUse.GetWeaponPrefab();
        var weapon = Instantiate(weaponPrefab); // TODo move to corect place in hand
        // set the weapon to the weapon scriptable object

        //  set the 
    }

    private void RegisterForMouseClick()
    {
        cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
        cameraRaycaster.notifyMouseClickObservers += OnMouseClick;
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
        currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
    }
}
