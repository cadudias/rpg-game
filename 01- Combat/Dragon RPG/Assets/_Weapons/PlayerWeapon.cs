using System;
using UnityEngine;

namespace RPG.Weapons
{
    [CreateAssetMenu(menuName = ("RPG/PlayerWeapon"))]
    public class PlayerWeapon : ScriptableObject
    {
        public Transform gripTransform;

        [SerializeField] GameObject weaponPrefab;
        [SerializeField] AnimationClip attackAnimation;

        public GameObject GetWeaponPrefab()
        {
            return weaponPrefab;
        }

        internal AnimationClip GetAttackAnimationClip()
        {
            return attackAnimation;
        }
    }
}
