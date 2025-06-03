using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class StoneManager : MonoBehaviour
{
    [HideInInspector] public int indexX;
    [HideInInspector] public int indexY;
    [HideInInspector] public Team team;
    [HideInInspector] public int id;
    /// <summary>Can stone make a move? Stone can move only when clicked once,
    /// then select the square which stone should move on.</summary>
    [HideInInspector] public bool isFocused;

}