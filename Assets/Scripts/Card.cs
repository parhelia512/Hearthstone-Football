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
    TMP_Text nameTMP; //카드의 이름 텍스트
    [SerializeField]
    TMP_Text attackTMP; //카드의 atk 텍스트
    [SerializeField]
    TMP_Text healthTMP; //카드의 HP 텍스트
    [SerializeField]
    Sprite cardFront; //플레이어가 봤을때 자신의 카드들 앞면
    [SerializeField]
    Sprite cardBack; //플레이어가 봤을때 상대의 카드의 뒷면

    public Item item; //아이템의 정보를 받아오기 위해 퍼블릭으로 구조
    bool isFront; //셋팅될 카드가 앞면인지 뒷면이지를 알기위한 불값
    public PRS originPRS;

    public void Setup(Item item, bool isFront)
    {
        this.item = item;
        this.isFront = isFront;

        if(this.isFront)
        {
            character.sprite = this.item.sprite; //사진을 넣어준다
            nameTMP.text = this.item.name; //이름을 넣어준다.
            attackTMP.text = this.item.attack.ToString(); //카드의 공격력을 넣어준다
            healthTMP.text = this.item.health.ToString(); //카드의 hp를 넣어준다.
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

    //두트윈을 사용하여 카드이동을 위한 함수
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
