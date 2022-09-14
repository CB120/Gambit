using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GridDirection
{ //to translate: X/Y are relative to the grid.
  //Pos/Neg = positive or negative direction.
  //Left/Right = 8th on left or right of straight-line when facing the direction
    XPosLeft,
    XPosRight,
    XNegLeft,
    XNegRight,
    YPosLeft,
    YPosRight,
    YNegLeft,
    YNegRight
}

public class ArcherUnit : Unit
{
    private int defaultRange;
    public override void Start()
    {
        defaultRange = attackRange;
        base.Start();
    }

    public override void MoveToCell(Cell c)
    {
        switch (c.height)
        {
            case 1: attackRange = defaultRange;
                break;
            case 2: attackRange = (int)(defaultRange * 1.25f);
                break;
            case 3: attackRange = (int)(defaultRange * 1.5f);
                break;
            case 4: attackRange = (int)(defaultRange * 1.75f);
                break;
        }
        base.MoveToCell(c);
    }

    //BEHOLD THE MIGHTY PizzaLOS, CONQUERER OF ALL HIGH-TIME-COMPLEXITY ALGORITHMS
    public override List<CellInfo> GetAttackCells()
    {
        //Create all quick-reference variables and output array
        List<CellInfo> infoList = new List<CellInfo>();
        Cell c = currentCell;
        int maxX = GridController.grid.GetLength(0);
        int maxY = GridController.grid.GetLength(1);
        int range = attackRange;
        Vector2Int v = position;

        bool[,] losFailGrid = new bool[maxX, maxY]; //true = NOT in LOS. false = in LOS, so in range. Sorry it's a bit fucky but the default value for bools is false

        //Iterate on straights
        //YPos
        for (int y = v.y + 1; y < maxY && y <= v.y + range; y++)
        {
            bool losFailed = false;
            Vector2Int losReference = new Vector2Int(v.x, y - 1); //straights will always have an angle of 0, so not diagonal
            losFailed = losFailGrid[losReference.x, losReference.y];

            if (GridController.grid[v.x, y])
            {
                CellInfo cellInfo = ResolveCellInfo(v.x, y, this, losFailed);
                infoList.Add(cellInfo);
                losFailGrid[v.x, y] = cellInfo.losFailed;
            }
            else
            {
                losFailGrid[v.x, y] = losFailed;
            }
        } 

        //YNeg
        for (int y = v.y - 1; y >= 0 && y >= v.y - range; y--)
        {
            bool losFailed = false;
            Vector2Int losReference = new Vector2Int(v.x, y + 1);
            losFailed = losFailGrid[losReference.x, losReference.y];

            if (GridController.grid[v.x, y])
            { 
                CellInfo cellInfo = ResolveCellInfo(v.x, y, this, losFailed);
                infoList.Add(cellInfo);
                losFailGrid[v.x, y] = cellInfo.losFailed;
            }
            else
            {
                losFailGrid[v.x, y] = losFailed;
            }
        }

        //XPos
        for (int x = v.x + 1; x < maxX && x <= v.x + range; x++)
        {
            bool losFailed = false;
            Vector2Int losReference = new Vector2Int(x - 1, v.y);
            losFailed = losFailGrid[losReference.x, losReference.y];

            if (GridController.grid[x, v.y])
            {
                CellInfo cellInfo = ResolveCellInfo(x, v.y, this, losFailed);
                infoList.Add(cellInfo);
                losFailGrid[x, v.y] = cellInfo.losFailed;
            }
            else
            {
                losFailGrid[x, v.y] = losFailed;
            }
        }

        //XNeg
        for (int x = v.x - 1; x >= 0 && x >= v.x - range; x--)
        {
            bool losFailed = false;
            Vector2Int losReference = new Vector2Int(x + 1, v.y);
            losFailed = losFailGrid[losReference.x, losReference.y];

            if (GridController.grid[x, v.y])
            {
                CellInfo cellInfo = ResolveCellInfo(x, v.y, this, losFailed);
                infoList.Add(cellInfo);
                losFailGrid[x, v.y] = cellInfo.losFailed;
            }
            else
            {
                losFailGrid[x, v.y] = losFailed;
            }
        }

        //Iterate on diagonals
        //YPos-XNeg
        int dx = v.x - 1;
        int dy = v.y + 1;
        while (GridController.FastMovesBetween(dx, dy, this) <= range && dx >= 0 && dy < maxY)
        {
            bool losFailed = false;
            Vector2Int losReference = new Vector2Int(dx + 1, dy - 1); //diagonals will always have an angle of 45 degrees
            losFailed = losFailGrid[losReference.x, losReference.y];

            if (GridController.grid[dx, dy])
            {
                CellInfo cellInfo = ResolveCellInfo(dx, dy, this, losFailed);
                infoList.Add(cellInfo);
                losFailGrid[dx, dy] = cellInfo.losFailed;
            }
            else
            {
                losFailGrid[dx, dy] = losFailed;
            }
            dx--;
            dy++;
        }

        //YPos-XPos
        dx = v.x + 1;
        dy = v.y + 1;
        while (GridController.FastMovesBetween(dx, dy, this) <= range && dx < maxX && dy < maxY)
        {
            bool losFailed = false;
            Vector2Int losReference = new Vector2Int(dx - 1, dy - 1);
            losFailed = losFailGrid[losReference.x, losReference.y];

            if (GridController.grid[dx, dy])
            {
                CellInfo cellInfo = ResolveCellInfo(dx, dy, this, losFailed);
                infoList.Add(cellInfo);
                losFailGrid[dx, dy] = cellInfo.losFailed;
            }
            else
            {
                losFailGrid[dx, dy] = losFailed;
            }
            dx++;
            dy++;
        }

        //YNeg-XPos
        dx = v.x + 1;
        dy = v.y - 1;
        while (GridController.FastMovesBetween(dx, dy, this) <= range && dx < maxX && dy >= 0)
        {
            bool losFailed = false;
            Vector2Int losReference = new Vector2Int(dx - 1, dy + 1);
            losFailed = losFailGrid[losReference.x, losReference.y];

            if (GridController.grid[dx, dy])
            {
                CellInfo cellInfo = ResolveCellInfo(dx, dy, this, losFailed);
                infoList.Add(cellInfo);
                losFailGrid[dx, dy] = cellInfo.losFailed;
            }
            else
            {
                losFailGrid[dx, dy] = losFailed;
            }
            dx++;
            dy--;
        }

        //YNeg-XNeg
        dx = v.x - 1;
        dy = v.y - 1;
        while (GridController.FastMovesBetween(dx, dy, this) <= range && dx >= 0 && dy >= 0)
        {
            bool losFailed = false;
            Vector2Int losReference = new Vector2Int(dx + 1, dy + 1);
            losFailed = losFailGrid[losReference.x, losReference.y];

            if (GridController.grid[dx, dy])
            {
                CellInfo cellInfo = ResolveCellInfo(dx, dy, this, losFailed);
                infoList.Add(cellInfo);
                losFailGrid[dx, dy] = cellInfo.losFailed;
            }
            else
            {
                losFailGrid[dx, dy] = losFailed;
            }
            dx--;
            dy--;
        }

        //Iterate on sectors
        //YPosLeft
        GridDirection currentDirection = GridDirection.YPosLeft;
        int dLimit = v.x - 2; //diagonal-Limit
        for (int y = v.y + 1; y < maxY && y < range + v.y; y++)
        { //YPos...
            for (int x = v.x - 1; x >= 0 && x > dLimit; x--)
            { //...iterating left!
                bool losFailed = false;
                bool losDiagonal = IsLOSDiagonal(x, y, this, currentDirection);
                Vector2Int losReference = GetLOSReference(x, y, currentDirection, losDiagonal);
                losFailed = losFailGrid[losReference.x, losReference.y];

                if (GridController.grid[x, y])
                {
                    CellInfo cellInfo = ResolveCellInfo(x, y, this, losFailed);
                    infoList.Add(cellInfo);
                    losFailGrid[x, y] = cellInfo.losFailed;
                }
                else
                {
                    losFailGrid[x, y] = losFailed;
                }
            }
            dLimit--;
        }

        //YPosRight
        currentDirection = GridDirection.YPosRight;
        dLimit = v.x + 2; //diagonal-Limit follows YPos-XPos line
        for (int y = v.y + 1; y < maxY && y < range + v.y; y++)
        {
            for (int x = v.x + 1; x < maxX && x < dLimit; x++)
            {
                bool losFailed = false;
                bool losDiagonal = IsLOSDiagonal(x, y, this, currentDirection);
                Vector2Int losReference = GetLOSReference(x, y, currentDirection, losDiagonal);
                losFailed = losFailGrid[losReference.x, losReference.y];

                if (GridController.grid[x, y])
                {
                    CellInfo cellInfo = ResolveCellInfo(x, y, this, losFailed);
                    infoList.Add(cellInfo);
                    losFailGrid[x, y] = cellInfo.losFailed;
                }
                else
                {
                    losFailGrid[x, y] = losFailed;
                }
            }
            dLimit++;
        }

        //XPosLeft
        currentDirection = GridDirection.XPosLeft;
        dLimit = v.y + 2;
        for (int x = v.x + 1; x < maxX && x < range + v.x; x++)
        {
            for (int y = v.y + 1; y < maxY && y < dLimit; y++)
            {
                bool losFailed = false;
                bool losDiagonal = IsLOSDiagonal(x, y, this, currentDirection);
                Vector2Int losReference = GetLOSReference(x, y, currentDirection, losDiagonal);
                losFailed = losFailGrid[losReference.x, losReference.y];

                if (GridController.grid[x, y])
                {
                    CellInfo cellInfo = ResolveCellInfo(x, y, this, losFailed);
                    infoList.Add(cellInfo);
                    losFailGrid[x, y] = cellInfo.losFailed;
                }
                else
                {
                    losFailGrid[x, y] = losFailed;
                }
            }
            dLimit++;
        }

        //XPosRight
        currentDirection = GridDirection.XPosRight;
        dLimit = v.y - 2;
        for (int x = v.x + 1; x < maxX && x < range + v.x; x++)
        {
            for (int y = v.y - 1; y >= 0 && y > dLimit; y--)
            {
                bool losFailed = false;
                bool losDiagonal = IsLOSDiagonal(x, y, this, currentDirection);
                Vector2Int losReference = GetLOSReference(x, y, currentDirection, losDiagonal);
                losFailed = losFailGrid[losReference.x, losReference.y];

                if (GridController.grid[x, y])
                {
                    CellInfo cellInfo = ResolveCellInfo(x, y, this, losFailed);
                    infoList.Add(cellInfo);
                    losFailGrid[x, y] = cellInfo.losFailed;
                }
                else
                {
                    losFailGrid[x, y] = losFailed;
                }
            }
            dLimit--;
        }

        //YNegLeft
        currentDirection = GridDirection.YNegLeft;
        dLimit = v.x + 2;
        for (int y = v.y - 1; y >= 0 && y > v.y - range; y--)
        {
            for (int x = v.x + 1; x < maxX && x < dLimit; x++)
            {
                bool losFailed = false;
                bool losDiagonal = IsLOSDiagonal(x, y, this, currentDirection);
                Vector2Int losReference = GetLOSReference(x, y, currentDirection, losDiagonal);
                losFailed = losFailGrid[losReference.x, losReference.y];

                if (GridController.grid[x, y])
                {
                    CellInfo cellInfo = ResolveCellInfo(x, y, this, losFailed);
                    infoList.Add(cellInfo);
                    losFailGrid[x, y] = cellInfo.losFailed;
                }
                else
                {
                    losFailGrid[x, y] = losFailed;
                }
            }
            dLimit++;
        }

        //YNegRight
        currentDirection = GridDirection.YNegRight;
        dLimit = v.x - 2;
        for (int y = v.y - 1; y >= 0 && y > v.y - range; y--)
        {
            for (int x = v.x - 1; x >= 0 && x > dLimit; x--)
            {
                bool losFailed = false;
                bool losDiagonal = IsLOSDiagonal(x, y, this, currentDirection);
                Vector2Int losReference = GetLOSReference(x, y, currentDirection, losDiagonal);
                losFailed = losFailGrid[losReference.x, losReference.y];

                if (GridController.grid[x, y])
                {
                    CellInfo cellInfo = ResolveCellInfo(x, y, this, losFailed);
                    infoList.Add(cellInfo);
                    losFailGrid[x, y] = cellInfo.losFailed;
                }
                else
                {
                    losFailGrid[x, y] = losFailed;
                }
            }
            dLimit--;
        }

        //XNegLeft
        currentDirection = GridDirection.XNegLeft;
        dLimit = v.y - 1;
        for (int x = v.x - 1; x >= 0 && x > v.x - range; x--)
        {
            for (int y = v.y - 1; y >= 0 && y > dLimit; y--)
            {
                bool losFailed = false;
                bool losDiagonal = IsLOSDiagonal(x, y, this, currentDirection);
                Vector2Int losReference = GetLOSReference(x, y, currentDirection, losDiagonal);
                losFailed = losFailGrid[losReference.x, losReference.y];

                if (GridController.grid[x, y])
                {
                    CellInfo cellInfo = ResolveCellInfo(x, y, this, losFailed);
                    infoList.Add(cellInfo);
                    losFailGrid[x, y] = cellInfo.losFailed;
                }
                else
                {
                    losFailGrid[x, y] = losFailed;
                }
            }
            dLimit--;
        }

        //XNegRight
        currentDirection = GridDirection.XNegRight;
        dLimit = v.y + 1;
        for (int x = v.x - 1; x >= 0 && x > v.x - range; x--)
        {
            for (int y = v.y + 1; y < maxY && y < dLimit; y++)
            {
                bool losFailed = false;
                bool losDiagonal = IsLOSDiagonal(x, y, this, currentDirection);
                Vector2Int losReference = GetLOSReference(x, y, currentDirection, losDiagonal);
                losFailed = losFailGrid[losReference.x, losReference.y];

                if (GridController.grid[x, y])
                {
                    CellInfo cellInfo = ResolveCellInfo(x, y, this, losFailed);
                    infoList.Add(cellInfo);
                    losFailGrid[x, y] = cellInfo.losFailed;
                }
                else
                {
                    losFailGrid[x, y] = losFailed;
                }
            }
            dLimit++;
        }

        return infoList;
    }

