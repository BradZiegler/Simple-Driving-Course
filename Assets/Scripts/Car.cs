using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour {
    [SerializeField] private float speed = 1f;
    [SerializeField] private float speedGainOverTime = 1f;

    void Update() {
        speed += speedGainOverTime * Time.deltaTime;
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}
