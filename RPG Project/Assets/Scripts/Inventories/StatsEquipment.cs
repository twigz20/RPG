using GameDevTV.Inventories;
using RPG.Combat;
using RPG.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Inventories
{
    public class StatsEquipment : Equipment, IModifierProvider
    {
        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            foreach (var slot in GetAllPopulatedSlots())
            {
                var item = GetItemInSlot(slot) as IModifierProvider;
                if (item == null) continue;
                // Don't count weapon/shield damage with ability... Probably need to rejig this
                if (slot == EquipLocation.Weapon || slot == EquipLocation.Shield)
                {
                    Fighter fighter = GetComponent<Fighter>();
                    var config = fighter.GetAbilityConfig();
                    if (config != null && fighter.CanUseAbility(config))
                    {
                        continue;
                    }
                }

                foreach (float modifier in item.GetAdditiveModifiers(stat))
                {
                    yield return modifier;
                }
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            foreach (var slot in GetAllPopulatedSlots())
            {
                var item = GetItemInSlot(slot) as IModifierProvider;
                if (item == null) continue;
                // Don't count weapon/shield damage with ability... Probably need to rejig this
                if (slot == EquipLocation.Weapon || slot == EquipLocation.Shield)
                {
                    Fighter fighter = GetComponent<Fighter>();
                    var config = fighter.GetAbilityConfig();
                    if (config != null && fighter.CanUseAbility(config))
                    {
                        continue;
                    }
                }

                foreach (float modifier in item.GetPercentageModifiers(stat))
                {
                    yield return modifier;
                }
            }
        }
    }
}