using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public AudioClip explosionClip;

    Collider sphereCollider;
    public ParticleSystem ps;

    private void Start()
    {
        sphereCollider = GetComponent<Collider>();
    }

    public float timeBeforeDeactivatingCollider = 1.0f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player p = other.GetComponent<Player>();
            p.OnExplosionHit();
        }
    }

    public void Explode()
    {
        ps.gameObject.SetActive(true);
        ps.Play();
        AudioSource.PlayClipAtPoint(explosionClip, Camera.main.transform.position);
        StartCoroutine(DeactivateCollision());

        Camera.main.GetComponent<CameraShake>().StartShake(1.0f);
    }

    IEnumerator DeactivateCollision()
    {
        yield return new WaitForSeconds(timeBeforeDeactivatingCollider);

        sphereCollider.enabled = false;

        yield return new WaitForSeconds(1.0f);

        Destroy(gameObject);
    }
}
