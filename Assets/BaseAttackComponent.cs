using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAttackComponent : MonoBehaviour
{
    AttackComponent attackComponent;

    void Start()
    {
        attackComponent = GetComponentInParent<AttackComponent>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            attackComponent.HitBall(other.GetComponent<Ball>());
        }
        else if (other.CompareTag("Player"))
        {
            attackComponent.HitPlayer(other.GetComponent<Player>());
        }
    }
}
