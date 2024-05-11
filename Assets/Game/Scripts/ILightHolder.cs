using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts
{
    public interface ILightHolder
    {
        void TakeDamage();
        float GetActiveRadius();
        int GetActivePower();
        Vector3 GetPosition();
    }
}