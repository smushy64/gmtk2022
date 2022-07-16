using UnityEngine;
using TMPro;

public class TestGunData : MonoBehaviour
{
    TMP_Text txt;
    // Start is called before the first frame update
    void Start()
    {
        txt = GetComponent<TMP_Text>();
        txt.text = new GunData().ToString();
    }

    float timer = 0f;
    private void Update()
    {
        timer += Time.deltaTime;
        if( timer > 3f ) {
            txt.text = new GunData().ToString();
            timer = 0f;
        }
    }
}
