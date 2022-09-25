using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Entity : MonoBehaviour
{
    [SerializeField]
    Item item;
    [SerializeField]
    SpriteRenderer entity;
    [SerializeField]
    SpriteRenderer character;
    [SerializeField]
    TMP_Text nameTMP; //ī���� �̸� �ؽ�Ʈ
    [SerializeField]
    TMP_Text attackTMP; //ī���� atk �ؽ�Ʈ
    [SerializeField]
    TMP_Text healthTMP; //ī���� HP �ؽ�Ʈ
    [SerializeField]
    GameObject sleepParticle;

    public int attack;
    public int health;
    public bool isMine;
    public bool isDie;
    public bool isBossOrEmpty;
    public bool attackable;
    public Vector3 originPos;
    int liveCount;

    public void SetUp(Item item)
    {
        attack = item.attack;
        health = item.health;

        this.item = item;
        character.sprite = this.item.sprite;
        nameTMP.text = this.item.name;
        attackTMP.text = attack.ToString();
        healthTMP.text = health.ToString();
    }

    void OnMouseDown()
    {
        if (isMine)
            EntityManager.Inst.EntityMouseDown(this);
    }

    void OnMouseUp()
    {
        if (isMine)
            EntityManager.Inst.EntityMouseUp();
    }

    void OnMouseDrag()
    {
        if (isMine)
            EntityManager.Inst.EntityMouseDrag();
    }

    public bool Damaged(int damage)
    {
        health -= damage;
        healthTMP.text = health.ToString();

        if(health <= 0)
        {
            isDie = true;
            return true;
        }
        return false;
    }

    public void MoveTransform(Vector3 pos, bool useDotween, float dotweenTime = 0)
    {
        if (useDotween)
            transform.DOMove(pos, dotweenTime);
        else
            transform.position = pos;
    }

    void Start()
    {
        TurnManger.OnTurnStarted += OnTurnStarted;
    }

    void OnDestroy()
    {
        TurnManger.OnTurnStarted -= OnTurnStarted;
    }

    void OnTurnStarted(bool myTurn)
    {
        if (isBossOrEmpty)
            return;

        if (isMine == myTurn)
            liveCount++;

        sleepParticle.SetActive(liveCount < 1);
    }
}
