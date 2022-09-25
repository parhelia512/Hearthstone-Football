using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class NotificationPanel : MonoBehaviour
{
    [SerializeField]
    TMP_Text notificationTMP;

    public void Show(string message)
    {
        notificationTMP.text = message;
        Sequence sequence = DOTween.Sequence()
            .Append(transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.InOutQuad)) //스케일이 커지고
            .AppendInterval(0.9f) //0.9초 이후
            .Append(transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InOutQuad)); //스케일이 다시 작아진다.
    }

    void Start() => ScaleZero();

    [ContextMenu("ScaleOne")]
    void ScaleOne() => transform.localScale = Vector3.one; //스케일 1으로 만들기

    [ContextMenu("ScaleZero")]
    public void ScaleZero() => transform.localScale = Vector3.zero; //스케일 0으로 만들기
}
