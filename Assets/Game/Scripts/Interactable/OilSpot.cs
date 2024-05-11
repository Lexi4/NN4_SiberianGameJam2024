using UnityEngine;

namespace Game.Scripts.Interactable
{
    public class OilSpot : MonoBehaviour
    {
        [SerializeField] public float Amount;

        public virtual void Interact()
        {
            Destroy(gameObject);
        }
    }
}