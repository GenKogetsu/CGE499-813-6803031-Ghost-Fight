using UnityEngine;

public static class SpriteFactory
{
    public static Sprite CreateCircle(int res = 64)
    {
        var tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
        var pixels = new Color[res * res];
        float center = res / 2f;
        float radius = center - 1f;

        for (int y = 0; y < res; y++)
        for (int x = 0; x < res; x++)
        {
            float d = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
            pixels[y * res + x] = d <= radius ? Color.white : Color.clear;
        }

        tex.SetPixels(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f), res);
    }
}
