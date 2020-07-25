using GameDevTV.Inventories;
using System.Collections;
using System.Collections.Generic;
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
    [CreateAssetMenu(fileName = "Consumable", menuName = ("Consumables/ChangeAttributes Item"))]
    public class ChangeAttributesItem : ActionItem
    {
        // CONFIG DATA

        // PUBLIC

        /// <summary>
        /// Trigger the use of this item.
        /// </summary>
        /// <param name="user">The character that is using this action.</param>
        public override void Use(GameObject user)
        {
            Debug.Log("Using action: " + this);
        }
    }
}