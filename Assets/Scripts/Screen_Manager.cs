using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class Screen_Manager : MonoBehaviour {
    [SerializeField] Slider gameMode;

	//Scenes 
	[SerializeField] GameObject[] scenes = new GameObject[5];
	[SerializeField] GameObject mainScreen;

	private void Start(){
	
		mainScreen.SetActive (true);
		for (int i = 0; i < scenes.Length; i++) {
			scenes [i].SetActive (false);		
		}
		scenes [0].SetActive (true);

	
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
