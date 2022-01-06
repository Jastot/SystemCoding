using System.Collections;
 using System.Collections.Generic;
 using UnityEngine;
 
 public class Tester : MonoBehaviour
 {
     [RangeAttribute(0, 20), SerializeField] private int _integer;
    [RangeAttribute(0f, 20f), SerializeField] private float _float;
    [RangeAttribute(0f, 20), SerializeField] private string _string;
 }
