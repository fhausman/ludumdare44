using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeabaleController : MonoBehaviour
{
    public GameObject keyObject;
    public bool notTakeable = false;
    private GameObject instance = null;

    public void DestroyInstance()
    {
        Destroy(instance);
        instance = null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player" || instance != null || notTakeable)
            return;

        keyObject.transform.position = transform.position + Vector3.up * 1f;
        instance = Instantiate(keyObject);
        instance.transform.parent = transform;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player")
            return;

        DestroyInstance();
    }
}
