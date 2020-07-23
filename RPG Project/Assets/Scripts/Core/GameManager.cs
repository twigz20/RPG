using GameDevTV.Utils;
using Hellmade.Sound;
using UnityEngine;

namespace RPG.Core
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] AudioClip backgroundAudioClip = null;
        private Audio backgroundAudio = null;
        private int backgroundAudioID = -1;

        private void Awake()
        {
            if (backgroundAudioClip != null)
            {
                backgroundAudioID = EazySoundManager.PrepareMusic(backgroundAudioClip, 0.35f, true, false, 0.5f, 1);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            if (backgroundAudioClip != null)
            {
                backgroundAudio = EazySoundManager.GetAudio(backgroundAudioID);
                backgroundAudio.Play();
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}