using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GatchaShop : MonoBehaviour
{

    public Text goldText;

    public int gold;

    void Start()
    {
        gold = 750;
        goldText.text = "���� �ݾ� : " + gold + "G";
    }

    void Update()
    {
        goldText.text = "���� �ݾ� : " + gold + "G";
    }

    public void BuyPack()
    {
        gold -= 100;
       // SceneManager.LoadScene()
    }

    public void OutShop()
    {
        SceneManager.LoadScene(0);
    }
}
