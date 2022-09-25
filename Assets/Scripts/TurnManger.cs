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
    [Tooltip("���� �� ��带 ���մϴ�")]
    ETurnMode eTurnMode;

    [SerializeField]
    [Tooltip("ī�� ��� �ӵ��� �������ϴ�")]
    bool fastMode;

    [SerializeField]
    [Tooltip("���� ī�� ������ ���մϴ�")]
    int startCardCount;

    [Header("Properties")]
    public bool isLoading; //���� ������ isloading�� true�� �Ͽ��� Ŭ�� ����
    public bool myTurn;

    enum ETurnMode { Random, My, Enemy}
    WaitForSeconds delaySec05 = new WaitForSeconds(0.5f);
    WaitForSeconds delaySec07 = new WaitForSeconds(0.7f);

    public static Action<bool> OnAddCard;
    public static event Action<bool> OnTurnStarted;

    void GameSetUp() //���� ���ϴ� �Լ�
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
            GameManager.Inst.Notification("���� ��");

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
