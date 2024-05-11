using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Player
{
    public class PlayerSFX : MonoBehaviour
    {
        [SerializeField] private AudioSource source;

        [SerializeField] private List<AudioClip> clips;

        [SerializeField] private float volume;

        // Start is called before the first frame update
        void Start()
        {
            source.volume = volume;
        }

        private void OnStep()
        {
            source.PlayOneShot(GetRandomClip());
        }

        private AudioClip GetRandomClip()
        {
            return clips[Random.Range(0, clips.Count)];
        }
    }
}