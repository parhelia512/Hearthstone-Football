using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndTurnButton : MonoBehaviour
{
    [SerializeField]
    Sprite active;
    [SerializeField]
    Sprite unactive;
    [SerializeField]
    Text btnText;

    void Start()
    {
        SetUp(false);
        TurnManger.OnTurnStarted += SetUp;
    }

    void OnDestroy()
    {
        TurnManger.OnTurnStarted -= SetUp;
    }
    public void SetUp(bool isActive)
    {
        GetComponent<Image>().sprite = isActive ? active : unactive;
        GetComponent<Button>().interactable = isActive;
        btnText.color = isActive ? new Color32(255, 195, 90, 255) : new Color32(55, 55, 55, 255);
    }
}
