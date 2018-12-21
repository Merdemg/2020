using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
public class UIBehaiviour : MonoBehaviour
{
	//public Text timerText;
	public float timer;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI timerProjectorText;

    public Slider hpBar;
	public int hpMax;
    bool hasStarted = false;
    DateTime startTime;
    float elapsedTime;
    float gameTime;

	void Start ()
	{
		if (timer == 0) {
			timer = 60;
		}
		
		if (hpMax == 0) {
			hpMax = 10;
		}
		hpBar.maxValue = hpMax;
		hpBar.value = hpBar.maxValue;
	}

	void Update ()
	{

        if (hasStarted)
        {
            if (timer < 0)
            {
                
                timerText.text = "" + 0;
            }
            if (timer < 255)
            {

                timer = timer - Time.deltaTime;

                TimeSpan timeDiff = DateTime.Now - startTime;
                double diff = timeDiff.TotalMilliseconds;
                float fDiff = (float)diff;
                
                //timerText.text = "" + Mathf.Floor(timer);
                timerText.text =    Mathf.Floor(gameTime - (fDiff/1000)).ToString();
                timerProjectorText.text = Mathf.Floor(gameTime - (fDiff / 1000)).ToString();

            }
        }
		
	}
    
    public void startTimer()
    {
        hasStarted = true;
    
    }

    public void stopTimer()
    {
        hasStarted = false; 
    }

    public void resetTimer(float time)
    {
        if (time != 0xff)
        {
            timer = time;
            timer = timer - Time.deltaTime;
            timerText.text = "" + Mathf.Floor(timer);
            gameTime = time;
            startTime = DateTime.Now;
        }
        else
        {
            timerText.text = "";
            timerProjectorText.text = "";
            timer = 255;
        }
    }
    
}
