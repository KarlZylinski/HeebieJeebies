using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    public float CameraFollowSpeed = 5.0f;
    public float CameraZoomSpeed = 5.0f;
    public float CameraRotateSpeed = 2.0f;
    public float Padding = 12.0f;
    public float MinSize = 30.0f;
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

    static private Vector2 GetWantedPos(List<Vector2> rotated_tracked_positions, float rotation)
    {
        var b = GetCameraBounds(rotated_tracked_positions);
        return Quaternion.Euler(0, 0, rotation) * new Vector2((b.min_x + b.max_x) / 2.0f, (b.min_y + b.max_y) / 2.0f);
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

    static private float GetWantedSize(List<Vector2> tracked_positions, float padding, float camera_aspect, float min_size)
    {
        var b = GetCameraBounds(tracked_positions);
        var diff = new Vector2(b.max_x - b.min_x, b.max_y - b.min_y) * 0.5f;
        var size = Mathf.Max(diff.x / camera_aspect + padding * diff.x, diff.y + padding * diff.y * camera_aspect);
        return Mathf.Max(size, min_size);
    }

    static private float GetWantedRotation(Vector2 player1_pos, Vector2 player2_pos)
    {
        var pos_diff = player2_pos - player1_pos;
        return Mathf.Atan2(pos_diff.y, pos_diff.x) * Mathf.Rad2Deg;
    }

    private void Interpolate()
    {
        var speed_scale = (_removed.Count + TrackedOther.Count) == 0 ? 0.3f : 1.0f;
        _camera.orthographicSize = _camera.orthographicSize + (_wanted_size - _camera.orthographicSize) * Time.deltaTime * speed_scale * CameraZoomSpeed;
        _camera.transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, _wanted_rotation), Time.deltaTime * CameraRotateSpeed);
        Vector2 current_pos = transform.position;
        Vector3 distance_to_move = (_wanted_pos - current_pos) * CameraFollowSpeed * speed_scale * Time.deltaTime;
        transform.position += distance_to_move;
    }

    void Update()
    {
        if (Data.Victory)
            return;

        _removed = _removed.FindAll(x => Time.time < x.end_of_life);

        var player1_pos = TrackedPlayer1.transform.position;
        var player2_pos = TrackedPlayer2.transform.position;

        _wanted_rotation = GetWantedRotation(player1_pos, player2_pos);
        Quaternion qr = Quaternion.Euler(0, 0, -_wanted_rotation);

        var tracked_positions = new List<Vector2>();
        tracked_positions.Add(qr * player1_pos);
        tracked_positions.Add(qr * player2_pos);

        TrackedOther.ForEach(x => tracked_positions.Add(qr * x.transform.position));
        _removed.ForEach(x => tracked_positions.Add(qr * x.pos));

        _wanted_pos = GetWantedPos(tracked_positions, _wanted_rotation);
        _wanted_size = GetWantedSize(tracked_positions, Padding, _camera.aspect, MinSize);
        Interpolate();
    }
}
