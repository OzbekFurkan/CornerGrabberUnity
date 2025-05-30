using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Team { WHITE, BLACK }

public class TeamManager
{
    public Team team;
    public bool[] visitedCorners = new bool[4];
    public StoneManager stone1;
    public StoneManager stone2;

    public TeamManager(Team team)
    {
        this.team = team;
        visitedCorners[0] = false;
        visitedCorners[1] = false;
        visitedCorners[2] = false;
        visitedCorners[3] = false;
    }

}