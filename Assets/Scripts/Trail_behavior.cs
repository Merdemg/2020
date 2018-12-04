using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Trail_behavior : MonoBehaviour {
	[SerializeField] Slider hpBar;
	[SerializeField] Slider trailBar;
	[SerializeField] int trailSpeed;


	private void Start(){
		trailBar.maxValue = hpBar.maxValue;
		trailBar.value = hpBar.value;
	}

	private void Update(){
		if (trailBar.value > hpBar.value) {		
			trailBar.value -= Time.deltaTime * trailSpeed;

		} else if (trailBar.value < hpBar.value) {
			trailBar.value = hpBar.value;
		}
	}
}
