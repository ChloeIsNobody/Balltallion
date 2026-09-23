using TMPro;
using UnityEngine;

namespace Balltallion
{
    public class ScoreFloaterSpawner : MonoBehaviour
    {
        public static ScoreFloaterSpawner Instance {get; private set;}
        
        [SerializeField] private Transform scoreFloaterPrefab;

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

        public void SpawnScoreFloater(Vector3 spawnPosition, string text, Color color)
        {
            Transform scoreFloater = Instantiate(scoreFloaterPrefab, spawnPosition, Quaternion.identity, transform);
            TextMeshPro textMesh = scoreFloater.GetComponentInChildren<TextMeshPro>();
            textMesh.text = text;
            textMesh.color = color;
            Destroy(scoreFloater.gameObject, 0.833f);
        }
    }
}
