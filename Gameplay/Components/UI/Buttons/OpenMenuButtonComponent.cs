using Base.Core.Components;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Base.Gameplay.Components
{
    [RequireComponent(typeof(Button))]
    public class OpenMenuButtonComponent : MyMonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text buttonText;
        public AudioComponent audioComponent;
        public AudioSource fxSource;
        public AudioClip click;
        private Color customTextColor = new Color(238f / 255f, 225f / 255f, 186f / 255f, 1f); // Hex color EEE1BA

        private void Start()
        {
            audioComponent = FindObjectOfType<AudioComponent>();
            fxSource = audioComponent.SFXAudioSource;
            click = audioComponent.menuClick;
        }

        public void OnBakeryMenuButton()
        {
            GameManager.AudioManager.PlaySFX(click,fxSource);
            GameManager.UIManager.HideAllAndOpenByTags("MainMenu", "BakeryTabs",true);
        }
        
        public void OnMissionsMenuButton()
        {
            GameManager.AudioManager.PlaySFX(click,fxSource);
            GameManager.UIManager.HideAllAndOpenByTags("MilestoneMenu","MilestoneTabs", true);

        }
        
        public void OnResearchMenuButton()
        {
            GameManager.AudioManager.PlaySFX(click,fxSource);
            GameManager.UIManager.HideAllAndOpenByTags("ResearchMenu", "BakeryTabs",true);
        }

        public void OnSettingMenuButton()
        {
            GameManager.UIManager.OpenPopupWindow(0, 0, "SettingWindow");
        }
    }
}