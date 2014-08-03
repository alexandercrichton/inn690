using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veis.Common.Math
{
    /// <summary>
    /// Basic and Generic Vector3 class to avoid virtual world specific classes.
    /// Designed to have extensions written to convert this to the various
    /// virtual environment classes in their associated extension classes
    /// </summary>
    public class Vector3
    {
        public float X;
        public float Y;
        public float Z;
        public static readonly Vector3 Zero = new Vector3(0.0f);

        public Vector3(float value)
        {
            X = value;
            Y = value;
            Z = value;
        }
        public Vector3(Vector3 vector)
        {
            X = vector.X;
            Y = vector.Y;
            Z = vector.Z;
        }
        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public static Vector3 Parse(string val)
        {
            try
            {
                float x = 0.0f;
                float y = 0.0f;
                float z = 0.0f;
                string[] values = val.Split(new[] { "<", ">", "," }, 3, StringSplitOptions.RemoveEmptyEntries);
                if (values.Length == 3)
                {
                    bool xOK = float.TryParse(values[0], out x);
                    bool yOK = float.TryParse(values[1], out y);
                    bool zOK = float.TryParse(values[2], out z);

                    if (xOK && yOK && zOK)
                        return new Vector3(x, y, z);
                }
                return Vector3.Zero; // Parsing has failed
 
            }
            catch (Exception)
            {
                return Vector3.Zero; // Parsing has failed
            }
        }
        public override string ToString()
        {
            return string.Format("<{0},{1},{2}>", X, Y, Z);
        }
    }
}
