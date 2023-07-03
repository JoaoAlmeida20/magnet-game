using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagneticObject : MonoBehaviour
{
    public float magnetStrength;
    public float rotationSpeed;
    public float diStrength;

    [HideInInspector] public bool isOn;
    [HideInInspector] public bool isRed;
    [HideInInspector] public bool isSuck;
    [HideInInspector] public PlayerController player;
    [HideInInspector] public bool isPlayer;

    // Start is called before the first frame update
    void Start()
    {
        isRed = true;

        if (TryGetComponent<PlayerController>(out player)) {
            isPlayer = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isOn) {
            isSuck = false;
        }
    }
}
