using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CashController : MonoBehaviour
{
    public static int Cash = 0;
    TextMeshProUGUI textMesh;

    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        textMesh.text = Cash + "$";
    }
}
