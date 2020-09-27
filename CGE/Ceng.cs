using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


/*
 * For the attention of anyone who reads this.
 * The console game engine is a port/conversion of the 
 * OneLoneCoder.com Command Line Game Engine
 * by @Javidx9
 * 
 * 
 * 
 * It maintains the principles of @Javidx9's engine but has differences
 * Licensed using GPLV3
 * 
 * This port is Copyright(c) 2020 Joe@joesblog.me.uk
 * It is free software, with bsolutely no warranty, the copyright holder 
 * is not responsible or liable for anything.
 * you can use, distribute, modify this software at your own risk
 * Please attribute me and @Javidx9
 * Also refer to the License included in the olc.h file in this project.
 * 
 * find my blog at http://joesblog.me.uk
 * 
 * Javid's links:
  https://www.github.com/onelonecoder
	https://www.onelonecoder.com
	https://www.youtube.com/javidx9

  I've also included the OLC c header file in this distribution.
 * 
 * 
 */

namespace JBL
{


  public unsafe class CEng
  {


    #region dllimports  structs

    const string KERNEL32 = "kernel32.dll";
    const string USER32 = "user32.dll";
    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

    [DllImport("kernel32.dll",
        EntryPoint = "GetStdHandle",
        SetLastError = true,
        CharSet = CharSet.Auto,
        CallingConvention = CallingConvention.StdCall)]
    private static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32", SetLastError = true)]
    static extern bool AttachConsole(uint dwProcessId);

    [DllImport("kernel32.dll",
        EntryPoint = "AllocConsole",
        SetLastError = true,
        CharSet = CharSet.Auto,
        CallingConvention = CallingConvention.StdCall)]
    private static extern int AllocConsole();

    [DllImport(KERNEL32, SetLastError = true)]
    internal static unsafe extern bool SetConsoleWindowInfo(IntPtr hConsoleOutput,
             bool absolute, ref SMALL_RECT consoleWindow);

    [DllImport(KERNEL32, SetLastError = true)]
    internal static extern bool SetConsoleScreenBufferSize(IntPtr hConsoleOutput, COORD size);

    [DllImport(KERNEL32, SetLastError = true)]
    internal static extern bool SetConsoleActiveScreenBuffer(IntPtr hConsoleOutput);

    [DllImport(USER32, SetLastError = true)]
    internal static extern short GetAsyncKeyState(int vKey);

    [DllImport(KERNEL32, CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = true)]
    internal static extern bool SetConsoleTitle(String title);

