using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

public class TargetData : MonoBehaviour 
    {
        public Transform target;
        public RectTransform pointerUI;
        public TMP_Text distanceText;
        public bool showDistance = true;
        [HideInInspector] public Vector3 edgeOffset;
    }