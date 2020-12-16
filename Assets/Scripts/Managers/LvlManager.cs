using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EpPathFinding.cs;

public class LvlManager : MonoBehaviour
{
    public int gridWidth = 80;
    public int gridHeight = 80;

    private Vector3 groundCenter = Vector3.zero;
    public float WorldCellSize { get; } = 1;

    public static LvlManager instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            this.enabled = false;
    }

    private BaseGrid CreateMovableGrid()
    {
        bool[][] movableMatrix = new bool[gridWidth][];
        for (int widthTrav = 0; widthTrav < gridWidth; widthTrav++)
        {
            movableMatrix[widthTrav] = new bool[gridHeight];
            for (int heightTrav = 0; heightTrav < gridHeight; heightTrav++)
            {
                movableMatrix[widthTrav][heightTrav] = true;
            }
        }

        return new StaticGrid(gridWidth, gridHeight, movableMatrix);
    }

    public List<Vector3> FindPath(Vector3 startPos, Vector3 endPos)
    {
        GridPos startGridPos = WorldPosToGrid(startPos);
        GridPos endGridPos = WorldPosToGrid(endPos);
        return FindPath(startGridPos, endGridPos);
    }

    public List<Vector3> FindPath(GridPos startPos, GridPos endPos)
    {
        BaseGrid searchGrid = CreateMovableGrid();

        JumpPointParam jpParam = new JumpPointParam(searchGrid, startPos, endPos, EndNodeUnWalkableTreatment.ALLOW, DiagonalMovement.Always, HeuristicMode.MANHATTAN);

        List<GridPos> resultPathList = JumpPointFinder.FindPath(jpParam);
        List<Vector3> worldPath = new List<Vector3>();

        for(int i = 0; i < resultPathList.Count; i++)
        {
            worldPath.Add(GridPosToWorld(resultPathList[i].x, resultPathList[i].y));
        }

        return worldPath;
    }

    public Vector3 GridPosToWorld(int x, int y)
    {
        float worldX = groundCenter.x - (gridWidth / 2f - 0.5f) * WorldCellSize + x * WorldCellSize;
        float worldY = groundCenter.y - (gridHeight / 2f - 0.5f) * WorldCellSize + y * WorldCellSize;
        return new Vector3(worldX, groundCenter.z, worldY);
    }

    public GridPos WorldPosToGrid(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt((worldPos.x - groundCenter.x + (gridWidth / 2f - 0.5f) * WorldCellSize) / WorldCellSize);
        int y = Mathf.RoundToInt((worldPos.z - groundCenter.y + (gridHeight / 2f - 0.5f) * WorldCellSize) / WorldCellSize);
        return new GridPos(x, y);
    }
}
