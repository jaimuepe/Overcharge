using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class ShipCharge : MonoBehaviour
{
    public float maximumCharge = 1000f;
    float shipCharge = 0f;
    public Player player1;
    public Player player2;
    public GameObject player1SurvivedText;
    public GameObject player2SurvivedText;
    public Image image;

    public float fadeInDelay = 2.0f;

    public float Charge { get { return shipCharge; } }

    // 75, 50, 25, 0

    public ParticleSystem fireLeak;
    public ParticleSystem fireLeak2;
    public ParticleSystem fireFromCabin;

    public Button btRestartGame;
    public Button btExitToDesktop;

    public void IncreaseCharge(float charge, bool leftPlayerScores)
    {
        shipCharge += charge;
        Debug.Log("Current ship charge (" + this.ToString() + ") is " + shipCharge);
        if (shipCharge >= maximumCharge)
        {
            if (leftPlayerScores)
            {
                ShipExplode(player2);
            }
            else
            {
                ShipExplode(player1);
            }
        }

        if (shipCharge / maximumCharge > 0.25f)
        {
            fireLeak.gameObject.SetActive(true);
        }
        if (shipCharge / maximumCharge > 0.5f)
        {
            fireLeak2.gameObject.SetActive(true);
        }
        if (shipCharge / maximumCharge > 0.7f)
        {
            fireFromCabin.gameObject.SetActive(true);
        }
        if (shipCharge / maximumCharge > 0.95f)
        {

        }
    }

    void ShipExplode(Player player)
    {

        player.gameObject.GetComponent<Animator>().enabled = false;
        player.GetComponent<Rigidbody>().freezeRotation = false;
        if (player == player1)
        {
            //player 2 survives
            StartCoroutine(player1Won(true));
            player.GetComponent<Rigidbody>().AddForce(5.0f * new Vector3(
                    -3f, 0f, 0.0f), ForceMode.Impulse);
        }
        else if (player.Equals(player2))
        {
            //player 1 survives
            StartCoroutine(player1Won(false));
            player.GetComponent<Rigidbody>().AddForce(5.0f * new Vector3(
                3f, 0f, 0.0f), ForceMode.Impulse);
        }
    }

    public bool IsChargeCompleted()
    {
        return shipCharge > maximumCharge;
    }

    IEnumerator player1Won(bool player1Survived)
    {
        yield return new WaitForSeconds(2);
        Camera.main.transform.parent.GetComponent<Animator>().SetTrigger("playerWin");

        if (player1Survived)
        {
            player1SurvivedText.SetActive(true);
        }
        else
        {
            player2SurvivedText.SetActive(true);
        }
        StartCoroutine(FadeInImage());
    }

    IEnumerator FadeInImage()
    {
        yield return new WaitForSeconds(2);
        float currentTime = 0f;

        while (currentTime < fadeInDelay)
        {
            float alpha = Mathf.Lerp(0f, 1f, currentTime / fadeInDelay);

            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);

            currentTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(1.0f);

        btRestartGame.gameObject.SetActive(true);
        btExitToDesktop.gameObject.SetActive(true);

        EventSystem.current.SetSelectedGameObject(btRestartGame.gameObject);
    }
}
