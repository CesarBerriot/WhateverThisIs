using System.Collections.Generic;
using UnityEngine;
using SF = UnityEngine.SerializeField;

public class Illusion : MonoBehaviour
{
    [SF] List<Transform> objects = new();
    [SF, Range(.5f, 10f)] float radius = 1f;
    [SF] bool debug = true;
    [SF, Range(.05f, 1f)] float debugSpheresRadius = .2f;
    List<Vector2> positions = new();
    List<Vector2> oppositePositions = new();
    List<float> interpolations = new();
    void Update()
    {
        if (objects.Count < 1)
            return;
        ComputePositions();
        ComputeOppositePositions();
        ComputeInterpolations();
        ApplyInterpolations();
    }
    void ComputePositions()
    {
        positions.Clear();
        float x, y;
        int _count = objects.Count;
        float _anglePerObject = Mathf.Deg2Rad * 180 / _count;
        for (int i = 0; i < _count; i++)
        {
            x = Mathf.Sin(_anglePerObject * i);
            y = Mathf.Cos(_anglePerObject * i);
            positions.Add(new Vector2(x, y) * radius);
        }
    }
    void ComputeOppositePositions()
    {
        oppositePositions.Clear();
        foreach (Vector2 _position in positions)
            oppositePositions.Add(-_position);
    }
    Vector3 PositionToWorld(Vector2 _position) => transform.position + transform.up * _position.y + transform.right * _position.x;
    void ComputeInterpolations()
    {
        interpolations.Clear();
        float _time = (float)Time.timeAsDouble;
        int _count = objects.Count;
        float _part = 1f / _count / 2f;
        for (int i = 0; i < _count; i++)
            interpolations.Add(Mathf.Sin(_time + i * 6f * _part) / 2f + .5f);
    }
    void ApplyInterpolations()
    {
        for (int i = 0; i < objects.Count; i++)
            if (objects[i])
                objects[i].transform.position = Vector3.Lerp(PositionToWorld(positions[i]), PositionToWorld(oppositePositions[i]), interpolations[i]);
    }
    private void OnDrawGizmos()
    {
        if (!debug)
            return;
        Gizmos.color = Color.blue;
        foreach (Vector2 _position in positions)
            Gizmos.DrawSphere(PositionToWorld(_position), debugSpheresRadius);
        Gizmos.color = Color.red;
        foreach (Vector2 _position in oppositePositions)
            Gizmos.DrawSphere(PositionToWorld(_position), debugSpheresRadius);
        Gizmos.color = Color.green;
        for (int i = 0; i < interpolations.Count; i++)
            Gizmos.DrawSphere(Vector3.Lerp(PositionToWorld(positions[i]), PositionToWorld(oppositePositions[i]), interpolations[i]), debugSpheresRadius);
    }
}
