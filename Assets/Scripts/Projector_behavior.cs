using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Projector_behavior : MonoBehaviour {
    [SerializeField] Slider hpBar;
    [SerializeField] Slider projectorBar;



    private void Start() {
        projectorBar.maxValue = hpBar.maxValue;
        projectorBar.value = hpBar.value;
    }

    private void Update()
    {
        projectorBar.value = hpBar.value;


    }
}
