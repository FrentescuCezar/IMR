using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RandomNumber : MonoBehaviour
{
    public TextMeshPro largeText;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Dartboard") {
            int randomNum = Random.Range(1, 100);
            largeText.text = randomNum.ToString();

            Debug.Log(largeText.text);
        }
    }
}
