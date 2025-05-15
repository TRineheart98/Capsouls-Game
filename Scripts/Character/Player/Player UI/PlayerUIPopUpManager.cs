using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerUIPopUpManager : MonoBehaviour
{
    [Header("YOU DIED")]
    [SerializeField] GameObject youDiedPopUpGameObject;
    [SerializeField] TextMeshProUGUI youDiedPopUpBackgroundText;
    [SerializeField] TextMeshProUGUI youDiedPopUpText;
    [SerializeField] CanvasGroup youDiedPopUpCanvasGroup;   //Allows us to set the alpha to fade over time

    [Header("BOSS DEFEATED Pop Up")]
    [SerializeField] GameObject bossDefeatedPopUpGameObject;
    [SerializeField] TextMeshProUGUI bossDefeatedPopUpBackgroundText;
    [SerializeField] TextMeshProUGUI bossDefeatedPopUpText;
    [SerializeField] CanvasGroup bossDefeatedPopUpCanvasGroup;

    [Header("BOSS DEFEATED Reward Pop Up")]
    [SerializeField] GameObject bossRewardPopUpGameObject;
    [SerializeField] TextMeshProUGUI bossRewardPopUpText;
    [SerializeField] GameObject bossRewardPopUpImage;
    [SerializeField] CanvasGroup bossRewardPopUpCanvasGroup;

    public void SendYouDiedPopUp()
    {
        //Activate post processing effects

        youDiedPopUpGameObject.SetActive(true);
        youDiedPopUpBackgroundText.characterSpacing = 0;
        //Stretch out the pop
        StartCoroutine(StretchPopUpTextOverTime(youDiedPopUpText, 8, 19));
        //Fade in the pop up
        StartCoroutine(FadeInPopUpOverTime(youDiedPopUpCanvasGroup, 5));
        //Wait, the fade out the pop up
        StartCoroutine(WaitThenFadeOutPopUpOverTime(youDiedPopUpCanvasGroup, 2, 5));

    }

    public void SendBossDefeatedPopUp(string bossDefeatedMessage)
    { 
        bossDefeatedPopUpText.text = bossDefeatedMessage;
        bossDefeatedPopUpBackgroundText.text = bossDefeatedMessage;
        bossDefeatedPopUpGameObject.SetActive(true);
        bossDefeatedPopUpBackgroundText.characterSpacing = 0;
        //Stretch out the pop
        StartCoroutine(StretchPopUpTextOverTime(bossDefeatedPopUpText, 8, 19));
        //Fade in the pop up
        StartCoroutine(FadeInPopUpOverTime(bossDefeatedPopUpCanvasGroup, 5));
        //Wait, the fade out the pop up
        StartCoroutine(WaitThenFadeOutPopUpOverTime(bossDefeatedPopUpCanvasGroup, 2, 5));

    }

    public void SendBossRewardPopUp()
    {
        bossRewardPopUpGameObject.SetActive(true);
        bossRewardPopUpText.characterSpacing = 0;

        // Stretch out the reward message text
        StartCoroutine(StretchPopUpTextOverTime(bossRewardPopUpText, 8, 19));

        // Fade in the whole reward pop-up (image + text)
        StartCoroutine(FadeInPopUpOverTime(bossRewardPopUpCanvasGroup, 5));

        // Wait, then fade out
        StartCoroutine(WaitThenFadeOutPopUpOverTime(bossRewardPopUpCanvasGroup, 2, 5));

    }

    private IEnumerator StretchPopUpTextOverTime(TextMeshProUGUI text, float duration, float stretchAmount)
    {
        if (duration > 0f)
        {
            text.characterSpacing = 0;
            float timer = 0;
            yield return null;

            while (timer < duration)
            {
                timer = timer + Time.deltaTime;
                text.characterSpacing = Mathf.Lerp(text.characterSpacing, stretchAmount, duration * (Time.deltaTime / 20));
                yield return null;
            }
        }
    }

    private IEnumerator FadeInPopUpOverTime(CanvasGroup canvas, float duration)
    {
        if (duration > 0)
        {
            canvas.alpha = 0;
            float timer = 0;
            yield return null;

            while (timer < duration)
            {
                timer = timer + Time.deltaTime;
                canvas.alpha = Mathf.Lerp(canvas.alpha, 1, duration * Time.deltaTime);
                yield return null;
            }
        }

        canvas.alpha = 1;

        yield return null;
    }

    private IEnumerator WaitThenFadeOutPopUpOverTime(CanvasGroup canvas, float duration, float delay)
    {
        if (duration > 0)
        {
            while (delay > 0)
            {
                delay = delay - Time.deltaTime;
                yield return null;
            }

            canvas.alpha = 1;
            float timer = 0;
            yield return null;

            while (timer < duration)
            {
                timer = timer + Time.deltaTime;
                canvas.alpha = Mathf.Lerp(canvas.alpha, 0, duration * Time.deltaTime);
                yield return null;
            }
        }

        canvas.alpha = 0;

        yield return null;
    }
}
