using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Order : MonoBehaviour
{
    [SerializeField]
    Renderer[] backRenders; //������ ���������� �������� �鷻����
    [SerializeField]
    Renderer[] middleRenders; //�߰��� ���������� �������� �̵鷻����
    [SerializeField]
    string sortingLayerName; //sorting �� ���̾��̸��� �����ִ� ��Ʈ��

    int originOrder; //ī�尡 �ʵ忡 ������ �� Ȯ�븦 ���� ����

    public void SetOriginOrder(int originOrder)
    {
        this.originOrder = originOrder;
        SetOrder(originOrder);
    }

    public void SetMostFrontOrder(bool IsMostFront)
    {
        SetOrder(IsMostFront ? 100 : originOrder); //IsMostFront�� Ʈ���� �������� 100�� �߰��ؼ� Ȯ�� ������� ������������ ������.
    }

    public void SetOrder(int order)
    {
        int mulOrder = order * 10; //ī����� ��ġ�� ���� �����ϱ� ���ؼ� 10�� �����ش�.

        foreach(var renderer in backRenders)
        {
            renderer.sortingLayerName = sortingLayerName;
            renderer.sortingOrder = mulOrder;
        }

        foreach(var renderer in middleRenders)
        {
            renderer.sortingLayerName = sortingLayerName;
            renderer.sortingOrder = mulOrder + 1;
        }
    }
}
