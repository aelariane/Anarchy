using Optimization.Caching;
using UnityEngine;

public static class NGUIMath
{
    private static float DistancePointToLineSegment(Vector2 point, Vector2 a, Vector2 b)
    {
        float sqrMagnitude = (b - a).sqrMagnitude;
        if (sqrMagnitude == 0f)
        {
            return (point - a).magnitude;
        }
        float num = Vector2.Dot(point - a, b - a) / sqrMagnitude;
        if (num < 0f)
        {
            return (point - a).magnitude;
        }
        if (num > 1f)
        {
            return (point - b).magnitude;
        }
        Vector2 b2 = a + num * (b - a);
        return (point - b2).magnitude;
    }

    public static Vector3 ApplyHalfPixelOffset(Vector3 pos)
    {
        RuntimePlatform platform = Application.platform;
        if (platform == RuntimePlatform.WindowsPlayer || platform == RuntimePlatform.WindowsWebPlayer || platform == RuntimePlatform.WindowsEditor || platform == RuntimePlatform.XBOX360)
        {
            pos.x -= 0.5f;
            pos.y += 0.5f;
        }
        return pos;
    }

    public static Vector3 ApplyHalfPixelOffset(Vector3 pos, Vector3 scale)
    {
        RuntimePlatform platform = Application.platform;
        if (platform == RuntimePlatform.WindowsPlayer || platform == RuntimePlatform.WindowsWebPlayer || platform == RuntimePlatform.WindowsEditor || platform == RuntimePlatform.XBOX360)
        {
            if (Mathf.RoundToInt(scale.x) == Mathf.RoundToInt(scale.x * 0.5f) * 2)
            {
                pos.x -= 0.5f;
            }
            if (Mathf.RoundToInt(scale.y) == Mathf.RoundToInt(scale.y * 0.5f) * 2)
            {
                pos.y += 0.5f;
            }
        }
        return pos;
    }

    public static Bounds CalculateAbsoluteWidgetBounds(Transform trans)
    {
        UIWidget[] componentsInChildren = trans.GetComponentsInChildren<UIWidget>();
        if (componentsInChildren.Length == 0)
        {
            return new Bounds(trans.position, Vectors.zero);
        }
        Vector3 vector = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3 vector2 = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        int i = 0;
        int num = componentsInChildren.Length;
        while (i < num)
        {
            UIWidget uiwidget = componentsInChildren[i];
            Vector2 a = uiwidget.relativeSize;
            Vector2 pivotOffset = uiwidget.pivotOffset;
            float num2 = (pivotOffset.x + 0.5f) * a.x;
            float num3 = (pivotOffset.y - 0.5f) * a.y;
            a *= 0.5f;
            Transform cachedTransform = uiwidget.cachedTransform;
            Vector3 lhs = cachedTransform.TransformPoint(new Vector3(num2 - a.x, num3 - a.y, 0f));
            vector2 = Vector3.Max(lhs, vector2);
            vector = Vector3.Min(lhs, vector);
            lhs = cachedTransform.TransformPoint(new Vector3(num2 - a.x, num3 + a.y, 0f));
            vector2 = Vector3.Max(lhs, vector2);
            vector = Vector3.Min(lhs, vector);
            lhs = cachedTransform.TransformPoint(new Vector3(num2 + a.x, num3 - a.y, 0f));
            vector2 = Vector3.Max(lhs, vector2);
            vector = Vector3.Min(lhs, vector);
            lhs = cachedTransform.TransformPoint(new Vector3(num2 + a.x, num3 + a.y, 0f));
            vector2 = Vector3.Max(lhs, vector2);
            vector = Vector3.Min(lhs, vector);
            i++;
        }
        Bounds result = new Bounds(vector, Vectors.zero);
        result.Encapsulate(vector2);
        return result;
    }

