using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class PowerAttackBehaviour : MonoBehaviour, ISpecialAbility
    {
        PowerAttackConfig config;

        public void SetConfig(PowerAttackConfig configToSet)
        {
            this.config = configToSet;
        }

        public void Use()
        {
            throw new System.NotImplementedException();
        }

        // Use this for initialization
        void Start()
        {
            print("power attack special ability attached to: " + gameObject.name);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