    CellInfo ResolveCellInfo(int x, int y, Unit u, bool losFailed)
    {
        //pre-check variables (I am going anal-efficient here)
        Cell c = currentCell;
        int fastMoves = GridController.grid[x, y].FastMovesTo(c);
        bool inRange = false;
        CellState state = CellState.Inactive;

        //validity checks - essentially if all are true, the cell is a valid space. if not, it isn't
        //range
        bool height = (GridController.grid[x, y].height <= c.height);
        bool distance = (fastMoves <= attackRange);

        //owner
        bool owner = true;
        if (GridController.grid[x, y].currentUnit) owner = (GridController.grid[x, y].currentUnit.ownerParticipant != ownerParticipant);

        //outpost
        bool outpost = true;
        if (GridController.grid[x, y].mode == CellMode.AttackOnly) outpost = !OutpostObjective.IsOutpostDead(GridController.grid[x, y]);

        //forest
        bool forest = true;
        if (GridController.grid[x, y].isForest) forest = (GridController.grid[x, y].FastMovesTo(c) <= World.GetRules().maxDistanceFromForestToAttack);

        //Line of Sight
        bool los = (!losFailed && height);

        //Final Info checks
        if (height && distance && owner && outpost && forest && los) inRange = true;
        if (inRange) state = CellState.AttackActive;

        //Forest outline stuff
        if (!forest)
        {
            if (GridController.grid[x, y].currentUnit) Unit.SetLayerRecursively(GridController.grid[x, y].currentUnit.gameObject, 0, true);
        }

        //Create our output object
        return new CellInfo(GridController.grid[x, y], state, inRange, !los); //height must be inverted because C#'s default bool value is false
    }