    [DllImport(KERNEL32, CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern bool ReadConsoleInput(IntPtr hConsoleInput, out InputRecord buffer, int numInputRecords_UseOne, out int numEventsRead);

    [DllImport(KERNEL32, SetLastError = true)]
    internal static extern bool GetConsoleScreenBufferInfo(IntPtr hConsoleOutput,
          out CONSOLE_SCREEN_BUFFER_INFO lpConsoleScreenBufferInfo);

    [DllImport(KERNEL32, SetLastError = true)]
    internal static unsafe extern bool WriteConsoleOutput(IntPtr hConsoleOutput, CHAR_INFO[] buffer, COORD bufferSize, COORD bufferCoord, ref SMALL_RECT writeRegion);

    [DllImport(KERNEL32, SetLastError = true)]
    static extern bool SetCurrentConsoleFontEx(
        IntPtr hConsoleOutput,
        bool MaximumWindow,
        ref CONSOLE_FONT_INFOEX ConsoleCurrentFontEx);
    #region close handler
    /*Close Handler*/
    internal delegate bool ConsoleCtrlHandlerRoutine(CtrlTypes controlType);
    [DllImport(KERNEL32, SetLastError = true)]
    //[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    //[ResourceExposure(ResourceScope.Process)]
    internal static extern bool SetConsoleCtrlHandler(ConsoleCtrlHandlerRoutine handler, bool addOrRemove);


    const uint ENABLE_PROCESSED_INPUT = 0x0001;
    const uint ENABLE_LINE_INPUT = 0x0002;
    const uint ENABLE_ECHO_INPUT = 0x0004;
    const uint ENABLE_WINDOW_INPUT = 0x0008;
    const uint ENABLE_MOUSE_INPUT = 0x0010;
    const uint ENABLE_INSERT_MODE = 0x0020;
    const uint ENABLE_QUICK_EDIT_MODE = 0x0040;
    const uint ENABLE_EXTENDED_FLAGS = 0x0080;
    const uint ENABLE_AUTO_POSITION = 0x0100;
    const uint ENABLE_VIRTUAL_TERMINAL_INPUT = 0x0200;



    [DllImport(KERNEL32, SetLastError = true)]
    internal static extern bool GetNumberOfConsoleInputEvents(
        IntPtr hConsoleInput,
        out uint lpcNumberOfEvents
        );


    [DllImport(KERNEL32, EntryPoint = "ReadConsoleInputW", CharSet = CharSet.Unicode)]
    static extern bool ReadConsoleInput(
   IntPtr hConsoleInput,
   [Out] InputRecord[] lpBuffer,
   uint nLength,
   out uint lpNumberOfEventsRead
   );
    [DllImport(KERNEL32, SetLastError = true)]
    static extern bool SetConsoleMode(
        IntPtr hConsoleHandle,
        uint dwMode
         );

    private const int MF_BYCOMMAND = 0x00000000;
    public const int SC_CLOSE = 0xF060;
    public const int SC_MINIMIZE = 0xF020;
    public const int SC_MAXIMIZE = 0xF030;
    public const int SC_SIZE = 0xF000;

    [DllImport(USER32)]
    public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

    [DllImport(USER32)]
    private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

    [DllImport(KERNEL32, ExactSpelling = true)]
    private static extern IntPtr GetConsoleWindow();

    // An enumerated type for the control messages
    // sent to the handler routine.
    public enum CtrlTypes
    {
      CTRL_C_EVENT = 0,
      CTRL_BREAK_EVENT,
      CTRL_CLOSE_EVENT,
      CTRL_LOGOFF_EVENT = 5,
      CTRL_SHUTDOWN_EVENT
    }
    /*Close Handler*/
    #endregion
    [StructLayoutAttribute(LayoutKind.Sequential)]
    internal struct SMALL_RECT
    {
      internal short Left;
      internal short Top;
      internal short Right;
      internal short Bottom;

      /// <summary>
      /// 
      /// </summary>
      /// <param name="value">[left,top,right,bottom]</param>
      public static explicit operator SMALL_RECT(short[] value)
      {
        return new SMALL_RECT() { Left = value[0], Top = value[1], Right = value[2], Bottom = value[3] };
      }

      public void set(short[] value)
      {
        Left = value[0]; Top = value[1]; Right = value[2]; Bottom = value[3];
      }

    }

    [StructLayout(LayoutKind.Explicit)]
    public struct CHAR_INFO
    {
      [FieldOffset(0)]
      public char UnicodeChar;
      [FieldOffset(0)]
      public char AsciiChar;
      [FieldOffset(2)] //2 bytes seems to work properly
      public UInt16 Attributes;
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    internal struct COORD
    {
      internal short X;
      internal short Y;

      public static implicit operator COORD(short[] value)
      {
        return new COORD() { X = value[0], Y = value[1] };
      }


    }


    // Win32's KEY_EVENT_RECORD
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct KeyEventRecord
    {
      internal bool keyDown;
      internal short repeatCount;
      internal short virtualKeyCode;
      internal short virtualScanCode;
      internal char uChar; // Union between WCHAR and ASCII char
      internal int controlKeyState;
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct MouseEventRecord
    {
      internal COORD dwMousePosition;
      internal int dwButtonState;
      internal int dwControlKeyState;
      internal int dwEventFlags;
    }

    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Auto)]
    internal struct InputRecord
    {
      [FieldOffset(0)]
      internal short eventType;

      [FieldOffset(4)]
      internal KeyEventRecord keyEvent;

      [FieldOffset(4)]
      internal MouseEventRecord mouseEvent;
      // This struct is a union!  Word alighment should take care of padding!
    }
    [StructLayoutAttribute(LayoutKind.Sequential)]
    internal struct CONSOLE_SCREEN_BUFFER_INFO
    {
      internal COORD dwSize;
      internal COORD dwCursorPosition;
      internal short wAttributes;
      internal SMALL_RECT srWindow;
      internal COORD dwMaximumWindowSize;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct CONSOLE_FONT_INFOEX
    {
      public uint cbSize;
      public uint nFont;
      public COORD dwFontSize;
      public int FontFamily;
      public int FontWeight;


      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] // Edit sizeconst if the font name is too big
      public string FaceName;
    }


    public struct sKeyState
    {
      public bool bPressed;
      public bool bReleased;
      public bool bHeld;
    }
    #endregion
    #region enums
    public enum COLOUR
    {
      FG_BLACK = 0x0000,
      FG_DARK_BLUE = 0x0001,
      FG_DARK_GREEN = 0x0002,
      FG_DARK_CYAN = 0x0003,
      FG_DARK_RED = 0x0004,
      FG_DARK_MAGENTA = 0x0005,
      FG_DARK_YELLOW = 0x0006,
      FG_GREY = 0x0007, // Thanks MS :-/
      FG_DARK_GREY = 0x0008,
      FG_BLUE = 0x0009,
      FG_GREEN = 0x000A,
      FG_CYAN = 0x000B,
      FG_RED = 0x000C,
      FG_MAGENTA = 0x000D,
      FG_YELLOW = 0x000E,
      FG_WHITE = 0x000F,
      BG_BLACK = 0x0000,
      BG_DARK_BLUE = 0x0010,
      BG_DARK_GREEN = 0x0020,
      BG_DARK_CYAN = 0x0030,
      BG_DARK_RED = 0x0040,
      BG_DARK_MAGENTA = 0x0050,
      BG_DARK_YELLOW = 0x0060,
      BG_GREY = 0x0070,
      BG_DARK_GREY = 0x0080,
      BG_BLUE = 0x0090,
      BG_GREEN = 0x00A0,
      BG_CYAN = 0x00B0,
      BG_RED = 0x00C0,
      BG_MAGENTA = 0x00D0,
      BG_YELLOW = 0x00E0,
      BG_WHITE = 0x00F0,
    }

    enum PIXEL_TYPE
    {
      PIXEL_SOLID = 0x2588,
      PIXEL_THREEQUARTERS = 0x2593,
      PIXEL_HALF = 0x2592,
      PIXEL_QUARTER = 0x2591,
    }


    #endregion
    #region consts
    const int STD_OUTPUT_HANDLE = -11;
    const int STD_INPUT_HANDLE = -10;
    const int CTRL_CLOSE_EVENT = 2;
    #endregion

    protected int nScreenHeight { get; set; }
    protected int nScreenWidth { get; set; }
    CHAR_INFO[] ca_bufScreen;
    IntPtr hConsole;
    IntPtr hConsoleIn;


    private SMALL_RECT m_rectWindow;
    private object lck_rectWindow = new object();
    private bool b_AtomActive = false;
    private ushort[] sa_keyNewState = new ushort[256];
    private ushort[] sa_keyOldState = new ushort[256];
    bool[] m_mouseOldState = new bool[5];
    bool[] m_mouseNewState = new bool[5];
    private sKeyState[] sa_Keys = new sKeyState[256];
    private sKeyState[] sa_mouse = new sKeyState[5];
    private ThreadStart ts_main;
    private Thread th_main;
    public short mousePosNewX;
    public short mousePosNewY;
    public short? mousePosOldX;
    public short? mousePosOldY;
    #region Constructors
    public CEng()
    {

      hConsole = GetStdHandle(STD_OUTPUT_HANDLE);
      hConsoleIn = GetStdHandle(STD_INPUT_HANDLE);
      nScreenWidth = 80;
      nScreenHeight = 30;

    }
    const int FF_DONTCARE = (0 << 4);
    const int FW_NORMAL = 400;

    public unsafe int Construct(int width, int height, short fontw, short fonth, string font = "")
    {

      nScreenWidth = width; nScreenHeight = height;

      // Change console visual size to a minimum so ScreenBuffer can shrink
      // below the actual visual size
      //l,t,r,b
      m_rectWindow = (SMALL_RECT)(new short[] { 0, 0, 1, 1 });
      SetConsoleWindowInfo(hConsole, true, ref m_rectWindow);


      // Set the size of the screen buffer
      COORD coord = new short[] { (short)nScreenWidth, (short)nScreenHeight };
      if (!SetConsoleScreenBufferSize(hConsole, coord))
        Error("SetConsoleScreenBufferSize");


      // Assign screen buffer to the console
      if (!SetConsoleActiveScreenBuffer(hConsole))
        return Error("SetConsoleActiveScreenBuffer");


      // Set the font size now that the screen buffer has been assigned to the console
      CONSOLE_FONT_INFOEX cfi = new CONSOLE_FONT_INFOEX();
      cfi.cbSize = (uint)Marshal.SizeOf(cfi);
      cfi.nFont = 0;
      cfi.dwFontSize.X = fontw;
      cfi.dwFontSize.Y = fonth;
      cfi.FontFamily = FF_DONTCARE;
      cfi.FontWeight = FW_NORMAL;
      if (String.IsNullOrWhiteSpace(font))
      {
        cfi.FaceName = "Consolas";
      }
      else cfi.FaceName = font;





      lock (lck_rectWindow)
        if (!SetCurrentConsoleFontEx(hConsole, false, ref cfi))
          return Error("SetCurrentConsoleFontEx");

      //get ScreenBufferInfo
      CONSOLE_SCREEN_BUFFER_INFO csbi;
      if (!GetConsoleScreenBufferInfo(hConsole, out csbi))
        return Error("GetConsoleScreenBufferInfo");

      if (nScreenHeight > csbi.dwMaximumWindowSize.Y)
        return Error("Screen Height / Font Height Too Big");
      if (nScreenWidth > csbi.dwMaximumWindowSize.X)
        return Error("Screen Width / Font Width Too Big");

      lock (lck_rectWindow)
        m_rectWindow.set(new short[] { 0, 0, (short)((short)nScreenWidth - 1), (short)((short)nScreenHeight - 1) });

      if (!SetConsoleWindowInfo(hConsole, true, ref m_rectWindow))
        return Error("SetConsoleWindowInfo");

      // Set flags to allow mouse input		
      if (!SetConsoleMode(hConsoleIn, ENABLE_EXTENDED_FLAGS | ENABLE_WINDOW_INPUT | ENABLE_MOUSE_INPUT))
        return Error("SetConsoleMode");


      IntPtr handle = GetConsoleWindow();
      IntPtr men = GetSystemMenu(handle, false);
      if (handle != IntPtr.Zero)
      {
        DeleteMenu(men, SC_MAXIMIZE, MF_BYCOMMAND);
        DeleteMenu(men, SC_SIZE, MF_BYCOMMAND);
      }
      // Allocate memory for screen buffer
      ca_bufScreen = new CHAR_INFO[nScreenWidth * nScreenHeight];
      // ca_bufScreen = (CHAR_INFO[])Enumerable.Repeat(new CHAR_INFO() { charData =0, attributes =0 }, nScreenWidth * nScreenHeight).ToArray();
      SetConsoleCtrlHandler(new ConsoleCtrlHandlerRoutine(CloseHandler), true);

      return 1;
    }
    #endregion

    #region virtual methods maybe replace with delgates
    public virtual bool OnUserCreate()
    {
      return false;
    }

    public virtual bool OnUserUpdate(long elapsedTime)
    {
      return false;
    }
    #endregion

    #region start stop methods

    public void Start()
    {
      b_AtomActive = true;
      // ThreadStart ts = new ThreadStart(GameThread);
      ts_main = new ThreadStart(GameThread);

      //Thread t = new Thread(ts);
      th_main = new Thread(ts_main);
      th_main.Start();
      //wait for end;
      th_main.Join();
    }

    public delegate void mouseEvent(int x, int y, int button, int? oldMouseX = null, int? oldMouseY = null);

    public event mouseEvent onMouseClick;
    public event mouseEvent onMouseDown;
    public event mouseEvent onMouseUp;
    public event mouseEvent onMouseMove;

    private void GameThread()
    {
      if (!OnUserCreateSet())
      {
        b_AtomActive = false;
        Error("On User Create not overridden");
      }
      DateTime tp1 = DateTime.Now;
      DateTime tp2 = DateTime.Now;

      while (b_AtomActive)
        while (b_AtomActive)
        {
          tp2 = DateTime.Now;
          var duration = (tp2 - tp1);
          tp1 = tp2;
          long elapsed = duration.Ticks;
          //handle key input
          for (int i = 0; i < 256; i++)
          {
            sa_keyNewState[i] = (ushort)GetAsyncKeyState(i);
            sa_Keys[i].bPressed = false;
            sa_Keys[i].bReleased = false;

            if (sa_keyNewState[i] != sa_keyOldState[i])
            {
              if ((sa_keyNewState[i] & 0x8000) == 0x8000)
              {
                sa_Keys[i].bPressed = !sa_Keys[i].bHeld;
                sa_Keys[i].bHeld = true;
              }
              else
              {
                sa_Keys[i].bReleased = true;
                sa_Keys[i].bHeld = false;
              }
            }

            sa_keyOldState[i] = sa_keyNewState[i];

          }
          //handle mouse input
          InputRecord[] inBuf = new InputRecord[32];
          uint events = 0;
          GetNumberOfConsoleInputEvents(hConsoleIn, out events);
          if (events > 0)
            ReadConsoleInput(hConsoleIn, inBuf, events, out events);

          for (uint i = 0; i < events; i++)
          {
            switch (inBuf[i].eventType)
            {
              case 0x0010: //focusevent
                {
                }
                break;

              case 0x0002: //mouse event
                {

                  switch (inBuf[i].mouseEvent.dwEventFlags)
                  {
                    case 0x0001: //move
                      {
                        mousePosOldX = mousePosNewX;
                        mousePosOldY = mousePosNewY;
                        mousePosNewX = inBuf[i].mouseEvent.dwMousePosition.X;
                        mousePosNewY = inBuf[i].mouseEvent.dwMousePosition.Y;
                        onMouseMove?.Invoke(mousePosNewX, mousePosNewY, -1, mousePosOldX, mousePosOldY);
                      }
                      break;

                    case 0x0: //click
                      {
                        for (int m = 0; m < 5; m++)
                          m_mouseNewState[m] = (inBuf[i].mouseEvent.dwButtonState & (1 << m)) > 0;
                      }
                      break;

                    default: break;
                  }

                }
                break;

              default: break;
            }
          }

          for (int m = 0; m < 5; m++)
          {
            sa_mouse[m].bPressed = false;
            sa_mouse[m].bReleased = false;

            if (m_mouseNewState[m] != m_mouseOldState[m])
            {
              if (m_mouseNewState[m])
              {
                sa_mouse[m].bPressed = true;
                sa_mouse[m].bHeld = true;
                onMouseClick?.Invoke(this.mousePosNewX, this.mousePosNewY, m);
                onMouseDown?.Invoke(this.mousePosNewX, this.mousePosNewY, m);
              }
              else
              {
                sa_mouse[m].bReleased = true;
                sa_mouse[m].bHeld = false;
                onMouseUp?.Invoke(this.mousePosNewX, this.mousePosNewY, m);
              }
            }

            m_mouseOldState[m] = m_mouseNewState[m];

          }
          if (!OnUserUpdate(elapsed))
            b_AtomActive = false;

          string s = $"Console Engine JBL based on OLC FPS:{elapsed / 1000}";
          SetConsoleTitle(s);

          lock (lck_rectWindow)
            while (ca_bufScreen == null) ;
          WriteConsoleOutput(hConsole, ca_bufScreen, new short[] { (short)nScreenWidth, (short)nScreenHeight }, new short[] { 0, 0 }, ref m_rectWindow);

        }
    }

    public bool OnUserCreateSet()
    {
      //return this.GetType().GetMethod("OnUserCreate").GetBaseDefinition() != this.GetType().GetMethod("OnUserCreate");
      return this.GetType().GetMethod("OnUserCreate").IsOverride();
    }

    public bool CloseHandler(CtrlTypes evt)
    {
      if (evt == CtrlTypes.CTRL_CLOSE_EVENT)
        b_AtomActive = false;


      //wait for thread to exit
      while (th_main.IsAlive)
      {
        ;
      }

      //ToDo Threading Stuff.

      return true;
    }
    #endregion

    #region drawMethods

    public virtual void Draw_C(int x, int y, char c = '█', short col = 0x000F)
    {
      Draw(x, y, (short)c, col);
    }
    public virtual void Draw(int x, int y, short c = 0x2588, short col = 0x000F)
    {
      if (x >= 0 && x < nScreenWidth && y >= 0 && y < nScreenHeight)
      {
        int p = y * nScreenWidth + x;
        ca_bufScreen[p].UnicodeChar = (char)c;
        ca_bufScreen[p].Attributes = (ushort)col;
      }
    }


    public void DrawCircle_C(int xc, int yc, int r, char c = '█', short col = 0x000F)
    {
      DrawCircle(xc, yc, r, (short)c, col);
    }
    public void DrawCircle(int xc, int yc, int r, short c = 0x2588, short col = 0x000F)
    {
      int x = 0;
      int y = r;
      int p = 3 - 2 * r;
      if (r == 0) return;

      while (y >= x) // only formulate 1/8 of circle
      {
        Draw(xc - x, yc - y, c, col);//upper left left
        Draw(xc - y, yc - x, c, col);//upper upper left
        Draw(xc + y, yc - x, c, col);//upper upper right
        Draw(xc + x, yc - y, c, col);//upper right right
        Draw(xc - x, yc + y, c, col);//lower left left
        Draw(xc - y, yc + x, c, col);//lower lower left
        Draw(xc + y, yc + x, c, col);//lower lower right
        Draw(xc + x, yc + y, c, col);//lower right right
        if (p < 0) p += 4 * x++ + 6;
        else p += 4 * (x++ - y--) + 10;
      }
    }

    public void FillCircle(int xc, int yc, int r, short c = 0x2588, short col = 0x000F)
    {
      // Taken from wikipedia
      int x = 0;
      int y = r;
      int p = 3 - 2 * r;
      if (r == 0) return;

      /* var drawline = [&](int sx, int ex, int ny)

     {
         for (int i = sx; i <= ex; i++)
           Draw(i, ny, c, col);
       };
       */

      while (y >= x)
      {
        // Modified to draw scan-lines instead of edges
        inter_drawline(xc - x, xc + x, yc - y, c, col);
        inter_drawline(xc - y, xc + y, yc - x, c, col);
        inter_drawline(xc - x, xc + x, yc + y, c, col);
        inter_drawline(xc - y, xc + y, yc + x, c, col);
        if (p < 0) p += 4 * x++ + 6;
        else p += 4 * (x++ - y--) + 10;
      }
    }

    private void inter_drawline(int sx, int ex, int ny, short c, short col)
    {
      for (int i = sx; i <= ex; i++)
        Draw(i, ny, c, col);
    }

    public void DrawLine(int x1, int y1, int x2, int y2, short c = 0x2588, short col = 0x000F)
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

        Draw(x, y, c, col);

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
          Draw(x, y, c, col);
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

        Draw(x, y, c, col);

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
          Draw(x, y, c, col);
        }
      }
    }


