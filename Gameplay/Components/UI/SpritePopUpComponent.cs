using Base.Core.Components;
using DG.Tweening;
using UnityEngine;
using EventType = Base.Core.Managers.EventType;
using Random = UnityEngine.Random;

namespace Base.Gameplay.Components
{
    public class SpritePopUpComponent : MyMonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite[] breadSprites;

        [SerializeField] private float lerpTime = 0.56f;
        [SerializeField] private Ease easeType;
        [SerializeField] private float sizeMulti;

        private void Start()
        {
          GameManager.EventsManager.AddListener(EventType.OnScoreChanged,OnBreadSold);
        }

        public void Init(Vector3 pointA, Vector3 pointB)
        {
            InitArt();

            transform.position = pointA;
            transform.localScale = Vector3.zero;

            transform.DOMove(pointB, lerpTime).SetEase(easeType);
            transform.DOScale(Vector3.one * sizeMulti, lerpTime).SetEase(easeType);
        }

        private void InitArt()
        {
            var random = Random.Range(0, breadSprites.Length);
            spriteRenderer.sprite = breadSprites[random];
        }

        private void OnBreadSold(object dataToPass)
        {
            var target = GameObject.Find("CoinsPanel");
            transform.DOMove(target.transform.position, lerpTime);
            transform.DOScale(Vector3.zero, lerpTime).onComplete += OnCollect;
        }

        private void OnCollect()
        {
            GameManager.PoolManager.ReturnToPool("SpritePopUp",this);
        }
    }
}