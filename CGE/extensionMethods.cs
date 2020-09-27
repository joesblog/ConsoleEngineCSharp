using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JBL
{
  public static class extMethods
  {
    public static bool IsOverride(this MethodInfo methodInfo)
    {
      // return (methodInfo.GetBaseDefinition() != methodInfo);
      return methodInfo.GetBaseDefinition().DeclaringType != methodInfo.DeclaringType;
    }

    public static void DrawLine<T>(this T[] data, int w, int x1, int y1, int x2, int y2, T val)
    {
      int x, y, dx, dy, dx1, dy1, px, py, xe, ye, i;
      dx = x2 - x1; dy = y2 - y1;
      dx1 = Math.Abs(dx); dy1 = Math.Abs(dy);
      px = 2 * dy1 - dx1; py = 2 * dx1 - dy1;
      if (dy1 <= dx1)
      {
        if (dx >= 0)
        {
          x = x1; y = y1; xe = x2;
        }
        else
        {
          x = x2; y = y2; xe = x1;
        }

        //Draw(x, y, c, col);

        for (i = 0; x < xe; i++)
        {
          x = x + 1;
          if (px < 0)
            px = px + 2 * dy1;
          else
          {
            if ((dx < 0 && dy < 0) || (dx > 0 && dy > 0)) y = y + 1; else y = y - 1;
            px = px + 2 * (dy1 - dx1);
          }
          data[y * w + x] = val;
          //Draw(x, y, c, col);
        }
      }
      else
      {
        if (dy >= 0)
        {
          x = x1; y = y1; ye = y2;
        }
        else
        {
          x = x2; y = y2; ye = y1;
        }

        data[y * w + x] = val;


        for (i = 0; y < ye; i++)
        {
          y = y + 1;
          if (py <= 0)
            py = py + 2 * dx1;
          else
          {
            if ((dx < 0 && dy < 0) || (dx > 0 && dy > 0)) x = x + 1; else x = x - 1;
            py = py + 2 * (dx1 - dy1);
          }
          data[y * w + x] = val;

        }
      }
    }

    public static int Push<T>(this T[] source, T Value)
    {
      Array.Resize<T>(ref source, source.Length + 1);
      source[source.GetUpperBound(0)] = Value;
      return source.GetUpperBound(0);
    }

    public static void Tofile(this bool[] dat, string fn, int w = 0)
    {

      string op = "";
      if (w > 0)
      {

        int c = 0;
        foreach (var i in dat)
        {

          if (i)
          {
            op += "#";
          }
          else
          {
            op += " ";
          }
          if (c++ % w == 0) op += Environment.NewLine;
        }


      }

      File.WriteAllText(fn, op);

    }

  }
}
