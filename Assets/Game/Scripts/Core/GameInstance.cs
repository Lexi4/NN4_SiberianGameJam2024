using System.Collections;
using Core;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Debug = System.Diagnostics.Debug;

namespace Game.Scripts.Core
{
    public class GameInstance : MonoBehaviour
    {
        private static GameInstance _instance;
    
        public UnityEvent<World> onLevelLoadedEvent;
        public World CurrentWorld;
    
        // Start is called before the first frame update
        void Start()
        {
            onLevelLoadedEvent ??= new UnityEvent<World>();
        }

        private void Awake()
        {
            var objects = GameObject.FindGameObjectsWithTag("GameInstance");

            if (objects.Length > 1)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public static GameInstance Get()
        {
            return _instance;
        }

        public void LoadScene(string levelName)
        {
            StartCoroutine(LoadSceneAsync(levelName));
        }

        private IEnumerator LoadSceneAsync(string levelName)
        {
            SceneManager.LoadScene(levelName, LoadSceneMode.Single);
            CurrentWorld = new World();
            onLevelLoadedEvent.Invoke(CurrentWorld);
            yield break;
        }

        private IEnumerator CreateWorld()
        {
            CurrentWorld = new World();
            yield break;
        }
    }
}
