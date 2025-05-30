using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareManager : MonoBehaviour
{
    public static Action<SquareManager> OnStoneClicked;

    [HideInInspector] public int indexX;
    [HideInInspector] public int indexY;

    [HideInInspector] public bool canMove = false;

    [SerializeField] private SpriteRenderer spriteRenderer;
    public static Color defaultColor;

    private void Awake()
    {
        defaultColor = spriteRenderer.color;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                Debug.Log("clicked on " + indexX + ", " + indexY);

                OnStoneClicked?.Invoke(this);
            }
        }
    }



}