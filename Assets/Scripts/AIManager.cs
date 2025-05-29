using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        // Get all black stones
        StoneManager[] stones = FindObjectsOfType<StoneManager>();
        List<StoneManager> blackStones = new List<StoneManager>();
        foreach (var stone in stones)
        {
            if (stone.team == Team.BLACK)
                blackStones.Add(stone);
        }

        // Directions for 8 neighbors
        int[] dx = {-1, -1, -1, 0, 0, 1, 1, 1};
        int[] dy = {-1, 0, 1, -1, 1, -1, 0, 1};

        float bestScore = float.MaxValue;
        StoneManager bestStone = null;
        int bestTargetX = -1, bestTargetY = -1;

        // Get all stone positions to avoid collisions
        HashSet<(int, int)> occupied = new HashSet<(int, int)>();
        foreach (var stone in stones)
            occupied.Add((stone.indexX, stone.indexY));

        // Try each black stone
        foreach (var stone in blackStones)
        {
            int validMoves = 0;
            float stoneBestScore = float.MaxValue;
            int stoneBestX = -1, stoneBestY = -1;

            // Try all possible moves for this stone
            for (int dir = 0; dir < 8; dir++)
            {
                int nx = stone.indexX + dx[dir];
                int ny = stone.indexY + dy[dir];
                
                // Check if move is valid
                if (nx < 0 || nx > 4 || ny < 0 || ny > 4) continue;
                if (occupied.Contains((nx, ny))) continue;
                
                validMoves++;

                // Heuristic: distance to nearest unvisited corner
                float minDist = float.MaxValue;
                int bestCorner = -1;
                (int, int)[] corners = { (0,0), (0,4), (4,0), (4,4) };
                for (int c = 0; c < 4; c++)
                {
                    if (!stone.visitedCorners[c])
                    {
                        float dist = Mathf.Abs(nx - corners[c].Item1) + Mathf.Abs(ny - corners[c].Item2);
                        if (dist < minDist)
                        {
                            minDist = dist;
                            bestCorner = c;
                        }
                    }
                }

                // Calculate score for this move
                float score = minDist;
                
                // Bonus if move lands on an unvisited corner
                if (bestCorner != -1 && nx == corners[bestCorner].Item1 && ny == corners[bestCorner].Item2)
                    score -= 2f; // Strongly prefer landing on a new corner
                
                // Penalty if stone has no unvisited corners left
                if (minDist == float.MaxValue)
                    score += 5f;

                // Update best move for this stone
                if (score < stoneBestScore)
                {
                    stoneBestScore = score;
                    stoneBestX = nx;
                    stoneBestY = ny;
                }
            }

            // If this stone has valid moves and its best move is better than current best
            if (validMoves > 0 && stoneBestScore < bestScore)
            {
                bestScore = stoneBestScore;
                bestStone = stone;
                bestTargetX = stoneBestX;
                bestTargetY = stoneBestY;
            }
        }

        // Make the best move found
        if (bestStone != null)
        {
            var gh = GameHandler.gameHandler;
            GameObject targetSquare = gh.squareMatrix[bestTargetX, bestTargetY];
            SquareManager sm = targetSquare.GetComponent<SquareManager>();
            bestStone.isFocused = true;
            gh.MoveStone(sm);
            bestStone.UpdateVisitedCorners();
        }
        else
        {
            Debug.LogWarning("AI couldn't find a valid move!");
        }
    }

}
