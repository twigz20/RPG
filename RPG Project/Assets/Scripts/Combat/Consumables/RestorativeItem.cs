using GameDevTV.Inventories;
using RPG.Attributes;
using UnityEngine;

namespace RPG.Combat
{
    /// <summary>
    /// An inventory item that can be placed in the action bar and "Used".
    /// </summary>
    /// <remarks>
    /// This class should be used as a base. Subclasses must implement the `Use`
    /// method.
    /// </remarks>
    [CreateAssetMenu(fileName = "Consumable", menuName = ("Consumables/Restorative Item"))]
    public class RestorativeItem : ActionItem
    {
        // CONFIG DATA
        [SerializeField] float healthRestoreAdditive = 0f;
        [SerializeField] float healthRestorePercentage = 0f;

        // PUBLIC

        /// <summary>
        /// Trigger the use of this item.
        /// </summary>
        /// <param name="user">The character that is using this action.</param>
        public override void Use(GameObject user)
        {
            Debug.Log("Using: " + this);

            Health health = user.GetComponent<Health>();
            health.Heal(healthRestoreAdditive);

            if (healthRestorePercentage > 0f)
            {
                health.Heal(health.GetMaxHealthPoints() * healthRestorePercentage / 100);
            }
        }
    }
}