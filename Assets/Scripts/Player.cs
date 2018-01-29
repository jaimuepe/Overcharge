using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Direction
{
    LEFT, RIGHT, UP, DOWN
};

public enum PlayerIndex
{
    LEFT, RIGHT
};

public class Player : MonoBehaviour
{
    public LayerMask groundLayer;

    public float baseSpeed;
    public float baseJumpForce;

    public string horizontalAxisName = "Horizontal";
    public string jumpButtonName = "Jump";
    public string attackButtonName = "Attack";
    public string tauntButtonName = "Taunt";

    public string horizontalAimAxisName = "Horizontal_aim";
    public string verticalAimAxisName = "Vertical_aim";

    public float baseCharge = 150.0f;
    public float maxCharge = 300.0f;
    float currentCharge;

    public AnimationCurve speedCurve;

    float baseScaleAbsValue;
    bool charging = false;

    Rigidbody rb;
    BoxCollider boxCollider;
    AttackComponent ac;

    Animator animator;
    Transform myTransform;

    float attackButtonPressedForSeconds = 0.0f;

    public Direction direction = Direction.RIGHT;

    public bool movementEnabled = true;

    public float Charge { get { return currentCharge; } }

    private void Awake()
    {
        baseScaleAbsValue = Mathf.Abs(transform.localScale.z);
    }

    void Start()
    {
        ac = GetComponentInChildren<AttackComponent>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider>();

        myTransform = transform;

        currentCharge = baseCharge;
    }

    public float strongAttackChargeTime = 2.0f;

    void Update()
    {
        float h = Input.GetAxis(horizontalAxisName);

        bool jumpButtonPressed = Input.GetButtonDown(jumpButtonName);
        bool jumpButtonReleased = Input.GetButtonUp(jumpButtonName);

        bool attackButtonPressed = Input.GetButtonDown(attackButtonName);
        bool attackButtonHold = Input.GetButton(attackButtonName);
        bool attackButtonReleased = Input.GetButtonUp(attackButtonName);

        bool grounded = Grounded;

        Vector2 velocity = rb.velocity;

        bool canDoActions = movementEnabled && !inTauntAnimation && !stunned;

        if (canDoActions)
        {
            velocity.x = baseSpeed * speedCurve.Evaluate(Charge / maxCharge) * h;
        }

        Vector3 boxSize = boxCollider.size;

        animator.SetBool("grounded", grounded);
        animator.SetBool("punching", false);
        animator.SetBool("megapunch", false);

        if (canDoActions && jumpButtonPressed)
        {
            if (grounded)
            {
                velocity.y = 0.0f;
                rb.AddForce(baseJumpForce * Vector3.up, ForceMode.Impulse);

                AudioSource.PlayClipAtPoint(jumpClip, Camera.main.transform.position);
            }
        }
        else if (canDoActions && jumpButtonReleased && !grounded && velocity.y > 0.0f)
        {
            velocity.y *= 0.1f;
        }

        rb.velocity = velocity;

        if (canDoActions && attackButtonPressed)
        {
            charging = true;
            attackButtonPressedForSeconds = 0.0f;
        }

        if (canDoActions && charging && attackButtonHold)
        {
            attackButtonPressedForSeconds += Time.deltaTime;
        }

        if (canDoActions && charging && attackButtonReleased)
        {
            Hit();
        }

        Direction currentDirection = direction;

        if (canDoActions && h > 0.0f)
        {
            currentDirection = Direction.RIGHT;
        }
        else if (canDoActions && h < 0.0f)
        {
            currentDirection = Direction.LEFT;
        }

        if (canDoActions)
        {
            animator.SetBool("moving", h != 0.0f);
        }

        animator.speed = speedCurve.Evaluate(Charge / maxCharge);

        if (currentDirection != direction)
        {
            Vector3 scale = myTransform.localScale;

            if (currentDirection == Direction.LEFT)
            {
                scale.z = -baseScaleAbsValue;
            }
            else
            {
                scale.z = baseScaleAbsValue;
            }

            myTransform.localScale = scale;
            direction = currentDirection;
        }

        if (canDoActions && h == 0.0f && grounded)
        {
            if (!inTauntAnimation && Input.GetButtonDown(tauntButtonName))
            {
                Taunt();
            }
        }
    }

