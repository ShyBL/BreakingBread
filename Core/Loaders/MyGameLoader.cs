using Base.Core.Components;
using Base.Core.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace Base.Core.Loaders
{
    public class MyGameLoader : MyMonoBehaviour
    {
        [SerializeField] private TMP_Text loadingManagerNameText;
        [SerializeField] private Slider sliderFillImage;
        [SerializeField] float fillAmountDuration = 0.1f;

        private string previousLoadingManagerName = "0";
        
        private void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        private void Start()
        {
            new GameManager(() =>
            {
                SceneManager.LoadScene("Main");
            });
        }

        private void Update()
        {
            if (GameManager.CurrentLoadingManagerName != previousLoadingManagerName)
            {
                DoLoadBar();
            }
        }


        private void DoLoadBar()
        {
            sliderFillImage.DOValue(GameManager.CurrentLoadingNum, fillAmountDuration).SetEase(Ease.InBounce);

            loadingManagerNameText.text = loadingManagerNameText == null
                ? "Loading Bakery. . ."
                : $"{GameManager.CurrentLoadingManagerName}  System";

            previousLoadingManagerName = GameManager.CurrentLoadingManagerName;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "Main")
            {
                Destroy(gameObject);
            }
        }
        
        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        
    }
}