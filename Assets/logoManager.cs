using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class logoManager : MonoBehaviour {
    [SerializeField] RawImage image;
    [SerializeField] RawImage imageInFightScene;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void getImage()
    {

    }

    void pickImage(int maxSize, RawImage image, bool isP1)
    {

        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            Debug.Log("Image path: " + path);
            if (path != null)
            {
                // Create Texture from selected image
                Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }

                //new
                //image.material.mainTexture = texture;
                image.texture = texture;
                imageInFightScene.texture = texture;

                //TextMesh.game
                //image.SetNativeSize();

                // SAVE tHE PAth
                PlayerPrefs.SetString("2020gymPic", path);
            }
        }, "Select a PNG image", "image/png", maxSize);

        Debug.Log("Permission result: " + permission);
    }
}
