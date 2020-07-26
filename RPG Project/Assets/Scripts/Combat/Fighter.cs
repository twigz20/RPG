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
using RPG.Abilities;
using GameDevTV.UI.Inventories;
using UnityEngine.UI;

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
        [HideInInspector] Dictionary<string, AbilityConfig> abilityConfigs;
        [HideInInspector] Dictionary<string, float> abilityTimers;

        [HideInInspector] Inventory abilityInventory;

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
            abilityInventory = GameObject.FindGameObjectWithTag("ActionSlotInventory").GetComponent<RPG.Abilities.AbilityUI>().GetInventory();
            abilityConfigs = new Dictionary<string, AbilityConfig>();
            abilityTimers = new Dictionary<string, float>();
        }

        private void Start() 
        {
            currentWeapon.ForceInit();
        }

        private void Update()
        {
            UpdateTimers();

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

        public bool CanUseAbility(AbilityConfig abilityConfig)
        {
            if (abilityTimers[abilityConfig.GetItemID()] > abilityConfig.cooldown)
            {
                return true;
            }

            return false;
        }

        private void UpdateTimers()
        {
            timeSinceLastAttack += Time.deltaTime;

            if (abilityTimers.Count > 0)
            {
                List<string> keys = new List<string>(abilityTimers.Keys);
                foreach (string key in keys)
                {
                    abilityTimers[key] += Time.deltaTime;
                }
            }

            if (abilityConfigs.Count > 0)
            {
                GameObject[] actionSlots = GameObject.FindGameObjectsWithTag("ActionSlot");
                foreach (GameObject actionSlot in actionSlots)
                {
                    ActionSlotUI actionSlotUI = actionSlot.GetComponent<ActionSlotUI>();
                    if (actionSlotUI != null)
                    {
                        InventoryItem item = actionSlotUI.GetItem();
                        if (item != null)
                        {
                            string key = item.GetItemID();
                            if (abilityTimers.ContainsKey(key))
                            {
                                float abilityTimer = abilityTimers[key];
                                float abilityCooldown = abilityConfigs[key].cooldown;
                                float result = 1 - (abilityTimer / abilityConfigs[key].cooldown);
                                Transform cooldownObject = actionSlot.transform.Find("Cooldown Image");
                                cooldownObject.gameObject.SetActive(result > 0);
                                Image image = cooldownObject.GetComponent<Image>();
                                image.fillAmount = result;
                                int seconds = (int)(abilityConfigs[key].cooldown - (abilityTimer % 60)) + 1;
                                cooldownObject.transform.Find("Text").gameObject.GetComponent<Text>().text = String.Format("{0:0}", seconds);
                            }
                        }
                    }
                }
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
                abilityTimers[abilityConfig.GetItemID()] = 0f;
            }
            else if (currentWeaponConfig.HasProjectile())
            {
                currentWeaponConfig.LaunchProjectile(RightHandTransform, LeftHandTransform, target, gameObject, damage);
            }
            else
            {
                target.TakeDamage(gameObject, damage);
            }

            abilityConfig = null;
        }

        public AbilityConfig GetAbilityConfig()
        {
            return abilityConfig;
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
            if (abilityConfigs.ContainsKey(abilityConfig.GetItemID()))
            {
                AbilityConfig config = abilityConfigs[abilityConfig.GetItemID()];
                if (CanUseAbility(config))
                {
                    this.abilityConfig = config;
                }
            } else
            {
                abilityConfigs.Add(abilityConfig.GetItemID(), abilityConfig);
                abilityTimers.Add(abilityConfig.GetItemID(), Mathf.Infinity);
                this.abilityConfig = abilityConfig;
            }
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