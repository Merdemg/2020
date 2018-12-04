using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GalleryManager : MonoBehaviour
{
    [SerializeField] Text myText;

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

    public void getGalleryImage(RawImage image)
    {
        pickImage(512, image);
    }

    void pickImage(int maxSize, RawImage image)
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