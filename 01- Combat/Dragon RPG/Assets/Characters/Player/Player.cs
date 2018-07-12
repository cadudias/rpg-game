using System;
using UnityEngine;
using UnityEngine.Assertions;

public class Player : MonoBehaviour, IDamagable {

    [SerializeField] int enemyLayer = 9;
    [SerializeField] float damagePerShot = 30f;
    [SerializeField] float minTimesBetweenHits = .5f;
    [SerializeField] float maxAttackRange = 2f;

    [SerializeField] Weapon weaponInUse;

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
        GameObject dominantHand = RequestDominantHand();
        var weapon = Instantiate(weaponPrefab, dominantHand.transform);
        // set the weapon postion based on weaponGrip
        weapon.transform.localPosition = weaponInUse.gripTransform.localPosition;
        weapon.transform.localRotation = weaponInUse.gripTransform.localRotation;
    }

    private GameObject RequestDominantHand()
    {
        var dominatHand = GetComponentsInChildren<DominantHand>();
        int numberOfDominantHands = dominatHand.Length;
        Assert.IsFalse(numberOfDominantHands <= 0, "No DominantHand found on Player, please add one");
        Assert.IsFalse(numberOfDominantHands > 1, "Multiple dominantHand added, please remove one");
        return dominatHand[0].gameObject;
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

            //currentTarget = enemy;

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
