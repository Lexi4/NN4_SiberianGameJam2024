using System;
using Game.Scripts.Interactable;
using UnityEngine;

namespace Game.Scripts.Player
{
    public class PlayerCore : MonoBehaviour
    {
        [SerializeField] private Transform interactPoint;
        [SerializeField] private float interactRadius;
        [SerializeField] private LayerMask interactableLayers;
        [SerializeField] private Lantern lantern;
        private PlayerInput _playerInput;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
        }

        void Start()
        {
            _playerInput.onInteract += OnInteract;
            _playerInput.onFlash += OnFlash;
        }

        private void OnFlash()
        {
            lantern.UseFlash();
        }

        private void OnInteract()
        {
            Collider2D overlap = Physics2D.OverlapCircle(
                interactPoint.position,
                interactRadius,
                interactableLayers);

            if (overlap != null)
            {
                if (overlap.transform.TryGetComponent(out OilSpot spot))
                {
                    AddFuelToLantern(spot.Amount);
                }
            }
        }

        private void AddFuelToLantern(float amount)
        {
            lantern.AddFuel(amount);
        }
    }
}