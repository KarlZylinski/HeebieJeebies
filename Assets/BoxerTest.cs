using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxerTest : MonoBehaviour
{
    public GameObject TrackedPlayer1;
    public GameObject TrackedPlayer2;
    public List<GameObject> TrackedOther;
    private const float MinSize = 15.0f;
    private float _vert_size;
    private const float Aspect = 1.777777777777778f;
    private float _rotation = 0;
    private Vector2 _wanted_pos;
    public bool EnableTrackOther = false;

    void Start()
    {
        _vert_size = 5;
        _wanted_pos = new Vector2();
        _rotation = 0;
    }

    void Update()
    {
        var player1_pos = TrackedPlayer1.transform.position;
        var player2_pos = TrackedPlayer2.transform.position;
        var tracked_positions = new List<Vector2>();
        tracked_positions.Add(player1_pos);
        tracked_positions.Add(player2_pos);
        if (EnableTrackOther)
            TrackedOther.ForEach(x => tracked_positions.Add(x.transform.position));
        
        _rotation = GetWantedRotation(player1_pos, player2_pos);

        List<Vector2> rotated_tracked_positions = new List<Vector2>();
        foreach (var t in tracked_positions)
            rotated_tracked_positions.Add(Quaternion.Euler(0, 0, -_rotation) * t);

        var size = GetWantedSize(rotated_tracked_positions, 0, Aspect, MinSize);
        _vert_size = Mathf.Max(size.x, size.y) / 2.0f;
        _wanted_pos = GetWantedPos(rotated_tracked_positions, _rotation);

        var hor_size = _vert_size * Aspect;
        var x1 = _wanted_pos.x - hor_size;
        var x2 = _wanted_pos.x + hor_size;
        var y1 = _wanted_pos.y + _vert_size;
        var y2 = _wanted_pos.y - _vert_size;

        var x1y1 = RotateAround(new Vector2(x1, y1), _wanted_pos, _rotation);
        var x2y1 = RotateAround(new Vector2(x2, y1), _wanted_pos, _rotation);
        var x2y2 = RotateAround(new Vector2(x2, y2), _wanted_pos, _rotation);
        var x1y2 = RotateAround(new Vector2(x1, y2), _wanted_pos, _rotation);

        Debug.DrawLine(x1y1, x2y1);
        Debug.DrawLine(x2y1, x2y2);
        Debug.DrawLine(x1y2, x2y2);
        Debug.DrawLine(x1y1, x1y2);
    }
    
    static private Vector2 GetWantedPos(List<Vector2> rotated_tracked_positions, float rotation)
    {
        var b = GetCameraBounds(rotated_tracked_positions);
        return Quaternion.Euler(0, 0, rotation) * new Vector2((b.min_x + b.max_x) / 2.0f, (b.min_y + b.max_y) / 2.0f);
    }

    static Vector2 RotateAround(Vector2 point, Vector2 pivot, float angle)
    {
        Vector3 diff = point - pivot;
        Vector3 p3 = pivot;
        return Quaternion.Euler(0, 0, angle) * diff + p3;
    }

    struct Bounds2D
    {
        public Bounds2D(float min_x, float min_y, float max_x, float max_y)
        {
            this.min_x = min_x;
            this.min_y = min_y;
            this.max_x = max_x;
            this.max_y = max_y;
        }

        public float min_x, min_y, max_x, max_y;
    }

    static private Bounds2D GetCameraBounds(List<Vector2> tracked_positions)
    {
        if (tracked_positions.Count < 2)
            return new Bounds2D(0, 0, 0, 0);

        var tp1 = tracked_positions[0];
        var tp2 = tracked_positions[1];

        var b = new Bounds2D(
            tp1.x < tp2.x ? tp1.x : tp2.x,
            tp1.y < tp2.y ? tp1.y : tp2.y,
            tp1.x > tp2.x ? tp1.x : tp2.x,
            tp1.y > tp2.y ? tp1.y : tp2.y);

        for (var i = 2; i < tracked_positions.Count; ++i)
        {
            Vector2 p = tracked_positions[i];

            if (p.x < b.min_x)
                b.min_x = p.x;

            if (p.x > b.max_x)
                b.max_x = p.x;

            if (p.y < b.min_y)
                b.min_y = p.y;

            if (p.y > b.max_y)
                b.max_y = p.y;
        }

        return b;
    }

    static private Vector2 GetWantedSize(List<Vector2> tracked_positions, float padding, float camera_aspect, float min_size)
    {
        var b = GetCameraBounds(tracked_positions);
        var diff = new Vector2(b.max_x - b.min_x, b.max_y - b.min_y);
        //var rotated_diff = RotateAround(diff, GetPointBetween(player1_pos, player2_pos), wanted_rotation);

        //var m = Padding * Mathf.Max(diff.magnitude - 70.0f, 0)/50.0f;

        return new Vector2(diff.x / camera_aspect + padding, diff.y + padding);
        //var size = horisontal_size;

        /*var cam_height = camera_size;
        var cam_width = cam_height * camera_aspect;

        var aspect = camera_aspect; // a = w / h
        var size = 0.0f; // h = w / a



        if (_size_direction == SizeDirection.x)
        {
            if (Time.time > _can_change_size_direction_at && cam_height < vertical_size)
            {
                size = vertical_size;
                _can_change_size_direction_at = Time.time + ChangeSizeDirectionCooldown;
                _size_direction = SizeDirection.y;
            }
            else
            {
                size = horisontal_size / aspect;
            }
        }
        else
        {
            if (Time.time > _can_change_size_direction_at && cam_width < horisontal_size)
            {
                size = horisontal_size / aspect;
                _can_change_size_direction_at = Time.time + ChangeSizeDirectionCooldown;
                _size_direction = SizeDirection.x;
            }
            else
            {
                size = vertical_size;
            }
        }*/
    }

    static private float GetWantedRotation(Vector2 player1_pos, Vector2 player2_pos)
    {
        var pos_diff = player2_pos - player1_pos;
        return Mathf.Atan2(pos_diff.y, pos_diff.x) * Mathf.Rad2Deg;
    }
}
