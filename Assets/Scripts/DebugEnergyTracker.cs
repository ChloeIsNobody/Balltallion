using System.Collections.Generic;
using UnityEngine;

namespace Balltallion
{
    public class DebugEnergyTracker : MonoBehaviour
    {
        [SerializeField] private List<BallBody> trackedBalls;
        [SerializeField] private float groundHeight;

        private float initialSystemEnergy;

        private void Awake()
        {
            initialSystemEnergy = GetSystemEnergy();
        }

        private void FixedUpdate()
        {
            DebugPanel.Instance.SetDebugLabel(0, "Initial Energy", initialSystemEnergy.ToString("F2"));
            DebugPanel.Instance.SetDebugLabel(1, "System Energy", GetSystemEnergy().ToString("F2"));
            
            float energyDiff = GetSystemEnergy() - initialSystemEnergy;
            DebugPanel.Instance.SetDebugLabel(2, "Energy Diff", energyDiff.ToString("F2"));
            
            for (int i = 0; i < trackedBalls.Count; i++)
            {
                BallBody ball = trackedBalls[i];
                DebugPanel.Instance.SetDebugLabel(i+3, $"Ball {i}", GetTotalEnergy(ball).ToString("F2"));
            }
        }

        private float GetSystemEnergy()
        {
            float energy = 0.0f;
            for (int i = 0; i < trackedBalls.Count; i++)
            {
                BallBody ball = trackedBalls[i];
                energy += GetTotalEnergy(ball);
            }
            return energy;
        }
        
        private float GetTotalEnergy(BallBody ball)
        {
            if (!ball) return 0.0f;
            return ball.GetKineticEnergy() + GetPotentialEnergy(ball);
        } 
        
        private float GetPotentialEnergy(BallBody ball)
        {
            Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
            return rb.mass * (-Physics2D.gravity.y * rb.gravityScale) * (rb.position.y - groundHeight);
        }
    }
}
