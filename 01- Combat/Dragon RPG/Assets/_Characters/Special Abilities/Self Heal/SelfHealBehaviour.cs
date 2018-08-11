using UnityEngine;

namespace RPG.Characters
{
    public class SelfHealBehaviour : AbilityBehaviour
    {        
        public void SetConfig(SelfHealConfig configToSet)
        {
            this.config = configToSet;
        }

        public override void Use(AbilityUseParams useParams)
        {
            PlayAbilitySound();
            HealHP();
            PlayParticleEffect();
        }

        private void HealHP()
        {
            Player player = gameObject.GetComponent<Player>();
            player.Heal((config as SelfHealConfig).GetHealAmount());
        }
    }
}
