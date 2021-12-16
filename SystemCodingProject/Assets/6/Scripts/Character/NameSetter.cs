using System.Collections;
using System.Collections.Generic;
using Characters;
using TMPro;
using UnityEngine;

public class NameSetter : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputField;
    
    
    public void GetName()
    {
        var obj = GameObject.FindObjectsOfType<ShipController>();
        obj[obj.Length].PlayerName = _inputField.text;
    }
}
