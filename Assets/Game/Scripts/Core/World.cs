using Game.Scripts.Core;
using UnityEngine;

namespace Core
{
    public class World
    {
        //TODO: Change to player class.
        public GameObject Player;
        public ObjectsRegistry ObjectsRegistry;

        public static World Get()
        {
            return GameInstance.Get().CurrentWorld;
        }

        public World()
        {
            Player = GameObject.FindGameObjectWithTag("Player");
            ObjectsRegistry = new ObjectsRegistry();
        }
    }
}