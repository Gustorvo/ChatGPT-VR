using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIAttenuationSystem : MonoBehaviour
{
    [SerializeField] Image indicator;
    [SerializeField] Sprite inactiveSprite, speakingSprite, listerningSprite, thinkingSprite;
    [SerializeField] Image[] dots;
    [SerializeField] float waveSpeed = 25;

    private float waveOffset = 0f;

    private void Awake()
    {
        Main.OnCurrentStatusChanges += SetIndicator;
    }
    public void SetIndicator(SystemStatus status)
    {
        ShouldAnimateDots(false);
        switch (status)
        {
            case SystemStatus.Inactive:
                indicator.sprite = inactiveSprite;
                break;
            case SystemStatus.Speaking:
                indicator.sprite = speakingSprite;
                ShouldAnimateDots(true);
                break;
            case SystemStatus.Listerning:
                AnimateMicrophone();
                indicator.sprite = listerningSprite;
                break;
            case SystemStatus.Thinking:
                indicator.sprite = thinkingSprite;
                ShouldAnimateDots(true);
                break;
            default:
                break;
        }
    }

    private void AnimateMicrophone()
    {
        StopAllCoroutines();
        StartCoroutine(FadeMainIconCoroutine());
    }

    private void ShouldAnimateDots(bool active)
    {
        indicator.color = Color.white;
        if (active)
        {
            StartCoroutine(FadeDotsCoroutine());
        }
        else
        {
            StopAllCoroutines();
            for (int i = 0; i < dots.Length; i++)
            {
                Color invisible = dots[i].color;
                invisible.a = 0;
                dots[i].color = invisible;
            }
        }
    }
    private void OnDestroy()
    {
        Main.OnCurrentStatusChanges -= SetIndicator;
    }

    private IEnumerator FadeDotsCoroutine()
    {
        Color lerpedColor = Color.white;

        while (true)
        {
            // Increment waveOffset based on time and speed
            waveOffset += Time.deltaTime * waveSpeed;
            // Calculate the alpha value for the wave color using a sine wave
            for (int i = 0; i < dots.Length; i++)
            {
                //float direction = dots[i].color.a > 0.9f ? -1 : 1;
                float alpha = Mathf.Sin(waveOffset - (i * waveSpeed)) * 0.5f + 0.5f;
                lerpedColor.a = alpha;
                dots[i].color = lerpedColor;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator FadeMainIconCoroutine()
    {
        Color lerpedColor = Color.white;
        while (true)
        {
            waveOffset += Time.deltaTime * waveSpeed;
            float alpha = Mathf.Sin(waveOffset) * 0.5f + 0.5f;
            lerpedColor.a = alpha;
            indicator.color = lerpedColor;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