    public static Bounds CalculateRelativeInnerBounds(Transform root, UISprite sprite)
    {
        if (sprite.type == UISprite.Type.Sliced)
        {
            Matrix4x4 worldToLocalMatrix = root.worldToLocalMatrix;
            Vector2 a = sprite.relativeSize;
            Vector2 pivotOffset = sprite.pivotOffset;
            Transform cachedTransform = sprite.cachedTransform;
            float num = (pivotOffset.x + 0.5f) * a.x;
            float num2 = (pivotOffset.y - 0.5f) * a.y;
            a *= 0.5f;
            float x = cachedTransform.localScale.x;
            float y = cachedTransform.localScale.y;
            Vector4 border = sprite.border;
            if (x != 0f)
            {
                border.x /= x;
                border.z /= x;
            }
            if (y != 0f)
            {
                border.y /= y;
                border.w /= y;
            }
            float x2 = num - a.x + border.x;
            float x3 = num + a.x - border.z;
            float y2 = num2 - a.y + border.y;
            float y3 = num2 + a.y - border.w;
            Vector3 vector = new Vector3(x2, y2, 0f);
            vector = cachedTransform.TransformPoint(vector);
            vector = worldToLocalMatrix.MultiplyPoint3x4(vector);
            Bounds result = new Bounds(vector, Vectors.zero);
            vector = new Vector3(x2, y3, 0f);
            vector = cachedTransform.TransformPoint(vector);
            vector = worldToLocalMatrix.MultiplyPoint3x4(vector);
            result.Encapsulate(vector);
            vector = new Vector3(x3, y3, 0f);
            vector = cachedTransform.TransformPoint(vector);
            vector = worldToLocalMatrix.MultiplyPoint3x4(vector);
            result.Encapsulate(vector);
            vector = new Vector3(x3, y2, 0f);
            vector = cachedTransform.TransformPoint(vector);
            vector = worldToLocalMatrix.MultiplyPoint3x4(vector);
            result.Encapsulate(vector);
            return result;
        }
        return NGUIMath.CalculateRelativeWidgetBounds(root, sprite.cachedTransform);
    }

    public static Bounds CalculateRelativeWidgetBounds(Transform root, Transform child)
    {
        UIWidget[] componentsInChildren = child.GetComponentsInChildren<UIWidget>();
        if (componentsInChildren.Length == 0)
        {
            return new Bounds(Vectors.zero, Vectors.zero);
        }
        Vector3 vector = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3 vector2 = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        Matrix4x4 worldToLocalMatrix = root.worldToLocalMatrix;
        int i = 0;
        int num = componentsInChildren.Length;
        while (i < num)
        {
            UIWidget uiwidget = componentsInChildren[i];
            Vector2 a = uiwidget.relativeSize;
            Vector2 pivotOffset = uiwidget.pivotOffset;
            Transform cachedTransform = uiwidget.cachedTransform;
            float num2 = (pivotOffset.x + 0.5f) * a.x;
            float num3 = (pivotOffset.y - 0.5f) * a.y;
            a *= 0.5f;
            Vector3 vector3 = new Vector3(num2 - a.x, num3 - a.y, 0f);
            vector3 = cachedTransform.TransformPoint(vector3);
            vector3 = worldToLocalMatrix.MultiplyPoint3x4(vector3);
            vector2 = Vector3.Max(vector3, vector2);
            vector = Vector3.Min(vector3, vector);
            vector3 = new Vector3(num2 - a.x, num3 + a.y, 0f);
            vector3 = cachedTransform.TransformPoint(vector3);
            vector3 = worldToLocalMatrix.MultiplyPoint3x4(vector3);
            vector2 = Vector3.Max(vector3, vector2);
            vector = Vector3.Min(vector3, vector);
            vector3 = new Vector3(num2 + a.x, num3 - a.y, 0f);
            vector3 = cachedTransform.TransformPoint(vector3);
            vector3 = worldToLocalMatrix.MultiplyPoint3x4(vector3);
            vector2 = Vector3.Max(vector3, vector2);
            vector = Vector3.Min(vector3, vector);
            vector3 = new Vector3(num2 + a.x, num3 + a.y, 0f);
            vector3 = cachedTransform.TransformPoint(vector3);
            vector3 = worldToLocalMatrix.MultiplyPoint3x4(vector3);
            vector2 = Vector3.Max(vector3, vector2);
            vector = Vector3.Min(vector3, vector);
            i++;
        }
        Bounds result = new Bounds(vector, Vectors.zero);
        result.Encapsulate(vector2);
        return result;
    }

