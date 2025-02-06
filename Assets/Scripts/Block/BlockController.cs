using UnityEngine;

namespace Block
{
    public class BlockController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        public void SetBlockAlpha(float alpha)
        {
            Color c = spriteRenderer.color;
            c.a = alpha;
            spriteRenderer.color = c;
        }
    }
}