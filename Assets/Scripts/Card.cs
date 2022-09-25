using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Card : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer card;
    [SerializeField]
    SpriteRenderer character;
    [SerializeField]
    TMP_Text nameTMP; //ī���� �̸� �ؽ�Ʈ
    [SerializeField]
    TMP_Text attackTMP; //ī���� atk �ؽ�Ʈ
    [SerializeField]
    TMP_Text healthTMP; //ī���� HP �ؽ�Ʈ
    [SerializeField]
    Sprite cardFront; //�÷��̾ ������ �ڽ��� ī��� �ո�
    [SerializeField]
    Sprite cardBack; //�÷��̾ ������ ����� ī���� �޸�

    public Item item; //�������� ������ �޾ƿ��� ���� �ۺ����� ����
    bool isFront; //���õ� ī�尡 �ո����� �޸������� �˱����� �Ұ�
    public PRS originPRS;

    public void Setup(Item item, bool isFront)
    {
        this.item = item;
        this.isFront = isFront;

        if(this.isFront)
        {
            character.sprite = this.item.sprite; //������ �־��ش�
            nameTMP.text = this.item.name; //�̸��� �־��ش�.
            attackTMP.text = this.item.attack.ToString(); //ī���� ���ݷ��� �־��ش�
            healthTMP.text = this.item.health.ToString(); //ī���� hp�� �־��ش�.
        }
        else
        {
            card.sprite = cardBack;
            nameTMP.text = "";
            attackTMP.text = "";
            healthTMP.text = "";
        }
    }

    void OnMouseOver()
    {
        if (isFront)
            CardManager.Inst.CardMouseOver(this);
    }

    void OnMouseExit()
    {
        if (isFront)
            CardManager.Inst.CardMouseExit(this);
    }

    void OnMouseDown()
    {
        if (isFront)
            CardManager.Inst.CardMouseDown();
    }

    void OnMouseUp()
    {
        if (isFront)
            CardManager.Inst.CardMouseUp();
    }

    //��Ʈ���� ����Ͽ� ī���̵��� ���� �Լ�
    public void MoveTransform(PRS prs, bool useDotwenn, float dotweenTime = 0)
    {
        if(useDotwenn)
        {
            transform.DOMove(prs.pos, dotweenTime);
            transform.DORotateQuaternion(prs.rot, dotweenTime);
            transform.DOScale(prs.scale, dotweenTime);
        }
        else
        {
            transform.position = prs.pos;
            transform.rotation = prs.rot;
            transform.localScale = prs.scale;
        }
    }
}
