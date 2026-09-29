using UnityEngine;
namespace Balltallion.BallSimulation
{
    public class Spring
    {
        private float stiffness = 0.2f;
        private float damping = 0.2f;
        private float dampingCoefficient = 0.2f;

        private float position = 0.0f;
        private float goalPosition = 0.0f;
        private float velocity = 0.0f;
        private float acceleration = 0.0f;

        public Spring(float stiffness=0.2f, float damping=0.2f) {
            this.stiffness = stiffness;
            this.damping = damping;
            RecalulateDampingCoefficient();
        }

        public void Update(float delta) {
            ApplyForce(-(position-goalPosition)*stiffness);
            ApplyForce(-velocity*damping*dampingCoefficient);

            velocity += 30.0f*acceleration*delta;
            position += 30.0f*velocity*delta;
            acceleration = 0.0f;
        }

        public void ApplyForce(float force) {
            acceleration += force;
        }

        public void NudgeVelocity(float amount) {
            velocity += amount;
        }

        public void Reset() {
            position = goalPosition;
            velocity = 0.0f;
            acceleration = 0.0f;
        }

        private void RecalulateDampingCoefficient() {
            dampingCoefficient = 2.0f * Mathf.Sqrt(stiffness);
        }

        public float GetStiffness() => stiffness;
        public void SetStiffness(float newStiffness) {stiffness = newStiffness; RecalulateDampingCoefficient();}

        public float GetDamping() => damping;
        public void SetDamping(float newDamping) {damping = newDamping; RecalulateDampingCoefficient();}

        public float GetPosition() => position;
        public void SetPosition(float newPosition) => position = newPosition;

        public float GetGoalPosition() => goalPosition;
        public void SetGoalPosition(float newGoalPosition) => goalPosition = newGoalPosition;

        public float GetVelocity() => velocity;
        public void SetVelocity(float newVelocity) => velocity = newVelocity;

        public float GetAcceleration() => acceleration;
    }
}