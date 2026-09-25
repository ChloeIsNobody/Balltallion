using NaughtyAttributes;
using TMPro;
using UnityEngine;

namespace Balltallion
{
    public class ScoreFloaterSpawner : MonoBehaviour
    {
        public static ScoreFloaterSpawner Instance {get; private set;}
        
        [SerializeField] private Transform scoreFloaterPrefab;

        [SerializeField, MinMaxSlider(0.0f, 50.0f)] private Vector2 damageScaling;
        [SerializeField, MinMaxSlider(0.0f, 5.0f)] private Vector2 sizeScaling;
        [SerializeField, Range(0.1f, 5.0f)] private float sizeScalingExponent = 1.0f;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogWarning("More than one ScoreFloaterSpawner in scene!");
                Destroy(gameObject);
            }
        }

        public void SpawnScoreFloater(Vector3 spawnPosition, int value, Color color)
        {
            Transform scoreFloater = Instantiate(scoreFloaterPrefab, spawnPosition, Quaternion.identity, transform);
            TextMeshPro textMesh = scoreFloater.GetComponentInChildren<TextMeshPro>();
            textMesh.text = value.ToString();
            textMesh.color = color;
            scoreFloater.localScale = Vector3.one * CalculateSize(value);
            Destroy(scoreFloater.gameObject, 0.833f);
        }

        public float CalculateSize(int value)
        {
            float t = Mathf.InverseLerp(damageScaling.x, damageScaling.y, value);
            t = Mathf.Pow(t, sizeScalingExponent);
            return Mathf.Lerp(sizeScaling.x, sizeScaling.y, t);
        }
    }
}
