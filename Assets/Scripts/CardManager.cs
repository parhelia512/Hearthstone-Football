using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;
public class CardManager : MonoBehaviour
{
    public static CardManager Inst { get; private set; } //싱글톤으로 제작
    void Awake() => Inst = this;

    [SerializeField]
    ItemSO itemSO;
    [SerializeField]
    GameObject cardPrefab; //카드의 프리팹을 넣어준다.
    [SerializeField]
    List<Card> myCards; //현재 자신의 카드 리스트
    [SerializeField] 
    List<Card> enemyCards; //상대방의 카드 리스트
    [SerializeField]
    Transform cardSpawnPoint; //카드 스폰 위치
    [SerializeField]
    Transform enemyCardSpawnPoint; //상대방의 카드 스폰 위치
    [SerializeField]
    Transform myCardLeft; //플레이어의 패 가장 왼쪽 위치
    [SerializeField]
    Transform myCardRight; //플레이어의 패 가장 오른쪽 위치
    [SerializeField]
    Transform enemyCardLeft; //적의 패 가장 왼쪽 위치
    [SerializeField]
    Transform enemyCardRight; //적의 패 가장 오른쪽 위치
    [SerializeField]
    CardState cardstate;

    List<Item> itemBuffer;
    Card selectCard;
    bool isMyCardDrag;
    bool onMyCardArea;
    int myPutCount;
    enum CardState { Nothing, CanMouseOver, CanMouseDrag}

    public Item PopItem() //카드를 뽑는것을 큐와 같이 관리
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
            Item item = itemSO.items[i]; //itemSO 의 배열의 총 크기만큼 가져온다
            for (int j = 0; j < item.percent; j++) //각각 배열에 퍼센트(뽑힐확률)
                itemBuffer.Add(item); //퍼센트 만큼 리스트에 채워준다.
        }

        for(int i = 0; i < itemBuffer.Count; i++) //같은카드가 연속적으로 들어가면 안되니 
        {
            int rand = Random.Range(i, itemBuffer.Count); //랜덤값을 통해서 다르게 들어가게 한다.
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
        onMyCardArea = Array.Exists(hits, x => x.collider.gameObject.layer == layer); //레이캐스트를 통하여 충돌한 레이어의 이름이 설정한 이름과 같다면 충돌 
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
        float[] objLerps = new float[objCount]; //카드들의 위치를 잡기 위한 변수
        List<PRS> results = new List<PRS>(objCount);

        switch(objCount)
        {
            case 1:
                objLerps = new float[] { 0.5f }; //카드의 위치 좌표값을 고정
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

    void EnlargeCard(bool isEnlarge, Card card) //마우스가 올라가면 카드 확대 함수
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
        if (TurnManger.Inst.isLoading) //게임 시작 x상태
            cardstate = CardState.Nothing;

        else if (!TurnManger.Inst.myTurn || myPutCount == 1 || EntityManager.Inst.isFullMyEntities) //내턴이 아닐땐 카드 확대만 가능
            cardstate = CardState.CanMouseOver;

        else if (TurnManger.Inst.myTurn && myPutCount == 0) //내턴일 때 카드 드래그 활성화
            cardstate = CardState.CanMouseDrag;
    }

    #endregion
}
