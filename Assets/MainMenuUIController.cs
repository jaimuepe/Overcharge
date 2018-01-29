using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUIController : MonoBehaviour
{

    public GameObject tutorialPanel;

    bool loadingGame = false;
    bool showingTutorial = false;

    public Image[] images;

    int count = 0;

    void Update()
    {
        if (Input.GetButtonDown("Fire_player1"))
        {
            count++;
        }

        if (count >= 20)
        {
            SceneManager.LoadSceneAsync(2);
        }

        if (showingTutorial && Input.GetButtonDown("Submit"))
        {
            HideTutorial();
        }
    }

    void HideTutorial()
    {
        showingTutorial = false;
        tutorialPanel.SetActive(false);
    }

    public void ShowTutorial()
    {
        if (loadingGame) { return; }


        if (!showingTutorial)
        {
            PlayButtonSound();
            tutorialPanel.SetActive(true);
            StartCoroutine(DelayAfterShowTutorial());
        }
    }

    public void Quit()
    {
        if (loadingGame) { return; }

        PlayButtonSound();
        Application.Quit();
    }

    public void StartGame()
    {
        PlayButtonSound();
        loadingGame = true;
        StartCoroutine(FadeOutUI());

    }

    public AudioClip buttonSound;

    void PlayButtonSound()
    {
        AudioSource.PlayClipAtPoint(buttonSound, Camera.main.transform.position);
    }

    IEnumerator DelayAfterShowTutorial()
    {
        yield return new WaitForSeconds(0.2f);
        showingTutorial = true;
    }

    public float fadeOutUIDelay = 2.0f;

    IEnumerator FadeOutUI()
    {
        float currentTime = 0f;
        while (currentTime < fadeOutUIDelay)
        {
            float alpha = Mathf.Lerp(1f, 0f, currentTime / fadeOutUIDelay);

            for (int j = 0; j < images.Length; j++)
            {
                Image img = images[j];
                images[j].color = new Color(img.color.r, img.color.g, img.color.b, alpha);
            }

            currentTime += Time.deltaTime;
            yield return null;
        }

        SceneManager.LoadSceneAsync(1);
    }
}
