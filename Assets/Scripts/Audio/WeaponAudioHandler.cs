using Balltallion.Simulation.Weapons;
using UnityEngine;

namespace Balltallion.Audio
{
    public class WeaponAudioHandler : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;

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
    }
}
