using GameDevTV.Saving;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics

{
    public class CinematicTrigger : MonoBehaviour, ISaveable
    {
        bool alreadyTriggered = false;

        private void OnTriggerEnter(Collider other)
        {
            if (!alreadyTriggered && other.gameObject.tag == "Player")
            {
                GetComponent<PlayableDirector>().Play();
                alreadyTriggered = true;
            }
        }

        [System.Serializable]
        struct CinematicsSaveData
        {
            public bool alreadyTriggered;
        }

        public object CaptureState()
        {
            CinematicsSaveData data = new CinematicsSaveData();
            data.alreadyTriggered = alreadyTriggered;
            return data;
        }

        public void RestoreState(object state)
        {
            CinematicsSaveData data = (CinematicsSaveData)state;
            alreadyTriggered = data.alreadyTriggered;
        }
    }
}