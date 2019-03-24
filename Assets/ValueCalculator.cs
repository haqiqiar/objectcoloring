using UnityEngine;

public static class ValueCalculator
{

    // Wave length in nanometres (nm)
    // 1nm = 1thousand millionth of a metre = 1/1 000 000 000 metre
    //     = 1^-9 metre
    public static Color tempToColor(float value)
    {
        float R, G, B, A;
        float wl = value;

        if (wl >= 0 && wl < 16)
        {
            R = -1 * (wl - 16) / (16 - 0);
            G = 0;
            B = 1;
        }
        else if (wl >= 16 && wl < 33)
        {
            R = 0;
            G = (wl - 16) / (33 - 16);
            B = 1;
        }
        else if (wl >= 33 && wl < 51)
        {
            R = 0;
            G = 1;
            B = -1 * (wl - 50) / (50- 33);
        }
        else if (wl >= 51 && wl < 68)
        {
            R = (wl - 51) / (68 - 51);
            G = 1;
            B = 0;
        }
        else if (wl >= 68 && wl < 84)
        {
            R = 1;
            G = -1 * (wl - 84) / (84 - 68);
            B = 0;
        }
        else if (wl >= 84 && wl <= 100)
        {
            R = 1;
            G = 0;
            B = 0;
        }
        else
        {
            R = 0;
            G = 0;
            B = 0;
        }
        // intensty is lower at the edges of the visible spectrum.
        if (wl > 100 || wl < 0)
        {
            A = 0;
        }
        else if (wl > 95)
        {
            A = (100 - wl) / (100 - 95);
        }
        else if (wl < 5)
        {
            A = (wl - 0) / (5 - 100);
        }
        else
        {
            A = 1;
        }
        int r = (int)Mathf.Round(R * 255f);
        int g = (int)Mathf.Round(G * 255);
        int b = (int)Mathf.Round(B * 255);
        int a = (int)Mathf.Round(A * 255);

        Color color = new Color(r, g, b, a);

        return color;

    }

    public static float Remap(float from, float fromMin, float fromMax, float toMin, float toMax)
    {
        var fromAbs = from - fromMin;
        var fromMaxAbs = fromMax - fromMin;

        var normal = fromAbs / fromMaxAbs;

        var toMaxAbs = toMax - toMin;
        var toAbs = toMaxAbs * normal;

        var to = toAbs + toMin;

        return to;
    }


}
