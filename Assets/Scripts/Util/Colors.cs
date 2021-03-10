using UnityEngine;
using System.Collections.Generic;

    public class Colors
    {
        public static Color[] generateColorPalette(int n)
        {
            Color[] cols = new Color[n];
            for (int i = 0; i < n; i++)
            {
                cols[i] = Color.HSVToRGB((float)i / (float)(n + 1), 0.85f, 1.0f);
            }
            return cols;
        }

        public static Color[] mapColorPalette(float[] data, Dictionary<float, Color> categoryMapping)
        {
            Color[] colorPalette = new Color[data.Length];

            for (int i = 0; i < data.Length;i++)
            {
                colorPalette[i] = categoryMapping[data[i]];
            }

                return colorPalette;
        }

        public static List<Color> getRandomPalette(int N_STEPS)
        {
            List<Color> colors = new List<Color>();

            for (int i = 0; i < N_STEPS; i++)
            {
                Color c = Random.ColorHSV(0.6f,1f);
                c.a = 1f;
                colors.Add(c);
            }
            return colors;
        }

        public static Color[] mapDiscreteColor(float[] values)
        {
            Color[] colors = new Color[values.Length];
        
            Dictionary<float, Color> mapping = new Dictionary<float, Color>();
            
            for (int i = 0; i < values.Length; i++)
            {
                if (!mapping.ContainsKey((values[i])))
                {
                    Color c = Random.ColorHSV();
                    mapping.Add(values[i], c);
                    colors[i] = c;
                }
                else
                {
                    colors[i] = mapping[values[i]];
                }
            }

            return colors;
        }

    public static List<Color> getNumberofCategories(Color[] values)
    {
        List<Color> types = new List<Color>();

        for (int i = 0; i < values.Length; i++)
        {
            if (!types.Contains(values[i]))
                types.Add(values[i]);
        }

        return types;
    }


    }
