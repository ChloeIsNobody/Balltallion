using UnityEngine;

namespace Balltallion
{
    public struct BallCollisionData
    {
        public BallBody otherBall;
        public Vector3 contactPoint;
        public Vector3 contactNormal;
        public float collisionPower;
    }
}