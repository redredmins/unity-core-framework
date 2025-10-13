using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedMinS
{
    public class AppManager : SingletonMonobehaviour<AppManager>
    {
        [Header("UI Management")]
        [SerializeField] private UIManager _ui;
        public UIManager ui => GetOrCreateModule<UIManager>(ref _ui);

        [Header("Scene Management")]
        [SerializeField] private SceneLoader _scene;
        public SceneLoader scene => GetOrCreateModule<SceneLoader>(ref _scene);

        [Header("Data Management")]
        [SerializeField] private DesignDataContainer _table;
        public DesignDataContainer table => GetOrCreateModule<DesignDataContainer>(ref _table);
        [SerializeField] private UserData _user;
        public UserData user => GetOrCreateModule<UserData>(ref _user);

        [Header("Sound Management")]
        [SerializeField] private SoundManager _sound;
        public SoundManager sound => GetOrCreateModule<SoundManager>(ref _sound);


        protected override void OnSingletonAwake()
        {
            base.OnSingletonAwake();
            InitializeModules();
        }

        private void InitializeModules()
        {
            Debug.Log("[AppManager] Initializing modules...");
            
            // 모듈들의 초기화를 강제로 트리거
            _ = ui;
            _ = scene;
            _ = table;
            _ = user;
            _ = sound;
            
            Debug.Log("[AppManager] All modules initialized successfully.");
        }


        private M GetOrCreateModule<M>(ref M member) where M : MonoBehaviour
        {
            if (member == null)
            {
                member = FindAnyObjectByType<M>();

                if (member == null)
                {
                    var obj = new GameObject($"[Module]{typeof(M).Name}");
                    obj.transform.SetParent(this.transform);
                    member = obj.AddComponent<M>();

                    Debug.Log($"[AppManager] Created new module: {typeof(M).Name}");
                }
                else
                {
                    Debug.Log($"[AppManager] Found existing module: {typeof(M).Name}");
                }
            }
            return member;
        }
        
        /// 애플리케이션 일시정지/재개
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                Debug.Log("[AppManager] Application paused");
            }
            else
            {
                Debug.Log("[AppManager] Application resumed");
            }
        }


        /// 애플리케이션 포커스 변경
        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
            {
                Debug.Log("[AppManager] Application gained focus");
            }
            else
            {
                Debug.Log("[AppManager] Application lost focus");
            }
        }
    }
}