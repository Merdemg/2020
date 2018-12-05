using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Animation_behavior : MonoBehaviour {
    [SerializeField] Animator comboAnim;
    [SerializeField] Animator bigHitAnim;

    [SerializeField] TextMeshProUGUI hitCountText;
    [SerializeField] TextMeshProUGUI dmgPercentText;
    RectTransform menuTransform;
    
    public int hitCount;
    public int dmgPercent;
    public float height;

    private void Start()
    {
        menuTransform = GetComponent<RectTransform>();
        height = menuTransform.rect.size.x;
        print(height);
    }

    public void Combo() {
        hitCountText.text = "" + hitCount;
        dmgPercentText.text = "" + dmgPercent;
        comboAnim.SetBool("Combo", true);
    }



    public void BigHit() {
        dmgPercentText.text = "" + dmgPercent;
        bigHitAnim.SetBool("Hit", true);
    }

    public void MenuOpen() {
         
        //menuTransform.localScale.y =
        //menuTransform.rect.size.x = 0;
        //print(height);
    }

    public void MenuClose()
    {
        


    }

}
