using System;
using UnityEngine;

namespace Base.Core.Managers
{
    public class AudioManager : BaseManager
    {
        private AudioClips audioConfig;
        
        public AudioManager(Action<BaseManager> onComplete) : base(onComplete)
        {
            audioConfig = GameManager.ConfigManager.GetConfig<AudioClips>();
            
            OnInitComplete();
        }
        
        public void PlaySFX(AudioClip clip, AudioSource sFXSource)
        {
            sFXSource.PlayOneShot(clip);
        }
    }

    public class AudioClips : BaseConfig
    {
        private string ClipName;

        public AudioClips(string clipName)
        {
            ClipName = clipName;
        }
    }
}