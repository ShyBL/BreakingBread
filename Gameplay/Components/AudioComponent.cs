using Base.Core.Components;
using UnityEngine;

namespace Base.Gameplay.Components
{
    public class AudioComponent : MyMonoBehaviour
    {
        [SerializeField] public AudioSource MusicAudioSource;
        [SerializeField] public AudioSource SFXAudioSource;
        
        public AudioClip shopBackground;
        public AudioClip backgroundMusic;
        public AudioClip buyClick;
        public AudioClip menuClick;
        public AudioClip popup;
        
        private void Start()
        {
            DontDestroyOnLoad(this);
            PlayBackgroundMusic(backgroundMusic, MusicAudioSource);
        }

        private void PlayBackgroundMusic(AudioClip clip, AudioSource musicSource)
        {
            musicSource.clip = clip;
            musicSource.Play();
        }
    }
}