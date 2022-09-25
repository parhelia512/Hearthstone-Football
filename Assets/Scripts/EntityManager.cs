using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EntityManager : MonoBehaviour
{
    public static EntityManager Inst { get; private set; }
    void Awake() => Inst = this;

    [SerializeField]
    GameObject entityPrefab;
    [SerializeField]
    List<Entity> myEntities;
    [SerializeField]
    List<Entity> EnemyEntities;
    [SerializeField]
    GameObject targetPicker;
    [SerializeField]
    GameObject damagePrefab;
    [SerializeField]
    Entity myEmptyEntity;
    [SerializeField]
    Entity myBossEntity;
    [SerializeField]
    Entity enemyEntity;

    const int MAX_ENTITY_COUNT = 6; //최대 소환 카드

    //나의 필드 꽉찼는지 확인하는 불
    public bool isFullMyEntities => myEntities.Count >= MAX_ENTITY_COUNT && !ExistMyEmptyEntity;
    bool isFullEnemyEntities => EnemyEntities.Count >= MAX_ENTITY_COUNT;
    bool ExistTargetPickEntity => targetPickEntity != null;
    bool ExistMyEmptyEntity => myEntities.Exists(x => x == myEmptyEntity);
    int MyEmptyEntityIndex => myEntities.FindIndex(x => x == myEmptyEntity);
    bool CanMouseInput => TurnManger.Inst.myTurn && !TurnManger.Inst.isLoading;

    Entity selectEntity;
    Entity targetPickEntity;
    WaitForSeconds delay1 = new WaitForSeconds(1);
    WaitForSeconds delay2 = new WaitForSeconds(2);

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
        AttackableReset(myTurn);

        if (!myTurn)
            StartCoroutine(AICo());
    }

    void Update()
    {
        ShowTargetPicker(ExistTargetPickEntity);    
    }

    void Attack(Entity attacker, Entity defender)
    {
        //공격자가 디펜더 위치로 이동하고 돌아온다. 그때 레이어위치가 공격자가 더 높다
        attacker.attackable = false;
        attacker.GetComponent<Order>().SetMostFrontOrder(true);

        Sequence sequence = DOTween.Sequence()
            .Append(attacker.transform.DOMove(defender.originPos, 0.4f)).SetEase(Ease.InSine)
            .AppendCallback(() =>
            {
                attacker.Damaged(defender.attack);
                defender.Damaged(attacker.attack);
                SpawnDamage(defender.attack, attacker.transform);
                SpawnDamage(attacker.attack, defender.transform);
            })
            .Append(attacker.transform.DOMove(attacker.originPos, 0.4f)).SetEase(Ease.OutSine)
            .OnComplete(() => AttackCallBack(attacker, defender)); //죽음처리
    }

    void AttackCallBack(params Entity[] entities)
    {
        entities[0].GetComponent<Order>().SetMostFrontOrder(false);

        foreach(var entity in entities)
        {
            if (!entity.isDie || entity.isBossOrEmpty)
                continue;

            if (entity.isMine)
                myEntities.Remove(entity);
            else
                EnemyEntities.Remove(entity);

            Sequence sequence = DOTween.Sequence()
                .Append(entity.transform.DOShakePosition(1.3f))
                .Append(entity.transform.DOScale(Vector3.zero, 0.3f)).SetEase(Ease.OutCirc)
                .OnComplete(() =>
                {
                    EntityAlignment(entity.isMine);
                    Destroy(entity.gameObject);
                });
        }
        StartCoroutine(ChaeckBossDie());
    }

    IEnumerator ChaeckBossDie()
    {
        yield return delay2;

        if (myBossEntity.isDie)
            StartCoroutine(GameManager.Inst.GameOver(false));

        if (enemyEntity.isDie)
            StartCoroutine(GameManager.Inst.GameOver(true));
    }

    public void DamageBoss(bool isMine, int damage)
    {
        var targetBossEntity = isMine ? myBossEntity : enemyEntity;
        targetBossEntity.Damaged(damage);
        StartCoroutine(ChaeckBossDie());
    }

    void ShowTargetPicker(bool isShow)
    {
        targetPicker.SetActive(isShow);
        if (ExistTargetPickEntity)
            targetPicker.transform.position = targetPickEntity.transform.position;
    }

    IEnumerator AICo()
    {
        CardManager.Inst.TryPutCard(false);
        yield return delay1;

        //attackable 이 true 인 모든 enemy를 가져와서 순서를 섞는다.
        var attackers = new List<Entity>(EnemyEntities.FindAll(x => x.attackable == true));
        for(int i = 0; i < attackers.Count; i++)
        {
            int rand = Random.Range(i, attackers.Count);
            Entity temp = attackers[i];
            attackers[i] = attackers[rand];
            attackers[rand] = temp;
        }

        //보스를 포함한 myEntities를 랜덤하게 시간차 공격
        foreach(var attacker in attackers)
        {
            var defenders = new List<Entity>(myEntities);
            defenders.Add(myBossEntity);
            int rand = Random.Range(0, defenders.Count);
            Attack(attacker, defenders[rand]);

            if (TurnManger.Inst.isLoading)
                yield break;

            yield return delay2;
        }

        TurnManger.Inst.EndTurn();
    }

    void EntityAlignment(bool isMine)
    {
        float targetY = isMine ? -4.35f : 4.15f; //스폰 위치 결정
        var targetEntities = isMine ? myEntities : EnemyEntities;

        for(int i= 0; i < targetEntities.Count; i++)
        {
            float targetX = (targetEntities.Count - 1) * -3.4f + i * 6.8f; //소환된 카드 위치 결정

            var targetEntity = targetEntities[i];
            targetEntity.originPos = new Vector3(targetX, targetY, 0);
            targetEntity.MoveTransform(targetEntity.originPos, true, 0.5f);
            targetEntity.GetComponent<Order>()?.SetOriginOrder(i);
        }
    }

    public void InsertMyEmptyEntity(float xPos) //필드에 소환하는 함수
    {
        if (isFullMyEntities) 
            return;

        if (!ExistMyEmptyEntity)
            myEntities.Add(myEmptyEntity);

        Vector3 emptyEntityPos = myEmptyEntity.transform.position;
        emptyEntityPos.x = xPos;
        myEmptyEntity.transform.position = emptyEntityPos;

        int _emptyEntityIndex = MyEmptyEntityIndex;
        myEntities.Sort((entity1, entity2) => entity1.transform.position.x.CompareTo(entity2.transform.position.x));

        if (MyEmptyEntityIndex != _emptyEntityIndex)
            EntityAlignment(true);
    }

    public void RemoveMyEmptyEntity()
    {
        if (!ExistMyEmptyEntity)
            return;

        myEntities.RemoveAt(MyEmptyEntityIndex);
        EntityAlignment(true);
    }

    public bool SpawnEntity(bool isMine, Item item, Vector3 spawnPos)
    {
        if(isMine)
        {
            if (isFullMyEntities || !ExistMyEmptyEntity)
                return false;
        }
        else
        {
            if (isFullEnemyEntities)
                return false;
        }

        var entityObject = Instantiate(entityPrefab, spawnPos, Utils.QI);
        var entity = entityObject.GetComponent<Entity>();

        if (isMine)
            myEntities[MyEmptyEntityIndex] = entity;
        else
            EnemyEntities.Insert(Random.Range(0, EnemyEntities.Count), entity);

        entity.isMine = isMine;
        entity.SetUp(item);
        EntityAlignment(isMine);

        return true;
    }

    void SpawnDamage(int damage, Transform tr)
    {
        if (damage <= 0)
            return;

        var damageComponent = Instantiate(damagePrefab).GetComponent<Damage>();
        damageComponent.SetUpTransform(tr);
        damageComponent.Damaged(damage);
    }

    public void EntityMouseDown(Entity entity)
    {
        if (!CanMouseInput)
            return;

        selectEntity = entity;
    }

    public void EntityMouseUp()
    {
        if (!CanMouseInput)
            return;

        //selectEntity, targetPickEntity 둘다 존재하면 공격, 바로 null, null로 변경
        if (selectEntity && targetPickEntity && selectEntity.attackable)
            Attack(selectEntity, targetPickEntity);

        selectEntity = null;
        targetPickEntity = null;
    }

    public void EntityMouseDrag()
    {
        if (!CanMouseInput || selectEntity == null)
            return;

        //에너미의 타겟엔티티 찾기
        bool existTarget = false;
        foreach(var hit in Physics2D.RaycastAll(Utils.MousePos, Vector3.forward))
        {
            Entity entity = hit.collider?.GetComponent<Entity>();
            if(entity != null && !entity.isMine && selectEntity.attackable)
            {
                targetPickEntity = entity;
                existTarget = true;
                break;
            }

            if (!existTarget)
                targetPickEntity = null;
        }
    }

    public void AttackableReset(bool isMine)
    {
        var targetEntities = isMine ? myEntities : EnemyEntities;
        targetEntities.ForEach(x => x.attackable = true);
    }
}
