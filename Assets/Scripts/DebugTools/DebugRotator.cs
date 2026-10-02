using UnityEngine;

namespace Balltallion.DebugTools
{
    public class DebugRotator : MonoBehaviour
    {
        [SerializeField, Range(-360.0f, 360.0f)] private float rotationSpeed;

        private void Update()
        {
            transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        }
    }
}
