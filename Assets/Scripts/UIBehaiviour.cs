using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIBehaiviour : MonoBehaviour
{
	//public Text timerText;
	public float timer;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI timerProjectorText;

    public Slider hpBar;
	public int hpMax;
    bool hasStarted = false;

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
               
                timerText.text = "" + Mathf.Floor(timer);
                timerProjectorText.text = "" + Mathf.Floor(timer);

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
        }
        else
        {
            timerText.text = "" + 236;
            timer = 255;
        }
    }
    
}
