using System.Collections.Generic;
using UnityEngine;

namespace Balltallion.Simulation.Projectiles
{
    public class SpriteRandomizer : MonoBehaviour
    {
        [SerializeField] private List<Sprite> sprites;

        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            spriteRenderer.sprite = sprites[Random.Range(0, sprites.Count)];
        }
    }
}