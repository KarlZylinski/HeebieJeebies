using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour {
    public float CameraFollowSpeed = 5.0f;
    public float CameraZoomSpeed = 5.0f;
    public List<GameObject> Tracked;

    private struct RemovedTrackedGO
    {
        public Vector3 pos;
        public float end_of_life;
    };

    private List<RemovedTrackedGO> _removed;
    private Camera _camera;
    private float _original_camera_size;

    void Start()
    {
        _camera = GetComponent<Camera>();
        _removed = new List<RemovedTrackedGO>();
        _original_camera_size = _camera.orthographicSize;
    }

    public void Add(GameObject t)
    {
        Tracked.Add(t);
    }

    public void Remove(GameObject t, float ttl)
    {
        if (!Tracked.Contains(t))
            return;

        Tracked.Remove(t);

        if (ttl == 0)
            return;

        RemovedTrackedGO rt = new RemovedTrackedGO();
        rt.pos = t.transform.position;
        rt.end_of_life = Time.time + ttl;
        _removed.Add(rt);
    }

    private Vector2 CalculateWantedPos()
    {
        if (Tracked.Count == 0)
            return new Vector2(0, 0);

        var first_pos = Tracked[0].transform.position;
        var num_tracked = 1;
        var tracked_added = new Vector2(0, 0);

        for (var i = 1; i < Tracked.Count; ++i)
        {
            Vector2 diff = Tracked[i].transform.position - first_pos;
            tracked_added += diff;
            ++num_tracked;
        }

        _removed = _removed.FindAll(x => Time.time < x.end_of_life);

        for (var i = 0; i < _removed.Count; ++i)
        {
            Vector2 diff = _removed[i].pos - first_pos;
            tracked_added += diff;
            ++num_tracked;
        }

        Vector2 first_pos_v2 = first_pos;
        return first_pos_v2 + (num_tracked == 0 ? new Vector2(0, 0) : (tracked_added * (1.0f / num_tracked)));
    }

    private float GetNewCameraSize()
    {
        if ((Tracked.Count + _removed.Count) < 2)
        {
            return _camera.orthographicSize + (_original_camera_size - _camera.orthographicSize) * CameraZoomSpeed / 5.0f * Time.deltaTime;
        }

        Vector2 f = Tracked[0].transform.position;

        float min_x = f.x;
        float min_y = f.y;
        float max_x = f.x;
        float max_y = f.y;

        for (var i = 1; i < Tracked.Count; ++i)
        {
            Vector2 p = Tracked[i].transform.position;

            if (p.x < min_x)
                min_x = p.x;

            if (p.x > max_x)
                max_x = p.x;

            if (p.y < min_y)
                min_y = p.y;

            if (p.y > max_y)
                max_y = p.y;
        }

        for (var i = 0; i < _removed.Count; ++i)
        {
            Vector2 p = _removed[i].pos;

            if (p.x < min_x)
                min_x = p.x;

            if (p.x > max_x)
                max_x = p.x;

            if (p.y < min_y)
                min_y = p.y;

            if (p.y > max_y)
                max_y = p.y;
        }

        float y_diff = (max_y - min_y);
        float x_diff = (max_x - min_x);

        return _camera.orthographicSize + (Mathf.Max(y_diff, x_diff)/1.5f - _camera.orthographicSize) * Time.deltaTime * CameraZoomSpeed;
    }

    void Update()
    {
        if (Data.Victory)
            return;

        var wanted_pos = CalculateWantedPos();
        _camera.orthographicSize = Mathf.Clamp(GetNewCameraSize(), _original_camera_size, Mathf.Infinity);
        Vector2 current_pos = transform.position;
        var distance_to_wanted = wanted_pos - current_pos;
        var distance_to_move = distance_to_wanted * CameraFollowSpeed * Time.deltaTime;
        transform.position += new Vector3(distance_to_move.x, distance_to_move.y, 0);
    }
}
