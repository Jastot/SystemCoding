using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Star : MonoBehaviour
{
    private Mesh _mesh;
    [SerializeField]private ColorPoint _center;
    [SerializeField,NonReorderable]private ColorPoint[] _points;
    [SerializeField]private int _frequency = 1;
    private Vector3[] _vertices;
    private Color[] _colors;
    private int[] _triangles;

    public int frequency => _frequency;
    public ColorPoint[] points => _points;
    
    
    private void Start()
    {
        UpdateMesh();
    }

    private void Reset()
    {
        UpdateMesh();
    }

    public void UpdateMesh()
    {
        GetComponent<MeshFilter>().mesh = _mesh = new Mesh();
        _mesh.name = "Star Mesh";
        if (_frequency < 1)
        {
            _frequency = 1;
        }

        _points ??= Array.Empty<ColorPoint>();
        var numberOfPoints = _frequency * _points.Length;
        if (_vertices == null || _vertices.Length != numberOfPoints + 1)
        {
            _vertices = new Vector3[numberOfPoints + 1];
            _colors = new Color[numberOfPoints + 1];
            _triangles = new int[numberOfPoints * 3];
            _mesh.Clear();
        }
        if (numberOfPoints >= 3)
        {
        
            _vertices[0] = _center.Position;
            _colors[0] = _center.Color;
            var angle = -360f / numberOfPoints;
            for (int repetitions = 0, v = 1, t = 1;
                repetitions < _frequency;
                repetitions++)
            {
                for (var p = 0; p < _points.Length; p++, v++, t += 3)
                {
                    _vertices[v] = Quaternion.Euler(0f, 0f, angle * (v - 1)) *
                                   _points[p].Position;
                    _colors[v] = _points[p].Color;
                    _triangles[t] = v;
                    _triangles[t + 1] = v + 1;
                    
                }
            }

            _triangles[_triangles.Length - 1] = 1;
        }

        _mesh.vertices = _vertices;
        _mesh.colors = _colors;
        _mesh.triangles = _triangles;
    }

    public void AddNewPoint(int index)
    {
        _points = ReSorting(index,_points.ToList());
    }

    private ColorPoint[] ReSorting(int index, List<ColorPoint> copyArray)
    {
        ColorPoint[] additional = new ColorPoint[copyArray.Count];
        copyArray.CopyTo(additional);
        copyArray.Add(new ColorPoint());
        for (int i = index; i < copyArray.Count - 1; i++)
        {
            copyArray[i + 1] = additional[i];
        }
        copyArray[index+1] = new ColorPoint();
        return copyArray.ToArray();
    }
    
    public void DeletePoint(int index)
    {
        var copyArray = _points.ToList();
        copyArray.RemoveAt(index);
        _points = copyArray.ToArray();
    }

    public void MovePointDown(int index)
    {
        if (index != _points.Length - 1)
        {
            var copyArray = _points.ToList();
            ColorPoint[] additional = new ColorPoint[copyArray.Count];
            copyArray.CopyTo(additional);
            copyArray[index + 1] = additional[index];
            copyArray[index] = additional[index + 1];
            _points = copyArray.ToArray();
        }
    }

    public void MovePointUp(int index)
    {
        if (index!=0)
        {
            var copyArray = _points.ToList();
            ColorPoint[] additional = new ColorPoint[copyArray.Count];
            copyArray.CopyTo(additional);
            copyArray[index - 1] = additional[index];
            copyArray[index] = additional[index - 1];
            _points = copyArray.ToArray();
        }
    }
}