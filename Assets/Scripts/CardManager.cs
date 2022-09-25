using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;
public class CardManager : MonoBehaviour
{
    public static CardManager Inst { get; private set; } //�̱������� ����
    void Awake() => Inst = this;

    [SerializeField]
    ItemSO itemSO;
    [SerializeField]
    GameObject cardPrefab; //ī���� �������� �־��ش�.
    [SerializeField]
    List<Card> myCards; //���� �ڽ��� ī�� ����Ʈ
    [SerializeField] 
    List<Card> enemyCards; //������ ī�� ����Ʈ
    [SerializeField]
    Transform cardSpawnPoint; //ī�� ���� ��ġ
    [SerializeField]
    Transform enemyCardSpawnPoint; //������ ī�� ���� ��ġ
    [SerializeField]
    Transform myCardLeft; //�÷��̾��� �� ���� ���� ��ġ
    [SerializeField]
    Transform myCardRight; //�÷��̾��� �� ���� ������ ��ġ
    [SerializeField]
    Transform enemyCardLeft; //���� �� ���� ���� ��ġ
    [SerializeField]
    Transform enemyCardRight; //���� �� ���� ������ ��ġ
    [SerializeField]
    CardState cardstate;

    List<Item> itemBuffer;
    Card selectCard;
    bool isMyCardDrag;
    bool onMyCardArea;
    int myPutCount;
    enum CardState { Nothing, CanMouseOver, CanMouseDrag}

    public Item PopItem() //ī�带 �̴°��� ť�� ���� ����
    {
        if (itemBuffer.Count == 0)
            SetupItemBuffer();

        Item item = itemBuffer[0];
        itemBuffer.RemoveAt(0);
        return item;
    }

    void SetupItemBuffer()
    {
        itemBuffer = new List<Item>(100);
        for (int i = 0; i < itemSO.items.Length; i++)
        {
            Item item = itemSO.items[i]; //itemSO �� �迭�� �� ũ�⸸ŭ �����´�
            for (int j = 0; j < item.percent; j++) //���� �迭�� �ۼ�Ʈ(����Ȯ��)
                itemBuffer.Add(item); //�ۼ�Ʈ ��ŭ ����Ʈ�� ä���ش�.
        }

        for(int i = 0; i < itemBuffer.Count; i++) //����ī�尡 ���������� ���� �ȵǴ� 
        {
            int rand = Random.Range(i, itemBuffer.Count); //�������� ���ؼ� �ٸ��� ���� �Ѵ�.
            Item temp = itemBuffer[i];
            itemBuffer[i] = itemBuffer[rand];
            itemBuffer[rand] = temp;
        }
    }

    void Start()
    {
        SetupItemBuffer();
        TurnManger.OnAddCard += AddCard;
        TurnManger.OnTurnStarted += OnTurnStarted;
    }

    void OnDestroy()
    {
        TurnManger.OnAddCard -= AddCard;
        TurnManger.OnTurnStarted -= OnTurnStarted;
    }

    void OnTurnStarted(bool myTurn)
    {
        if (myTurn)
            myPutCount = 0;
    }

    void Update()
    {
        if (isMyCardDrag)
            CardDrag();

        DetectCardArea();
        SetCardState();
    }

    void CardDrag()
    {
        if (cardstate != CardState.CanMouseDrag)
            return;

        if(!onMyCardArea)
        {
            selectCard.MoveTransform(new PRS(Utils.MousePos, Utils.QI, selectCard.originPRS.scale), false);
            EntityManager.Inst.InsertMyEmptyEntity(Utils.MousePos.x);
        }
    }

    void DetectCardArea()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(Utils.MousePos, Vector3.forward);

