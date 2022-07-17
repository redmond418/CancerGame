using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    [SerializeField] Transform goalHeart;
    [SerializeField] Volume volume;
    [SerializeField] GameObject clearedUIsParent;
    [SerializeField] Image line0;
    [SerializeField] Image line1;
    [SerializeField] Image blackout;
    [SerializeField] TextMeshProUGUI clearText;
    [SerializeField] TextMeshProUGUI nextText;
    [System.NonSerialized] public bool isPlaying = true;
    ChromaticAberration chroma;
    Material blackoutMaterial;
    Material clearTextMaterial;
    Material nextTextMaterial;
    Camera mainCam;
    bool goal = false;
    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
        volume.profile.TryGet(out chroma);
        blackoutMaterial = blackout.material;
        clearTextMaterial = clearText.fontMaterial;
        nextTextMaterial = nextText.fontMaterial;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Restart(float delay)
    {
        blackoutMaterial.SetFloat("_Value", 1);
        blackout.enabled = true;
        blackoutMaterial.DOFloat(0, "_Value", 1f)
            .SetDelay(delay)
            .SetEase(Ease.InOutSine);
    }

    public void Goal()
    {
        goal = true;
        isPlaying = false;
        chroma.active = true;
        DOTween.To(() => chroma.intensity.value, n => chroma.intensity.value = n, 0.6f, 0.2f)
            .SetEase(Ease.OutExpo)
            .OnComplete(() =>
            {
                DOTween.To(() => chroma.intensity.value, n => chroma.intensity.value = n, 0f, 0.6f)
                    .SetEase(Ease.InCubic)
                    .OnComplete(() => chroma.active = false);
            });
        mainCam.DOOrthoSize(mainCam.orthographicSize + 0.5f, 0.2f)
            .SetEase(Ease.OutQuart)
            .OnComplete(() =>
            {
                mainCam.transform.DOMove(goalHeart.position + Vector3.forward * mainCam.transform.position.z, 1f)
                .SetEase(Ease.InOutQuint);
                mainCam.DOOrthoSize(7f, 1f)
                    .SetEase(Ease.InOutQuint)
                    .OnComplete(() =>
                    {
                        clearedUIsParent.SetActive(true);
                        line0.DOFillAmount(1f, 1.5f)
                            .SetEase(Ease.OutExpo);
                        DOTween.To(() => clearTextMaterial.GetFloat("_FaceDilate"), n => clearTextMaterial.SetFloat("_FaceDilate", n), 0, 1f)
                            .SetEase(Ease.InOutCubic);
                        line1.DOFillAmount(1f, 1.5f)
                            .SetDelay(0.5f)
                            .SetEase(Ease.OutExpo);
                        DOTween.To(() => nextTextMaterial.GetFloat("_FaceDilate"), n => nextTextMaterial.SetFloat("_FaceDilate", n), 0, 1f)
                            .SetDelay(0.5f)
                            .SetEase(Ease.InOutCubic);
                        clearedUIsParent.transform.DORotate(Vector3.forward * -180f, 2f)
                            .SetDelay(1f)
                            .SetEase(Ease.InOutQuart);
                    });
            });
    }
}
