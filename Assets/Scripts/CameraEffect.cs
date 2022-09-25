using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffect : MonoBehaviour
{
    [SerializeField]
    Material effectMat;

    void OnRenderImage(RenderTexture _src, RenderTexture _dest)
    {
        if (effectMat == null)
            return;

        Graphics.Blit(_src, _dest, effectMat);
    }

    void OnDestroy()
    {
        SetGrayScale(false);
    }

    public void SetGrayScale(bool isGrayScale)
    {
        effectMat.SetFloat("_GrayscaleAmount", isGrayScale ? 1 : 0);
        effectMat.SetFloat("_DarkAmount", isGrayScale ? 0.12f : 0);
    }
}
