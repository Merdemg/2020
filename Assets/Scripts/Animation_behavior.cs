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

    public int hitCount;
    public int dmgPercent;

    public void Combo() {
        hitCountText.text = "" + hitCount;
        dmgPercentText.text = "" + dmgPercent;
        comboAnim.SetBool("Combo", true);
    }



    public void BigHit() {
        dmgPercentText.text = "" + dmgPercent;
        bigHitAnim.SetBool("Hit", true);
    }


}
