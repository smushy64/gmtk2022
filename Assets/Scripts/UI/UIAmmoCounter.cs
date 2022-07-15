using UnityEngine;
using TMPro;

public class UIAmmoCounter : MonoBehaviour
{

    TMP_Text countText, totalText;
    void Awake() {
        countText = transform.Find("Count").GetComponent<TMP_Text>();
        totalText = transform.Find("Total").GetComponent<TMP_Text>();
    }

    public void UpdateCounter( int count, int total ) {
        countText.text = count.ToString("D3");
        totalText.text = total.ToString("D3");
    }

}
