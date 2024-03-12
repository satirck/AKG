using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Numerics;

namespace AKG_1
{
    public class ObjParser
    {
        public List<List<int>> FList = new List<List<int>>();
        public List<Vector4> VList = new List<Vector4>();

        public ObjParser(string path)
        {
            Vector4 max = new Vector4(float.MinValue, float.MinValue, float.MinValue, -float.MinValue);
            Vector4 min = new Vector4(float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue);

            var space = new[] { " " };
            var slash = new[] { "/" };
            using (StreamReader sr = new StreamReader(path))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Length > 4)
                    {
                        if (line[0] == 'v' && line[1] == ' ' && line.Length > 7)
                        {
                            var parts = line.Split(space, StringSplitOptions.RemoveEmptyEntries);
                            Vector4 v3 = new Vector4(
                                float.Parse(parts[1], CultureInfo.InvariantCulture),
                                float.Parse(parts[2], CultureInfo.InvariantCulture),
                                float.Parse(parts[3], CultureInfo.InvariantCulture), 1);

                            min.X = min.X > v3.X ? v3.X : min.X;
                            max.X = max.X < v3.X ? v3.X : max.X;

                            min.Y = min.Y > v3.Y ? v3.Y : min.Y;
                            max.Y = max.Y < v3.Y ? v3.Y : max.Y;

                            min.Z = min.Z > v3.Z ? v3.Z : min.Z;
                            max.Z = max.Z < v3.Z ? v3.Z : max.Z;

                            VList.Add(v3);
                        }
                        else if (line[0] == 'f' && line[1] == ' ' && line.Length > 7)
                        {
                            var v = new List<int>();
                            var parts = line.Split(space, StringSplitOptions.RemoveEmptyEntries);
                            for (var i = 1; i < parts.Length; i++)
                            {
                                var lparts = parts[i].Split(slash, StringSplitOptions.RemoveEmptyEntries);
                                v.Add(int.Parse(lparts[0], CultureInfo.InvariantCulture));
                            }
                            FList.Add(v);
                        }
                    }
                }
            }


            Vector4 dif = Vector4.Abs(max - min);

            var scal = 1.0f / Math.Max(dif.X, Math.Max(dif.Y, dif.Z));

            Service.ScalingCof = scal;
            Service.Delta = scal / 10.0f;

        }
    }

}