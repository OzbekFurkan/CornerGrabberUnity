using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager uiManager;

    [SerializeField] private TextMeshProUGUI black_visited_corners;
    [SerializeField] private TextMeshProUGUI white_visited_corners;
    public TextMeshProUGUI winner_text;

    private void Awake()
    {
        if (uiManager == null)
            uiManager = this;
    }

    private void Update()
    {
        white_visited_corners.text = GameHandler.gameHandler.TotalVisitedCornerAmount(Team.WHITE) + "";
        black_visited_corners.text = GameHandler.gameHandler.TotalVisitedCornerAmount(Team.BLACK) + "";
    }

}
