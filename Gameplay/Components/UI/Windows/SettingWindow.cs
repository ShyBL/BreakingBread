using Base.Core.Components;
using Base.Core.Managers;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
namespace Base.Gameplay.Components
{
    public class SettingWindow : MyMonoBehaviour, IUIComponent
    {
        [SerializeField] private AudioMixer myMixer; 
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Slider masterSlider;

        private void Start()
        {
            LoadVolumeSettings();
            GameManager.UIManager.RegisterUIComponent(this);
        }

        public void SetMasterVolume()
        {
            float volume = masterSlider.value; 
            myMixer.SetFloat("master", Mathf.Log10(volume)*20);
            GameManager.SettingsManager.ChangeValue(volume,SettingsType.MasterVolume);
        }
        
        public void SetMusicVolume()
        {
            float volume = musicSlider.value; 
            myMixer.SetFloat("music", Mathf.Log10(volume)*20);
            GameManager.SettingsManager.ChangeValue(volume,SettingsType.MusicVolume);
        }
        
        public void SetSFXVolume()
        {
            float volume = sfxSlider.value; 
            myMixer.SetFloat("sfx", Mathf.Log10(volume)*20);
            GameManager.SettingsManager.ChangeValue(volume,SettingsType.SFXVolume);
        }
       
        public void OnClose()
        {
            GameManager.UIManager.HideOne("SettingWindow", false);
        }
        
        private void LoadVolumeSettings()
        {
            masterSlider.value = GameManager.SettingsManager.GetValueAsFloat(SettingsType.MasterVolume);
            musicSlider.value = GameManager.SettingsManager.GetValueAsFloat(SettingsType.MusicVolume);
            sfxSlider.value = GameManager.SettingsManager.GetValueAsFloat(SettingsType.SFXVolume);

            myMixer.SetFloat("master", Mathf.Log10(masterSlider.value) * 20);
            myMixer.SetFloat("music", Mathf.Log10(musicSlider.value) * 20);
            myMixer.SetFloat("sfx", Mathf.Log10(sfxSlider.value) * 20);
        }
        
        private void OnDestroy()
        {
            GameManager.UIManager.UnRegisterUIComponent(this);
        }

        
        public string UITag { get; set; } = "SettingWindow";
        public UIScenarios[] UIScenarios { get; set; } = { Core.Managers.UIScenarios.Welcome };
        public void HideUI(bool isHide)
        {
            gameObject.SetActive(isHide);
        }

        public void LockUI(bool isLock)
        {

        }
        
        public void DoTweenOpenClose(bool isOpen)
        {
            
        }
    }
}