    static bool IsLOSDiagonal(int x, int y, Unit u, GridDirection d)
    { //there's a way to do this without providing the direction, for later
        int latOffset; //if facing in the direction of iteration, latitudinal offset is the distance left and right
        int longOffset; //if facing in the direction of iteration, longitudinal offset is the distance forwards from the Unit

        //to make the atan maths easier to understand (I guess? this is all very confusing, sorry)
        if (d == GridDirection.XPosLeft || d == GridDirection.XPosRight || d == GridDirection.XNegLeft || d == GridDirection.XNegRight)
        {
            latOffset = Mathf.Abs(u.position.y - y);
            longOffset = Mathf.Abs(u.position.x - x);
        }
        else
        {
            longOffset = Mathf.Abs(u.position.y - y);
            latOffset = Mathf.Abs(u.position.x - x);
        }

        float angle = Mathf.Rad2Deg * Mathf.Atan(latOffset / longOffset);

        return angle >= 22.5f;
    }

    static Vector2Int GetLOSReference(int x, int y, GridDirection d, bool diagonal)
    {
        if (diagonal)
        {
            switch (d)
            {
                case GridDirection.XPosLeft:
                    return new Vector2Int(x - 1, y - 1); //diagonals return the previous longitudinal row, and the direction opposite to its 'side' (so if ...Left, it will return the Cell diagonally-down-right)
                case GridDirection.XPosRight:
                    return new Vector2Int(x - 1, y + 1);
                case GridDirection.XNegLeft:
                    return new Vector2Int(x + 1, y + 1);
                case GridDirection.XNegRight:
                    return new Vector2Int(x + 1, y - 1);
                case GridDirection.YPosLeft:
                    return new Vector2Int(x + 1, y - 1);
                case GridDirection.YPosRight:
                    return new Vector2Int(x - 1, y - 1);
                case GridDirection.YNegLeft:
                    return new Vector2Int(x - 1, y + 1);
                case GridDirection.YNegRight:
                    return new Vector2Int(x + 1, y + 1);
            }
        }
        else
        {
            switch (d)
            {
                case GridDirection.XPosLeft:
                    return new Vector2Int(x - 1, y); //straights return the previous longitudinal row and its current latitudinal position
                case GridDirection.XPosRight:
                    return new Vector2Int(x - 1, y);
                case GridDirection.XNegLeft:
                    return new Vector2Int(x + 1, y);
                case GridDirection.XNegRight:
                    return new Vector2Int(x + 1, y);
                case GridDirection.YPosLeft:
                    return new Vector2Int(x, y - 1);
                case GridDirection.YPosRight:
                    return new Vector2Int(x, y - 1);
                case GridDirection.YNegLeft:
                    return new Vector2Int(x, y + 1);
                case GridDirection.YNegRight:
                    return new Vector2Int(x, y + 1);
            }
        }
        return new Vector2Int(-1, -1);
    }
}
