using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.Scripts.UI
{
    public class LevelUI : MonoBehaviour
    {
        [SerializeField] private Button restart, mainMenu, resume,mainMenu2;
        [SerializeField] private GameObject fin, menu, gameOver;

        private void Start()
        {
            restart.onClick.AddListener(OnRestart);
            mainMenu.onClick.AddListener(OnMenu);
            mainMenu2.onClick.AddListener(OnMenu);
            resume.onClick.AddListener(OnResume);
        }

        private void OnDestroy()
        {
            restart.onClick.RemoveAllListeners();
            mainMenu.onClick.RemoveAllListeners();
            mainMenu2.onClick.RemoveAllListeners();
            resume.onClick.RemoveAllListeners();
        }

        private void OnRestart()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void OnMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }

        private void OnResume()
        {
            Time.timeScale = 1f;
            HideAll();
        }

        private void HideAll()
        {
            fin.SetActive(false);
            menu.SetActive(false);
            gameOver.SetActive(false);
        }

        public void ShowFin()
        {
            Time.timeScale = 0f;
            fin.SetActive(true);
            menu.SetActive(false);
            gameOver.SetActive(false);
        }

        public void ShowMenu()
        {
            Time.timeScale = 0f;
            fin.SetActive(false);
            menu.SetActive(true);
            gameOver.SetActive(false);
        }

        public void ShowGameOver()
        {
            Time.timeScale = 0f;
            fin.SetActive(false);
            menu.SetActive(false);
            gameOver.SetActive(true);
        }
    }
}