    public AudioClip jumpClip;

    void Hit()
    {
        if (!canAttack) { return; }

        ATTACK_TYPE attackType;
        if (attackButtonPressedForSeconds > strongAttackChargeTime)
        {
            animator.SetBool("megapunch", true);
            attackType = ATTACK_TYPE.CHARGED;
        }
        else
        {
            animator.SetBool("punching", true);
            attackType = ATTACK_TYPE.BASE;
        }

        ac.Hit(attackType);
        attackButtonPressedForSeconds = 0.0f;
        charging = false;

        canAttack = false;
        StartCoroutine(DelayBetweenAttacks());
    }

    public float delayBetweenAttacks = 1f;
    bool canAttack = true;

    IEnumerator DelayBetweenAttacks()
    {
        yield return new WaitForSeconds(delayBetweenAttacks);
        canAttack = true;
    }

    void Taunt()
    {
        inTauntAnimation = true;
        animator.SetBool("taunt", true);
        StartCoroutine(TauntTimer());
    }

    bool inTauntAnimation;

    IEnumerator TauntTimer()
    {
        yield return new WaitForSeconds(2.3f);
        inTauntAnimation = false;
        animator.SetBool("taunt", false);
    }

    public bool Grounded
    {
        get
        {
            bool grounded = false;

            Vector3 firstRayPosition = myTransform.position +
                new Vector3(boxCollider.center.z, boxCollider.center.y)
                - 0.5f * new Vector3(boxCollider.size.z, boxCollider.size.y)
                + 0.01f * Vector3.up;

            for (int i = 0; i < 4; i++)
            {

                Vector3 position = firstRayPosition + i * boxCollider.size.z * 0.3333f * Vector3.right;

                Ray ray = new Ray(
                    position,
                    Vector3.down);

                Debug.DrawRay(position, 0.1f * Vector3.down, Color.red);

                if (Physics.Raycast(
                    ray,
                    0.1f,
                    groundLayer))
                {
                    grounded = true;
                    break;
                }
            }
            return grounded;
        }
    }

    public void IncreaseCharge(float charge)
    {
        currentCharge += charge;
        currentCharge = Mathf.Clamp(currentCharge, 0.0f, maxCharge);
    }

    public void SubstractCharge(float charge)
    {
        currentCharge -= charge;
        currentCharge = Mathf.Clamp(currentCharge, 0.0f, maxCharge);
    }

    public void ResetPlayer()
    {
        movementEnabled = false;
        currentCharge = baseCharge;
    }

    bool stunned = false;

    public void OnHit()
    {
        stunned = true;
        animator.SetTrigger("rekted");
        StartCoroutine(RestoreFromStun(recoveryTimeFromBasicHit));
    }

    public void OnChargedHit()
    {
        stunned = true;
        animator.SetTrigger("rekted");
        StartCoroutine(RestoreFromStun(recoveryTimeFromChargedHit));
    }

    public void OnExplosionHit()
    {
        stunned = true;
        animator.SetBool("rekted", true);
        StartCoroutine(RestoreFromStun(recoveryTimeFromExplosion));
    }

    public float recoveryTimeFromBasicHit = 1.0f;
    public float recoveryTimeFromChargedHit = 2.0f;
    public float recoveryTimeFromExplosion = 3.0f;

    IEnumerator RestoreFromStun(float recoveryTime)
    {
        yield return new WaitForSeconds(recoveryTime);
        animator.SetBool("rekted", false);
        stunned = false;
    }
}
