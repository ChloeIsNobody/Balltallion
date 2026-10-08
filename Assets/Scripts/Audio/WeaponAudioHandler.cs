using Balltallion.Simulation.Weapons;
using UnityEngine;
using UnityEngine.Audio;

namespace Balltallion.Audio
{
    public class WeaponAudioHandler : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioResource parrySound;

        private AudioResource attackSound;
        
        public void LoadWeaponData(WeaponDataSO data)
        {
            attackSound = data.attackSound;
        }
        
        public void PlayAttackSound()
        {
            if (!attackSound) return;
            audioSource.Stop();
            audioSource.resource = attackSound;
            audioSource.Play();
        }
        
        public void PlayParrySound()
        {
            if (!parrySound) return;
            audioSource.Stop();
            audioSource.resource = parrySound;
            audioSource.Play();
        }
    }
}
