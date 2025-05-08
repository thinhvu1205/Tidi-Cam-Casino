using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ImageManager
{
    static ImageManager _Instance;
    string _basePath;

    public static ImageManager Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = new ImageManager();
                _Instance._basePath = Application.persistentDataPath + "/Images/";
                if (!Directory.Exists(_Instance._basePath))
                {
                    Directory.CreateDirectory(_Instance._basePath);
                }
            }
            return _Instance;
        }
    }


    public bool ImageExists(string name)
    {
        return File.Exists(_basePath + name);
    }

    public void SaveImage(string name, byte[] bytes)
    {
        Debug.Log("SaveImage:" + (_basePath + name));
        File.WriteAllBytes(_basePath + name, bytes);
    }

    public Texture2D LoadTexture2D(string name)
    {
        Texture2D texture = null;
        if (ImageExists(name))
        {
            texture = BytesToTexture2D(LoadImage(name));
        }
        return texture;
    }

    public Sprite LoadSprite(string name)
    {
        Sprite sprite = null;
        if (ImageExists(name))
        {
            sprite = BytesToSprite(LoadImage(name));
        }
        return sprite;
    }

    byte[] LoadImage(string name)
    {
        byte[] bytes = new byte[0];
        if (ImageExists(name))
            bytes = File.ReadAllBytes(_basePath + name);
        return bytes;
    }

    Texture2D BytesToTexture2D(byte[] bytes)
    {
        Texture2D texture = new Texture2D(120, 120);
        texture.LoadImage(bytes);

        return texture;
    }

    Sprite BytesToSprite(byte[] bytes)
    {
        Texture2D texture = BytesToTexture2D(bytes);

        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

        return sprite;
    }
}
