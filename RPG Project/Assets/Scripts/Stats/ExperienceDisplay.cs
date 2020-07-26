using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class ExperienceDisplay : MonoBehaviour
    {
        BaseStats baseStats;
        Experience experience;
        [SerializeField] Slider slider;
        [SerializeField] Text text;

        private void Awake()
        {
            baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
            experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
        }

        private void Update()
        {
            float maxEXP = baseStats.GetStat(Stat.ExperienceToLevelUp);
            float currentEXP = experience.GetPoints();
            text.text = String.Format("{0:0}/{1:0}", currentEXP, maxEXP);
            slider.maxValue = maxEXP;
            slider.value = currentEXP;
        }
    }
}