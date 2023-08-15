using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class Entity : MonoBehaviour
{
    
    
    private Rigidbody _rb;
    private int[,,] _cubesInfo;
    private Vector3 _cubesInfoStartPosition;
    private Cube[] _cubes;

    private float firstAction = 0f;
    private float secondAction = 0f;
    public Cube checkingCube;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        
        _rb.useGravity = false;
        _rb.useGravity = true;
        
        _rb.mass = transform.childCount;
        CollectCubes();
        RecalculateCubes();
    }

    private void CollectCubes()
    {
        Vector3 min = Vector3.one * float.MaxValue;
        Vector3 max = Vector3.one * float.MinValue;

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            min = Vector3.Min(min, child.localPosition);
            max = Vector3.Max(max, child.localPosition);
        }

        Vector3Int delta = Vector3Int.RoundToInt(max - min);
        _cubesInfo = new int[delta.x + 1, delta.y + 1, delta.z + 1];
        _cubesInfoStartPosition = min;
        _cubes = GetComponentsInChildren<Cube>();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            Vector3Int grid = GridPosition(child.localPosition);
            _cubesInfo[grid.x, grid.y, grid.z] = i + 1;
            _cubes[i].Id = i + 1;
        }
    }

    private void RecalculateCubes()
    {
        List<int> freeCubeIds = new List<int>();
        for (int i = 0; i < _cubes.Length; i++)
        {
            if (_cubes[i] != null)
            {
                freeCubeIds.Add(_cubes[i].Id);
            }   
        }

        if (freeCubeIds.Count == 0)
        {
            Destroy(gameObject);
            return;
        }

        List<CubeGroup> groups = new List<CubeGroup>();
        int currentGroup = 0;

        while (freeCubeIds.Count > 0)
        {
            groups.Add(new CubeGroup());
            int id = freeCubeIds[0];
            groups[currentGroup].Cubes.Add(id);
            freeCubeIds.Remove(id);
            checkCube(id);
            currentGroup++;

            void checkCube(int id)
            {
                checkingCube = _cubes[id - 1];
                
                if(checkingCube.TryGetComponent<ColorCube>(out ColorCube colorCube))
                {
                    colorCube.SetColor(GizmoManager.instance.GetGroupColor(currentGroup));
                }
                // yield return new WaitForSeconds(firstAction);

                Vector3Int gridPosition = GridPosition(_cubes[id - 1].transform.localPosition);
                // Debug.Log(gridPosition);

                checkNeighbor(Vector3Int.up);
                checkingCube = _cubes[id - 1];
                checkNeighbor(Vector3Int.right);
                checkingCube = _cubes[id - 1];
                checkNeighbor(Vector3Int.down);
                checkingCube = _cubes[id - 1];
                checkNeighbor(Vector3Int.left);
                checkingCube = _cubes[id - 1];
                checkNeighbor(Vector3Int.forward);
                checkingCube = _cubes[id - 1];
                checkNeighbor(Vector3Int.back);

                void checkNeighbor(Vector3Int direction)
                {   
                    int id = GetNeighbor(gridPosition, direction);
                    if (freeCubeIds.Remove(id))
                    {
                        Debug.DrawLine(_cubes[id - 1].transform.position, _cubes[id - 1].transform.position + direction, Color.green, secondAction);
                        groups[currentGroup].Cubes.Add(id);
                        checkCube(id);
                    }
                    else
                    {
                        Debug.DrawLine(GridPointInWorld(gridPosition) , GridPointInWorld(gridPosition) + direction, Color.red, secondAction);
                        // yield return new WaitForSeconds(firstAction);
                    }
                }
            }
        }

        if (groups.Count < 2)
            return;

        for (int i = 1; i < groups.Count; i++)
        {
            GameObject entity = new GameObject("Entity");
            var firstCube = _cubes[groups[i].Cubes[0] - 1].transform;
            entity.transform.SetPositionAndRotation(firstCube.position, firstCube.rotation);
            entity.transform.localScale = firstCube.lossyScale;

            foreach (int id in groups[i].Cubes)
            {
                _cubes[id - 1].transform.parent = entity.transform;
            }

            entity.AddComponent<Entity>();
        }

        CollectCubes();
    }


    public void DetouchCube(Cube cube)
    {
        Vector3Int grid = GridPosition(cube.transform.localPosition);
        _cubesInfo[grid.x, grid.y, grid.z] = 0;
        _cubes[cube.Id - 1] = null;

        cube.transform.parent = null;
        var rb = cube.gameObject.AddComponent<Rigidbody>();

        RecalculateCubes();
    }



    private Vector3Int GridPosition(Vector3 localPosition)
    {
        return Vector3Int.RoundToInt(localPosition - _cubesInfoStartPosition);
    }

    private Vector3 GridPointInWorld(Vector3Int gridPos)
    {
        return gridPos + _cubesInfoStartPosition * transform.localScale.x;
    }

    private int GetNeighbor(Vector3Int position, Vector3Int direction)
    {
        Vector3Int gridPosition = position + direction;
        if (gridPosition.x < 0 || gridPosition.x >= _cubesInfo.GetLength(0)
            || gridPosition.y < 0 || gridPosition.y >= _cubesInfo.GetLength(1)
            || gridPosition.z < 0 || gridPosition.z >= _cubesInfo.GetLength(2))
            return 0;

        return _cubesInfo[gridPosition.x, gridPosition.y, gridPosition.z];
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
            return;


        Gizmos.matrix = transform.localToWorldMatrix;
        for (int x = 0; x < _cubesInfo.GetLength(0); x++)
        {
            for (int y = 0; y < _cubesInfo.GetLength(1); y++)
            {
                for (int z = 0; z < _cubesInfo.GetLength(2); z++)
                {
                    Vector3 position = _cubesInfoStartPosition + new Vector3(x, y, z);
                    if (_cubesInfo[x, y, z] == 0)
                    {
                        Gizmos.color = Color.green;
                        Gizmos.DrawSphere(position, 0.1f);
                    }
                    else
                    {
                        Gizmos.color = Color.red;
                        Gizmos.DrawSphere(position, 0.2f);
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if(checkingCube != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(checkingCube.transform.position, Vector3.one * 1.1f);
        }
    }
}

public class CubeGroup
{
    public List<int> Cubes = new List<int>();
}