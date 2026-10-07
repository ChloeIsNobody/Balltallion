using UnityEngine;

namespace Balltallion.UI
{
    public class SimpleWaveAnimator : MonoBehaviour
    {
        [SerializeField] private float frequency = 1.0f;
        [SerializeField] private float timeOffset = 0.0f;

        [Header("Position")]
        [SerializeField] private Vector3 positionOffset;
        [SerializeField] private float positionTimeOffset;
        
        [Header("Rotation")]
        [SerializeField] private float rotationOffset;
        [SerializeField] private float rotationTimeOffset;
        
        [Header("Scale")]
        [SerializeField] private Vector3 scaleOffset;
        [SerializeField] private float scaleTimeOffset;

        private Vector3 initialPosition;
        private float initialRotation;
        private Vector3 initialScale;

        private void Awake()
        {
            initialPosition = transform.localPosition;
            initialRotation = transform.localEulerAngles.z;
            initialScale = transform.localScale;
        }

        private void Update()
        {
            transform.localPosition = initialPosition + positionOffset * Mathf.Sin(Mathf.Deg2Rad * 360.0f * (frequency * Time.unscaledTime + timeOffset + positionTimeOffset));
            float rotation = initialRotation + rotationOffset * Mathf.Sin(Mathf.Deg2Rad * 360.0f * (frequency * Time.unscaledTime + timeOffset + rotationTimeOffset));
            transform.localRotation = Quaternion.Euler(0.0f, 0.0f, rotation);
            transform.localScale = initialScale + scaleOffset * Mathf.Sin(Mathf.Deg2Rad * 360.0f * (frequency * Time.unscaledTime + timeOffset + scaleTimeOffset));
        }
    }
}
