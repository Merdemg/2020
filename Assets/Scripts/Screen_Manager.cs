using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class Screen_Manager : MonoBehaviour {
    public Slider gameMode;



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
        SceneManager.LoadScene("Logo_Selection", LoadSceneMode.Single);
    }

    public void ProjectorScene() {
        SceneManager.LoadScene("Projector", LoadSceneMode.Single);
    }

	public void GameScene(){
        SceneManager.LoadScene("Game_Scene", LoadSceneMode.Single);
    }
}
