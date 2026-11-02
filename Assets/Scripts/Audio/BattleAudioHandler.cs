using UnityEngine;
using UnityEngine.Audio;

namespace Balltallion.Audio
{
    public class BattleAudioHandler : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioResource battleMusic;
        
        public void PlayBattleMusic()
        {
            if (!battleMusic) return;

            audioSource.Stop();
            audioSource.resource = battleMusic;
            audioSource.loop = true;
            audioSource.Play();
        }

        public void StopBattleMusic()
        {
            audioSource.Stop();
        }
    }
}