    public static Bounds CalculateRelativeWidgetBounds(Transform trans)
    {
        return NGUIMath.CalculateRelativeWidgetBounds(trans, trans);
    }

    public static Vector3[] CalculateWidgetCorners(UIWidget w)
    {
        Vector2 relativeSize = w.relativeSize;
        Vector2 pivotOffset = w.pivotOffset;
        Vector4 relativePadding = w.relativePadding;
        float num = pivotOffset.x * relativeSize.x - relativePadding.x;
        float num2 = pivotOffset.y * relativeSize.y + relativePadding.y;
        float x = num + relativeSize.x + relativePadding.x + relativePadding.z;
        float y = num2 - relativeSize.y - relativePadding.y - relativePadding.w;
        Transform cachedTransform = w.cachedTransform;
        return new Vector3[]
        {
            cachedTransform.TransformPoint(num, num2, 0f),
            cachedTransform.TransformPoint(num, y, 0f),
            cachedTransform.TransformPoint(x, y, 0f),
            cachedTransform.TransformPoint(x, num2, 0f)
        };
    }

    public static int ClampIndex(int val, int max)
    {
        return (val >= 0) ? ((val >= max) ? (max - 1) : val) : 0;
    }

    public static int ColorToInt(Color c)
    {
        int num = 0;
        num |= Mathf.RoundToInt(c.r * 255f) << 24;
        num |= Mathf.RoundToInt(c.g * 255f) << 16;
        num |= Mathf.RoundToInt(c.b * 255f) << 8;
        return num | Mathf.RoundToInt(c.a * 255f);
    }

    public static Vector2 ConstrainRect(Vector2 minRect, Vector2 maxRect, Vector2 minArea, Vector2 maxArea)
    {
        Vector2 zero = Vectors.v2zero;
        float num = maxRect.x - minRect.x;
        float num2 = maxRect.y - minRect.y;
        float num3 = maxArea.x - minArea.x;
        float num4 = maxArea.y - minArea.y;
        if (num > num3)
        {
            float num5 = num - num3;
            minArea.x -= num5;
            maxArea.x += num5;
        }
        if (num2 > num4)
        {
            float num6 = num2 - num4;
            minArea.y -= num6;
            maxArea.y += num6;
        }
        if (minRect.x < minArea.x)
        {
            zero.x += minArea.x - minRect.x;
        }
        if (maxRect.x > maxArea.x)
        {
            zero.x -= maxRect.x - maxArea.x;
        }
        if (minRect.y < minArea.y)
        {
            zero.y += minArea.y - minRect.y;
        }
        if (maxRect.y > maxArea.y)
        {
            zero.y -= maxRect.y - maxArea.y;
        }
        return zero;
    }

    public static Rect ConvertToPixels(Rect rect, int width, int height, bool round)
    {
        Rect result = rect;
        if (round)
        {
            result.xMin = (float)Mathf.RoundToInt(rect.xMin * (float)width);
            result.xMax = (float)Mathf.RoundToInt(rect.xMax * (float)width);
            result.yMin = (float)Mathf.RoundToInt((1f - rect.yMax) * (float)height);
            result.yMax = (float)Mathf.RoundToInt((1f - rect.yMin) * (float)height);
        }
        else
        {
            result.xMin = rect.xMin * (float)width;
            result.xMax = rect.xMax * (float)width;
            result.yMin = (1f - rect.yMax) * (float)height;
            result.yMax = (1f - rect.yMin) * (float)height;
        }
        return result;
    }

    public static Rect ConvertToTexCoords(Rect rect, int width, int height)
    {
        Rect result = rect;
        if ((float)width != 0f && (float)height != 0f)
        {
            result.xMin = rect.xMin / (float)width;
            result.xMax = rect.xMax / (float)width;
            result.yMin = 1f - rect.yMax / (float)height;
            result.yMax = 1f - rect.yMin / (float)height;
        }
        return result;
    }