    public void DrawString(int x, int y, string c, short col = 0x000f)
    {
      if (ca_bufScreen == null) return;
      for (int i = 0; i < c.Length; i++)
      {
        ca_bufScreen[y * nScreenWidth + x + i].UnicodeChar = c[i];
        ca_bufScreen[y * nScreenWidth + x + i].Attributes = (ushort)col;

      }
    }


    public void DrawTriangle(int x1, int y1, int x2, int y2, int x3, int y3, short c = 0x2588, short col = 0x000F)
    {
      DrawLine(x1, y1, x2, y2, c, col);
      DrawLine(x2, y2, x3, y3, c, col);
      DrawLine(x3, y3, x1, y1, c, col);
    }

    public dynamic interSwap(ref int x, ref int y)
    {
      var t = x;
      x = y;
      y = t;
      return new { t, x, y };


    }

    // https://www.avrfreaks.net/sites/default/files/triangles.c
    public void FillTriangle(int x1, int y1, int x2, int y2, int x3, int y3, short c = 0x2588, short col = 0x000F)
    {
      //auto SWAP = [](int & x, int & y) { int t = x; x = y; y = t; };

      // auto drawline = [&](int sx, int ex, int ny) { for (int i = sx; i <= ex; i++) Draw(i, ny, c, col); };

      int t1x, t2x, y, minx, maxx, t1xp, t2xp;
      bool changed1 = false;
      bool changed2 = false;
      int signx1, signx2, dx1, dy1, dx2, dy2;
      int e1, e2;
      // Sort vertices
      if (y1 > y2) { interSwap(ref y1, ref y2); interSwap(ref x1, ref x2); }
      if (y1 > y3) { interSwap(ref y1, ref y3); interSwap(ref x1, ref x3); }
      if (y2 > y3) { interSwap(ref y2, ref y3); interSwap(ref x2, ref x3); }

      t1x = t2x = x1; y = y1;   // Starting points
      dx1 = (int)(x2 - x1); if (dx1 < 0) { dx1 = -dx1; signx1 = -1; }
      else signx1 = 1;
      dy1 = (int)(y2 - y1);

      dx2 = (int)(x3 - x1); if (dx2 < 0) { dx2 = -dx2; signx2 = -1; }
      else signx2 = 1;
      dy2 = (int)(y3 - y1);

      if (dy1 > dx1)
      {   // swap values
        interSwap(ref dx1, ref dy1);
        changed1 = true;
      }
      if (dy2 > dx2)
      {   // swap values
        interSwap(ref dy2, ref dx2);
        changed2 = true;
      }

      e2 = (int)(dx2 >> 1);
      // Flat top, just process the second half
      if (y1 == y2) goto next;
      e1 = (int)(dx1 >> 1);

      for (int i = 0; i < dx1;)
      {
        t1xp = 0; t2xp = 0;
        if (t1x < t2x) { minx = t1x; maxx = t2x; }
        else { minx = t2x; maxx = t1x; }
        // process first line until y value is about to change
        while (i < dx1)
        {
          i++;
          e1 += dy1;
          while (e1 >= dx1)
          {
            e1 -= dx1;
            if (changed1) t1xp = signx1;//t1x += signx1;
            else goto next1;
          }
          if (changed1) break;
          else t1x += signx1;
        }
      // Move line
      next1:
        // process second line until y value is about to change
        while (true)
        {
          e2 += dy2;
          while (e2 >= dx2)
          {
            e2 -= dx2;
            if (changed2) t2xp = signx2;//t2x += signx2;
            else goto next2;
          }
          if (changed2) break;
          else t2x += signx2;
        }
      next2:
        if (minx > t1x) minx = t1x; if (minx > t2x) minx = t2x;
        if (maxx < t1x) maxx = t1x; if (maxx < t2x) maxx = t2x;
        inter_drawline(minx, maxx, y, c, col);// drawline(minx, maxx, y);    // Draw line from min to max points found on the y
                                              // Now increase y
        if (!changed1) t1x += signx1;
        t1x += t1xp;
        if (!changed2) t2x += signx2;
        t2x += t2xp;
        y += 1;
        if (y == y2) break;

      }
    next:
      // Second half
      dx1 = (int)(x3 - x2); if (dx1 < 0) { dx1 = -dx1; signx1 = -1; }
      else signx1 = 1;
      dy1 = (int)(y3 - y2);
      t1x = x2;

      if (dy1 > dx1)
      {   // swap values
        interSwap(ref dy1, ref dx1);
        changed1 = true;
      }
      else changed1 = false;

      e1 = (int)(dx1 >> 1);

      for (int i = 0; i <= dx1; i++)
      {
        t1xp = 0; t2xp = 0;
        if (t1x < t2x) { minx = t1x; maxx = t2x; }
        else { minx = t2x; maxx = t1x; }
        // process first line until y value is about to change
        while (i < dx1)
        {
          e1 += dy1;
          while (e1 >= dx1)
          {
            e1 -= dx1;
            if (changed1) { t1xp = signx1; break; }//t1x += signx1;
            else goto next3;
          }
          if (changed1) break;
          else t1x += signx1;
          if (i < dx1) i++;
        }
      next3:
        // process second line until y value is about to change
        while (t2x != x3)
        {
          e2 += dy2;
          while (e2 >= dx2)
          {
            e2 -= dx2;
            if (changed2) t2xp = signx2;
            else goto next4;
          }
          if (changed2) break;
          else t2x += signx2;
        }
      next4:

        if (minx > t1x) minx = t1x; if (minx > t2x) minx = t2x;
        if (maxx < t1x) maxx = t1x; if (maxx < t2x) maxx = t2x;
        inter_drawline(minx, maxx, y, c, col);
        if (!changed1) t1x += signx1;
        t1x += t1xp;
        if (!changed2) t2x += signx2;
        t2x += t2xp;
        y += 1;
        if (y > y3) return;
      }
    }
    void Clip(int x, int y)
    {
      if (x < 0) x = 0;
      if (x >= nScreenWidth) x = nScreenWidth;
      if (y < 0) y = 0;
      if (y >= nScreenHeight) y = nScreenHeight;
    }

