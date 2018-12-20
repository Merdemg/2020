using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class logoManager : MonoBehaviour {
    [SerializeField] RawImage image;
    [SerializeField] RawImage imageInFightScene;
    [SerializeField] TextMeshPro websiteText;

	// Use this for initialization
	void Start () {
        if (PlayerPrefs.GetString("2020gymPic") != null)
        {
            Texture2D tex;
            if (NativeGallery.LoadImageAtPath(PlayerPrefs.GetString("2020gymPic"), 512) != null)
            {
                tex = NativeGallery.LoadImageAtPath(PlayerPrefs.GetString("2020gymPic"), 512);
            }
        }

        if (PlayerPrefs.GetString("2020website") != null)
        {
            websiteText.text = PlayerPrefs.GetString("2020website");
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void getImage()
    {
        pickImage(512);
    }

    void pickImage(int maxSize)
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

    public void setWebsite(string str)
    {
        if (str.Length > 30)
        {
            str = str.Substring(0, 30);
        }

        websiteText.text = str;
        PlayerPrefs.SetString("2020website", str);
    }
}
