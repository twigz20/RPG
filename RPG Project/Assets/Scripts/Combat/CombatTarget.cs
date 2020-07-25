using UnityEngine;
using RPG.Attributes;
using RPG.Control;
using GameDevTV.Inventories;

namespace RPG.Combat
{
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        public CursorType GetCursorType()
        {
            return CursorType.Combat;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (!callingController.GetComponent<Fighter>().CanAttack(gameObject))
            {
                return false;
            }

            if (Input.GetMouseButton(0))
            {
                callingController.GetComponent<Fighter>().Attack(gameObject);
                callingController.GetComponent<Fighter>().MoveToTarget = true;
            }
            if (Input.GetMouseButton(1))
            {
                callingController.GetComponent<Fighter>().SetTarget(gameObject);
                callingController.GetComponent<Fighter>().MoveToTarget = false;
            }

            var actionStore = callingController.GetComponent<ActionStore>();
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                actionStore.Use(0, callingController.gameObject);
            }

            return true;
        }
    }
}