        int layer = LayerMask.NameToLayer("MyCardArea");
        onMyCardArea = Array.Exists(hits, x => x.collider.gameObject.layer == layer); //����ĳ��Ʈ�� ���Ͽ� �浹�� ���̾��� �̸��� ������ �̸��� ���ٸ� �浹 
    }

    void AddCard(bool isMine)
    {
        var cardObject = Instantiate(cardPrefab, cardSpawnPoint.position, Utils.QI);
        var card = cardObject.GetComponent<Card>();
        card.Setup(PopItem(), isMine);
        (isMine ? myCards : enemyCards).Add(card);

        SetOriginOrder(isMine);
        CardAlignment(isMine);
    }

    void SetOriginOrder(bool isMine)
    {
        int Count = isMine ? myCards.Count : enemyCards.Count;
        for(int i = 0; i < Count; i++)
        {
            var targetCard = isMine ? myCards[i] : enemyCards[i];
            targetCard?.GetComponent<Order>().SetOriginOrder(i);
        }
    }

    void CardAlignment(bool isMine)
    {
        List<PRS> originCardPRSs = new List<PRS>();
        if (isMine)
            originCardPRSs = RoundAlignment(myCardLeft, myCardRight, myCards.Count, 0.5f, Vector3.one * 10f);
        else
            originCardPRSs = RoundAlignment(enemyCardLeft, enemyCardRight, enemyCards.Count, -0.5f, Vector3.one * 10f);

        var targetCards = isMine ? myCards : enemyCards;
        for(int i = 0; i < targetCards.Count; i++)
        {
            var targetCard = targetCards[i];

            targetCard.originPRS = originCardPRSs[i];
            targetCard.MoveTransform(targetCard.originPRS, true, 0.7f);
        }
    }

    List<PRS> RoundAlignment(Transform leftTr, Transform rightTr, int objCount, float height, Vector3 scale)
    {
        float[] objLerps = new float[objCount]; //ī����� ��ġ�� ��� ���� ����
        List<PRS> results = new List<PRS>(objCount);

        switch(objCount)
        {
            case 1:
                objLerps = new float[] { 0.5f }; //ī���� ��ġ ��ǥ���� ����
                break;
            case 2:
                objLerps = new float[] { 0.27f, 0.73f };
                break;
            case 3:
                objLerps = new float[] { 0.1f, 0.5f, 0.9f };
                break;
            default:
                float interval = 1f / (objCount - 1);
                for (int i = 0; i < objCount; i++)
                    objLerps[i] = interval * i;
                break;
        }

        for(int i = 0; i < objCount; i++)
        {
            var targetPos = Vector3.Lerp(leftTr.position, rightTr.position, objLerps[i]);
            var targetRot = Utils.QI;
            if(objCount >= 4)
            {
                float curve = Mathf.Sqrt(Mathf.Pow(height, 2) - Mathf.Pow(objLerps[i] - 0.5f, 2));
                curve = height >= 0 ? curve : -curve;
                targetPos.y += curve;
                targetRot = Quaternion.Slerp(leftTr.rotation, rightTr.rotation, objLerps[i]);
            }
            results.Add(new PRS(targetPos, targetRot, scale));
        }
        return results;
    }

    public bool TryPutCard(bool isMine)
    {
        if (isMine && myPutCount >= 1)
            return false;

        if (!isMine && enemyCards.Count <= 0)
            return false;

        Card card = isMine ? selectCard : enemyCards[Random.Range(0, enemyCards.Count)];
        var spawnPos = isMine ? Utils.MousePos : enemyCardSpawnPoint.position;
        var targetCards = isMine ? myCards : enemyCards;

        if (EntityManager.Inst.SpawnEntity(isMine, card.item, spawnPos))
        {
            targetCards.Remove(card);
            card.transform.DOKill();
            DestroyImmediate(card.gameObject);
            if(isMine)
            {
                selectCard = null;
                myPutCount++;
            }

            CardAlignment(isMine);
            return true;
        }
        else
        {
            targetCards.ForEach(x => x.GetComponent<Order>().SetMostFrontOrder(false));
            CardAlignment(isMine);
            return false;
        }
    }

    #region MyCard

    public void CardMouseOver(Card card)
    {
        if (cardstate == CardState.Nothing)
            return;

        selectCard = card;
        EnlargeCard(true, card);
    }

    public void CardMouseExit(Card card)
    {
        EnlargeCard(false, card);
    }

    public void CardMouseDown()
    {
        if (cardstate != CardState.CanMouseDrag)
            return;

        isMyCardDrag = true;
    }

    public void CardMouseUp()
    {
        isMyCardDrag = false;

        if (cardstate != CardState.CanMouseDrag)
            return;

        if (onMyCardArea)
            EntityManager.Inst.RemoveMyEmptyEntity();
        else
            TryPutCard(true);
    }

    void EnlargeCard(bool isEnlarge, Card card) //���콺�� �ö󰡸� ī�� Ȯ�� �Լ�
    {
        if (isEnlarge)
        {
            Vector3 enlargepos = new Vector3(card.originPRS.pos.x, -1f, -10f);
            card.MoveTransform(new PRS(enlargepos, Utils.QI, Vector3.one * 30f), false);
        }
        else
            card.MoveTransform(card.originPRS, false);

        card.GetComponent<Order>().SetMostFrontOrder(isEnlarge);
    }

    private void SetCardState()
    {
        if (TurnManger.Inst.isLoading) //���� ���� x����
            cardstate = CardState.Nothing;

        else if (!TurnManger.Inst.myTurn || myPutCount == 1 || EntityManager.Inst.isFullMyEntities) //������ �ƴҶ� ī�� Ȯ�븸 ����
            cardstate = CardState.CanMouseOver;

        else if (TurnManger.Inst.myTurn && myPutCount == 0) //������ �� ī�� �巡�� Ȱ��ȭ
            cardstate = CardState.CanMouseDrag;
    }

    #endregion
}
