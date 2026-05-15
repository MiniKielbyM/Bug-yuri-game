using UnityEngine;
using System.Collections.Generic;

public class Breakables : MonoBehaviour
{
    [Range(2, 10)]
    public int shatterDetail = 4; // Number of subdivisions per axis
    public float explosionForce = 5f;
    public float explosionTorque = 5f;
    public float pieceLifetime = 3f;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Shatter()
    {
        if (spriteRenderer.sprite == null)
        {
            Debug.LogError("No sprite assigned to SpriteRenderer.");
            return;
        }

        // Get sprite texture and bounds
        Texture2D texture = spriteRenderer.sprite.texture;
        Rect spriteRect = spriteRenderer.sprite.textureRect;
        Vector2 pivot = spriteRenderer.sprite.pivot / spriteRect.size;

        // Create a readable copy of the texture using RenderTexture for build compatibility
        Texture2D readableTexture = new Texture2D((int)spriteRect.width, (int)spriteRect.height, TextureFormat.RGBA32, false);

        if (texture.isReadable)
        {
            // Extract just the sprite portion
            Color[] spritePixels = texture.GetPixels(
                (int)spriteRect.x,
                (int)spriteRect.y,
                (int)spriteRect.width,
                (int)spriteRect.height
            );
            readableTexture.SetPixels(spritePixels);
            readableTexture.Apply();
        }
        else
        {
            // Use RenderTexture for proper copying in builds
            RenderTexture rt = RenderTexture.GetTemporary(texture.width, texture.height);
            Graphics.Blit(texture, rt);
            RenderTexture.active = rt;

            Texture2D tempTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
            tempTexture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
            tempTexture.Apply();

            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(rt);

            // Extract sprite portion into final readable texture
            Color[] spritePixels = tempTexture.GetPixels(
                (int)spriteRect.x,
                (int)spriteRect.y,
                (int)spriteRect.width,
                (int)spriteRect.height
            );
            readableTexture.SetPixels(spritePixels);
            readableTexture.Apply();

            Destroy(tempTexture);
        }

        // Calculate piece size (now based on extracted sprite, not original texture)
        float pieceWidth = spriteRect.width / shatterDetail;
        float pieceHeight = spriteRect.height / shatterDetail;

        // Create pieces
        for (int x = 0; x < shatterDetail; x++)
        {
            for (int y = 0; y < shatterDetail; y++)
            {
                // Extract piece texture (coordinates now relative to extracted sprite, not full texture)
                Color[] pixels = readableTexture.GetPixels(
                    (int)(x * pieceWidth),
                    (int)(y * pieceHeight),
                    (int)pieceWidth,
                    (int)pieceHeight
                );

                Texture2D pieceTex = new Texture2D((int)pieceWidth, (int)pieceHeight, TextureFormat.RGBA32, false);
                pieceTex.SetPixels(pixels);
                pieceTex.Apply();

                // Create GameObject for piece
                GameObject piece = new GameObject("SpritePiece");
                piece.transform.position = transform.position;
                piece.transform.rotation = transform.rotation;

                SpriteRenderer pieceRenderer = piece.AddComponent<SpriteRenderer>();
                pieceRenderer.sprite = Sprite.Create(
                    pieceTex,
                    new Rect(0, 0, pieceTex.width, pieceTex.height),
                    new Vector2(0.5f, 0.5f),
                    spriteRenderer.sprite.pixelsPerUnit
                );
                pieceRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
                pieceRenderer.sortingOrder = spriteRenderer.sortingOrder;

                // Add physics
                Rigidbody2D rb = piece.AddComponent<Rigidbody2D>();
                rb.gravityScale = 1f;
                rb.AddForce(Random.insideUnitCircle * explosionForce, ForceMode2D.Impulse);
                rb.AddTorque(Random.Range(-explosionTorque, explosionTorque), ForceMode2D.Impulse);

                // Destroy after lifetime
                Destroy(piece, pieceLifetime);
            }
        }

        // Clean up the readable texture copy
        Destroy(readableTexture);

        // Destroy original object
        Destroy(gameObject);
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collided with: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Weapon"))
        {
            Shatter();
        }
    }
}
