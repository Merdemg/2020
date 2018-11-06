using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBehaiviour : MonoBehaviour
{
	public Text timerText;
	public float timer;
	public Text timerShadow;


	public Slider hpBar;
	public int hpMax;

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
		
		if (timer < 0){
			timerShadow.text = "" + 0;
			timerText.text = "" + 0;
		}
		if (timer > 0) {

			timer = timer - Time.deltaTime;
			timerShadow.text = "" + Mathf.Floor (timer);
			timerText.text = "" + Mathf.Floor (timer);

		} else {
		
			return;
		}
	}



    public void setP1health()
    {

    }
}
