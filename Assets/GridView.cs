using UnityEngine;

public class GridDrawer : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public float cellSize = 1f;
    public Material lineMaterial;

    void Start()
    {
        DrawGrid();
    }

    void DrawGrid()
    {
        GameObject gridParent = new GameObject("Grid");

        for (int x = 0; x <= width; x++)
        {
            CreateLine(new Vector3(x * cellSize, 0, 0), new Vector3(x * cellSize, height * cellSize, 0), gridParent.transform);
        }

        for (int y = 0; y <= height; y++)
        {
            CreateLine(new Vector3(0, y * cellSize, 0), new Vector3(width * cellSize, y * cellSize, 0), gridParent.transform);
        }
    }

    void CreateLine(Vector3 start, Vector3 end, Transform parent)
    {
        GameObject lineObj = new GameObject("Line");
        lineObj.transform.parent = parent;

        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.widthMultiplier = 0.05f;
        lr.material = lineMaterial;
        lr.useWorldSpace = true;
    }
}
