using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using UnityEngine;

public class GizmoManager : MonoBehaviour
{
    public static GizmoManager instance;
    
    public Color[] groupsColor;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public Color GetGroupColor(int groupIndex)
    {
        return groupsColor[groupIndex];
    }
}
