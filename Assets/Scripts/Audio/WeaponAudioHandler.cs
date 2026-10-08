using Balltallion.Simulation.Weapons;
using UnityEngine;

namespace Balltallion.Audio
{
    public class WeaponAudioHandler : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip parrySound;

        private AudioClip attackSound;
        
        public void LoadWeaponData(WeaponDataSO data)
        {
            attackSound = data.attackSound;
        }
        
        public void PlayAttackSound()
        {
            if (attackSound != null)
                audioSource.PlayOneShot(attackSound);
        }
        
        public void PlayParrySound()
        {
            if (parrySound != null)
                audioSource.PlayOneShot(parrySound);
        }
    }
}
