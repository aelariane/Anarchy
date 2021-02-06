using UnityEngine;

public class FengMath
{
    public static float GetHorizontalAngle(Vector3 from, Vector3 to)
    {
        Vector3 vector = to - from;
        return -Mathf.Atan2(vector.z, vector.x) * 57.29578f;
    }

    public static Quaternion GetHorizontalRotation(Vector3 from, Vector3 to)
    {
        Vector3 vector = from - to;
        float y = -Mathf.Atan2(vector.z, vector.x) * 57.29578f;
        return Quaternion.Euler(0f, y, 0f);
    }
}