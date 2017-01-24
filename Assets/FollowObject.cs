using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour {
    private enum SizeDirection
    {
        y, x
    };

    public float CameraFollowSpeed = 5.0f;
    public float CameraZoomSpeed = 5.0f;
    public float CameraRotateSpeed = 2.0f;
    public float Padding = 12.0f;
    public float MinSize = 30.0f;
    SizeDirection _size_direction;
    public float ChangeSizeDirectionCooldown = 0.5f;
    private float _can_change_size_direction_at;
    private Vector2 _wanted_pos;
    private float _wanted_rotation;
    private float _wanted_size;
    public GameObject TrackedPlayer1;
    public GameObject TrackedPlayer2;
    public List<GameObject> TrackedOther;

    private struct RemovedTrackedGO
    {
        public Vector3 pos;
        public float end_of_life;
    };

    private List<RemovedTrackedGO> _removed;
    private Camera _camera;

    void Start()
    {
        _camera = GetComponent<Camera>();
        _removed = new List<RemovedTrackedGO>();

        _wanted_pos = new Vector2();
        _wanted_rotation = 0;
        _wanted_size = _camera.orthographicSize;
        _can_change_size_direction_at = 0;
        _size_direction = SizeDirection.x;
    }

    public void Add(GameObject t)
    {
        TrackedOther.Add(t);
    }

    public void Remove(GameObject t, float ttl)
    {
        if (!TrackedOther.Contains(t))
            return;

        TrackedOther.Remove(t);

        if (ttl == 0)
            return;

        RemovedTrackedGO rt = new RemovedTrackedGO();
        rt.pos = t.transform.position;
        rt.end_of_life = Time.time + ttl;
        _removed.Add(rt);
    }

    static private Vector2 GetWantedPos(Vector2 player1_pos, Vector2 player2_pos, List<Vector2> tracked_positions)
    {
        Vector2 first_pos = player1_pos;
        var tracked_added = new Vector2(0, 0);
        tracked_added += player2_pos - first_pos;
        var num_tracked = 2;

        for (var i = 0; i < tracked_positions.Count; ++i)
        {
            Vector2 diff = tracked_positions[i] - first_pos;
            tracked_added += diff;
            ++num_tracked;
        }

        return first_pos + tracked_added * (1.0f / num_tracked);
    }

    static Vector3 RotateAround(Vector3 point, Vector3 pivot, float angle)
    {
        return Quaternion.Euler(0, 0, -Mathf.Abs(angle)) * (point - pivot) + pivot;
    }

    static private Vector2 GetCameraBounds(Vector2 player1_pos, Vector2 player2_pos, List<Vector2> tracked_positions)
    {
        var p1 = player1_pos;
        var p2 = player2_pos;
        
        var min_x = p2.x < p1.x ? p2.x : p1.x;
        var min_y = p2.y < p1.y ? p2.y : p1.y;
        var max_x = p2.x > p1.x ? p2.x : p1.x;
        var max_y = p2.y > p1.y ? p2.y : p1.y;

        for (var i = 0; i < tracked_positions.Count; ++i)
        {
            Vector2 p = tracked_positions[i];

            if (p.x < min_x)
                min_x = p.x;

            if (p.x > max_x)
                max_x = p.x;

            if (p.y < min_y)
                min_y = p.y;

            if (p.y > max_y)
                max_y = p.y;
        }
        
        return new Vector2(max_x - min_x, max_y - min_y);
    }

    private float GetWantedSize(Vector2 player1_pos, Vector2 player2_pos, List<Vector2> tracked_positions, float padding, Camera camera, float wanted_rotation)
    {
        var diff = GetCameraBounds(player1_pos, player2_pos, tracked_positions);
        var rotated_diff = RotateAround(diff, (player1_pos + player2_pos) * 0.5f, wanted_rotation);

        var m = Padding * Mathf.Max(diff.magnitude - 70.0f, 0)/80.0f;

        var horisontal_size = rotated_diff.x + padding * 2.0f + m;
        var vertical_size = rotated_diff.y + padding * 2.0f + m;

        var cam_height = camera.orthographicSize * 2.0f;
        var cam_width = cam_height * camera.aspect;

        var aspect = camera.aspect; // a = w / h
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
        }
        
        if (Time.time > _can_change_size_direction_at && cam_height < vertical_size)
        {
            size = vertical_size;
            _can_change_size_direction_at = Time.time + ChangeSizeDirectionCooldown;
        }

        return Mathf.Max(MinSize, size) / 2.0f;
    }

    static private float GetWantedRotation(Vector2 player1_pos, Vector2 player2_pos)
    {
        var pos_diff = player2_pos - player1_pos;
        return Mathf.Atan2(pos_diff.y, pos_diff.x) * Mathf.Rad2Deg;
    }

    private void Interpolate()
    {
        _camera.orthographicSize = _camera.orthographicSize + (_wanted_size - _camera.orthographicSize) * Time.deltaTime * CameraZoomSpeed;
        _camera.transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, _wanted_rotation), Time.deltaTime * CameraRotateSpeed);
        Vector2 current_pos = transform.position;
        Vector3 distance_to_move = (_wanted_pos - current_pos) * CameraFollowSpeed * Time.deltaTime;
        transform.position += distance_to_move;
    }

    void Update()
    {
        if (Data.Victory)
            return;

        _removed = _removed.FindAll(x => Time.time < x.end_of_life);

        var tracked_positions = new List<Vector2>();
        TrackedOther.ForEach(x => tracked_positions.Add(x.transform.position));
        _removed.ForEach(x => tracked_positions.Add(x.pos));

        var player1_pos = TrackedPlayer1.transform.position;
        var player2_pos = TrackedPlayer2.transform.position;
        _wanted_pos = GetWantedPos(player1_pos, player2_pos, tracked_positions);
        _wanted_rotation = GetWantedRotation(player1_pos, player2_pos);
        _wanted_size = GetWantedSize(player1_pos, player2_pos, tracked_positions, Padding, _camera, _wanted_rotation);
        Interpolate();
    }
}
