using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;
using RPG.Stats;
using System.Collections.Generic;
using GameDevTV.Utils;
using System;
using GameDevTV.Inventories;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] WeaponConfig defaultWeapon = null;

        Health target;
        Equipment equipment;
        float timeSinceLastAttack = Mathf.Infinity;
        WeaponConfig currentWeaponConfig;
        LazyValue<Weapon> currentWeapon;

        bool moveToTarget = true;
        [HideInInspector] AbilityConfig abilityConfig = null;

        public Transform RightHandTransform { get => rightHandTransform; set => rightHandTransform = value; }
        public Transform LeftHandTransform { get => leftHandTransform; set => leftHandTransform = value; }
        public bool MoveToTarget { get => moveToTarget; set => moveToTarget = value; }

        private void Awake() {
            currentWeaponConfig = defaultWeapon;
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
            equipment = GetComponent<Equipment>();
            if (equipment != null)
            {
                equipment.equipmentUpdated += UpdateWeapon;
            }
        }

        private void Start() 
        {
            currentWeapon.ForceInit();
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;

            if (target == null) return;
            if (target.IsDead())
            {
                target = null;
                return;
            }

            if (!GetIsInRange(target.transform))
            {
                if (MoveToTarget)
                {
                    GetComponent<Mover>().MoveTo(target.transform.position, 1f);
                } else if(abilityConfig != null) {
                    MoveToAbilityRange();
                }
            }
            else
            {
                GetComponent<Mover>().Cancel();
                AttackBehaviour();
            }
        }

        private void MoveToAbilityRange()
        {
            var distance = Vector3.Distance(transform.position, target.transform.position);

            if (distance > abilityConfig.GetRange())
            {
                GetComponent<Mover>().MoveTo(target.transform.position, 1f);
            }
        }

        private Weapon SetupDefaultWeapon()
        {
            return AttachWeapon(defaultWeapon);
        }

        public void EquipWeapon(WeaponConfig weapon)
        {
            currentWeaponConfig = weapon;
            currentWeapon.value = AttachWeapon(weapon);
        }

        private void UpdateWeapon()
        {
            var weapon = equipment.GetItemInSlot(EquipLocation.Weapon) as WeaponConfig;
            if (weapon == null) 
            {
                EquipWeapon(defaultWeapon);
            } else
            {
                EquipWeapon(weapon);
            }
        }

        private Weapon AttachWeapon(WeaponConfig weapon)
        {
            Animator animator = GetComponent<Animator>();
            return weapon.Spawn(RightHandTransform, LeftHandTransform, animator);
        }

        public Health GetTarget()
        {
            return target;
        } 

        private void AttackBehaviour()
        {
            transform.LookAt(target.transform);
            if (timeSinceLastAttack > timeBetweenAttacks)
            {
                // This will trigger the Hit() event.
                TriggerAttack();
                timeSinceLastAttack = 0;
            }
        }

        private void TriggerAttack()
        {
            GetComponent<Animator>().ResetTrigger("stopAttack");
            GetComponent<Animator>().SetTrigger("attack");
        }

        // Animation Event
        void Hit()
        {
            if(target == null) { return; }

            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);

            if (currentWeapon.value != null)
            {
                currentWeapon.value.OnHit();
            }

            if(abilityConfig != null)
            {
                abilityConfig.UseAbility(RightHandTransform, LeftHandTransform, target, gameObject, damage);
            } else if (currentWeaponConfig.HasProjectile())
            {
                currentWeaponConfig.LaunchProjectile(RightHandTransform, LeftHandTransform, target, gameObject, damage);
            }
            else
            {
                target.TakeDamage(gameObject, damage);
            }

            abilityConfig = null;
        }

        void Shoot()
        {
            Hit();
        }

        private bool GetIsInRange(Transform targetTransform)
        {
            float range = abilityConfig != null ? abilityConfig.GetRange() : currentWeaponConfig.GetRange();
            return Vector3.Distance(transform.position, targetTransform.position) < range;
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) { return false; }
            if (!GetComponent<Mover>().CanMoveTo(combatTarget.transform.position)
                && !GetIsInRange(combatTarget.transform)) 
            { 
                return false; 
            }
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }

        public void SetTarget(GameObject combatTarget)
        {
            target = combatTarget.GetComponent<Health>();
            transform.LookAt(target.transform.position);
        }

        public void Attack(GameObject combatTarget, bool _abilityUsed = false)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        public void Cancel()
        {
            StopAttack();
            target = null;
            GetComponent<Mover>().Cancel();
        }

        private void StopAttack()
        {
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("stopAttack");
            abilityConfig = null;
        }

        public void UseAbility(AbilityConfig abilityConfig)
        {
            this.abilityConfig = abilityConfig;
        }

        public object CaptureState()
        {
            return currentWeaponConfig.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string)state;
            WeaponConfig weapon = UnityEngine.Resources.Load<WeaponConfig>(weaponName);
            EquipWeapon(weapon);
        }
    }
}