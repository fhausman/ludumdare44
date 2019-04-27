using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakebaleController : MonoBehaviour
{
    public GameObject keyObject;

    private GameObject instance;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player")
            return;

        keyObject.transform.position = transform.position + Vector3.up * 1f;
        instance = Instantiate(keyObject);
        instance.transform.parent = transform;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player")
            return;

        Destroy(instance);
    }
}