    public static string DecimalToHex(int num)
    {
        num &= 16777215;
        return num.ToString("X6");
    }

    public static char DecimalToHexChar(int num)
    {
        if (num > 15)
        {
            return 'F';
        }
        if (num < 10)
        {
            return (char)(48 + num);
        }
        return (char)(65 + num - 10);
    }

    public static float DistanceToRectangle(Vector2[] screenPoints, Vector2 mousePos)
    {
        bool flag = false;
        int val = 4;
        for (int i = 0; i < 5; i++)
        {
            Vector3 vector = screenPoints[NGUIMath.RepeatIndex(i, 4)];
            Vector3 vector2 = screenPoints[NGUIMath.RepeatIndex(val, 4)];
            if (vector.y > mousePos.y != vector2.y > mousePos.y && mousePos.x < (vector2.x - vector.x) * (mousePos.y - vector.y) / (vector2.y - vector.y) + vector.x)
            {
                flag = !flag;
            }
            val = i;
        }
        if (!flag)
        {
            float num = -1f;
            for (int j = 0; j < 4; j++)
            {
                Vector3 v = screenPoints[j];
                Vector3 v2 = screenPoints[NGUIMath.RepeatIndex(j + 1, 4)];
                float num2 = NGUIMath.DistancePointToLineSegment(mousePos, v, v2);
                if (num2 < num || num < 0f)
                {
                    num = num2;
                }
            }
            return num;
        }
        return 0f;
    }

    public static float DistanceToRectangle(Vector3[] worldPoints, Vector2 mousePos, Camera cam)
    {
        Vector2[] array = new Vector2[4];
        for (int i = 0; i < 4; i++)
        {
            array[i] = cam.WorldToScreenPoint(worldPoints[i]);
        }
        return NGUIMath.DistanceToRectangle(array, mousePos);
    }

    public static Color HexToColor(uint val)
    {
        return NGUIMath.IntToColor((int)val);
    }

    public static int HexToDecimal(char ch)
    {
        switch (ch)
        {
            case '0':
                return 0;

            case '1':
                return 1;

            case '2':
                return 2;

            case '3':
                return 3;

            case '4':
                return 4;

            case '5':
                return 5;

            case '6':
                return 6;

            case '7':
                return 7;

            case '8':
                return 8;

            case '9':
                return 9;

            default:
                switch (ch)
                {
                    case 'a':
                        break;

                    case 'b':
                        return 11;

                    case 'c':
                        return 12;

                    case 'd':
                        return 13;

                    case 'e':
                        return 14;

                    case 'f':
                        return 15;

                    default:
                        return 15;
                }
                break;

            case 'A':
                break;

            case 'B':
                return 11;

            case 'C':
                return 12;

            case 'D':
                return 13;

            case 'E':
                return 14;

            case 'F':
                return 15;
        }
        return 10;
    }

    public static string IntToBinary(int val, int bits)
    {
        string text = string.Empty;
        int i = bits;
        while (i > 0)
        {
            if (i == 8 || i == 16 || i == 24)
            {
                text += " ";
            }
            text += (((val & 1 << --i) == 0) ? '0' : '1');
        }
        return text;
    }

    public static Color IntToColor(int val)
    {
        float num = 0.003921569f;
        Color black = Color.black;
        black.r = num * (float)(val >> 24 & 255);
        black.g = num * (float)(val >> 16 & 255);
        black.b = num * (float)(val >> 8 & 255);
        black.a = num * (float)(val & 255);
        return black;
    }

    public static float Lerp(float from, float to, float factor)
    {
        return from * (1f - factor) + to * factor;
    }

    public static Rect MakePixelPerfect(Rect rect)
    {
        rect.xMin = (float)Mathf.RoundToInt(rect.xMin);
        rect.yMin = (float)Mathf.RoundToInt(rect.yMin);
        rect.xMax = (float)Mathf.RoundToInt(rect.xMax);
        rect.yMax = (float)Mathf.RoundToInt(rect.yMax);
        return rect;
    }

