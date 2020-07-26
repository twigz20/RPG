using GameDevTV.Inventories;
using GameDevTV.UI.Inventories;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities
{
    public class AbilityUI : MonoBehaviour
    {
        // CONFIG DATA
        [SerializeField] AbilitySlotUI InventoryItemPrefab = null;

        // CACHE
        [SerializeField] Inventory abilityInventory;

        // LIFECYCLE METHODS

        private void Awake()
        {
            
        }

        private void Start()
        {
            Redraw();
        }

        // PRIVATE

        private void Redraw()
        {
            GameObject[] abilitySlots = GameObject.FindGameObjectsWithTag("Ability");
            int count = Mathf.Min(abilitySlots.Length, abilityInventory.GetSize());
            for (int i = 0; i < count; i++)
            {
                AbilitySlotUI slot = abilitySlots[i].GetComponent<AbilitySlotUI>();
                abilityInventory.AddItemToSlot(i, slot.GetItem(), 1);
                slot.Setup(abilityInventory, i);
            }
        }

        public Inventory GetInventory()
        {
            return abilityInventory;
        }
    }
}