using UnityEngine;

namespace Balltallion
{
    [CreateAssetMenu(fileName = "GlobalPhysicsDataSO", menuName = "Balltallion/GlobalPhysicsDataSO")]
    public class GlobalPhysicsDataSO : ScriptableObject
    {
        [SerializeField, Range(0.0f, 100.0f)] public float maxVelocity;
        [SerializeField, Range(0.0f, 25.0f)] public float minVelocityXAfterCollision;
        [SerializeField, Range(0.0f, 25.0f)] public float minVelocityYAfterFloorBounce;
    }
}
