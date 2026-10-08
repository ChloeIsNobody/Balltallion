using Balltallion.Battle;
using UnityEngine;

namespace Balltallion
{
    public class BallAudioHandler : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;

        private AudioClip attackSound;
        private AudioClip damageSound;
        private AudioClip bounceSound;

        public void LoadBallData(BallDataSO data)
        {
            attackSound = data.attackSound;
            damageSound = data.damageSound;
            bounceSound = data.bounceSound;
        }

        public void PlayAttackSound()
        {
            audioSource.PlayOneShot(attackSound);
        }

        public void PlayDamageSound()
        {
            audioSource.PlayOneShot(damageSound);
        }

        public void PlayBounceSound()
        {
            audioSource.PlayOneShot(bounceSound);
        }
    }
}