using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GalleryManager : MonoBehaviour
{
    [SerializeField] Text myText;
    [SerializeField] Texture2D defaultTexture;

    [SerializeField] Dropdown dropdownP1, dropdownP2;

    [SerializeField] RawImage imageP1, imageP2;

    [SerializeField] float minScale = 0.3f;

    // Use this for initialization
    void Start()
    {
        //pickImage(512);
        //NativeGallery.GetImageFromGallery();
        myText.text = "test";
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void getGalleryImageP1(RawImage image)
    {
        getGalleryImage(image, true);
    }

    public void getGalleryImageP2(RawImage image)
    {
        getGalleryImage(image, false);
    }

    void getGalleryImage(RawImage image, bool isP1)
    {
        pickImage(512, image, isP1);
    }

    public void findImageforProfile(int index, bool isP1)
    {
        string s = "2020profilePic" + index;
        Texture2D tex;
        if (PlayerPrefs.GetString(s) != null && NativeGallery.LoadImageAtPath(PlayerPrefs.GetString(s), 512) != null)
        {
            tex = NativeGallery.LoadImageAtPath(PlayerPrefs.GetString(s), 512);
        }
        else
        {
            tex = defaultTexture;
        }

        if (isP1)
        {
            imageP1.texture = tex;
        }
        else
        {
            imageP2.texture = tex;
        }

    }



    public void setImageForProfile()
    {

    }

    public void changeScaleP1(float sliderV)
    {
        float scale = minScale + ((1.0f - minScale) * sliderV);
        imageP1.transform.localScale *= scale;
    }

    public void changeScaleP2(float sliderV)
    {
        float scale = minScale + ((1.0f - minScale) * sliderV);
        imageP2.transform.localScale *= scale;
    }



    void pickImage(int maxSize, RawImage image, bool isP1)
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            Debug.Log("Image path: " + path);
            if (path != null)
            {
                myText.text = "Path found";

                // Create Texture from selected image
                Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }
                myText.text = "Texture found";

                //new
                //image.material.mainTexture = texture;
                image.texture = texture;

                image.SetNativeSize();

                int i;
                // SAVE THE PATH TO PLAYERPREFS for profile
                if (isP1)
                {
                    i = dropdownP1.value;
                }
                else
                {
                    i = dropdownP2.value;
                }
                string s = "2020profilePic" + i;
                PlayerPrefs.SetString(s, path);




                // Assign texture to a temporary quad and destroy it after 5 seconds
                //GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                //quad.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2.5f;
                //quad.transform.forward = Camera.main.transform.forward;
                //quad.transform.localScale = new Vector3(1f, texture.height / (float)texture.width, 1f);

                //Material material = quad.GetComponent<Renderer>().material;
                //if (!material.shader.isSupported) // happens when Standard shader is not included in the build
                //    material.shader = Shader.Find("Legacy Shaders/Diffuse");

                //new
                // material.mainTexture = texture;



                //Destroy(quad, 5f);

                // If a procedural texture is not destroyed manually, 
                // it will only be freed after a scene change
                //Destroy(texture, 5f);
            }
        }, "Select a PNG image", "image/png", maxSize);

        Debug.Log("Permission result: " + permission);
    }
}