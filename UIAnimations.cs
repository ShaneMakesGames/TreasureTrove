using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;

public class UIAnimations
{
    #region Motion
    public static IEnumerator MoveLocal(RectTransform rectTransform, Vector3 newVector, float animTime)
    {
        Vector3 startPos = rectTransform.localPosition;
        float timePassed = 0;

        while (timePassed < animTime)
        {
            timePassed += Time.deltaTime;
            rectTransform.localPosition  = Vector3.Lerp(startPos, newVector, timePassed / animTime);
            yield return null;
        }
    }

    public static IEnumerator MoveLocalX(RectTransform rectTransform, float newXPos, float animTime)
    {
        float startPos = rectTransform.localPosition.x;
        float timePassed = 0;

        while (timePassed < animTime)
        {
            timePassed += Time.deltaTime;
            float lerpedPos = Mathf.Lerp(startPos, newXPos, timePassed / animTime);
            rectTransform.localPosition = new Vector3(lerpedPos, rectTransform.localPosition.y, rectTransform.localPosition.z);
            yield return null;
        }
    }

    public static IEnumerator MoveLocalY(RectTransform rectTransform, float newYPos, float animTime)
    {
        float startPos = rectTransform.localPosition.y;
        float timePassed = 0;

        while (timePassed < animTime)
        {
            timePassed += Time.deltaTime;
            float lerpedPos = Mathf.Lerp(startPos, newYPos, timePassed / animTime);
            rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, lerpedPos, rectTransform.localPosition.z);
            yield return null;
        }
    }

    #endregion

    #region Color/Alpha

    public static IEnumerator ChangeImageColorCoroutine(Image image, Color newColor, float animTime)
    {
        Color startColor = image.color;
        float timePassed = 0;

        while (timePassed < animTime)
        {
            timePassed += Time.deltaTime;            
            image.color = Color.Lerp(startColor, newColor, timePassed / animTime);
            yield return null;
        }
    }

    public static IEnumerator FadeImageAlphaOverTimeCoroutine(Image image, float newAlpha, float animTime)
    {
        float startAlpha = image.color.a;
        float timePassed = 0;

        while (timePassed < animTime)
        {
            timePassed += Time.deltaTime;
            Color newColor = image.color;
            float lerpedAlpha = Mathf.Lerp(startAlpha, newAlpha, timePassed / animTime);
            newColor.a = lerpedAlpha;
            image.color = newColor;
            yield return null;
        }
    }

    public static IEnumerator FadeCanvasGroupAlphaOverTimeCoroutine(CanvasGroup cg, float newAlpha, float animTime)
    {
        float startAlpha = cg.alpha;
        float timePassed = 0;

        while (timePassed < animTime)
        {
            timePassed += Time.deltaTime;
            float lerpedAlpha = Mathf.Lerp(startAlpha, newAlpha, timePassed / animTime);
            cg.alpha = lerpedAlpha;
            yield return null;
        }
    }

    public static IEnumerator FadeTextMeshProAlphaOverTimeCoroutine(TextMeshPro textMesh, float newAlpha, float animTime)
    {
        float startAlpha = textMesh.alpha;
        float timePassed = 0;

        while (timePassed < animTime)
        {
            timePassed += Time.deltaTime;
            float lerpedAlpha = Mathf.Lerp(startAlpha, newAlpha, timePassed / animTime);
            textMesh.alpha = lerpedAlpha;
            yield return null;
        }
    }

    #endregion

    #region Scale
    public static IEnumerator ScaleWidthOverTimeCoroutine(Image image, float newWidth, float animTime)
    {
        float startWidth = image.rectTransform.sizeDelta.x;
        float timePassed = 0;

        while (timePassed < animTime)
        {
            timePassed += Time.deltaTime;
            float lerpedWidth = Mathf.Lerp(startWidth, newWidth, timePassed / animTime);
            image.rectTransform.sizeDelta = new Vector2(lerpedWidth, image.rectTransform.sizeDelta.y);
            yield return null;
        }
    }

    public static IEnumerator ScaleHeightOverTimeCoroutine(Image image, float newWidth, float animTime)
    {
        float startWidth = image.rectTransform.sizeDelta.x;
        float timePassed = 0;

        while (timePassed < animTime)
        {
            timePassed += Time.deltaTime;
            float lerpedWidth = Mathf.Lerp(startWidth, newWidth, timePassed / animTime);
            image.rectTransform.sizeDelta = new Vector2(lerpedWidth, image.rectTransform.sizeDelta.y);
            yield return null;
        }
    }

    public static IEnumerator ScaleImageOverTimeCoroutine(Image image, Vector2 newSizeDelta, float animTime)
    {
        Vector2 startVector = image.rectTransform.sizeDelta;
        float timePassed = 0;

        while (timePassed < animTime)
        {
            timePassed += Time.deltaTime;
            Vector2 lerpedVector = Vector2.Lerp(startVector, newSizeDelta, timePassed / animTime);
            image.rectTransform.sizeDelta = lerpedVector;
            yield return null;
        }
    }

    #endregion

    #region Text

    public static IEnumerator AnimateTypingCoroutine(TextMeshProUGUI textMesh, string finalString, float waitTime)
    {
        textMesh.text = "";
        for (int i = 0; i < finalString.Length; i++)
        {
            char currentChar = finalString[i];
            textMesh.text += currentChar;

            if (currentChar == '\\') continue;
            yield return new WaitForSeconds(waitTime);
        }
    }

    public static IEnumerator AnimateNumericalIncrease(TextMeshProUGUI textMesh, int startingAmount, int endAmount, float animTime)
    {
        float timePassed = 0;
        while (timePassed < animTime)
        {
            float lerpedAmount = Mathf.Lerp(startingAmount, endAmount, timePassed / animTime);
            int convertedInt = (int)lerpedAmount;
            textMesh.text = convertedInt.ToString();
            timePassed += Time.deltaTime;
            yield return null;
        }

        textMesh.text = endAmount.ToString();
    }

    public const float RANDOM_NUMBER_TEXT_UPDATE_TIME = 0.05f;
    public const float RANDOM_NUMBER_TEXT_TOTAL_TIME = 0.85f;
    public static IEnumerator AlternateRandomNumbersCoroutine(TextMeshProUGUI textMesh, int numberCount)
    {
        textMesh.text = GenerateRandomSequenceOfNumbers(numberCount);

        float timePassed = 0;
        float timeSinceTextLastUpdated = 0;
        while (timePassed < RANDOM_NUMBER_TEXT_TOTAL_TIME)
        {
            timePassed += Time.deltaTime;
            timeSinceTextLastUpdated += Time.deltaTime;

            if (timeSinceTextLastUpdated > RANDOM_NUMBER_TEXT_UPDATE_TIME)
            {
                textMesh.text = GenerateRandomSequenceOfNumbers(numberCount);
                timeSinceTextLastUpdated = 0;
            }
            yield return null;
        }
    }

    public static string GenerateRandomSequenceOfNumbers(int numberCount)
    {
        string sequence = "";
        for (int i = 0; i < numberCount; i++)
        {
            sequence += UnityEngine.Random.Range(0, 10).ToString();
        }
        return sequence;
    }

    public const float SCROLLING_NUMBER_TEXT_UPDATE_TIME = 0.025f;
    public const float SCROLLING_NUMBER_TEXT_LOCK_TIME = 0.15f;
    public static IEnumerator AnimateScrollingNumbersCoroutine(TextMeshProUGUI textMesh, string finalString)
    {
        textMesh.text = finalString;
        int indexOfTextToLock = 0;
        int textLength = finalString.Length;

        float timeSinceTextLastUpdated = SCROLLING_NUMBER_TEXT_UPDATE_TIME;
        float timeSinceTextLastLocked = 0;
        while (indexOfTextToLock < textLength)
        {
            timeSinceTextLastUpdated += Time.deltaTime;
            timeSinceTextLastLocked += Time.deltaTime;

            // Locks this digit in, it will no longer be updated
            if (timeSinceTextLastLocked > SCROLLING_NUMBER_TEXT_LOCK_TIME)
            {
                StringBuilder sb = new StringBuilder(textMesh.text);
                sb[indexOfTextToLock] = finalString[indexOfTextToLock];
                textMesh.text = sb.ToString();
                indexOfTextToLock++;

                timeSinceTextLastLocked = 0;
            }

            // Updates the text to a random sequence of numbers
            if (timeSinceTextLastUpdated >= SCROLLING_NUMBER_TEXT_UPDATE_TIME)
            {
                StringBuilder sb = new StringBuilder(textMesh.text);
                for (int i = indexOfTextToLock; i < textLength; i++)
                {
                    sb[i] = UnityEngine.Random.Range(0, 10).ToString()[0];
                }
                textMesh.text = sb.ToString();
                timeSinceTextLastUpdated = 0;
            }
            yield return null;
        }
    }
    #endregion
}