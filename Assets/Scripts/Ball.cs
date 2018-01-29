using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public float maxCharge = 200.0f;
    public float baseVelocity = 10.0f;

    public float chargeDecayRateDelta = 0.1f;

    float baseVelocitySqr;

    float currentCharge = 0.0f;

    public float Charge { get { return currentCharge; } }

    Rigidbody rb;
    SphereCollider sphereCollider;

    Vector3 startPosition;

    float maxVelocity;

    public TrailRenderer trailRenderer;

    public SpriteRenderer maskSpriteRenderer;
    public AnimationCurve bloomCurve;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();

        baseVelocitySqr = baseVelocity * baseVelocity;

        startPosition = transform.position;

        maxVelocity = baseVelocity * ballSpeedCurve.Evaluate(1.0f);
    }

    public void OnHit()
    {
    }

    public void IncreaseCharge(float charge)
    {
        currentCharge += charge;
        LimitVelocity();
    }

    public Explosion explosionPrefab;

    void Explode()
    {
        Explosion explosionInstance = Instantiate(explosionPrefab);
        explosionInstance.transform.position = transform.position;
        explosionInstance.Explode();

        Reset();
        Player[] players = FindObjectsOfType<Player>();
        for (int i = 0; i < players.Length; i++)
        {
            players[i].ResetPlayer();
            players[i].movementEnabled = true;
        }
    }

    public void SubstractCharge(float charge)
    {
        currentCharge -= charge;
        LimitVelocity();
    }

    private void Update()
    {
        Vector3 velocity = rb.velocity;
        velocity.y -= 5.0f * Time.deltaTime;
        rb.velocity = velocity;

        LimitVelocity();

        currentCharge = Mathf.Clamp(currentCharge, 0.0f, Mathf.Infinity);
        sphereCollider.material.bounciness = (currentCharge / maxCharge);

        maskSpriteRenderer.color = new Color(
             maskSpriteRenderer.color.r,
              maskSpriteRenderer.color.g,
               maskSpriteRenderer.color.b,
               bloomCurve.Evaluate(currentCharge / maxCharge));
    }

    public AnimationCurve ballSpeedCurve;

    void LimitVelocity()
    {
        float chargePercent = currentCharge / maxCharge;

        float maxCurrentVelocitySqr =
            baseVelocitySqr * ballSpeedCurve.Evaluate(chargePercent);

        if (rb.velocity.sqrMagnitude > maxCurrentVelocitySqr)
        {
            Vector3 velocityNorm = rb.velocity.normalized;
            rb.velocity = velocityNorm * baseVelocity * ballSpeedCurve.Evaluate(chargePercent);
        }
    }

    public void Reset()
    {
        rb.velocity = Vector3.zero;
        currentCharge = 0.0f;
        transform.position = startPosition;
        trailRenderer.Clear();
    }

    public AudioClip[] clipsHitBall;

    public float chargeExchangedOnHitAtMaxVelocity;

    public ParticleSystemPoolManager sparksPoolManager;

    private void OnCollisionEnter(Collision collision)
    {
        if (currentCharge >= maxCharge)
        {
            Explode();
            return;
        }

        ParticleSystem ps = sparksPoolManager.Request();
        ps.gameObject.SetActive(true);
        ps.transform.position = collision.contacts[0].point;

        ParticleSystem.Burst burst = new ParticleSystem.Burst();

        float speedPercent = rb.velocity.magnitude / maxVelocity;
        float min = speedPercent * 50;

        if (!collision.gameObject.CompareTag("Player"))
        {
            ParticleSystem.MinMaxCurve mmCurve = new ParticleSystem.MinMaxCurve(min, 4 * min);
            burst.count = mmCurve;
            burst.cycleCount = 1;
            burst.repeatInterval = 0.01f;

            ps.emission.SetBurst(0, burst);
            ps.Play();
        }
        else
        {
            if (rb.velocity.magnitude > 0.1f * maxVelocity)
            {
                ParticleSystem.MinMaxCurve mmCurve = new ParticleSystem.MinMaxCurve(min, 4 * min);
                burst.count = mmCurve;
                burst.cycleCount = 1;
                burst.repeatInterval = 0.01f;

                ps.emission.SetBurst(0, burst);
                ps.Play();

                Player p = collision.gameObject.GetComponent<Player>();

                int pos = Random.Range(0, clipsHitBall.Length);
                AudioSource.PlayClipAtPoint(clipsHitBall[pos], Camera.main.transform.position);

                float chargePercent = currentCharge / maxCharge;
                float requestedExchangeCharge = chargePercent * chargeExchangedOnHitAtMaxVelocity;

                float exchangeCharge = AttackComponent.GetExchangeCharge(
                    requestedExchangeCharge,
                    currentCharge,
                    p.Charge,
                    p.maxCharge);

                p.IncreaseCharge(exchangeCharge);
                SubstractCharge(exchangeCharge);
                p.OnHit();
            }
        }
    }
}
