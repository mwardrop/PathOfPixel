using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureColorSwapper
{
    public static Texture2D SwapColors(Texture2D originalTexture, Color oldColor, Color newColor)
    {
        //Create a new Texture2D, which will be the copy.
        Texture2D texture = new Texture2D(originalTexture.width, originalTexture.height);
        //Choose your filtermode and wrapmode here.
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        int y = 0;
        while (y < texture.height)
        {
            int x = 0;
            while (x < texture.width)
            {
                Color currentColor = originalTexture.GetPixel(x, y);
                if(currentColor.r == 1)
                {
                    if(currentColor.a > 0)
                    {
                        Debug.Log("Found sold red and not transparent");
                    }
                }
                if (currentColor == oldColor)
                {
                    
                    texture.SetPixel(x, y, newColor);
                }
                else
                {
                    //This line of code is REQUIRED. Do NOT delete it. This is what copies the image as it was, without any change.
                    texture.SetPixel(x, y, originalTexture.GetPixel(x, y));
                }
                ++x;
            }
            ++y;
        }
        //This finalizes it. If you want to edit it still, do it before you finish with .Apply(). Do NOT expect to edit the image after you have applied. It did NOT work for me to edit it after this function.
        texture.Apply();

        //Return the variable, so you have it to assign to a permanent variable and so you can use it.
        return texture;
    }
}
