using GameDevTV.Inventories;
using RPG.Attributes;
using RPG.Stats;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Abilities", menuName = ("Abilities/New Ability"))]
    public class AbilityConfig : ActionItem
    {
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] Projectile projectile;
        [SerializeField] float damage = 5f;
        [SerializeField] float percentageBonus = 0;
        [SerializeField] float range = 2f;
        [SerializeField] bool isRightHanded = true;

        private void Awake()
        {
            if (projectile == null)
            {
                throw new Exception("Projectile not assigned.");
            }
        }

        private void SetAnimator(Animator animator)
        {
            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if (animatorOverride != null)
            {
                animator.runtimeAnimatorController = animatorOverride;
            }
            else if (overrideController != null)
            {
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }
        }

        private Transform GetTransform(Transform rightHand, Transform leftHand)
        {
            Transform handTransform;
            if (isRightHanded) handTransform = rightHand;
            else handTransform = leftHand;
            return handTransform;
        }

        public void UseAbility(Transform rightHand, Transform leftHand, Health target, GameObject instigator, float calculatedDamage)
        {
            Projectile projectileInstance = Instantiate(projectile, GetTransform(rightHand, leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target, instigator, calculatedDamage + damage);
        }

        public float GetDamage()
        {
            return damage;
        }

        public float GetPercentageBonus()
        {
            return percentageBonus;
        }

        public float GetRange()
        {
            return range;
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return damage;
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return percentageBonus;
            }
        }

        /// <summary>
        /// Trigger the use of this ability.
        /// </summary>
        /// <param name="user">The character that is using this action.</param>
        public override void Use(GameObject user)
        {
            Debug.Log("Using Ability: " + this);

            Fighter fighter = user.GetComponent<Fighter>();
            fighter.UseAbility(this);
        }
    }
}