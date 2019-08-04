using UnityEngine;
using System;
using _scripts.PixelSpriteGenerator;

namespace _scripts.PixelSpriteConsumer
{
    public class PixelSpriteController
    {
        private PsgMask mask;
        private PsgOptions options;
        private float spritePadding;

        
        public int[] templateData;
        public int width = 8;
        public int height = 8;
        public bool mirrorX;
        public bool mirrorY;

        public Sprite GetSprite(int seed)
        {
            mask = new PsgMask (new int[] {
                0, 0, 0, 0, 0, 0,
                0, 1, 1, 0, 1, 1,
                1, 1, 1, 0, 1,-1,
                1, 1, 0, 1, 1,-1,
                0, 1, 0, 1, 1, 2,
                0, 0, 1, 1, 1, 2,
                0, 1, 1, 1, 2, 2,
                0, 1, 1, 1, 2, 2,
                0, 1, 1, 1, 2,-1,
                0, 1, 1, 1, 1,-1,
                0, 0, 0, 1, 1, 1,
                0, 0, 0, 0, 0, 0
            }, 6, 12, true, false);
            
            spritePadding = 1f;

            options = new PsgOptions () {
                Colored = true,
                EdgeBrightness = 0.3f,
                ColorVariations = 0.2f,
                BrightnessNoise = 0.3f,
                Saturation = 0.5f,
                RNGSeed = seed
            };
            
            return GenerateSprite();
        }

        private Sprite GenerateSprite()
        {
            var psgSprite = new PsgSprite (mask, options);

            if (mask.mirrorX) {
                width = mask.width * 2;

            } else {
                width = mask.width;
            }

            if (mask.mirrorY) {
                height = mask.height * 2;
            } else {
                height = mask.height;
            }

            mirrorX = mask.mirrorX;
            mirrorY = mask.mirrorY;

            var tex = psgSprite.texture;
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Point;
            
            var sprite = Sprite.Create(tex, new Rect(0, 0, (float)width, (float)height), new Vector2(0.5f, 0.5f), 32f);
            
            return sprite;
        }
    }
}