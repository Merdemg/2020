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

    [SerializeField] TextMeshProUGUI p1name1, p1name2, p2name1, p2name2;

    // [SerializeField] TextMeshProUGUI p1name, p2name;

    // Text currentText;

    [SerializeField] TMP_InputField p1inputf, p2inputf;

    GalleryManager galleryMan;

    [SerializeField] Text debugText;

    int p1Selection, p2Selection;

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
            names.Add("New Profile 1");
            names.Add("New Profile 2");

            PlayerPrefs.SetString("name0", "New Profile 1");
            PlayerPrefs.SetString("name1", "New Profile 2");
        }

        //names.Add("NEW");
        names.Add("ADD NEW");
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
        p2inputf.text = "";

        p2name1.text = names[index];
        p2name2.text = names[index];
    }

    public void p1dropdownChoice(int index)
    {

        Debug.Log("---------------- names length: " + names.Count);
        Debug.Log(index + " chosen");
        dropdownChoice(index, true);
        p1inputf.text = "";

        p1name1.text = names[index];
        p1name2.text = names[index];
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
        p1inputf.ActivateInputField();
        p1inputf.Select();
        TouchScreenKeyboard.Open(p1inputf.text);
        //p1dropdownChoice(dropdownP1.value);
    }

    public void newNameP2()
    {
        newName(false);
        p2inputf.ActivateInputField();
        p2inputf.Select();
        TouchScreenKeyboard.Open(p2inputf.text);
        //p2dropdownChoice(dropdownP2.value);
    }

    void newName(bool isP1)
    {   // Bring in the text box - ADD LATER
        Debug.Log("new name");

        // new profile>
        names.Remove("ADD NEW");


        string s = "name" + names.Count;
        PlayerPrefs.SetString(s, "New Profile");
        names.Add("New Profile");

        names.Add("ADD NEW");

        dropdownP1.ClearOptions();
        dropdownP1.AddOptions(names);
        dropdownP2.ClearOptions();
        dropdownP2.AddOptions(names);

        if (isP1)
        {
            dropdownP1.value = names.Count - 2;
            galleryMan.getGalleryImageP1();
            //p1inputf.gameObject.SetActive(true);
            //p2inputf.gameObject.SetActive(false);
        }
        else
        {
            dropdownP2.value = names.Count - 2;
            galleryMan.getGalleryImageP2();
            //p1inputf.gameObject.SetActive(false);
            //p2inputf.gameObject.SetActive(true);
        }
    }

    public void P1getName(string name)
    {
        Debug.Log("get name");
        getName(name, true);
        //p1inputf.gameObject.SetActive(false);
        dropdownP1.value = names.Count - 1;
        p1dropdownChoice(dropdownP1.value);
    }

    public void P2getName(string name)
    {
        Debug.Log("get name");
        getName(name, false);
        //p2inputf.gameObject.SetActive(false);
        dropdownP2.value = names.Count - 1;
        p2dropdownChoice(dropdownP2.value);
    }

    void getName(string newName, bool isP1)
    {
        Debug.Log(names.Count + " at getName func");

        string s = "name" + names.Count;
        PlayerPrefs.SetString(s, newName);
        //names.Remove("NEW");

        names.Add(newName);
        //names.Add("NEW");

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

        dropdownP1.ClearOptions();
        dropdownP1.AddOptions(names);
        dropdownP2.ClearOptions();
        dropdownP2.AddOptions(names);


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

        dropdownP1.ClearOptions();
        dropdownP1.AddOptions(names);
        dropdownP2.ClearOptions();
        dropdownP2.AddOptions(names);


        dropdownP2.value = 0;
    }


    public void editP1name(string newName)
    {
        string s = "name" + dropdownP1.value;
        PlayerPrefs.SetString(s, newName);
        names[dropdownP1.value] = newName;

        dropdownP1.ClearOptions();
        dropdownP1.AddOptions(names);
        dropdownP2.ClearOptions();
        dropdownP2.AddOptions(names);

        p1dropdownChoice(dropdownP1.value);
    }

    public void editP2name(string newName)
    {
        string s = "name" + dropdownP2.value;
        PlayerPrefs.SetString(s, newName);
        names[dropdownP2.value] = newName;

        dropdownP1.ClearOptions();
        dropdownP1.AddOptions(names);
        dropdownP2.ClearOptions();
        dropdownP2.AddOptions(names);

        p2dropdownChoice(dropdownP2.value);
    }


    public void saveSelections()
    {
        p1Selection = dropdownP1.value;
        p2Selection = dropdownP2.value;
    }

    public string getP1name()
    {
        return names[p1Selection];
    }

    public string getP2name()
    {
        return names[p2Selection];
    }
}
