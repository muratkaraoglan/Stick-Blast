using UnityEngine;

namespace Grid
{
    public class Node : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        public void SetSpriteColor(Color color)
        {
            spriteRenderer.color = color;
        }
    }
}