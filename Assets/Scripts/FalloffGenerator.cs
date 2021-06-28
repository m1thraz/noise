using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FalloffGenerator {

    public static float[,] GenerateFalloffMap( int size)
    {
        // creating float array with a given size
        float[,] map = new float[size, size];

        //for loops to populate float array
        for( int i = 0; i < size; i++)
        {
            for ( int j = 0; j < size; j++)
            {
                // example i = 10 size = 100  10/100 = 0.1 * 2 = 0.2 -1 = -0.2
                // example i = 70 size = 100  10/100 = 0.7 * 2 = 1.4 -1 = 0.4
                float x = i / (float)size * 2 - 1;
                float y = j / (float)size * 2 - 1;

                float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                map[i, j] = Evaluate(value);
            }
        }

        return map;
    }

    static float Evaluate(float value)
    {
        float a = 3;
        float b = 2.2f;

        // function to lesser the strength of the falloff map f(x) = x^a / x^a +(b -b x^a)
        return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
    }

}
