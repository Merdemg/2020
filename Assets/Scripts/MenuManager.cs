using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MenuManager : MonoBehaviour
{
    [SerializeField] TMP_Dropdown dropdownP1, dropdownP2;
    int NAME_LIST_LENGTH = 100;
    [SerializeField]
    List<string> names = new List<string>();

   // [SerializeField] TextMeshProUGUI p1name, p2name;
    
   // Text currentText;

    [SerializeField] TMP_InputField p1inputf, p2inputf;

    GalleryManager galleryMan;

    [SerializeField] Text debugText;

    // Use this for initialization
    void Start()
    {
        galleryMan = GetComponent<GalleryManager>();

       // currentText = p1name;
        populateList();
        Debug.Log("names length: " + names.Count);

        p1dropdownChoice(dropdownP1.value);
        p2dropdownChoice(dropdownP2.value);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log( names.Count.ToString());
    //    Debug.Log("names length: " + names.Count);
    }

    void populateList()
    {
        for (int i = 0; i < NAME_LIST_LENGTH; i++)
        {
            string s = "name" + i;
            if (PlayerPrefs.GetString(s) != null && PlayerPrefs.GetString(s) != "")
            {
                names.Add(PlayerPrefs.GetString(s));
            }
        }

        debugText.text = names.Count + " obj in names.";
        if (names.Count < 1)
        {
            names.Add("Ali Gafour");
            names.Add("Christoph Sonnen");

            PlayerPrefs.SetString("name0", "Ali Gafour");
            PlayerPrefs.SetString("name1", "Christoph Sonnen");
        }

        names.Add("NEW");
        Debug.Log("names length: " + names.Count);
        dropdownP1.AddOptions(names);
        dropdownP2.AddOptions(names);


        // get them pics yo
        //galleryMan.findImageforProfile(dropdownP1.value, true);
        //galleryMan.findImageforProfile(dropdownP2.value, false);

        
    }

    public void p2dropdownChoice(int index)
    {
        Debug.Log(index + " chosen");
        dropdownChoice(index, false);
    }

    public void p1dropdownChoice(int index)
    {

        Debug.Log("---------------- names length: " + names.Count);
        Debug.Log(index + " chosen");
        dropdownChoice(index, true);
    }

    void dropdownChoice(int index, bool isP1)
    {
        Debug.Log("---------------- names length: " + names.Count);
        if (index + 1 >= names.Count)
        {
            newName(isP1);
        }
        else
        {
            // currentText.text = names[index];
            galleryMan.findImageforProfile(index, isP1);
        }
    }

    public void newNameP1()
    {
        newName(true);
    }

    public void newNameP2()
    {
        newName(false);
    }

    void newName(bool isP1)
    {   // Bring in the text box - ADD LATER
        Debug.Log("new name");
        if (isP1)
        {
            p1inputf.gameObject.SetActive(true);
            p2inputf.gameObject.SetActive(false);
        }
        else
        {
            p1inputf.gameObject.SetActive(false);
            p2inputf.gameObject.SetActive(true);
        }
    }

    public void P1getName(string name)
    {
        Debug.Log("get name");
        getName(name, true);
        p1inputf.gameObject.SetActive(false);
        dropdownP1.value = names.Count -2;
        p1dropdownChoice(dropdownP1.value);
    }

    public void P2getName(string name)
    {
        Debug.Log("get name");
        getName(name, false);
        p2inputf.gameObject.SetActive(false);
        dropdownP2.value = names.Count -2;
        p2dropdownChoice(dropdownP2.value);
    }

    void getName(string newName, bool isP1)
    {
        Debug.Log(names.Count + " at getName func");

        string s = "name" + names.Count;
        PlayerPrefs.SetString(s, newName);
        names.Remove("NEW");

        names.Add(newName);
        names.Add("NEW");

        dropdownP1.ClearOptions();
        dropdownP1.AddOptions(names);
        dropdownP2.ClearOptions();
        dropdownP2.AddOptions(names);

        p1inputf.gameObject.SetActive(false);
        p2inputf.gameObject.SetActive(false);
    }

    public void printNamesSize()
    {
        debugText.text = "Names size: " + names.Count;
    }


    public void deleteP1profile()
    {
        string s = "name" + dropdownP1.value;
        names.Remove(PlayerPrefs.GetString(s));

        for (int i = dropdownP1.value; i < NAME_LIST_LENGTH; i++)
        {
            string s2 = "name" + i;
            if (PlayerPrefs.GetString(s2) != null)
            {
                PlayerPrefs.DeleteKey(s2);
                string s3 = "name" + (i+1);
                if (PlayerPrefs.GetString(s3) != null)
                {
                    PlayerPrefs.SetString(s2, PlayerPrefs.GetString(s3));
                }
            }

            s2 = "2020profilePic" + i;
            if (PlayerPrefs.GetString(s2) != null)
            {
                PlayerPrefs.DeleteKey(s2);
                string s3 = "2020profilePic" + (i + 1);
                if (PlayerPrefs.GetString(s3) != null)
                {
                    PlayerPrefs.SetString(s2, PlayerPrefs.GetString(s3));
                }
            }


        }

        dropdownP1.value = 0;
    }

    public void deleteP2profile()
    {
        string s = "name" + dropdownP2.value;
        names.Remove(PlayerPrefs.GetString(s));

        for (int i = dropdownP2.value; i < NAME_LIST_LENGTH; i++)
        {
            string s2 = "name" + i;
            if (PlayerPrefs.GetString(s2) != null)
            {
                PlayerPrefs.DeleteKey(s2);
                string s3 = "name" + (i + 1);
                if (PlayerPrefs.GetString(s3) != null)
                {
                    PlayerPrefs.SetString(s2, PlayerPrefs.GetString(s3));
                }
            }

            s2 = "2020profilePic" + i;
            if (PlayerPrefs.GetString(s2) != null)
            {
                PlayerPrefs.DeleteKey(s2);
                string s3 = "2020profilePic" + (i + 1);
                if (PlayerPrefs.GetString(s3) != null)
                {
                    PlayerPrefs.SetString(s2, PlayerPrefs.GetString(s3));
                }
            }


        }

        dropdownP2.value = 0;
    }

    
    public void editP1name(string newName)
    {
        string s = "name" + dropdownP1.value;
        PlayerPrefs.SetString(s, newName);

        dropdownP1.ClearOptions();
        dropdownP1.AddOptions(names);
        dropdownP2.ClearOptions();
        dropdownP2.AddOptions(names);
    }

    public void editP2name(string newName)
    {
        string s = "name" + dropdownP2.value;
        PlayerPrefs.SetString(s, newName);

        dropdownP1.ClearOptions();
        dropdownP1.AddOptions(names);
        dropdownP2.ClearOptions();
        dropdownP2.AddOptions(names);
    }

}