    public void Fill(int x1, int y1, int x2, int y2, short c = 0x2588, short col = 0x000F)
    {
      Clip(x1, y1);
      Clip(x2, y2);
      for (int x = x1; x < x2; x++)
        for (int y = y1; y < y2; y++)
          Draw(x, y, c, col);
    }
    #endregion
    public int Error(string txt)
    {
      var ercode = Marshal.GetLastWin32Error();
      Debug.WriteLine(txt + ercode);
      return 0;
    }



  }

  public class CButton
  {
    public delegate void CButtonEventArgs(CButton sender, object data);

    public int x;
    public int y;
    public int w;
    public int h;

    public event CButtonEventArgs onClicked;

    public void Clicked(object data)
    {
      onClicked?.Invoke(this, data);
    }

    public virtual void Draw(CEng c)
    {

      ;
    }

  }


  public class CEngSprite
  {
    public int nWidth = 0;
    public int nHeight = 0;
    public short[] sa_glyphs;
    public short[] sa_colours;

    public CEngSprite() { }
    public CEngSprite(int w, int h)
    {
      Create(w, h);
    }
    public CEngSprite(string sfile)
    {

    }
    public virtual void Create(int w, int h)
    {
      nWidth = w;
      nHeight = h;

      sa_glyphs = new short[w * h];
      sa_colours = new short[w * h];

    }
  }

  public class CEngVec2
  {
    public int x;
    public int y;

    public CEngVec2(int _x, int _y) { x = _x; y = _y; }
    public CEngVec2() { }
  }

  public class CEngFontV2 : CEngVec2
  {
    public CEngFontV2(int _x, int _y) : base(_x, _y)
    {
    }
  }

  public unsafe class CEngFontSprite
  {

    Dictionary<char, List<CEngFontV2>> fsprites;
    static ushort start = 0x21;
    static ushort end = 0x7e;
    static float startingSize = 18f;
    public int charW = 0;
    public int charH = 0;
    public short glyph = 0x2588;
    public void Create(int w, int h, string spriteName)
    {
      charW = w;
      charH = h;

      fsprites = new Dictionary<char, List<CEngFontV2>>();

      InstalledFontCollection ifc = new InstalledFontCollection();
      var fams = ifc.Families;
      var fnt = fams.Where(o => o.Name == spriteName).FirstOrDefault();

      if (fnt != null)
      {
        //int cf = 15;

        var comchars = commonGBChars();
        int fl = comchars.Length;

        var cSize = startingSize;
        Size target = new Size(charW, charH);
        var mfont = new Font(fnt, 12, FontStyle.Regular);
        for (int cf = 0; cf < comchars.Length; cf++)
        {
          Image bmp = new Bitmap(charW, charH);
          var g = Graphics.FromImage(bmp);

          string str = new string(comchars[cf], 1);
          var xfont = FindFont(g, str, target, mfont);
          g.DrawString(str, xfont, new SolidBrush(Color.Red), 0f, 0f);

          bmp.Save(@"C:\_temp\test1.png", ImageFormat.Png);

          var dat = ((Bitmap)bmp).LockBits(new Rectangle(0, 0, charW, charH),
          System.Drawing.Imaging.ImageLockMode.ReadWrite,
          bmp.PixelFormat);

          byte bpp = (byte)Image.GetPixelFormatSize(bmp.PixelFormat);
          byte* scan0 = (byte*)dat.Scan0.ToPointer();

          if (!fsprites.ContainsKey(comchars[cf]))
            fsprites.Add(comchars[cf], new List<CEngFontV2>());

          for (int y = 0; y < dat.Height; ++y)
          {
            for (int x = 0; x < dat.Width; ++x)
            {
              byte* d = scan0 + y * dat.Stride + x * bpp / 8;
              if (d[2] != 0)
              {

                fsprites[comchars[cf]].Add(new CEngFontV2(x, y));
              }
            }
          }

        ((Bitmap)bmp).UnlockBits(dat);

        }


      }
      else
      {
        throw new Exception("Font does not exist or is not installed.");
      }


    }


    private Font FindFont(
   System.Drawing.Graphics g,
   string longString,
   Size Room,
   Font PreferedFont
)
    {
      // you should perform some scale functions!!!
      SizeF RealSize = g.MeasureString(longString, PreferedFont);
      float HeightScaleRatio = Room.Height / RealSize.Height;
      float WidthScaleRatio = Room.Width / RealSize.Width;

      float ScaleRatio = (HeightScaleRatio < WidthScaleRatio)
         ? ScaleRatio = HeightScaleRatio
         : ScaleRatio = WidthScaleRatio;

      float ScaleFontSize = PreferedFont.Size * ScaleRatio;

      return new Font(PreferedFont.FontFamily, ScaleFontSize);
    }


    public static char[] commonGBChars()
    {




      char[] r = new char[(end - start) + 1];

      int c = 0;
      for (ushort i = start; i <= end; i++)
      {
        r[c++] = (char)i;
      }
      return r;

    }

    public void DrawStringBox(CEng en, Rectangle pos, string str, CEng.COLOUR col = CEng.COLOUR.BG_WHITE, bool wrap = true, int lheight = 1, float kern = 0.1f)
    {



      bool[] mask = new bool[pos.Width * pos.Height];
      // how many chars can we fit per line
      int cpl = (int)((float)pos.Width / ((float)charW + kern));

      //how many lines
      int lamt = pos.Height / (charH + lheight);

      //buffer
      string[] ln = new string[lamt];

      //current line
      int cl = 0;

      //current char in line
      int cp = 0;

      //current char
      char cc = '\0';
      str = str.Replace('\r', '\0');
      for (int i = 0; i < str.Length; i++)
      {
        cc = str[i];

        bool nl = (cc == '\n' && cl < lamt)
        || (cp + 1 > cpl && cl < lamt);


        bool eof = (cl > lamt);

        if (nl && !eof)
        {
          cl++;
          cp = 0;
          continue;
        }
        else if (nl && eof)
        {
          break;
        }

        if (cc == '\0') continue;


        if (fsprites.ContainsKey(cc))
        {
          var fs = fsprites[cc];


          var sx = (int)Math.Ceiling((cp++) * (charW + kern));
          var sy = cl * (charH + lheight);

          foreach (var f in fs)
          {
            var _x = sx + f.x;
            var _y = sy + f.y;

            mask[_y * pos.Width + _x] = true;
          }

        }
        else if (cc == ' ')
        {
          cp++;
        }


      }
      for (int y = 0; y < pos.Height; y++)
      {
        for (int x = 0; x < pos.Width; x++)
        {
          bool v = mask[y * pos.Width + x];
          if (v)
          {

            en.Draw(x + pos.X, y + pos.Y, default, (short)col);
          }

        }
      }
      mask.Tofile(@"C:\_temp\test.txt", pos.Width);


    }
  }
}