    public static Rect MakePixelPerfect(Rect rect, int width, int height)
    {
        rect = NGUIMath.ConvertToPixels(rect, width, height, true);
        rect.xMin = (float)Mathf.RoundToInt(rect.xMin);
        rect.yMin = (float)Mathf.RoundToInt(rect.yMin);
        rect.xMax = (float)Mathf.RoundToInt(rect.xMax);
        rect.yMax = (float)Mathf.RoundToInt(rect.yMax);
        return NGUIMath.ConvertToTexCoords(rect, width, height);
    }

    public static int RepeatIndex(int val, int max)
    {
        if (max < 1)
        {
            return 0;
        }
        while (val < 0)
        {
            val += max;
        }
        while (val >= max)
        {
            val -= max;
        }
        return val;
    }

    public static float RotateTowards(float from, float to, float maxAngle)
    {
        float num = NGUIMath.WrapAngle(to - from);
        if (Mathf.Abs(num) > maxAngle)
        {
            num = maxAngle * Mathf.Sign(num);
        }
        return from + num;
    }

    public static Vector3 SpringDampen(ref Vector3 velocity, float strength, float deltaTime)
    {
        if (deltaTime > 1f)
        {
            deltaTime = 1f;
        }
        float d = 1f - strength * 0.001f;
        int num = Mathf.RoundToInt(deltaTime * 1000f);
        Vector3 vector = Vectors.zero;
        for (int i = 0; i < num; i++)
        {
            vector += velocity * 0.06f;
            velocity *= d;
        }
        return vector;
    }

    public static Vector2 SpringDampen(ref Vector2 velocity, float strength, float deltaTime)
    {
        if (deltaTime > 1f)
        {
            deltaTime = 1f;
        }
        float d = 1f - strength * 0.001f;
        int num = Mathf.RoundToInt(deltaTime * 1000f);
        Vector2 vector = Vectors.v2zero;
        for (int i = 0; i < num; i++)
        {
            vector += velocity * 0.06f;
            velocity *= d;
        }
        return vector;
    }

    public static float SpringLerp(float strength, float deltaTime)
    {
        if (deltaTime > 1f)
        {
            deltaTime = 1f;
        }
        int num = Mathf.RoundToInt(deltaTime * 1000f);
        deltaTime = 0.001f * strength;
        float num2 = 0f;
        for (int i = 0; i < num; i++)
        {
            num2 = Mathf.Lerp(num2, 1f, deltaTime);
        }
        return num2;
    }

    public static float SpringLerp(float from, float to, float strength, float deltaTime)
    {
        if (deltaTime > 1f)
        {
            deltaTime = 1f;
        }
        int num = Mathf.RoundToInt(deltaTime * 1000f);
        deltaTime = 0.001f * strength;
        for (int i = 0; i < num; i++)
        {
            from = Mathf.Lerp(from, to, deltaTime);
        }
        return from;
    }

    public static Vector2 SpringLerp(Vector2 from, Vector2 to, float strength, float deltaTime)
    {
        return Vector2.Lerp(from, to, NGUIMath.SpringLerp(strength, deltaTime));
    }

    public static Vector3 SpringLerp(Vector3 from, Vector3 to, float strength, float deltaTime)
    {
        return Vector3.Lerp(from, to, NGUIMath.SpringLerp(strength, deltaTime));
    }

    public static Quaternion SpringLerp(Quaternion from, Quaternion to, float strength, float deltaTime)
    {
        return Quaternion.Slerp(from, to, NGUIMath.SpringLerp(strength, deltaTime));
    }

    public static float Wrap01(float val)
    {
        return val - (float)Mathf.FloorToInt(val);
    }

    public static float WrapAngle(float angle)
    {
        while (angle > 180f)
        {
            angle -= 360f;
        }
        while (angle < -180f)
        {
            angle += 360f;
        }
        return angle;
    }
}