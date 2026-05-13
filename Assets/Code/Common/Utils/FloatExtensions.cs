using UnityEngine;

public static class FloatExtensions
{
    public static string ToKMB(this float num)
    {
        float absoluteNum = Mathf.Abs(num);
        
        if (absoluteNum >= 1000000000)
            return (num / 1000000000D).ToString("0.##B");
        
        if (absoluteNum >= 1000000)
            return (num / 1000000D).ToString("0.##M");
        
        if (absoluteNum >= 1000)
            return (num / 1000D).ToString("0.#K");

        return num.ToString();
    }
}