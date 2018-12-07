using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class Screen_Manager : MonoBehaviour {
    [SerializeField] Slider gameMode;

	//Scenes 
	[SerializeField] GameObject[] scenes = new GameObject[6];
	[SerializeField] GameObject mainScreen;

    string p1name, p2name;
    Texture2D p1tex, p2tex;

	private void Start(){
	
		mainScreen.SetActive (true);
		for (int i = 0; i < scenes.Length; i++) {
			scenes [i].SetActive (false);		
		}
		scenes [0].SetActive (true);

	
	}

    public void PlayerSelect() {
        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i].SetActive(false);
        }

        mainScreen.SetActive(false);
        scenes[5].SetActive(true);
    }
    
    public void GameMode() {
        if (gameMode.value == 1)
        {
            ProjectorScene();
        }
        else {
            GameScene();

        }
    }

    public void setP2info()
    {
        p2name = FindObjectOfType<MenuManager>().getP2name();
        p2tex = FindObjectOfType<GalleryManager>().getP2tex();
    }

    public void setP1info()
    {
        p2name = FindObjectOfType<MenuManager>().getP1name();
        p2tex = FindObjectOfType<GalleryManager>().getP1tex();
    }

    public string getP1name()
    {
        return p1name;
    }

    public string getP2name()
    {
        return p2name;
    }

    public Texture2D getP1tex()
    {
        return p1tex;
    }

    public Texture2D getP2tex()
    {
        return p2tex;
    }

    public void LogoScreen() {
		for (int i = 0; i < scenes.Length; i++) {
			scenes [i].SetActive (false);		
		}

		scenes [1].SetActive (true);
			
	}

    public void ProjectorScene() {

		for (int i = 0; i < scenes.Length; i++) {
			scenes [i].SetActive (false);		
		}

		mainScreen.SetActive (false);
		scenes [3].SetActive (true);
        
    }

	public void GameScene(){

		for (int i = 0; i < scenes.Length; i++) {
			scenes [i].SetActive (false);		
		}
		mainScreen.SetActive (false);
		scenes [2].SetActive (true);
        
    }
    public void PairingScreen()
    {
		for (int i = 0; i < scenes.Length; i++) {
			scenes [i].SetActive (false);		
		}

		scenes [0].SetActive (true);

        
    }
    public void ConnectingScreen() {
        scenes[4].SetActive(true);

    }



}
