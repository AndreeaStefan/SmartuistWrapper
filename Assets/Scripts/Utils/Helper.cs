
using System.Collections.Generic;
using System.Linq;

public class Helper
{
    /// <summary>  </summary>
    public static List<int> GetArmIndices()
    {
        var list = new List<int>();
        list.AddRange(Enumerable.Range(13, 6));
        return list;
    }

    public static List<float> GetLimits()
    {
        var list = new List<float>() {0,
            0, 0, 0, 0, 0, 0, // legs
            0, 0, 0, 0, 0, 0,
            5, 5, //upperArm
            45, 45, //arm
            0, 0};

        return list;
    }

    /// <summary>  </summary>
    public static List<int> GetLeftArm()
    {
        var list = new List<int>();
        list.Add(13);
        list.Add(15);
        list.Add(17);
        return list;
    }

    /// <summary>  </summary>
    public static List<int> GetRightArm()
    {
        var list = new List<int>();
        list.Add(14);
        list.Add(16);
        list.Add(18);
        return list;
    }

    /// <summary>  </summary>
    public static List<int> GetLegIndices()
    {
        var list = new List<int>();
        list.AddRange(Enumerable.Range(1, 6)); ;
        return list;
    }

    /// <summary>  </summary>
    public static List<int> GetUpperLegIndices()
    {
        var list = new List<int>();
        list.AddRange(Enumerable.Range(1, 2)); ;
        return list;
    }

    /// <summary>  </summary>
    public static List<int> GetUpperArmIndices()
    {
        var list = new List<int>();
        list.AddRange(Enumerable.Range(13, 2)); ;
        return list;
    }

    /// <summary>  </summary>
    public static List<int> GetMainBodyIndices()
    {
        var list = new List<int>();
        list.Add(0);
        list.AddRange(Enumerable.Range(7, 6)); 
        return list;
    }
}