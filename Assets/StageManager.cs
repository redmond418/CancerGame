using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class StageManager : MonoBehaviour
{
    public static PlayerController player;
    [SerializeField] PlayerController playerCharactor;
    [SerializeField] Transform goalHeart;
    [SerializeField] Volume volume;
    [System.NonSerialized] public bool isPlaying = true;
    ChromaticAberration chroma;
    Camera mainCam;
    bool goal = false;
    // Start is called before the first frame update
    void Start()
    {
        player = playerCharactor;
        mainCam = Camera.main;
        volume.profile.TryGet(out chroma);
    }
    /*
    // Update is called once per frame
    void Update()
    {
        
    }*/

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
                    .SetEase(Ease.InOutQuint);
            });
    }
}
