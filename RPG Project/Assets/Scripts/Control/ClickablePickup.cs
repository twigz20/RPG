using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using RPG.Movement;
using UnityEngine;

namespace RPG.Control
{
    public class ClickablePickup : MonoBehaviour, IRaycastable
    {
        Pickup pickup;

        private void Awake()
        {
            pickup = GetComponent<Pickup>();
        }

        public CursorType GetCursorType()
        {
            if (pickup.CanBePickedUp())
            {
                return CursorType.Pickup;
            } else
            {
                return CursorType.FullPickup;
            }
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                pickup.PickupItem();
            }
            return true;
        }

        public bool CanPickup(PlayerController callingController)
        {
            Mover mover = callingController.GetComponent<Mover>();
            if (mover.CanMoveTo(transform.position)
                && GetIsInRange(mover.transform))
            {
                return true;
            }
            return false;
        }

        public bool GetIsInRange(Transform targetTransform)
        {
            return Vector3.Distance(transform.position, targetTransform.position) < 1f;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                pickup.PickupItem();
            }
        }
    }
}