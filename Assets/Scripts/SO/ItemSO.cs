using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public string name; //��ȯ�� ī���� �̸�
    public int attack;  //��ȯ�� ī���� ���ݷ�
    public int health;  //��ȯ�� ī���� �����
    public Sprite sprite; //��ȯ�� ī���� ��������Ʈ �̹���
    public float percent; //���� Ȯ��
    public bool isDefence; //�Ͻ������� ���� ����
}

[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Object/ItemSO")]
public class ItemSO : ScriptableObject
{
    public Item[] items;
}
