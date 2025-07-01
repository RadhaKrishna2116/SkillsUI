using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SkillsUIController : MonoBehaviour
{
    public CanvasGroup miniButtonGroup;
    public CanvasGroup skillsWindowGroup;
    public float transitionTime = 0.5f;

    private bool isTransitioning = false;

    private void Start()
    {
        skillsWindowGroup.alpha = 0;
        skillsWindowGroup.gameObject.SetActive(false);

        miniButtonGroup.alpha = 1;
        miniButtonGroup.gameObject.SetActive(true);
    }

    public void OnMiniButtonClicked()
    {
        if (!isTransitioning)
            StartCoroutine(ShowSkillsWindow());
    }

    public void OnCloseButtonClicked()
    {
        if (!isTransitioning)
            StartCoroutine(HideSkillsWindow());
    }

    private IEnumerator ShowSkillsWindow()
    {
        isTransitioning = true;

        // Fade out MiniButton
        float timer = 0f;
        while (timer < transitionTime)
        {
            timer += Time.deltaTime;
            float t = timer / transitionTime;
            miniButtonGroup.alpha = 1f - t;
            yield return null;
        }

        miniButtonGroup.alpha = 0f;
        miniButtonGroup.gameObject.SetActive(false);

        // Fade in SkillsWindow
        skillsWindowGroup.gameObject.SetActive(true);
        timer = 0f;
        while (timer < transitionTime)
        {
            timer += Time.deltaTime;
            float t = timer / transitionTime;
            skillsWindowGroup.alpha = t;
            yield return null;
        }

        skillsWindowGroup.alpha = 1f;
        isTransitioning = false;
    }

    private IEnumerator HideSkillsWindow()
    {
        isTransitioning = true;

        // Fade out SkillsWindow
        float timer = 0f;
        while (timer < transitionTime)
        {
            timer += Time.deltaTime;
            float t = timer / transitionTime;
            skillsWindowGroup.alpha = 1f - t;
            yield return null;
        }

        skillsWindowGroup.alpha = 0f;
        skillsWindowGroup.gameObject.SetActive(false);

        // Fade in MiniButton
        miniButtonGroup.gameObject.SetActive(true);
        timer = 0f;
        while (timer < transitionTime)
        {
            timer += Time.deltaTime;
            float t = timer / transitionTime;
            miniButtonGroup.alpha = t;
            yield return null;
        }

        miniButtonGroup.alpha = 1f;
        isTransitioning = false;
    }
}