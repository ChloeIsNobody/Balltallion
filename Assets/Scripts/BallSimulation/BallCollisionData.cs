using UnityEngine;

namespace Balltallion.BallSimulation
{
    public struct BallCollisionData
    {
        public BallBody otherBall;
        public Vector3 contactPoint;
        public Vector3 contactNormal;
        public float collisionPower;
    }
}