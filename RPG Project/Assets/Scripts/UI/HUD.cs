using RPG.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI
{
    public class HUD : MonoBehaviour
    {
        Fighter fighter;
        [SerializeField] GameObject enemyInfo;

        private void Awake()
        {
            fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
        }

        private void Start()
        {
        }

        private void Update()
        {
            bool hasTarget = fighter.GetTarget() != null;
            enemyInfo.SetActive(hasTarget);
        }
    }
}