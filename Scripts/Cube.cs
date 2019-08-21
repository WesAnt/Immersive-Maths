using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Cube : MonoBehaviour {

    

    public TextMeshPro numText;

    [System.NonSerialized] public bool hasBeenPlaced1 = false;
    [System.NonSerialized] public bool hasBeenPlaced2 = false;
    [System.NonSerialized] public bool hasBeenPlaced3 = false;

    [System.NonSerialized] public int shouldBePlaced = 0;

    [System.NonSerialized] public int num;
    [System.NonSerialized] public bool correctNumber;
    [System.NonSerialized] public bool correctNumber2Operator;
    [System.NonSerialized] public bool op1Allowed = false; // Needed for subraction and division (order of operands matters)
    [System.NonSerialized] public bool op2Allowed = false; // Needed for subraction and division (order of operands matters)

    void Awake()
    {
        correctNumber = true;
        correctNumber2Operator = false;
    }
}
