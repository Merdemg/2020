using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {
    [SerializeField] Dropdown dropdown;
    int NAME_LIST_LENGTH;
    List<string> names = new List<string>();

    [SerializeField] Text p1name, p2name;
    Text currentText;


    // Use this for initialization
    void Start () {
        currentText = p1name;
        populateList();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void populateList()
    {
        for (int i = 0; i < NAME_LIST_LENGTH; i++)
        {
            string s = "name" + i;
            if (PlayerPrefs.GetString(s) != null)
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
        dropdown.AddOptions(names);
    }

    public void dropdownChoice(int index)
    {
        if (index > names.Count)
        {
            newName();
        }
        else
        {
            currentText.text = names[index];
        }
    }

    void newName()
    {   // Bring in the text box - ADD LATER

    }

    public void getName(string newName)
    {
        string s = "name" + names.Count;
        PlayerPrefs.SetString(s, newName);
        names.Add(newName);
    }
}
