public class Map
{
    public int width;
    public int height;
    public float[,] value;

    public Map(int mapWidth, int mapHeight)
    {
        width = mapWidth;
        height = mapHeight;
        value = new float[width, height];
    }

    public Map(float[,] map)
    {
        width = map.GetLength(0);
        height = map.GetLength(1);
        value = new float[width, height];

        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                value[i, j] = map[i, j];
            }
        }
    }

    public void SetValue(int x, int y, float setValue)
    {
        value[x,y] = setValue;
        if (value[x,y] > 1)
        {
            value[x,y] = 1;
        }
        else if (value[x,y] < 0)
        {
            value[x,y] = 0;
        }
    }

    public static Map operator ~(Map map)
    {
        float minHeight = float.MaxValue;
        float maxHeight = float.MinValue;
        for (int j = 0; j < map.height; j++)
        {
            for (int i = 0; i < map.width; i++)
            {
                if (map.value[i, j] < minHeight)
                {
                    minHeight = map.value[i, j];
                }
                if (map.value[i, j] > maxHeight)
                {
                    maxHeight = map.value[i, j];
                }
            }
        }
        for (int j = 0; j < map.height; j++)
        {
            for (int i = 0; i < map.width; i++)
            {
                //Requires alternative for Mathf.InverseLerp() incase UnityEngine is not being used
                map.SetValue(i, j, UnityEngine.Mathf.InverseLerp(minHeight, maxHeight, map.value[i, j]));
            }
        }
        return map;
    }

    //Combines the maps by halving each side and adding them together i.e 0.8 * 0.3 -> 0.55, 1 * 0.1 -> 0.55, 0.4 * 0.8 -> 0.6
    public static Map operator * (Map left, Map right)
    {
        if (left.width != right.width || left.height != right.width)
        {
            throw new MapSizeMismatch("The width and height of both maps must match.");
        }

        Map combinedMap = new Map(left.width, left.height);

        for (int j = 0; j < left.height; j++)
        {
            for (int i = 0; i < left.width; i++)
            {
                combinedMap.SetValue(i, j, (left.value[i, j] * 0.5f) + (right.value[i, j] * 0.5f));
            }
        }
        return combinedMap;
    }

    //Removes the right map from the left i.e 0.8 - 0.3 -> 0.5, 1 - 0.1 -> 0.9, 0.4 - 0.8 -> 0
    public static Map operator -(Map left, Map right)
    {
        if (left.width != right.width || left.height != right.width)
        {
            throw new MapSizeMismatch("The width and height of both maps must match.");
        }

        Map removedMap = new Map(left.width, left.height);

        for (int j = 0; j < left.height; j++)
        {
            for (int i = 0; i < left.width; i++)
            {
                removedMap.SetValue(i, j, left.value[i, j] - right.value[i, j]);
            }
        }
        return removedMap;
    }

    //Removes the right map from the left i.e 0.8 - 0.3 -> 0.5, 1 - 0.1 -> 0.9, 0.4 - 0.8 -> 0
    public static Map operator +(Map left, Map right)
    {
        if (left.width != right.width || left.height != right.width)
        {
            throw new MapSizeMismatch("The width and height of both maps must match.");
        }

        Map removedMap = new Map(left.width, left.height);

        for (int j = 0; j < left.height; j++)
        {
            for (int i = 0; i < left.width; i++)
            {
                removedMap.SetValue(i, j, left.value[i, j] + right.value[i, j]);
            }
        }
        return removedMap;
    }

    //Negates the mapping i.e 0.4 -> 0.6, 0.9 -> 0.1
    public static Map operator ! (Map value)
    {
        Map reverseMap = value;
        for (int j = 0; j < reverseMap.height; j++)
        {
            for (int i = 0; i < reverseMap.width; i++)
            {
                reverseMap.SetValue(i, j, 1 - reverseMap.value[i, j]);
            }
        }
        return reverseMap;
    }

    public static Map ReverseMap(Map map)
    {
        Map reverseMap = map;
        for (int j = 0; j < reverseMap.height; j++)
        {
            for (int i = 0; i < reverseMap.width; i++)
            {
                reverseMap.SetValue(i, j, 1 - reverseMap.value[i, j]);
            }
        }
        return reverseMap;
    }

    //Adds a float value to each element in the map
    public static Map operator + (Map left, float right)
    {
        for (int j = 0; j < left.height; j++)
        {
            for (int i = 0; i < left.width; i++)
            {
                left.SetValue(i, j, left.value[i, j] + right);
            }
        }
        return left;
    }

    //Removes a float value from each element in the map
    public static Map operator -(Map left, float right)
    {
        for (int j = 0; j < left.height; j++)
        {
            for (int i = 0; i < left.width; i++)
            {
                left.SetValue(i, j, left.value[i, j] - right);
            }
        }
        return left;
    }

    //Multiplies each element by a float value
    public static Map operator *(Map left, float right)
    {
        for (int j = 0; j < left.height; j++)
        {
            for (int i = 0; i < left.width; i++)
            {
                left.SetValue(i, j, left.value[i, j] * right);
            }
        }
        return left;
    }

    //Divides each element by a float value
    public static Map operator /(Map left, float right)
    {
        for (int j = 0; j < left.height; j++)
        {
            for (int i = 0; i < left.width; i++)
            {
                left.SetValue(i, j, left.value[i, j] / right);
            }
        }
        return left;
    }
}

[System.Serializable]
public class MapSizeMismatch : System.Exception
{
    public MapSizeMismatch() : base() { }
    public MapSizeMismatch(string message) : base(message) { }
    public MapSizeMismatch(string message, System.Exception inner) : base(message, inner) { }

    // A constructor is needed for serialization when an
    // exception propagates from a remoting server to the client. 
    protected MapSizeMismatch(System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}