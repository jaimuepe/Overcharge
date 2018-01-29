using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    Camera cameraMain;

    public float shake;

    float shakeAmmount = 0.45f;
    float decreaseFactor = 1.1f;

    Vector3 defaultPosition;

    bool shaking = false;

    private void Start()
    {
        cameraMain = Camera.main;
    }

    public void StartShake(float shakeTime)
    {
        shake = shakeTime;
        shaking = true;
        defaultPosition = transform.position;
    }

    public void StopShake()
    {
        shake = 0.0f;
    }

    void Update()
    {
        if (shaking && shake > 0.0f)
        {
            cameraMain.transform.localPosition = Random.insideUnitSphere * shakeAmmount;
            shake -= Time.deltaTime * decreaseFactor;
        }
        else if (shaking)
        {
            shaking = false;
            shake = 0.0f;
            transform.position = defaultPosition;
        }
    }
}
