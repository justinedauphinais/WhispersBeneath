using UnityEngine;

public class SpiritHover : MonoBehaviour
{
    public float amplitude = 0.25f;
    public float frequency = 1f;
    float _y0, _phase;

    void Start()
    {
        _y0 = UnityEngine.Random.Range(1f, 2f);
        _phase = Random.value * Mathf.PI * 2f;
    }

    void Update()
    {
        Vector3 p = transform.position;
        p.y = _y0 + Mathf.Sin(Time.time * frequency + _phase) * amplitude;
        transform.position = p;
    }
}
