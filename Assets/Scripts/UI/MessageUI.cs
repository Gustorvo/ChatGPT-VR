using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class MessageUI : MonoBehaviour
{
    [SerializeField] Color assistantBGColor, userBGColor;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Image BGImage;
    [SerializeField] Sprite assistantLogo, userLogo;
    [SerializeField] Image logo;

    private CanvasGroup canvasGrp;

    private void Awake()
    {
    }

    public void SetText(string text) => this.text.text = text;

    public void SetLogoAndBackgroundColor(Role role)
    {
        canvasGrp = GetComponent<CanvasGroup>();

        // start invisible
        canvasGrp.alpha = 0;

        // set background color
        BGImage.color = role == Role.Assistant ? assistantBGColor : userBGColor;

        // set logo
        logo.sprite = role == Role.Assistant ? assistantLogo : userLogo;

        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        float alpha = canvasGrp.alpha;
        for (float a = 0; a <= 1; a += 0.2f)
        {
            canvasGrp.alpha = a;
            yield return new WaitForSeconds(.1f);
        }
        canvasGrp.alpha = 1f;
    }
}
