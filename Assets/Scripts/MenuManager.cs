using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] Dropdown dropdownP1, dropdownP2;
    int NAME_LIST_LENGTH = 5;
    List<string> names = new List<string>();

    [SerializeField] Text p1name, p2name;
    Text currentText;

    [SerializeField] InputField p1inputf, p2inputf;

    // Use this for initialization
    void Start()
    {
        currentText = p1name;
        populateList();
    }

    // Update is called once per frame
    void Update()
    {

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
    }

    public void p2dropdownChoice(int index)
    {
        Debug.Log(index + " chosen");
        dropdownChoice(index, false);
    }

    public void p1dropdownChoice(int index)
    {
        Debug.Log(index + " chosen");
        dropdownChoice(index, true);
    }

    void dropdownChoice(int index, bool isP1)
    {
        if (index >= names.Count -1)
        {
            newName(isP1);
        }
        else
        {
           // currentText.text = names[index];
        }
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
        getName(name, true);
        p1inputf.gameObject.SetActive(false);
        dropdownP1.value = names.Count -1;
    }

    public void P2getName(string name)
    {
        getName(name, false);
        p2inputf.gameObject.SetActive(false);
        dropdownP2.value = names.Count -1;
    }

    void getName(string newName, bool isP1)
    {
        string s = "name" + names.Count;
        PlayerPrefs.SetString(s, newName);
        names.Add(newName);

        dropdownP1.ClearOptions();
        dropdownP1.AddOptions(names);
        dropdownP2.ClearOptions();
        dropdownP2.AddOptions(names);

        p1inputf.gameObject.SetActive(false);
        p2inputf.gameObject.SetActive(false);
    }
}
