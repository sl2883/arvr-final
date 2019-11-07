﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectExploder : MonoBehaviour
{
    private IEnumerator coroutine;

    [SerializeField]
    public GameObject mainHitObject;

    [SerializeField]
    public GameObject particlePrefab;

    [SerializeField]
    public GameObject referencePoint;

    public int particleCount;

    public float particleMinSize, particleMaxSize;
    private Vector3 basePos;
    private Quaternion baseRot;

    private bool alreadyExploded;
    // Start is called before the first frame update
    void Start()
    {
        alreadyExploded = false;

        basePos = referencePoint.transform.position;
        baseRot = referencePoint.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (!alreadyExploded)
        {
            coroutine = ExplodeHitObject(10.0f);
            StartCoroutine(coroutine);
        }
    }

    IEnumerator ExplodeHitObject(float waitTime)
    {
        alreadyExploded = true;
        yield return new WaitForSeconds(waitTime);
        Exploding();
        
    }

    /*
     * private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("hitItem"))
        {
            Exploding();
        }
    }
    */

    void Exploding()
    {
        Explode();
        mainHitObject.SetActive(false);
        Destroy(mainHitObject);
        alreadyExploded = true;
    }

    void Explode()
    {
        GameObject clone;
        for (int i = 0; i < particleCount; i++)
        {
            clone = Instantiate(particlePrefab, basePos, baseRot);
            clone.transform.localScale = Random.Range(particleMinSize, particleMaxSize)
                                                                * particlePrefab.gameObject.transform.localScale;
        }
    }
}
