using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCollider : MonoBehaviour {

    public GameObject overChargedText;
    public ShipCharge sc;

    void Start() {

    }

    public AudioClip scoreClip;

    public Explosion ballExplosionPrefab;
    public Vector3 explosionLocation;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            bool leftPlayerScores = gameObject.CompareTag("BallColliderPlayer2");
            float ballCharge = other.GetComponent<Ball>().Charge;
            sc.IncreaseCharge(ballCharge, leftPlayerScores);
            if(!sc.IsChargeCompleted()) {
                if (leftPlayerScores)
                {
                    overChargedText.SetActive(true);
                }
                else
                {
                    overChargedText.SetActive(true);
                }
                
                AudioSource.PlayClipAtPoint(scoreClip, Camera.main.transform.position);
                
                StartCoroutine(HideSurvivorMessage(leftPlayerScores));
                GameManager.instance.ResetPositions(gameObject.tag);
            }

            Explosion explosionCopy = Instantiate(ballExplosionPrefab);
            explosionCopy.transform.position = new Vector3(
                transform.position.x,
                transform.position.y,
                explosionCopy.transform.position.z);

            explosionCopy.Explode();
        }
    }

    IEnumerator HideSurvivorMessage(bool leftPlayerScores)
    {
        yield return new WaitForSeconds(2);
        if (leftPlayerScores == true)
        {
            overChargedText.SetActive(false);
        }
        else
        {
            overChargedText.SetActive(false);
        }
    }
}
