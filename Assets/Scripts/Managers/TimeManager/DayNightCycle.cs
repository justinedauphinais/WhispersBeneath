using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public float rotationSpeed;
    public float dayLengthMinutes;

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        transform.Rotate(new Vector3(1, 0, 0) * rotationSpeed * Time.deltaTime);
    }
}
