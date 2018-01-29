using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Player player1;
    public Player player2;

    public Image player1BatteryFill;
    public Image player2BatteryFill;

    void Update()
    {
        if (!GameManager.instance.GameHasStarted) { return; }

        if (readyToQuit && Input.anyKeyDown)
        {
            Application.Quit();
        }

        Vector3 scale1 = player1BatteryFill.rectTransform.localScale;

        scale1.x = player1.Charge / player1.maxCharge;

        player1BatteryFill.rectTransform.localScale = scale1;

        Vector3 scale2 = player2BatteryFill.rectTransform.localScale;

        scale2.x = player2.Charge / player2.maxCharge;

        player2BatteryFill.rectTransform.localScale = scale2;
    }

    public void ExitGame()
    {
        StartCoroutine(EndGame());
    }

    public Image credits;

    bool readyToQuit = false;

    IEnumerator EndGame()
    {
        credits.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        readyToQuit = true;
    }

    public void RestartGame()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }
}
