using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Pool;

namespace Core
{
    public class ObjectsRegistry
    {
        [NotNull] private Dictionary<string, HashSet<GameObject>> _registry = new();

        public static ObjectsRegistry Get()
        {
            return World.Get().ObjectsRegistry;
        }

        public void Register(string name, GameObject gameObject)
        {
            if (_registry.ContainsKey(name))
            {
                _registry[name].Add(gameObject);
            }
            else
            {
                var newSet = new HashSet<GameObject> { gameObject };
                _registry.Add(name,newSet);
            }
        }
        
        public void RegisterMany(string name, GameObject[] gameObjects)
        {
            foreach (var gameObject in gameObjects)
                Register(name, gameObject);
        }

        public void Unregister(GameObject gameObject)
        {
            List<string> rowsToRemove = new List<string>();
            foreach (var row in _registry)
            {
                if (!row.Value.Contains(gameObject)) continue;

                if (row.Value.Count == 1)
                {
                    rowsToRemove.Add(row.Key);
                }
                else
                {
                    row.Value.Remove(gameObject);
                }
            }
            foreach (var rowKey in rowsToRemove)
            {
                _registry.Remove(rowKey);
            }
        }

        public void UnregisterMany(GameObject[] gameObjects)
        {
            foreach (var gameObject in gameObjects)
                Unregister(gameObject);
        }
    }
}