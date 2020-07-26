using System;
using RPG.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        Fighter fighter;
        Slider slider;

        private void Awake()
        {
            fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
            slider = GetComponent<Slider>();
        }

        private void Update()
        {
            if (fighter.GetTarget() == null)
            {
                return;
            }
            Health health = fighter.GetTarget();
            slider.maxValue = health.GetMaxHealthPoints();
            slider.value = health.GetHealthPoints();
        }
    }
}