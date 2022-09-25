using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public string name; //소환될 카드의 이름
    public int attack;  //소환될 카드의 공격력
    public int health;  //소환될 카드의 생명력
    public Sprite sprite; //소환될 카드의 스프라이트 이미지
    public float percent; //뽑힐 확률
    public bool isDefence; //하스스톤의 도발 구현
}

[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Object/ItemSO")]
public class ItemSO : ScriptableObject
{
    public Item[] items;
}
