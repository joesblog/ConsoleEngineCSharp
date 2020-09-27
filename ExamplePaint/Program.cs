using JBL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamplePaint
{
  class Program
  {
    const int w = 320;
    const int h = 240;
    const bool lineDrawing = true;




    public class ColorSelectButton : CButton
    {
      public delegate void ColorButtonEventArgs(CButton sender, object data);

      public CEng.COLOUR COL { get; set; }
      public event ColorButtonEventArgs onClick;


      public void Draw(CEng c, CEng.COLOUR col)
      {
        base.Draw(c);
        //Draw(x, y, default, (short)((ColorButton)i).COL);
        c.Draw(x, y, default, (short)col);
      }
    }

    class JoePaint : CEng
    {
      public short selectedFG = (short)CEng.COLOUR.FG_WHITE;
      public short selectedBG = (short)CEng.COLOUR.BG_RED;
      public List<CButton> buttons = new List<CButton>();

      public JBL.CEngFontSprite fs = new CEngFontSprite();
      public override bool OnUserCreate()
      {


        return true;//base.OnUserCreate();



      }

      private void Joe1_onMouseClick(int x, int y, int button)
      {

      }


      public int[] mask1;
      public override bool OnUserUpdate(long elapsedTime)
      {

        var ts = TimeSpan.FromMilliseconds(elapsedTime);

        // Fill(0, 0, 500, 200, default, selectedBG);

        /* fs.DrawStringBox(this,
         new System.Drawing.Rectangle(10, 10, 100, 100),
         "hello\n how are you? ", COLOUR.BG_RED);


         DrawString(40, 40, "hello\n how are you? ");*/

         //Drawn Bitmap
        for (int y = 0; y < h; y++)
          for (int x = 0; x < w; x++)
          {
            /*if (mask1[(y * w) + x])
            {
              Draw(x, y, 9608, (short)CGE.COLOUR.FG_YELLOW);
            }*/
            Draw(x, y, default, (short)mask1[y * w + x]);
          }

          //Pallette
        foreach (var i in buttons)
        {
          for (int y = i.y; y < i.y + i.h; y++)
            for (int x = i.x; x < i.x + i.w; x++)
            {
              //((ColorSelectButton)i).Draw(i )
              Draw(x, y, default, (short)((ColorSelectButton)i).COL);
            }
        }

        //Color Picker
        DrawCircle_C(this.mousePosNewX, this.mousePosNewY, 5, default, (short)jx.selectedFG);// (short)CGE.COLOUR.BG_DARK_BLUE);
         FillCircle(this.mousePosNewX, this.mousePosNewY, 1, 9608, (short)CEng.COLOUR.FG_YELLOW);
        return true;
      }



    }
    static JoePaint jx;



    static void Main(string[] args)
    {


      jx = new JoePaint();
      int res = jx.Construct(320, 240, 4, 4, "Consolas");
      jx.mask1 = new int[320 * 240];
      //jx.fs.Create(20, 20, "Consolas");


      // var x1 = Enum.GetNames(typeof(CGE.COLOUR)).Where(o => o.StartsWith("FG")).ToList();


      gen_ColorButtons();

      if (lineDrawing)
      {
        jx.onMouseDown += Jx_onMouseDown_linedrawing;
        jx.onMouseUp += Jx_onMouseUp_linedrawing;
        jx.onMouseMove += Jx_onMouseMove_linedrawing;
        jx.onMouseClick += Jx_onMouseClick_linedrawing;
      }
      else
      {
        jx.onMouseDown += Jx_onMouseDown;
        jx.onMouseUp += Jx_onMouseUp;
        jx.onMouseMove += Jx_onMouseMove;
      }

      if (res == 1)
      {
        jx.Start();

      }
      //Console.ReadLine();
    }
    /// <summary>
    /// buttons used to select various colours.
    /// </summary>
    static void gen_ColorButtons()
    {
      var dict = Enum.GetValues(typeof(CEng.COLOUR))
       .Cast<CEng.COLOUR>()
       //.Where(t=>t.ToString().StartsWith("FG"))
       .Distinct()
       .ToDictionary(t => (int)t, t => t.ToString());


      //8 px wide 16 pix heigh
      int bw = 4;
      int bh = 8;
      int startPosX = 300;
      int startPosY = 10;
      int cols = 3;

      int c = 0;
      int cx = startPosX;
      int cy = startPosY;
      foreach (var i in dict)
      {
        ColorSelectButton cb = new ColorSelectButton();
        cb.COL = (CEng.COLOUR)i.Key;
        cb.x = cx;
        cx += bw;

        cb.y = cy;
        // cy += 16;
        cb.w = bw;
        cb.h = bh;

        cb.onClick += (_cb, _d) => {
          if (_d.GetType() == typeof(object[]) && ((object[])_d).Length == 2)
          {
            object[] o = (object[])_d;
            if (_cb.GetType() == typeof(ColorSelectButton))
            {
              jx.selectedFG = (short)((ColorSelectButton)_cb).COL;
            }


          }

        };
        jx.buttons.Add(cb);


        if (c++ % cols == 0)
        {
          cy += bh;
          cx = startPosX;
        }



      }
    }

    private static void Jx_onMouseClick_linedrawing(int x, int y, int button, int? oldMouseX = null, int? oldMouseY = null)
    {
      //check for collisions on buttons
      Debug.WriteLine(button);
      List<CButton> bs = new List<CButton>();
      bool collision = false;
      foreach (var i in jx.buttons)
      {
        if (x >= i.x && x <= (i.x + i.w))
        {
          if (y >= i.y && y <= (i.y + i.h))
          {
            //Debug.WriteLine(((ColorButton)i).COL.ToString());

            //   i.Clicked(new object[] {'F',i });
            jx.selectedFG = (short)((ColorSelectButton)i).COL;
          }

        }
      }
    }

    static bool drawmode = false;

    private static void Jx_onMouseMove_linedrawing(int x, int y, int button, int? oldMouseX = null, int? oldMouseY = null)
    {
      if (drawmode)
        if (oldMouseX != null && oldMouseY != null)
        {
          //jx.DrawLine(oldMouseY.Value, oldMouseY.Value, x, y, 9608, (short)CGE.COLOUR.FG_RED);
          jx.mask1.DrawLine(w, oldMouseX.Value, oldMouseY.Value, x, y, jx.selectedFG);
        }
    }

    private static void Jx_onMouseUp_linedrawing(int x, int y, int button, int? oldMouseX = null, int? oldMouseY = null)
    {
      drawmode = false;
      jx.mousePosOldY = jx.mousePosOldY = null;
    }

    private static void Jx_onMouseDown_linedrawing(int x, int y, int button, int? oldMouseX = null, int? oldMouseY = null)
    {



      drawmode = true;
    }

    private static void Jx_onMouseDown(int x, int y, int button, int? oldMouseX = null, int? oldMouseY = null)
    {
      drawmode = true;
    }

    private static void Jx_onMouseMove(int x, int y, int button, int? oldMouseX = null, int? oldMouseY = null)
    {

      if (drawmode)
        jx.mask1[y * w + x] = jx.selectedFG;
    }

    private static void Jx_onMouseUp(int x, int y, int button, int? oldMouseX = null, int? oldMouseY = null)
    {
      drawmode = false;

    }

    private static void X_onMouseClick(int x, int y, int button, int? oldMouseX = null, int? oldMouseY = null)
    {
      // jx.mask1[y * 240 + x] = true;

    }
  }

}
