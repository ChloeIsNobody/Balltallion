using System.Collections.Generic;
using Balltallion.Simulation.Teams;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Balltallion.Scenes.MainMenu
{
    public class FallingBallSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject fallingBallPrefab;
        [SerializeField] private List<BallDataSO> balls;
        [SerializeField] private Rect spawnRect;
        
        [Space]
        [SerializeField] private float spawnRate = 1.0f;
        [SerializeField] private float ballLifetime = 10.0f;
        [SerializeField, MinMaxSlider(0.0f, 2.0f)] private Vector2 angularVelocity;

        private float spawnTimer;

        private void Update()
        {
            if (spawnTimer > 0.0f) spawnTimer -= Time.deltaTime;
            else
            {
                spawnTimer = 1.0f/spawnRate;
                SpawnFallingBall();
            }
        }

        private void SpawnFallingBall()
        {
            BallDataSO ballData = balls[Random.Range(0, balls.Count-1)];
            
            GameObject fallingBall = Instantiate(fallingBallPrefab,  spawnRect.center, Quaternion.identity, transform);
            
            SpriteRenderer spriteRenderer = fallingBall.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = ballData.ballSprite;
            
            Rigidbody2D rbody = fallingBall.GetComponent<Rigidbody2D>();
            rbody.MovePosition(GetRandomPositionInRect(spawnRect));

            float randomFlip = Random.Range(0, 2) == 0 ? 1 : -1;
            rbody.angularVelocity = randomFlip * Random.Range(angularVelocity.x, angularVelocity.y) * 360.0f;
            
            Destroy(fallingBall, ballLifetime);
        }
        
        private Vector2 GetRandomPositionInRect(Rect rect)
        {
            Vector2 result = Vector2.zero;
            result.x = Random.Range(rect.x - rect.width/2.0f, rect.x + rect.width/2.0f);
            result.y = Random.Range(rect.y - rect.height/2.0f, rect.y + rect.height/2.0f);
            return result;
        }
        
        private void OnDrawGizmosSelected()
        {
            DrawRect(spawnRect, Color.darkOrange);
        }

        private void DrawRect(Rect rect, Color color)
        {
            Gizmos.color = color;
            Vector2 center = new Vector2(rect.x, rect.y);
            Vector2 size = new Vector2(rect.width, rect.height);
            Gizmos.DrawWireCube(center, size);
        }
    }
}