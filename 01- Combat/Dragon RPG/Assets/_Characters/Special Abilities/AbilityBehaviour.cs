using UnityEngine;

namespace RPG.Characters
{
    public abstract class AbilityBehaviour : MonoBehaviour
    {
        AbilityConfig config;

        public abstract void Use(AbilityUseParams useParams);

    }
}
