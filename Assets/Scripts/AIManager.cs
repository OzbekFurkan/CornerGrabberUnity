using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AIManager : MonoBehaviour
{
    private void OnEnable()
    {
        GameHandler.OnWhoseTurnChanged += OnWhoseTurnChanged;
    }
    private void OnDisable()
    {
        GameHandler.OnWhoseTurnChanged -= OnWhoseTurnChanged;
    }

    private void OnWhoseTurnChanged(WhoseTurn whoseTurn)
    {
        if (whoseTurn == WhoseTurn.BLACK)
            ComputerMove();
    }

    private void ComputerMove()
    {
        StoneManager[] stones = FindObjectsOfType<StoneManager>();
        List<StoneManager> blackStones = new List<StoneManager>
    {
        GameHandler.gameHandler.blackTeam.stone1,
        GameHandler.gameHandler.blackTeam.stone2
    };

        int[] dx = { -1, -1, -1, 0, 0, 1, 1, 1 };
        int[] dy = { -1, 0, 1, -1, 1, -1, 0, 1 };

        HashSet<(int, int)> occupied = new HashSet<(int, int)>();
        foreach (var stone in stones)
            occupied.Add((stone.indexX, stone.indexY));

        // Define corners and track which are visited or already being targeted
        (int, int)[] corners = { (0, 0), (0, 4), (4, 0), (4, 4) };
        bool[] cornerVisited = GameHandler.gameHandler.blackTeam.visitedCorners;
        HashSet<int> claimedCorners = new HashSet<int>();

        // Store best move per stone
        List<(StoneManager stone, int targetX, int targetY, float score)> bestMoves = new List<(StoneManager, int, int, float)>();

        foreach (var stone in blackStones)
        {
            float stoneBestScore = float.MaxValue;
            int stoneBestX = -1, stoneBestY = -1;
            int chosenCornerIndex = -1;

            for (int dir = 0; dir < 8; dir++)
            {
                int nx = stone.indexX + dx[dir];
                int ny = stone.indexY + dy[dir];

                if (nx < 0 || nx > 4 || ny < 0 || ny > 4) continue;
                if (occupied.Contains((nx, ny))) continue;

                float minDist = float.MaxValue;
                int bestCorner = -1;

                for (int c = 0; c < 4; c++)
                {
                    if (!cornerVisited[c] && !claimedCorners.Contains(c))
                    {
                        float dist = Mathf.Abs(nx - corners[c].Item1) + Mathf.Abs(ny - corners[c].Item2);
                        if (dist < minDist)
                        {
                            minDist = dist;
                            bestCorner = c;
                        }
                    }
                }

                float score = minDist;

                if (bestCorner != -1 && nx == corners[bestCorner].Item1 && ny == corners[bestCorner].Item2)
                    score -= 2f;

                if (minDist == float.MaxValue)
                    score += 5f;

                if (score < stoneBestScore)
                {
                    stoneBestScore = score;
                    stoneBestX = nx;
                    stoneBestY = ny;
                    chosenCornerIndex = bestCorner;
                }
            }

            if (stoneBestX != -1)
            {
                if (chosenCornerIndex != -1)
                    claimedCorners.Add(chosenCornerIndex);

                bestMoves.Add((stone, stoneBestX, stoneBestY, stoneBestScore));
                occupied.Add((stoneBestX, stoneBestY)); // reserve the spot to prevent next stone from taking it
            }
        }

        if (bestMoves.Count > 0)
        {
            // Pick the move with the best score among all stones
            var bestMove = bestMoves.OrderBy(m => m.score).First();
            var gh = GameHandler.gameHandler;
            GameObject targetSquare = gh.squareMatrix[bestMove.targetX, bestMove.targetY];
            SquareManager sm = targetSquare.GetComponent<SquareManager>();
            bestMove.stone.isFocused = true;
            gh.MoveStone(sm);
            GameHandler.gameHandler.SetCorner(bestMove.stone, bestMove.targetX, bestMove.targetY);
        }
        else
        {
            GameHandler.gameHandler.whoseTurn = WhoseTurn.WHITE;
            Debug.LogWarning("AI couldn't find a valid move!");
        }
    }

}
