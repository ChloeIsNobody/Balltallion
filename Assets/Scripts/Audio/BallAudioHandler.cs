using Balltallion.Battle;
using Balltallion.Simulation.Weapons;
using UnityEngine;
using UnityEngine.Audio;

namespace Balltallion.Audio
{
    public class BallAudioHandler : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;

        private AudioResource damageSound;
        private AudioResource bounceSound;

        public void LoadBallData(BallDataSO data)
        {
            damageSound = data.damageSound;
            bounceSound = data.bounceSound;
        }

        public void PlayDamageSound()
        {
            if (!damageSound) return;
            audioSource.Stop();
            audioSource.resource = damageSound;
            audioSource.Play();
        }

        public void PlayBounceSound()
        {
            if (!bounceSound) return;
            audioSource.Stop();
            audioSource.resource = bounceSound;
            audioSource.Play();
        }
    }
}