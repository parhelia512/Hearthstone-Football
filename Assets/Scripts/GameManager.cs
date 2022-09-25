using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Inst { get; private set; }
    void Awake() => Inst = this;

    WaitForSeconds delay2 = new WaitForSeconds(2);

    [Multiline(10)]
    [SerializeField]
    string cheatInfo;
    [SerializeField]
    NotificationPanel notificationPanel;
    [SerializeField]
    ResutPanel resultpanel;
    [SerializeField]
    GameObject endTurnBtn;
    [SerializeField]
    GameStartPanel gamestartpanel;
    [SerializeField]
    CameraEffect cameraeffect;

    void Start()
    {
        UISetup();    
    }

    void UISetup()
    {
        notificationPanel.ScaleZero();
        resultpanel.ScaleZero();
        gamestartpanel.Active(true);
        cameraeffect.SetGrayScale(false);
    }

    void Update()
    {
#if UNITY_EDITOR
        InputCheatKey();
#endif
    }

    void InputCheatKey()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
            TurnManger.OnAddCard?.Invoke(true);

        if (Input.GetKeyDown(KeyCode.Keypad2))
            TurnManger.OnAddCard?.Invoke(false);

        if (Input.GetKeyDown(KeyCode.Keypad3))
            TurnManger.Inst.EndTurn();

        if (Input.GetKeyDown(KeyCode.Keypad4))
            CardManager.Inst.TryPutCard(false);

        if (Input.GetKeyDown(KeyCode.Keypad5))
            EntityManager.Inst.DamageBoss(true, 19);

        if (Input.GetKeyDown(KeyCode.Keypad6))
            EntityManager.Inst.DamageBoss(false, 19);
    }

    public void StartGame()
    {
        StartCoroutine(TurnManger.Inst.StartGameCo());
    }

    public void Notification(string message)
    {
        notificationPanel.Show(message);
    }

    public IEnumerator GameOver(bool isMyWin)
    {
        TurnManger.Inst.isLoading = true;
        endTurnBtn.SetActive(false);
        yield return delay2;

        TurnManger.Inst.isLoading = true;
        resultpanel.Show(isMyWin ? "½Â¸®" : "ÆÐ¹è");
        cameraeffect.SetGrayScale(true);
    }
}
