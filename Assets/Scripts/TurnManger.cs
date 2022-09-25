using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TurnManger : MonoBehaviour
{
    public static TurnManger Inst { get; private set; }
    void Awake() => Inst = this;

    [Header("Develop")]
    [SerializeField]
    [Tooltip("시작 턴 모드를 정합니다")]
    ETurnMode eTurnMode;

    [SerializeField]
    [Tooltip("카드 배분 속도가 빨라집니다")]
    bool fastMode;

    [SerializeField]
    [Tooltip("시작 카드 개수를 정합니다")]
    int startCardCount;

    [Header("Properties")]
    public bool isLoading; //턴이 끝나면 isloading을 true로 하여서 클릭 방지
    public bool myTurn;

    enum ETurnMode { Random, My, Enemy}
    WaitForSeconds delaySec05 = new WaitForSeconds(0.5f);
    WaitForSeconds delaySec07 = new WaitForSeconds(0.7f);

    public static Action<bool> OnAddCard;
    public static event Action<bool> OnTurnStarted;

    void GameSetUp() //턴을 정하는 함수
    {
        if (fastMode)
            delaySec05 = new WaitForSeconds(0.05f);

        switch(eTurnMode)
        {
            case ETurnMode.Random:
                myTurn = Random.Range(0, 2) == 0;
                break;
            case ETurnMode.My:
                myTurn = true;
                break;
            case ETurnMode.Enemy:
                myTurn = false;
                break;
        }
    }

    public IEnumerator StartGameCo()
    {
        GameSetUp();
        isLoading = true;

        for (int i = 0; i < startCardCount; i++)
        {
            yield return delaySec05;
            OnAddCard?.Invoke(false);
            yield return delaySec05;
            OnAddCard?.Invoke(true);
        }
        StartCoroutine(StartTurnCo());
    }

    IEnumerator StartTurnCo()
    {
        isLoading = true;

        if (myTurn)
            GameManager.Inst.Notification("나의 턴");

        yield return delaySec07;
        OnAddCard?.Invoke(myTurn);
        yield return delaySec07;
        isLoading = false;
        OnTurnStarted?.Invoke(myTurn);
    }

    public void EndTurn()
    {
        myTurn = !myTurn;
        StartCoroutine(StartTurnCo());
    }
}
