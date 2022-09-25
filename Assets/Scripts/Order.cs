using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Order : MonoBehaviour
{
    [SerializeField]
    Renderer[] backRenders; //뒤쪽의 렌더러들을 가져오는 백렌더러
    [SerializeField]
    Renderer[] middleRenders; //중간의 렌더러들을 가져오는 미들렌더러
    [SerializeField]
    string sortingLayerName; //sorting 할 레이어이름을 정해주는 스트링

    int originOrder; //카드가 필드에 나왔을 때 확대를 위한 변수

    public void SetOriginOrder(int originOrder)
    {
        this.originOrder = originOrder;
        SetOrder(originOrder);
    }

    public void SetMostFrontOrder(bool IsMostFront)
    {
        SetOrder(IsMostFront ? 100 : originOrder); //IsMostFront가 트루라면 샛오더에 100을 추가해서 확대 폴스라면 오리진오더를 보낸다.
    }

    public void SetOrder(int order)
    {
        int mulOrder = order * 10; //카드들이 겹치는 것을 방지하기 위해서 10을 곱해준다.

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
