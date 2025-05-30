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

    /// <summary>distance to corners respectively 0,0, 0,4, 4,0, 4,4</summary>
    [HideInInspector] public float[,] distanceToCorners = new float[4, 2];

    // Start is called before the first frame update
    void Start()
    {
        UpdateDistances();
    }


    private void OnEnable()
    {

    }
    private void OnDisable()
    {

    }

    private void UpdateDistances()
    {
        distanceToCorners[0, 0] = Mathf.Abs(indexX - 0);//vertical distance
        distanceToCorners[0, 1] = Mathf.Abs(indexY - 0);//horizontal distance

        distanceToCorners[1, 0] = Mathf.Abs(indexX - 0);
        distanceToCorners[1, 1] = Mathf.Abs(indexY - 4);

        distanceToCorners[2, 0] = Mathf.Abs(indexX - 4);
        distanceToCorners[2, 1] = Mathf.Abs(indexY - 0);

        distanceToCorners[3, 0] = Mathf.Abs(indexX - 4);
        distanceToCorners[3, 1] = Mathf.Abs(indexY - 4);
    }

}