using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Button play, credits, quit, returnToMenu;
        [SerializeField] private GameObject menuScreen, creditsScreen;
        
        void Start()
        {
            play.onClick.AddListener(OnPlay);
            credits.onClick.AddListener(OnCredits);
            quit.onClick.AddListener(OnQuit);
            returnToMenu.onClick.AddListener(OnReturn);
        }

        private void OnDisable()
        {
            play.onClick.RemoveAllListeners();
            credits.onClick.RemoveAllListeners();
            quit.onClick.RemoveAllListeners();
            returnToMenu.onClick.RemoveAllListeners();
        }

        private void OnPlay()
        {
        }

        private void OnCredits()
        {
            menuScreen.SetActive(false);
            creditsScreen.SetActive(true);
        }

        private void OnQuit()
        {
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }

        private void OnReturn()
        {
            menuScreen.SetActive(true);
            creditsScreen.SetActive(false);
        }
    }
}