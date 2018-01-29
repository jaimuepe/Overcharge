using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public float disablePlayersDelay = 2.0f;

    public Player player1;
    public Player player2;
    public Ball ball;

    int marcador1, marcador2;
    Rigidbody rigidBodyBall;
    Rigidbody rigidBodyPlayer1;
    Rigidbody rigidBodyPlayer2;
    public Text marcador;
    public Animation cameraAnimation;
    BallCollider bc;

    bool gameHasStarted = false;
    public bool GameHasStarted { get { return gameHasStarted; } }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        marcador1 = 0;
        marcador2 = 0;
        rigidBodyBall = ball.GetComponent<Rigidbody>();
        rigidBodyPlayer1 = player1.GetComponent<Rigidbody>();
        rigidBodyPlayer2 = player2.GetComponent<Rigidbody>();

        SetupGame();
    }

    public float fadeInUIDelay = 2.0f;
    public float delayBeforeCountdown;

    public Image[] fadeInImages;
    public SpriteRenderer[] fadeInSpriteRenderers;

    public Image countdownImage;
    public Image fightMsgImage;

    public bool debugPlayStartupAnimations = false;

    void SetupGame()
    {
        if (debugPlayStartupAnimations)
        {
            player1.gameObject.SetActive(false);
            player2.gameObject.SetActive(false);
            ball.gameObject.SetActive(false);

            for (int i = 0; i < fadeInImages.Length; i++)
            {
                Image img = fadeInImages[i];
                img.color = new Color(img.color.r, img.color.g, img.color.b, 0.0f);
            }

            for (int i = 0; i < fadeInSpriteRenderers.Length; i++)
            {
                SpriteRenderer sr = fadeInSpriteRenderers[i];
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.0f);
            }
            StartCoroutine(FadeInSprites());
        }
        else
        {
            gameHasStarted = true;
            Animator cameraAnimator = Camera.main.GetComponent<Animator>();
            cameraAnimator.enabled = true;
        }
    }

    IEnumerator FadeInUI()
    {
        float currentTime = 0f;
        while (currentTime < fadeInUIDelay)
        {
            float alpha = Mathf.Lerp(0f, 1f, currentTime / fadeInUIDelay);

            for (int j = 0; j < fadeInImages.Length; j++)
            {
                Image img = fadeInImages[j];
                img.color = new Color(img.color.r, img.color.g, img.color.b, alpha);
            }

            currentTime += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator FadeInSprites()
    {
        float currentTime = 0f;
        while (currentTime < fadeInUIDelay)
        {
            float alpha = Mathf.Lerp(0f, 1f, currentTime / fadeInUIDelay);

            for (int k = 0; k < fadeInSpriteRenderers.Length; k++)
            {
                SpriteRenderer sr = fadeInSpriteRenderers[k];
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);
            }

            currentTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        Animator countdownAnimator = countdownImage.GetComponent<Animator>();
        countdownAnimator.StartPlayback();

        countdownImage.color = new Color(
            countdownImage.color.r,
            countdownImage.color.g,
            countdownImage.color.b,
            0.0f);

        countdownImage.gameObject.SetActive(true);

        yield return new WaitForSeconds(2.58f);

        fightMsgImage.gameObject.SetActive(true);

        player1.gameObject.SetActive(true);
        player2.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        Animator cameraAnimator = Camera.main.transform.parent.GetComponent<Animator>();
        cameraAnimator.enabled = true;
        StartCoroutine(FadeInUI());

        gameHasStarted = true;

        yield return new WaitForSeconds(0.5f);
        ball.gameObject.SetActive(true);
    }

    public void PlayerScores(PlayerIndex index)
    {

    }

    public void ResetPositions(string tag)
    {
        GameObject ballColliderPlayer1 = GameObject.FindGameObjectWithTag("BallColliderPlayer1");
        GameObject ballColliderPlayer2 = GameObject.FindGameObjectWithTag("BallColliderPlayer2");

        ball.Reset();

        rigidBodyBall.velocity = new Vector3(0, 0, 0);

        if ("BallColliderPlayer2".Equals(tag))
        {
            player1.ResetPlayer();
            player2.ResetPlayer();


            float distanceBetweenBallcolliderPlayer = Vector3.Distance(ballColliderPlayer2.transform.position, rigidBodyPlayer1.transform.position);
            if (distanceBetweenBallcolliderPlayer < 15.5)
            {
                rigidBodyPlayer1.AddForce(45.0f * new Vector3((
                    distanceBetweenBallcolliderPlayer - 19f) * 0.03f, 0.3f, 0.0f), ForceMode.Impulse);
            }
            marcador1++;
        }
        else
        {
            player1.ResetPlayer();
            player2.ResetPlayer();

            float distanceBetweenBallcolliderPlayer = Vector3.Distance(ballColliderPlayer1.transform.position, rigidBodyPlayer2.transform.position);
            if (distanceBetweenBallcolliderPlayer < 15.5)
            {
                rigidBodyPlayer2.AddForce(45.0f * new Vector3((
                    19f - distanceBetweenBallcolliderPlayer) * 0.03f, 0.3f, 0.0f), ForceMode.Impulse);
            }
            marcador2++;
        }

        StartCoroutine(EnablePlayersTimer());

        if (Camera.current != null)
        {
            CameraTranslate();
        }
        marcador.text = marcador1 + " - " + marcador2;
    }

    private void CameraTranslate()
    {

    }

    IEnumerator EnablePlayersTimer()
    {
        yield return new WaitForSeconds(disablePlayersDelay);
        player1.movementEnabled = true;
        player2.movementEnabled = true;
    }
}
