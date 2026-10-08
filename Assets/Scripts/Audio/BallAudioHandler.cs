using Balltallion.Battle;
using Balltallion.Simulation.Weapons;
using UnityEngine;

namespace Balltallion.Audio
{
    public class BallAudioHandler : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;

        private AudioClip damageSound;
        private AudioClip bounceSound;

        public void LoadBallData(BallDataSO data)
        {
            damageSound = data.damageSound;
            bounceSound = data.bounceSound;
        }

        public void PlayDamageSound()
        {
            if (damageSound != null)
                audioSource.PlayOneShot(damageSound);
        }

        public void PlayBounceSound()
        {
            if (bounceSound != null)
                audioSource.PlayOneShot(bounceSound);
        }
    }
}