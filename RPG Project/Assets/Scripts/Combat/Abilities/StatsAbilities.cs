using RPG.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    public class StatsAbilities : MonoBehaviour, IModifierProvider
    {
        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            Fighter fighter = GetComponent<Fighter>();
            var item = fighter.GetAbilityConfig();
            if (item != null && fighter.CanUseAbility(item))
            {
                foreach (float modifier in item.GetAdditiveModifiers(stat))
                {
                    yield return modifier;
                }
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            Fighter fighter = GetComponent<Fighter>();
            var item = fighter.GetAbilityConfig();
            if (item != null && fighter.CanUseAbility(item))
            {
                foreach (float modifier in item.GetPercentageModifiers(stat))
                {
                    yield return modifier;
                }
            }
        }
    }
}