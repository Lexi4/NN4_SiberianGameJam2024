using System;
using Game.Scripts.UI;
using UnityEngine;

namespace Game.Scripts
{
    public class FinTrigger : MonoBehaviour
    {
        [SerializeField] private LevelUI levelUI;


        private void OnTriggerEnter2D(Collider2D other)
        {
            levelUI.ShowFin();
        }
    }
}