using System;
using DG.Tweening;
using Firebase.Extensions;

namespace Base.Core.Managers
{
    public class GameManager
    {
        public static GameManager Instance;
        
        //Core
        public MonitorManager MonitorManager;
        public AnalyticsManager AnalyticsManager;
        public EventsManager EventsManager;
        public ConfigManager ConfigManager;
        public SaveManager SaveManager;

        public TimeManager DateTimeManager;
        public StoreManager StoreManager;
        public PurchasingManager PurchaseManager;

        //Gameplay-Core
        public FactoryManager FactoryManager;
        public PoolingManager PoolManager;

        public AudioManager AudioManager;
        public UIManager UIManager;
        public CurrencyManager CurrencyManager;

        //Gameplay
        public GameplayManager GameplayManager;
        public UpgradesManager UpgradeManager;
        public MilestonesManager MilestoneManager;
        public ResearchsManager ResearchManager;
        public AdsManager AdsManager;
        public SettingsManager SettingsManager;

        private Action onCompleteAction;
        public string CurrentLoadingManagerName;
        public int CurrentLoadingNum;

        public GameManager(Action onComplete)
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                MyDebug.LogError($"Two {typeof(GameManager)} instances exist, didn't create new one");
                return;
            }

            onCompleteAction = onComplete;
            InitFirebase(InitManagers);
            DOTween.Init();
        }

        public T GetManager<T>() where T : BaseManager
        {
            if (typeof(T) == typeof(UpgradesManager))
            {
                return UpgradeManager as T;
            }
            else if (typeof(T) == typeof(ResearchsManager))
            {
                return ResearchManager as T;
            }
            else if (typeof(T) == typeof(MilestonesManager))
            {
                return MilestoneManager as T;
            }
            else
            {
                // Handle other manager types or return null if needed.
                return null;
            }
        }

        private void InitManagers()
        {
            new MonitorManager(result =>
            {
                CurrentLoadingManagerName = "Monitor";
                CurrentLoadingNum += 1;
                MonitorManager = (MonitorManager)result;

                new AnalyticsManager(result =>
                {
                    CurrentLoadingManagerName = "Analytics";
                    CurrentLoadingNum += 1;
                    AnalyticsManager = (AnalyticsManager)result;

                    new EventsManager(result =>
                    {
                        CurrentLoadingManagerName = "Events";
                        CurrentLoadingNum += 1;
                        EventsManager = (EventsManager)result;

                        new ConfigManager(result =>
                        {
                            CurrentLoadingManagerName = "Configuration";
                            CurrentLoadingNum += 1;
                            ConfigManager = (ConfigManager)result;

                            new SaveManager(result =>
                            {
                                CurrentLoadingManagerName = "Save & Load";
                                CurrentLoadingNum += 1;
                                SaveManager = (SaveManager)result;

                                new TimeManager(result =>
                                {
                                    CurrentLoadingManagerName = "Date & Time ";
                                    CurrentLoadingNum += 1;
                                    DateTimeManager = (TimeManager)result;

                                    new CurrencyManager(result =>
                                    {
                                        CurrentLoadingManagerName = "Currency";
                                        CurrentLoadingNum += 1;
                                        CurrencyManager = (CurrencyManager)result;

                                        new FactoryManager(result =>
                                        {
                                            CurrentLoadingManagerName = "Factory";
                                            CurrentLoadingNum += 1;
                                            FactoryManager = (FactoryManager)result;

                                            new PoolingManager(result =>
                                            {
                                                CurrentLoadingManagerName = "Pool";
                                                CurrentLoadingNum += 1;
                                                PoolManager = (PoolingManager)result;

                                                new UpgradesManager(result =>
                                                {
                                                    CurrentLoadingManagerName = "Upgrades";
                                                    CurrentLoadingNum += 1;
                                                    UpgradeManager = (UpgradesManager)result;

                                                    new MilestonesManager(result =>
                                                    {
                                                        CurrentLoadingManagerName = "Milestones";
                                                        CurrentLoadingNum += 1;
                                                        MilestoneManager = (MilestonesManager)result;

                                                        new ResearchsManager(result =>
                                                        {
                                                            CurrentLoadingManagerName = "Research";
                                                            CurrentLoadingNum += 1;
                                                            ResearchManager = (ResearchsManager)result;

                                                            new GameplayManager(result =>
                                                            {
                                                                CurrentLoadingManagerName = "Gameplay";
                                                                CurrentLoadingNum += 1;
                                                                GameplayManager = (GameplayManager)result;

                                                                new AudioManager(result =>
                                                                {
                                                                    CurrentLoadingManagerName = "Audio";
                                                                    CurrentLoadingNum += 1;
                                                                    AudioManager = (AudioManager)result;

                                                                    new SettingsManager(result =>
                                                                    {
                                                                        CurrentLoadingManagerName = "Volume";
                                                                        CurrentLoadingNum += 1;
                                                                        SettingsManager = (SettingsManager)result;

                                                                        new UIManager(result =>
                                                                        {
                                                                            CurrentLoadingManagerName =
                                                                                "User Interface";
                                                                            CurrentLoadingNum += 1;
                                                                            UIManager = (UIManager)result;
                                                                            onCompleteAction.Invoke();
                                                                        });
                                                                        
                                                                        // new AdsManager(result =>
                                                                        // {
                                                                        //     CurrentLoadingManagerName = "Advertisement";
                                                                        //     AdsManager = (AdsManager)result;
                                                                        // }
                                                                        
                                                                        // new PurchasingManager(result =>
                                                                        // {
                                                                        //     CurrentLoadingManagerName = "Purchasing";
                                                                        //     PurchaseManager = (PurchasingManager)result;
                                                                        // }
                                                                        
                                                                        // new StoreManager(result =>
                                                                        // {
                                                                        //     CurrentLoadingManagerName = "Stores";
                                                                        //     StoreManager = (StoreManager)result;
                                                                        //     onCompleteAction.Invoke();
                                                                        // }
                                                                    });
                                                                });
                                                            });
                                                        });
                                                    });
                                                });
                                            });
                                        });
                                    });
                                });
                            });
                        });
                    });
                });
            });
        }

        private void InitFirebase(Action onComplete)
        {
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                var dependencyStatus = task.Result;

                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    var defaultInstance = Firebase.FirebaseApp.DefaultInstance;
                }
                else
                {
                    UnityEngine.Debug.LogError(System.String.Format(
                        "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                }

                onComplete.Invoke();
            });
        }
    }
}