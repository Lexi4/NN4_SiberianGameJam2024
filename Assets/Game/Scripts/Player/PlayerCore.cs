using System;
using Game.Scripts.Interactable;
using UnityEngine;

namespace Game.Scripts.Player
{
    public class PlayerCore : MonoBehaviour, ILightHolder
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
            Collider2D[] res = Physics2D.OverlapCircleAll(interactPoint.position,
                interactRadius,
                interactableLayers);

            for (int i = 0; i < res.Length; i++)
            {
                var overlap = res[i];
                
                if (overlap == null) continue;


                if (overlap.transform.TryGetComponent(out OilSpot spot))
                {
                    AddFuelToLantern(spot.Amount);
                    spot.Interact();
                }
                else if (overlap.transform.TryGetComponent(out Bonfire bonfire))
                {
                    if (bonfire.IsActive) return;
                    if (lantern.TryActivateBonfire(bonfire.Cost))
                    {
                        bonfire.SetActive();
                    }
                }
            }
        }

        private void AddFuelToLantern(float amount)
        {
            lantern.AddFuel(amount);
        }

        public void TakeDamage()
        {
            Debug.Log("Game Over!!!");
        }

        public float GetActiveRadius() => lantern.LanternRadius;

        public int GetActivePower() => lantern.LanternPower;

        public Vector3 GetPosition() => transform.position;
    }
}