using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes
{
    public class HealthDisplay : MonoBehaviour
    {
        Health health;
        [SerializeField] Slider slider;
        [SerializeField] Text text;
        [SerializeField] Image image;

        private void Awake()
        {
            health = GameObject.FindWithTag("Player").GetComponent<Health>();
        }

        private void Update()
        {
            float maxHealth = health.GetMaxHealthPoints();
            float currentHealth = health.GetHealthPoints();
            text.text = String.Format("{0:0}/{1:0}", currentHealth, maxHealth);
            slider.maxValue = maxHealth;
            slider.value = currentHealth;
            image.fillAmount = currentHealth/maxHealth;
        }
    }
}