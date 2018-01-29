using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ATTACK_TYPE
{
    BASE, CHARGED
};

public class AttackComponent : MonoBehaviour
{
    public float hitboxDuration;

    public float hitForceToBall;

    public float strongAttackCharge;
    public float baseAttackCharge;

    float hitboxCountdown = 0.0f;
    bool hitboxEnabled = false;

    Player player;
    public Collider basicAttackCollider;
    public Collider chargedAttackCollider;

    HashSet<GameObject> objectsCollidedThisEvt = new HashSet<GameObject>();

    void Start()
    {
        player = GetComponentInParent<Player>();
        basicAttackCollider.enabled = false;
        chargedAttackCollider.enabled = false;
    }

    void Update()
    {
        if (hitboxEnabled)
        {
            if (hitboxCountdown > 0.0f)
            {
                hitboxCountdown -= Time.deltaTime;
            }
            else
            {
                hitboxEnabled = false;
                chargedAttackCollider.enabled = false;
                basicAttackCollider.enabled = false;
                objectsCollidedThisEvt.Clear();
            }
        }
    }

    ATTACK_TYPE attackTypeThisEvt;

    public void Hit(ATTACK_TYPE attackType)
    {
        attackTypeThisEvt = attackType;

        if (!hitboxEnabled)
        {
            hitboxEnabled = true;

            if (attackType == ATTACK_TYPE.BASE)
            {
                basicAttackCollider.enabled = true;
            }
            else
            {
                chargedAttackCollider.enabled = true;
            }

            hitboxCountdown = hitboxDuration;
        }
    }

    public AudioClip[] clipsHitPlayer;
    public AudioClip[] clipsHitBall;

    public void HitPlayer(Player otherPlayer)
    {
        if (otherPlayer.gameObject.GetInstanceID() != transform.parent.gameObject.GetInstanceID())
        {
            float difference = otherPlayer.maxCharge - otherPlayer.Charge;

            float charge = attackTypeThisEvt == ATTACK_TYPE.BASE
            ? baseAttackCharge : strongAttackCharge;

            float exchangeCharge = GetExchangeCharge(
                charge,
                player.Charge,
                otherPlayer.Charge,
                otherPlayer.maxCharge);

            int pos = Random.Range(0, clipsHitPlayer.Length);
            AudioSource.PlayClipAtPoint(clipsHitPlayer[pos], Camera.main.transform.position);

            player.SubstractCharge(exchangeCharge);
            otherPlayer.IncreaseCharge(exchangeCharge);

            if (attackTypeThisEvt == ATTACK_TYPE.BASE)
            {
                otherPlayer.OnHit();
            }
            else
            {
                otherPlayer.OnChargedHit();
            }
        }
    }

    public void HitBall(Ball ball)
    {
        Rigidbody ballRb = ball.GetComponent<Rigidbody>();
        Player player = transform.parent.GetComponent<Player>();

        ballRb.velocity = Vector3.zero;

        Vector3 hitDirectionNorm = (ballRb.transform.position - transform.position).normalized;

        float charge = attackTypeThisEvt == ATTACK_TYPE.BASE
            ? baseAttackCharge : strongAttackCharge;

        ballRb.AddForce(hitForceToBall * (charge / (baseAttackCharge + strongAttackCharge)) * hitDirectionNorm,
            ForceMode.Impulse);

        float exchangeCharge = GetExchangeCharge(
            charge,
            player.Charge,
            ball.Charge,
            ball.maxCharge);

        int pos = Random.Range(0, clipsHitBall.Length);
        AudioSource.PlayClipAtPoint(clipsHitBall[pos], Camera.main.transform.position);

        player.SubstractCharge(exchangeCharge);
        ball.IncreaseCharge(exchangeCharge);
        ball.OnHit();
    }

    public static float GetExchangeCharge(
        float requestedExchangeCharge,
        float currentSourceCharge,
        float currentTargetCharge,
        float maxTargetCharge)
    {
        float difference = maxTargetCharge - currentTargetCharge;

        if (currentSourceCharge > requestedExchangeCharge)
        {
            if (difference > requestedExchangeCharge)
            {
                return requestedExchangeCharge;
            }
            else
            {
                return difference;
            }
        }
        else
        {
            if (difference > currentSourceCharge)
            {
                return currentSourceCharge;
            }
            else
            {
                return difference;
            }
        }
    }
}
