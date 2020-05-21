﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public class Window : Form
{
    public Window(int width, int height)
    {
        StartPosition = FormStartPosition.CenterScreen;
        DoubleBuffered = true;
        ClientSize = new Size(width, height);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
    }
}
public enum PixelMode
{
    Normal,
    Mask,
    Alpha
}
public enum WindowMode
{
    Window,
    Borderless,
    Fullscreen
}
public enum PositionMode
{
    Normal,
    Center,
    CenterX,
    CenterY,
}
public abstract unsafe class FormGameEngine
{
    public static Random rnd = new Random();
    private Window window;
    private BackgroundWorker bgw;
    private Bitmap[] buffers;
    private int bufferIndex;
    private Sprite fontSprite;
    private PixelMode ePixelMode;
    public string AppName;
    private int nWindowWidth;
    private int nWindowHeight;
    private int nScreenWidth;
    private int nScreenHeight;
    private int nPixelWidth;
    private int nPixelHeight;
    private Vector mousePos;
    private Vector pmousePos;
    private Pixel[] pixelData;
    private float fShowTextTime;
    private bool bRunning;
    public void Construct(int screenWidth, int screenHeight, int pixelWidth = 1, int pixelHeight = 1, WindowMode windowMode = WindowMode.Window)
    {
        mousePos = new Vector();
        pmousePos = new Vector();

        bufferIndex = 0;
        buffers = new Bitmap[5];

        bRunning = true;
        fShowTextTime = 1f;
        ePixelMode = PixelMode.Normal;

        if (pixelWidth < 1) pixelWidth = 1;
        if (pixelHeight < 1) pixelHeight = 1;

        if (windowMode == WindowMode.Window || windowMode == WindowMode.Borderless)
        {
            nScreenWidth = screenWidth;
            nScreenHeight = screenHeight;
            nPixelWidth = pixelWidth;
            nPixelHeight = pixelHeight;
            nWindowWidth = screenWidth * pixelWidth;
            nWindowHeight = screenHeight * pixelHeight;
            window = new Window(nWindowWidth, nWindowHeight);
            if (windowMode == WindowMode.Borderless) 
                window.FormBorderStyle = FormBorderStyle.None;
        }
        else
        {
            window = new Window(100, 100);
            window.FormBorderStyle = FormBorderStyle.None;
            window.WindowState = FormWindowState.Maximized;
            window.Bounds = Screen.PrimaryScreen.Bounds;
            nWindowWidth = window.Width;
            nWindowHeight = window.Height;
            nPixelWidth = pixelWidth;
            nPixelHeight = pixelHeight;
            nScreenWidth = nWindowWidth / nPixelWidth;
            nScreenHeight = nWindowHeight / nPixelHeight;
        }

        pixelData = new Pixel[nScreenWidth * nScreenHeight];
        for (int i = 0; i < pixelData.Length; i++)
            pixelData[i] = Pixel.Black;

        fontSprite = new Sprite(128, 48);
        string data = "";
        data += "?Q`0001oOch0o01o@F40o0<AGD4090LAGD<090@A7ch0?00O7Q`0600>00000000";
        data += "O000000nOT0063Qo4d8>?7a14Gno94AA4gno94AaOT0>o3`oO400o7QN00000400";
        data += "Of80001oOg<7O7moBGT7O7lABET024@aBEd714AiOdl717a_=TH013Q>00000000";
        data += "720D000V?V5oB3Q_HdUoE7a9@DdDE4A9@DmoE4A;Hg]oM4Aj8S4D84@`00000000";
        data += "OaPT1000Oa`^13P1@AI[?g`1@A=[OdAoHgljA4Ao?WlBA7l1710007l100000000";
        data += "ObM6000oOfMV?3QoBDD`O7a0BDDH@5A0BDD<@5A0BGeVO5ao@CQR?5Po00000000";
        data += "Oc``000?Ogij70PO2D]??0Ph2DUM@7i`2DTg@7lh2GUj?0TO0C1870T?00000000";
        data += "70<4001o?P<7?1QoHg43O;`h@GT0@:@LB@d0>:@hN@L0@?aoN@<0O7ao0000?000";
        data += "OcH0001SOglLA7mg24TnK7ln24US>0PL24U140PnOgl0>7QgOcH0K71S0000A000";
        data += "00H00000@Dm1S007@DUSg00?OdTnH7YhOfTL<7Yh@Cl0700?@Ah0300700000000";
        data += "<008001QL00ZA41a@6HnI<1i@FHLM81M@@0LG81?O`0nC?Y7?`0ZA7Y300080000";
        data += "O`082000Oh0827mo6>Hn?Wmo?6HnMb11MP08@C11H`08@FP0@@0004@000000000";
        data += "00P00001Oab00003OcKP0006@6=PMgl<@440MglH@000000`@000001P00000000";
        data += "Ob@8@@00Ob@8@Ga13R@8Mga172@8?PAo3R@827QoOb@820@0O`0007`0000007P0";
        data += "O`000P08Od400g`<3V=P0G`673IP0`@3>1`00P@6O`P00g`<O`000GP800000000";
        data += "?P9PL020O`<`N3R0@E4HC7b0@ET<ATB0@@l6C4B0O`H3N7b0?P01L3R000000020";
        int px = 0;
        int py = 0;
        for (int b = 0; b < 1024; b += 4)
        {
            uint sym1 = (uint)data[b + 0] - 48;
            uint sym2 = (uint)data[b + 1] - 48;
            uint sym3 = (uint)data[b + 2] - 48;
            uint sym4 = (uint)data[b + 3] - 48;
            uint r = (sym1 << 18) | (sym2 << 12) | (sym3 << 6) | sym4;
            for (int i = 0; i < 24; i++)
            {
                int k = (r & (1 << i)) != 0 ? 255 : 0;
                fontSprite.SetPixel(px, py, Pixel.RGB(k, k, k, k));
                if (++py == 48)
                {
                    px++;
                    py = 0;
                }
            }
        }

        Input.Link(window);
        OnUserCreate();
        Time.Start();
        window.FormClosing += WindowFormClosing;
        bgw = new BackgroundWorker();
        bgw.WorkerSupportsCancellation = true;
        bgw.DoWork += ActionThread;
        bgw.RunWorkerAsync();
        window.ShowDialog();
    }
    private void WindowFormClosing(object sender, FormClosingEventArgs e)
    {
        bgw.CancelAsync();
        bgw.DoWork -= ActionThread;
        bRunning = false;
    }
    private void ActionThread(object sender, DoWorkEventArgs e)
    {
        while (!bgw.CancellationPending && bRunning)
        {
            Time.Calculate();
            fShowTextTime += Time.fElapsedTime;
            if (fShowTextTime >= 1f)
            {
                try { window.Invoke(new Action(() => window.Text = AppName + ": " + (int)(1f / Time.fElapsedTime))); } catch { };
                fShowTextTime = 0f;
            }
            pmousePos = mousePos.Clone();
            mousePos = Input.MousePos();
            Input.UpdateCurrentKeys();
            OnUserUpdate(Time.fElapsedTime);
            Input.UpdatePreviousKeys();
            DrawPixelsToBuffer(ref buffers[bufferIndex]);
            //DrawPixelsToBuffer(ref buffers[bufferIndex]);
            window.BackgroundImage = buffers[bufferIndex];
            window.Invalidate();
            bufferIndex = (bufferIndex + 1) % buffers.Length;
            buffers[bufferIndex]?.Dispose();
        }
    }
    private void DrawPixelsToBuffer(ref Bitmap buffer)
    {
        int w = nScreenWidth;
        int h = nScreenHeight;
        int pw = nPixelWidth;
        int ph = nPixelHeight;
        buffer = new Bitmap(w * pw, h * ph);
        Rectangle area = new Rectangle(0, 0, buffer.Width, buffer.Height);
        BitmapData bmpData = buffer.LockBits(area, ImageLockMode.WriteOnly, buffer.PixelFormat);
        byte* pixel = (byte*)bmpData.Scan0;
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                Pixel p = pixelData[y * w + x];
                for (int yy = 0; yy < ph; yy++)
                {
                    for (int xx = 0; xx < pw; xx++)
                    {
                        pixel[(y * ph + yy) * w * pw * 4 + (x * pw + xx) * 4 + 0] = p.b;
                        pixel[(y * ph + yy) * w * pw * 4 + (x * pw + xx) * 4 + 1] = p.g;
                        pixel[(y * ph + yy) * w * pw * 4 + (x * pw + xx) * 4 + 2] = p.r;
                        pixel[(y * ph + yy) * w * pw * 4 + (x * pw + xx) * 4 + 3] = 255;
                    }
                }
            }
        }
        buffer.UnlockBits(bmpData);
    }
    public void MoveCursor(Vector posToScreen)
    {
        Vector windowPos = new Vector(window.Left, window.Top);
        Vector newPos = windowPos + posToScreen * PixelSize() + new Vector(8, 31);
        Cursor.Position = new Point((int)newPos.x, (int)newPos.y);
    }
    public void MoveCursor(float x, float y) => MoveCursor(new Vector(x, y));
    public abstract void OnUserCreate();
    public abstract void OnUserUpdate(float fElapsedTime);
    public bool Focused() => window.Focused;
    public int WindowWidth() => nWindowWidth;
    public int WindowHeight() => nWindowHeight;
    public int ScreenWidth() => nScreenWidth;
    public int ScreenHeight() => nScreenHeight;
    public int PixelWidth() => nPixelWidth;
    public int PixelHeight() => nPixelHeight;
    public Vector ScreenMousePos() => new Vector((int)(mousePos.x / nPixelWidth), (int)(mousePos.y / nPixelHeight));
    public Vector ScreenPMousePos() => new Vector((int)(pmousePos.x / nPixelWidth), (int)(pmousePos.y / nPixelHeight));
    public float MouseX() => (int)(mousePos.x / nPixelWidth);
    public float MouseY() => (int)(mousePos.y / nPixelHeight);
    public Vector WindowMousePos() => mousePos.Clone();
    public Vector WindowPMousePos() => pmousePos.Clone();
    public Vector ChangeInScreenMouse() => ScreenMousePos() - ScreenPMousePos();
    public Vector Middle() => ScreenSize() / 2f;
    public Vector ScreenSize() => new Vector(nScreenWidth, nScreenHeight);
    public Vector WindowSize() => new Vector(nWindowWidth, nWindowHeight);
    public Vector PixelSize() => new Vector(nPixelWidth, nPixelHeight);
    public Vector RandomVectorOnScreen() => new Vector(rnd.Next(nScreenWidth), rnd.Next(nScreenHeight));
    public void Exit() => Application.Exit();
    public void SetPixelMode(PixelMode pixelMode) => ePixelMode = pixelMode;
    public void Clear(Pixel p)
    {
        int w = nScreenWidth;
        int h = nScreenHeight;
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
                pixelData[y * w + x] = p;
    }
    public void DrawPixel(float x, float y, Pixel p)
    {
        if (x >= 0 && x < nScreenWidth && y >= 0 && y < nScreenHeight)
        {
            if (ePixelMode == PixelMode.Normal)
            {
                pixelData[(int)y * nScreenWidth + (int)x] = p;
            }
            else if (ePixelMode == PixelMode.Mask) 
            {
                if (p.a == 255)
                    pixelData[(int)y * nScreenWidth + (int)x] = p;
            }
            else
            {
                if (p.a == 0) return;
                Pixel d = pixelData[(int)y * nScreenWidth + (int)x];
                float a = p.a / 255.0f;
                float c = 1f - a;
                float r = a * p.r + c * d.r;
                float g = a * p.g + c * d.g;
                float b = a * p.b + c * d.b;
                pixelData[(int)y * nScreenWidth + (int)x] = Pixel.RGB((int)r, (int)g, (int)b);
            }
        }
    }
    public void DrawPixel(Vector v, Pixel p) => DrawPixel(v.x, v.y, p);
    public void DrawString(float x, float y, string text, Pixel p, int scale = 1, PositionMode pm = PositionMode.Normal)
    {
        if (text == "" || scale < 1) return;
        int sx = 0;
        int sy = 0;
        int offx = 0;
        int offy = 0;
        if (scale < 1) scale = 1;
        PixelMode m = ePixelMode;
        if (p.a != 255) SetPixelMode(PixelMode.Alpha);
        else SetPixelMode(PixelMode.Mask);
        if (pm != PositionMode.Normal)
        {
            for (int cr = 0; cr < text.Length; cr++)
            {
                char c = text[cr];
                if (c == '\n')
                {
                    sx = 0;
                    sy += 8 * scale;
                    if (offy < sy && (pm == PositionMode.CenterY || pm == PositionMode.Center)) 
                        offy = sy;
                }
                else if (c == '\t')
                {
                    sx += 4 * 8 * scale;
                }
                else
                {
                    sx += 8 * scale;
                }
                if (offx < sx && (pm == PositionMode.CenterX || pm == PositionMode.Center)) 
                    offx = sx;
            }
            if (pm == PositionMode.CenterY || pm == PositionMode.Center) offy += 8 * scale;
            sx = 0;
            sy = 0;
        }
        for (int cr = 0; cr < text.Length; cr++)
        {
            char c = text[cr];
            if (c == '\n')
            {
                sx = 0; 
                sy += 8 * scale;
                continue;
            }
            if (c == '\t')
            {
                sx += 4 * 8 * scale;
                continue;
            }
            else
            {
                int ox = (c - 32) % 16;
                int oy = (c - 32) / 16;
                if (scale > 1)
                {
                    for (int i = 0; i < 8; i++)
                        for (int j = 0; j < 8; j++)
                            if (fontSprite.GetPixel(i + ox * 8, j + oy * 8).r > 0)
                                for (int si = 0; si < scale; si++)
                                    for (int sj = 0; sj < scale; sj++)
                                        DrawPixel(x + sx - offx / 2 + (i * scale) + si, y + sy - offy / 2 + (j * scale) + sj, p);
                }
                else
                {
                    for (int i = 0; i < 8; i++)
                        for (int j = 0; j < 8; j++)
                            if (fontSprite.GetPixel(i + ox * 8, j + oy * 8).r > 0)
                                DrawPixel(x + sx - offx / 2 + i, y + sy - offy / 2 + j, p);
                }
                sx += 8 * scale;
            }
        }
        SetPixelMode(m);
    }
    public void DrawString(Vector v, string text, Pixel p, int scale = 1, PositionMode pm = PositionMode.Normal) => DrawString(v.x, v.y, text, p, scale, pm);
    public void DrawSprite(Sprite s, float x, float y, PositionMode pm = PositionMode.Normal)
    {
        if (pm == PositionMode.CenterX || pm == PositionMode.Center) x -= s.Width / 2;
        if (pm == PositionMode.CenterY || pm == PositionMode.Center) y -= s.Height / 2;
        int maxw = (s.Width + x) > nScreenWidth ? nScreenWidth - (int)x : s.Width;
        int maxh = (s.Height + y) > nScreenHeight ? nScreenHeight - (int)y : s.Height;
        int minw = x < 0 ? -(int)x : 0;
        int minh = y < 0 ? -(int)y : 0;
        for (int j = minh; j < maxh; j++)
            for (int i = minw; i < maxw; i++)
                DrawPixel(i + x, j + y, s.GetPixel(i, j));
    }
    public void DrawSprite(Sprite s, Vector v, PositionMode pm = PositionMode.Normal) => DrawSprite(s, v.x, v.y, pm);
    public void DrawPartialSprite(Sprite s, float x, float y, int sx, int sy, int w, int h, PositionMode pm = PositionMode.Normal)
    {
        if (w + sx > s.Width) w = s.Width - sx;
        if (h + sy > s.Height) h = s.Height - sy;
        if (pm == PositionMode.CenterX || pm == PositionMode.Center) x -= w / 2;
        if (pm == PositionMode.CenterY || pm == PositionMode.Center) y -= h / 2;
        int maxw = (x + w) > nScreenWidth ? nScreenWidth - ((int)x - sx) : w + sx;
        int maxh = (y + h) > nScreenHeight ? nScreenHeight - ((int)y - sy) : h + sy;
        int minw = x < 0 ? -((int)x - sx) : sx;
        int minh = y < 0 ? -((int)y - sy) : sy;
        for (int j = minh; j < maxh; j++)
            for (int i = minw; i < maxw; i++)
                DrawPixel(i + x - sx, j + y - sy, s.GetPixel(i, j));
    }
    public void DrawPartialSprite(Sprite s, Vector v, int sx, int sy, int w, int h, PositionMode pm = PositionMode.Normal) => DrawPartialSprite(s, v.x, v.y, sx, sy, w, h, pm);
    public void DrawWarpedSprite(Sprite s, Vector v1, Vector v2, Vector v3, Vector v4)
    {
        FillTexturedTriangle(v1, 0, 0, v2, 1, 0, v3, 1, 1, s);
        FillTexturedTriangle(v1, 0, 0, v4, 0, 1, v3, 1, 1, s);
    }
    public void DrawPartialWarpedSprite(Sprite s, Vector v1, Vector v2, Vector v3, Vector v4, float sx, float sy, float w, float h)
    {
        Vector uv1 = new Vector(sx / s.Width, sy / s.Height);
        Vector uv2 = uv1 + new Vector(w / s.Width, 0);
        Vector uv3 = uv1 + new Vector(w / s.Width, h / s.Height);
        Vector uv4 = uv1 + new Vector(0, h / s.Height);
        FillTexturedTriangle(v1, uv1.x, uv1.y, v2, uv2.x, uv2.y, v3, uv3.x, uv3.y, s);
        FillTexturedTriangle(v1, uv1.x, uv1.y, v4, uv4.x, uv4.y, v3, uv3.x, uv3.y, s);
    }
    public void DrawRotatedSprite(Sprite s, float x, float y, float angle, PositionMode pm = PositionMode.Normal)
    {
        if (pm == PositionMode.CenterX || pm == PositionMode.Center) x -= s.Width / 2;
        if (pm == PositionMode.CenterY || pm == PositionMode.Center) y -= s.Height / 2;
        Vector v1 = new Vector(x, y);
        Vector v2 = v1 + new Vector(s.Width, 0);
        Vector v3 = v1 + s.Bounds;
        Vector v4 = v1 + new Vector(0, s.Height);
        Vector m = (v1 + v2 + v3 + v4) / 4f;
        DrawWarpedSprite(s, v1.Rotate(angle, m), v2.Rotate(angle, m), v3.Rotate(angle, m), v4.Rotate(angle, m));
    }
    public void DrawRotatedSprite(Sprite s, Vector v, float angle, PositionMode pm = PositionMode.Normal) => DrawRotatedSprite(s, v.x, v.y, angle, pm);
    public void DrawCircle(float x, float y, float radius, Pixel p, PositionMode pm = PositionMode.Center)
    {
        if (pm == PositionMode.Normal || pm == PositionMode.CenterY) x += radius;
        if (pm == PositionMode.Normal || pm == PositionMode.CenterX) y += radius;
        int x0 = 0;
        int y0 = (int)radius;
        int d = (int)(3 - 2 * radius);
        if (radius == 0) return;
        while (y0 >= x0)
        {
            DrawPixel(x + x0, y - y0, p);
            DrawPixel(x + y0, y - x0, p);
            DrawPixel(x + y0, y + x0, p);
            DrawPixel(x + x0, y + y0, p);
            DrawPixel(x - x0, y + y0, p);
            DrawPixel(x - y0, y + x0, p);
            DrawPixel(x - y0, y - x0, p);
            DrawPixel(x - x0, y - y0, p);
            if (d < 0) d += 4 * x0++ + 6;
            else d += 4 * (x0++ - y0--) + 10;
        }
    }
    public void DrawCircle(Vector v, float radius, Pixel p, PositionMode pm = PositionMode.Center) => DrawCircle(v.x, v.y, radius, p, pm);
    public void FillCircle(float x, float y, float radius, Pixel p, PositionMode pm = PositionMode.Center)
    {
        if (pm == PositionMode.Normal || pm == PositionMode.CenterY) x += radius;
        if (pm == PositionMode.Normal || pm == PositionMode.CenterX) y += radius;
        for (int x0 = -(int)radius; x0 < radius; x0++)
        {
            int height = (int)Math.Sqrt(radius * radius - x0 * x0);
            for (int y0 = -height; y0 < height; y0++)
                DrawPixel(x0 + x, y0 + y, p);
        }
    }
    public void FillCircle(Vector v, float radius, Pixel p, PositionMode pm = PositionMode.Center) => FillCircle(v.x, v.y, radius, p, pm);
    public void DrawLine(float x1, float y1, float x2, float y2, Pixel p, uint pattern = 0xffffffff)
    {
        x1 = (int)x1; y1 = (int)y1;
        x2 = (int)x2; y2 = (int)y2;
        int x, y, dx, dy, dx1, dy1, px, py, xe, ye, i;
        dx = (int)(x2 - x1); dy = (int)(y2 - y1);
        bool rol() => ((pattern = (pattern << 1) | (pattern >> 31)) & 1) != 0;
        if (dx == 0)
        {
            if (y2 < y1)
            {
                float t = y1;
                y1 = y2;
                y2 = t;
            }
            for (y = (int)y1; y <= y2; y++)
                if (rol()) DrawPixel(x1, y, p);
            return;
        }
        if (dy == 0)
        {
            if (x2 < x1)
            {
                float t = x1;
                x1 = x2;
                x2 = t;
            }
            for (x = (int)x1; x <= x2; x++)
                if (rol()) DrawPixel(x, y1, p);
            return;
        }
        dx1 = Math.Abs(dx); dy1 = Math.Abs(dy);
        px = 2 * dy1 - dx1; py = 2 * dx1 - dy1;
        if (dy1 <= dx1)
        {
            if (dx >= 0)
            {
                x = (int)x1;
                y = (int)y1;
                xe = (int)x2;
            }
            else
            {
                x = (int)x2;
                y = (int)y2;
                xe = (int)x1;
            }
            if (rol()) DrawPixel(x, y, p);
            for (i = 0; x < xe; i++)
            {
                x++;
                if (px < 0) px += 2 * dy1;
                else
                {
                    if ((dx < 0 && dy < 0) || (dx > 0 && dy > 0)) y++;
                    else y--;
                    px += 2 * (dy1 - dx1);
                }
                if (rol()) DrawPixel(x, y, p);
            }
        }
        else
        {
            if (dy >= 0)
            {
                x = (int)x1;
                y = (int)y1;
                ye = (int)y2;
            }
            else
            {
                x = (int)x2;
                y = (int)y2;
                ye = (int)y1;
            }
            if (rol()) DrawPixel(x, y, p);
            for (i = 0; y < ye; i++)
            {
                y++;
                if (py <= 0) py += 2 * dx1;
                else
                {
                    if ((dx < 0 && dy < 0) || (dx > 0 && dy > 0)) x++;
                    else x--;
                    py += 2 * (dx1 - dy1);
                }
                if (rol()) DrawPixel(x, y, p);
            }
        }
    }
    public void DrawLine(Line l, Pixel p, uint pattern = 0xffffffff) => DrawLine(l.sp.x, l.sp.y, l.ep.x, l.ep.y, p, pattern);
    public void DrawLine(Vector v1, Vector v2, Pixel p, uint pattern = 0xffffffff) => DrawLine(v1.x, v1.y, v2.x, v2.y, p, pattern);
    public void DrawLine(float x1, float y1, Vector v2, Pixel p, uint pattern = 0xffffffff) => DrawLine(x1, y1, v2.x, v2.y, p, pattern);
    public void DrawLine(Vector v1, float x2, float y2, Pixel p, uint pattern = 0xffffffff) => DrawLine(v1.x, v1.y, x2, y2, p, pattern);
    public void DrawRectangle(float x, float y, float w, float h, Pixel p, PositionMode pm = PositionMode.Normal)
    {
        if (pm == PositionMode.CenterX || pm == PositionMode.Center) x -= w / 2;
        if (pm == PositionMode.CenterY || pm == PositionMode.Center) y -= h / 2;
        DrawLine(x, y, x + w, y, p);
        DrawLine(x + w, y, x + w, y + h, p);
        DrawLine(x + w, y + h, x, y + h, p);
        DrawLine(x, y + h, x, y, p);
    }
    public void DrawRectangle(Vector v, Vector s, Pixel p, PositionMode pm = PositionMode.Normal) => DrawRectangle(v.x, v.y, s.x, s.y, p, pm);
    public void DrawRectangle(Vector v, float w, float h, Pixel p, PositionMode pm = PositionMode.Normal) => DrawRectangle(v.x, v.y, w, h, p, pm);
    public void DrawRectangle(float x, float y, Vector s, Pixel p, PositionMode pm = PositionMode.Normal) => DrawRectangle(x, y, s.x, s.y, p, pm);
    public void FillRectangle(float x, float y, float w, float h, Pixel p, PositionMode pm = PositionMode.Normal)
    {
        if (pm == PositionMode.CenterX || pm == PositionMode.Center) x -= w / 2;
        if (pm == PositionMode.CenterY || pm == PositionMode.Center) y -= h / 2;
        if (x < 0)
        {
            w += x;
            x = 0;
        }
        if (y < 0)
        {
            h += y;
            y = 0;
        }
        if (x + w >= nScreenWidth) w = nScreenWidth - x - 1;
        if (y + h >= nScreenHeight) h = nScreenHeight - y - 1;
        for (int i = (int)x; i <= x + w; i++)
            for (int j = (int)y; j <= y + h; j++)
                DrawPixel(i, j, p);
    }
    public void FillRectangle(Vector v, Vector s, Pixel p, PositionMode pm = PositionMode.Normal) => FillRectangle(v.x, v.y, s.x, s.y, p, pm);
    public void FillRectangle(Vector v, float w, float h, Pixel p, PositionMode pm = PositionMode.Normal) => FillRectangle(v.x, v.y, w, h, p, pm);
    public void FillRectangle(float x, float y, Vector s, Pixel p, PositionMode pm = PositionMode.Normal) => FillRectangle(x, y, s.x, s.y, p, pm);
    public void DrawTriangle(float x1, float y1, float x2, float y2, float x3, float y3, Pixel p)
    {
        DrawLine(x1, y1, x2, y2, p);
        DrawLine(x2, y2, x3, y3, p);
        DrawLine(x3, y3, x1, y1, p);
    }
    public void DrawTriangle(Vector v1, Vector v2, Vector v3, Pixel p) => DrawTriangle(v1.x, v1.y, v2.x, v2.y, v3.x, v3.y, p);
    public void DrawTriangle(Vector v1, float x2, float y2, float x3, float y3, Pixel p) => DrawTriangle(v1.x, v1.y, x2, y2, x3, y3, p);
    public void DrawTriangle(float x1, float y1, Vector v2, float x3, float y3, Pixel p) => DrawTriangle(x1, y1, v2.x, v2.y, x3, y3, p);
    public void DrawTriangle(float x1, float y1, float x2, float y2, Vector v3, Pixel p) => DrawTriangle(x1, y1, x2, y2, v3.x, v3.y, p);
    public void DrawTriangle(Vector v1, Vector v2, float x3, float y3, Pixel p) => DrawTriangle(v1.x, v1.y, v2.x, v2.y, x3, y3, p);
    public void DrawTriangle(Vector v1, float x2, float y2, Vector v3, Pixel p) => DrawTriangle(v1.x, v1.y, x2, y2, v3.x, v3.y, p);
    public void DrawTriangle(float x1, float y1, Vector v2, Vector v3, Pixel p) => DrawTriangle(x1, y1, v2.x, v2.y, v3.x, v3.y, p);
    public void FillTriangle(float x1, float y1, float x2, float y2, float x3, float y3, Pixel p)
    {
        x1 = (int)x1; y1 = (int)y1;
        x2 = (int)x2; y2 = (int)y2;
        x3 = (int)x3; y3 = (int)y3;
        int t1x, t2x, y, minx, maxx, t1xp, t2xp;
        bool changed1 = false;
        bool changed2 = false;
        int signx1, signx2, dx1, dy1, dx2, dy2;
        int e1, e2;
        if (y1 > y2) { float t = y1; y1 = y2; y2 = t; ; t = x1; x1 = x2; x2 = t; }
        if (y1 > y3) { float t = y1; y1 = y3; y3 = t; ; t = x1; x1 = x3; x3 = t; }
        if (y2 > y3) { float t = y2; y2 = y3; y3 = t; ; t = x2; x2 = x3; x3 = t; }
        t1x = t2x = (int)x1; y = (int)y1;
        dx1 = (int)(x2 - x1); if (dx1 < 0) { dx1 = -dx1; signx1 = -1; }
        else signx1 = 1;
        dy1 = (int)(y2 - y1);
        dx2 = (int)(x3 - x1); if (dx2 < 0) { dx2 = -dx2; signx2 = -1; }
        else signx2 = 1;
        dy2 = (int)(y3 - y1);
        if (dy1 > dx1)
        {
            int t = dx1; dx1 = dy1; dy1 = t;
            changed1 = true;
        }
        if (dy2 > dx2)
        {
            int t = dx2; dx2 = dy2; dy2 = t;
            changed2 = true;
        }
        e2 = dx2 >> 1;
        if (y1 == y2) goto next;
        e1 = dx1 >> 1;
        for (int i = 0; i < dx1;)
        {
            t1xp = 0; t2xp = 0;
            if (t1x < t2x) { minx = t1x; maxx = t2x; }
            else { minx = t2x; maxx = t1x; }
            while (i < dx1)
            {
                i++;
                e1 += dy1;
                while (e1 >= dx1)
                {
                    e1 -= dx1;
                    if (changed1) t1xp = signx1;
                    else goto next1;
                }
                if (changed1) break;
                else t1x += signx1;
            }
        next1:
            while (true)
            {
                e2 += dy2;
                while (e2 >= dx2)
                {
                    e2 -= dx2;
                    if (changed2) t2xp = signx2;
                    else goto next2;
                }
                if (changed2) break;
                else t2x += signx2;
            }
        next2:
            if (minx > t1x) minx = t1x;
            if (minx > t2x) minx = t2x;
            if (maxx < t1x) maxx = t1x;
            if (maxx < t2x) maxx = t2x;
            DrawLine(minx, y, maxx, y, p);
            if (!changed1) t1x += signx1;
            t1x += t1xp;
            if (!changed2) t2x += signx2;
            t2x += t2xp;
            y += 1;
            if (y == y2) break;

        }
    next:
        dx1 = (int)(x3 - x2); if (dx1 < 0) { dx1 = -dx1; signx1 = -1; }
        else signx1 = 1;
        dy1 = (int)(y3 - y2);
        t1x = (int)x2;
        if (dy1 > dx1)
        {
            int t = dx1; dx1 = dy1; dy1 = t;
            changed1 = true;
        }
        else changed1 = false;
        e1 = dx1 >> 1;
        for (int i = 0; i <= dx1; i++)
        {
            t1xp = 0; t2xp = 0;
            if (t1x < t2x) { minx = t1x; maxx = t2x; }
            else { minx = t2x; maxx = t1x; }
            while (i < dx1)
            {
                e1 += dy1;
                while (e1 >= dx1)
                {
                    e1 -= dx1;
                    if (changed1) { t1xp = signx1; break; }
                    else goto next3;
                }
                if (changed1) break;
                else t1x += signx1;
                if (i < dx1) i++;
            }
        next3:
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
            if (minx > t1x) minx = t1x;
            if (minx > t2x) minx = t2x;
            if (maxx < t1x) maxx = t1x;
            if (maxx < t2x) maxx = t2x;
            DrawLine(minx, y, maxx, y, p);
            if (!changed1) t1x += signx1;
            t1x += t1xp;
            if (!changed2) t2x += signx2;
            t2x += t2xp;
            y += 1;
            if (y > y3) return;
        }
    }
    public void FillTriangle(Vector v1, Vector v2, Vector v3, Pixel p) => FillTriangle(v1.x, v1.y, v2.x, v2.y, v3.x, v3.y, p);
    public void FillTriangle(Vector v1, float x2, float y2, float x3, float y3, Pixel p) => FillTriangle(v1.x, v1.y, x2, y2, x3, y3, p);
    public void FillTriangle(float x1, float y1, Vector v2, float x3, float y3, Pixel p) => FillTriangle(x1, y1, v2.x, v2.y, x3, y3, p);
    public void FillTriangle(float x1, float y1, float x2, float y2, Vector v3, Pixel p) => FillTriangle(x1, y1, x2, y2, v3.x, v3.y, p);
    public void FillTriangle(Vector v1, Vector v2, float x3, float y3, Pixel p) => FillTriangle(v1.x, v1.y, v2.x, v2.y, x3, y3, p);
    public void FillTriangle(Vector v1, float x2, float y2, Vector v3, Pixel p) => FillTriangle(v1.x, v1.y, x2, y2, v3.x, v3.y, p);
    public void FillTriangle(float x1, float y1, Vector v2, Vector v3, Pixel p) => FillTriangle(x1, y1, v2.x, v2.y, v3.x, v3.y, p);
    public void FillTexturedTriangle(float x1, float y1, double u1, double v1, float x2, float y2, double u2, double v2, float x3, float y3, double u3, double v3, Sprite s)
    {
        x1 = (int)x1; y1 = (int)y1;
        x2 = (int)x2; y2 = (int)y2;
        x3 = (int)x3; y3 = (int)y3;
        if (y2 < y1)
        {
            Utility.Swap(ref y1, ref y2);
            Utility.Swap(ref x1, ref x2);
            Utility.Swap(ref u1, ref u2);
            Utility.Swap(ref v1, ref v2);
        }
        if (y3 < y1)
        {
            Utility.Swap(ref y1, ref y3);
            Utility.Swap(ref x1, ref x3);
            Utility.Swap(ref u1, ref u3);
            Utility.Swap(ref v1, ref v3);
        }
        if (y3 < y2)
        {
            Utility.Swap(ref y2, ref y3);
            Utility.Swap(ref x2, ref x3);
            Utility.Swap(ref u2, ref u3);
            Utility.Swap(ref v2, ref v3);
        }

        float dy1 = y2 - y1;
        float dx1 = x2 - x1;
        double dv1 = v2 - v1;
        double du1 = u2 - u1;

        float dy2 = y3 - y1;
        float dx2 = x3 - x1;
        double dv2 = v3 - v1;
        double du2 = u3 - u1;

        double tex_u, tex_v;

        double dax_step = 0f, dbx_step = 0f,
               du1_step = 0f, dv1_step = 0f,
               du2_step = 0f, dv2_step = 0f;

        if (dy1 != 0) dax_step = dx1 / (double)Math.Abs(dy1);
        if (dy2 != 0) dbx_step = dx2 / (double)Math.Abs(dy2);

        if (dy1 != 0) du1_step = du1 / Math.Abs(dy1);
        if (dy1 != 0) dv1_step = dv1 / Math.Abs(dy1);

        if (dy2 != 0) du2_step = du2 / Math.Abs(dy2);
        if (dy2 != 0) dv2_step = dv2 / Math.Abs(dy2);

        if (dy1 != 0)
        {
            for (int i = (int)y1; i <= y2; i++)
            {
                int ax = (int)(x1 + (i - y1) * dax_step);
                int bx = (int)(x1 + (i - y1) * dbx_step);

                double tex_su = u1 + (i - y1) * du1_step;
                double tex_sv = v1 + (i - y1) * dv1_step;

                double tex_eu = u1 + (i - y1) * du2_step;
                double tex_ev = v1 + (i - y1) * dv2_step;

                if (ax > bx)
                {
                    Utility.Swap(ref ax, ref bx);
                    Utility.Swap(ref tex_su, ref tex_eu);
                    Utility.Swap(ref tex_sv, ref tex_ev);
                }

                double tstep = 1f / (bx - ax);
                double t = 0f;

                for (int j = ax; j < bx; j++)
                {
                    tex_u = (1.0 - t) * tex_su + t * tex_eu;
                    tex_v = (1.0 - t) * tex_sv + t * tex_ev;

                    Pixel sample = s.GetPixel((int)(tex_u * s.Width), (int)(tex_v * s.Height));
                    DrawPixel(j, i, sample);
                    t += tstep;
                }
            }
        }

        dy1 = y3 - y2;
        dx1 = x3 - x2;
        dv1 = v3 - v2;
        du1 = u3 - u2;

        if (dy1 != 0) dax_step = dx1 / (double)Math.Abs(dy1);
        if (dy2 != 0) dbx_step = dx2 / (double)Math.Abs(dy2);

        du1_step = 0;
        dv1_step = 0;

        if (dy1 != 0) du1_step = du1 / Math.Abs(dy1);
        if (dy1 != 0) dv1_step = dv1 / Math.Abs(dy1);

        if (dy1 != 0)
        {
            for (int i = (int)y2; i <= y3; i++)
            {
                int ax = (int)(x2 + (i - y2) * dax_step);
                int bx = (int)(x1 + (i - y1) * dbx_step);

                double tex_su = u2 + (i - y2) * du1_step;
                double tex_sv = v2 + (i - y2) * dv1_step;

                double tex_eu = u1 + (i - y1) * du2_step;
                double tex_ev = v1 + (i - y1) * dv2_step;

                if (ax > bx)
                {
                    Utility.Swap(ref ax, ref bx);
                    Utility.Swap(ref tex_su, ref tex_eu);
                    Utility.Swap(ref tex_sv, ref tex_ev);
                }

                double tstep = 1f / (bx - ax);
                double t = 0f;

                for (int j = ax; j < bx; j++)
                {
                    tex_u = (1.0 - t) * tex_su + t * tex_eu;
                    tex_v = (1.0 - t) * tex_sv + t * tex_ev;

                    Pixel sample = s.GetPixel((int)(tex_u * s.Width), (int)(tex_v * s.Height));
                    DrawPixel(j, i, sample);
                    t += tstep;
                }
            }
        }
    }
    public void FillTexturedTriangle(Vector p1, float u1, float v1, float x2, float y2, float u2, float v2, float x3, float y3, float u3, float v3, Sprite s) => FillTexturedTriangle(p1.x, p1.y, u1, v1, x2, y2, u2, v2, x3, y3, u3, v3, s);
    public void FillTexturedTriangle(float x1, float y1, double u1, double v1, Vector p2, double u2, double v2, float x3, float y3, double u3, double v3, Sprite s) => FillTexturedTriangle(x1, y1, u1, v1, p2.x, p2.y, u2, v2, x3, y3, u3, v3, s);
    public void FillTexturedTriangle(float x1, float y1, double u1, double v1, float x2, float y2, double u2, double v2, Vector p3, double u3, double v3, Sprite s) => FillTexturedTriangle(x1, y1, u1, v1, x2, y2, u2, v2, p3.x, p3.y, u3, v3, s);
    public void FillTexturedTriangle(Vector p1, double u1, double v1, Vector p2, double u2, double v2, float x3, float y3, double u3, double v3, Sprite s) => FillTexturedTriangle(p1.x, p1.y, u1, v1, p2.x, p2.y, u2, v2, x3, y3, u3, v3, s);
    public void FillTexturedTriangle(Vector p1, double u1, double v1, float x2, float y2, double u2, double v2, Vector p3, double u3, double v3, Sprite s) => FillTexturedTriangle(p1.x, p1.y, u1, v1, x2, y2, u2, v2, p3.x, p3.y, u3, v3, s);
    public void FillTexturedTriangle(float x1, float y1, double u1, double v1, Vector p2, double u2, double v2, Vector p3, double u3, double v3, Sprite s) => FillTexturedTriangle(x1, y1, u1, v1, p2.x, p2.y, u2, v2, p3.x, p3.y, u3, v3, s);
    public void FillTexturedTriangle(Vector p1, double u1, double v1, Vector p2, double u2, double v2, Vector p3, double u3, double v3, Sprite s) => FillTexturedTriangle(p1.x, p1.y, u1, v1, p2.x, p2.y, u2, v2, p3.x, p3.y, u3, v3, s);
    public void SaveFrame(string locationpath, string name, string format = "bmp")
    {
        Bitmap toSave = new Bitmap(nWindowWidth, nWindowHeight);
        DrawPixelsToBuffer(ref toSave);
        ImageFormat imf;
        format.ToLower();
        switch (format)
        {
            case "png": imf = ImageFormat.Png; break;
            case "bmp": imf = ImageFormat.Bmp; break;
            case "gif": imf = ImageFormat.Gif; break;
            case "jpeg": imf = ImageFormat.Jpeg; break;
            case "tiff": imf = ImageFormat.Tiff; break;
            default: imf = ImageFormat.Bmp; format = "bmp"; break;
        }
        toSave.Save($"{locationpath}/{name}.{format}", imf);
        toSave.Dispose();
    }
}
public struct Pixel
{
    public static Pixel White = new Pixel(255, 255, 255);
    public static Pixel Grey = new Pixel(192, 192, 192);
    public static Pixel DarkGrey = new Pixel(128, 128, 128);
    public static Pixel VeryDarkGrey = new Pixel(64, 64, 64);
    public static Pixel Red = new Pixel(255, 0, 0);
    public static Pixel DarkRed = new Pixel(128, 0, 0);
    public static Pixel VeryDarkRed = new Pixel(64, 0, 0);
    public static Pixel Yellow = new Pixel(255, 255, 0);
    public static Pixel DarkYellow = new Pixel(128, 128, 0);
    public static Pixel VeryDarkYellow = new Pixel(64, 64, 0);
    public static Pixel Green = new Pixel(0, 255, 0);
    public static Pixel DarkGreen = new Pixel(0, 128, 0);
    public static Pixel VeryDarkGreen = new Pixel(0, 64, 0);
    public static Pixel Cyan = new Pixel(0, 255, 255);
    public static Pixel DarkCyan = new Pixel(0, 128, 128);
    public static Pixel VeryDarkCyan = new Pixel(0, 64, 64);
    public static Pixel Blue = new Pixel(0, 0, 255);
    public static Pixel DarkBlue = new Pixel(0, 0, 128);
    public static Pixel VeryDarkBlue = new Pixel(0, 0, 64);
    public static Pixel Magenta = new Pixel(255, 0, 255);
    public static Pixel DarkMagenta = new Pixel(128, 0, 128);
    public static Pixel VeryDarkMagenta = new Pixel(64, 0, 64);
    public static Pixel Black = new Pixel(0, 0, 0);
    public static Pixel Blank = new Pixel(0, 0, 0, 0);
    public readonly byte r;
    public readonly byte g;
    public readonly byte b;
    public readonly byte a;
    public readonly uint val;
    private Pixel(int red, int green, int blue, int alpha = 255)
    {
        if (red < 0) red = 0;
        if (red > 255) red = 255;
        if (green < 0) green = 0;
        if (green > 255) green = 255;
        if (blue < 0) blue = 0;
        if (blue > 255) blue = 255;
        if (alpha < 0) alpha = 0;
        if (alpha > 255) alpha = 255;
        r = (byte)red;
        g = (byte)green;
        b = (byte)blue;
        a = (byte)alpha;
        val = (uint)(a << 24) + (uint)(r << 16) + (uint)(g << 8) + b;
    }
    public static Pixel Random() => new Pixel(FormGameEngine.rnd.Next(256), FormGameEngine.rnd.Next(256), FormGameEngine.rnd.Next(256));
    public static Pixel RandomFull() => new Pixel(FormGameEngine.rnd.Next(256), FormGameEngine.rnd.Next(256), 
                                                  FormGameEngine.rnd.Next(256), FormGameEngine.rnd.Next(256));
    public static Pixel RGB(int red, int green, int blue, int alpha = 255) => new Pixel(red, green, blue, alpha);
    public static Pixel HEX(uint value)
    {
        byte b = (byte)(value & 0xff);
        byte g = (byte)((value >> 8) & 0xff);
        byte r = (byte)((value >> 16) & 0xff);
        byte a = (byte)((value >> 24) & 0xff);
        return new Pixel(r, g, b, a);
    }
    public static Pixel HSV(int hue, int saturation, int value, int alpha = 255)
    {
        if (hue < 0) hue = 0;
        if (hue >= 360) hue = 359;
        if (saturation < 0) saturation = 0;
        if (saturation > 100) saturation = 100;
        if (value < 0) value = 0;
        if (value > 100) value = 100;
        float c, x, m, r, g, b;
        c = (value / 100f) * (saturation / 100f);
        float num = hue / 60f % 2 - 1;
        if (num < 0) num *= -1;
        x = c * (1 - num);
        m = value / 100f - c;
        if (hue >= 0 && hue < 60)
        {
            r = (c + m) * 255;
            g = (x + m) * 255;
            b = (0 + m) * 255;
        }
        else if (hue >= 60 && hue < 120)
        {
            r = (x + m) * 255;
            g = (c + m) * 255;
            b = (0 + m) * 255;
        }
        else if (hue >= 120 && hue < 180)
        {
            r = (0 + m) * 255;
            g = (c + m) * 255;
            b = (x + m) * 255;
        }
        else if (hue >= 180 && hue < 240)
        {
            r = (0 + m) * 255;
            g = (x + m) * 255;
            b = (c + m) * 255;
        }
        else if (hue >= 240 && hue < 300)
        {
            r = (x + m) * 255;
            g = (0 + m) * 255;
            b = (c + m) * 255;
        }
        else if (hue >= 300 && hue < 360)
        {
            r = (c + m) * 255;
            g = (0 + m) * 255;
            b = (x + m) * 255;
        }
        else return White;
        return new Pixel((int)r, (int)g, (int)b, alpha);
    }
    public static bool operator ==(Pixel p1, Pixel p2) => p1.val == p2.val;
    public static bool operator !=(Pixel p1, Pixel p2) => p1.val != p2.val;
    public static Pixel operator +(Pixel p, int s) => new Pixel(p.r + s, p.g + s, p.b + s, p.a);
    public static Pixel operator +(int s, Pixel p) => new Pixel(p.r + s, p.g + s, p.b + s, p.a);
    public static Pixel operator -(Pixel p, int s) => new Pixel(p.r - s, p.g - s, p.b - s, p.a);
    public static Pixel operator -(int s, Pixel p) => new Pixel(p.r - s, p.g - s, p.b - s, p.a);
    public static Pixel operator +(Pixel p1, Pixel p2) => new Pixel(p1.r + p2.r, p1.g + p2.g, p1.b + p2.b, p1.a);
    public static Pixel operator -(Pixel p1, Pixel p2) => new Pixel(p1.r - p2.r, p1.g - p2.g, p1.b - p2.b, p1.a);
}
public class Sprite
{
    private readonly Pixel[] pixelData;
    public readonly int Width;
    public readonly int Height;
    public readonly Vector Bounds;
    public Sprite(string filepath, int? width = null, int? height = null)
    {
        Bitmap spriteData;
        if (width != null && height != null)
            spriteData = new Bitmap(new Bitmap(filepath), (int)width, (int)height);
        else
            spriteData = new Bitmap(filepath);
        Width = spriteData.Width;
        Height = spriteData.Height;
        Bounds = new Vector(Width, Height);
        pixelData = new Pixel[Width * Height];
        unsafe
        {
            Rectangle area = new Rectangle(0, 0, Width, Height);
            BitmapData bd = spriteData.LockBits(area, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            byte* pixel = (byte*)bd.Scan0;
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    int b = pixel[y * Width * 4 + x * 4 + 0];
                    int g = pixel[y * Width * 4 + x * 4 + 1];
                    int r = pixel[y * Width * 4 + x * 4 + 2];
                    int a = pixel[y * Width * 4 + x * 4 + 3];
                    pixelData[y * Width + x] = Pixel.RGB(r, g, b, a);
                }
            }
            spriteData.UnlockBits(bd);
        }
    }
    public Sprite(int width, int height)
    {
        Width = width;
        Height = height;
        pixelData = new Pixel[Width * Height];
        for (int i = 0; i < pixelData.Length; i++)
            pixelData[i] = Pixel.Blank;
    }
    public Pixel GetPixel(float x, float y)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
            return pixelData[(int)y * Width + (int)x];
        return Pixel.Blank;
    }
    public Pixel GetPixel(Vector v) => GetPixel(v.x, v.y);
    public void SetPixel(float x, float y, Pixel p)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
            pixelData[(int)y * Width + (int)x] = p;
    }
    public void SetPixel(Vector v, Pixel p) => SetPixel(v.x, v.y, p);
}
public class Sound
{
    private readonly SoundPlayer sp;
    private bool bPlaying;
    public Sound(string filepath)
    {
        sp = new SoundPlayer(filepath);
        bPlaying = false;
    }
    ~Sound()
    {
        sp.Dispose();
    }
    public void PlayOnce()
    {
        if (!bPlaying)
        {
            sp.Play();
            bPlaying = true;
        }
    }
    public void Play()
    {
        sp.Play();
    }
    public void Stop()
    {
        if (bPlaying)
        {
            sp.Stop();
            bPlaying = false;
        }
    }
    public void PlayLoop()
    {
        if (!bPlaying)
        {
            sp.PlayLooping();
            bPlaying = true;
        }
    }
}
public class ButtonUI
{
    public Vector pos;
    public Vector size;
    public string text;
    public int textSize;
    public Pixel textColor;
    public Pixel idleColor;
    public Pixel hoverColor;
    public Pixel activeColor;
    public bool clicked;
    public bool hover;
    public ButtonUI(float px, float py, float sx, float sy, string text)
    {
        clicked = false;
        hover = false;
        pos = new Vector(px, py);
        size = new Vector(sx, sy);
        this.text = text;
        textSize = 1;
        textColor = Pixel.Black;
        idleColor = Pixel.Grey;
        hoverColor = Pixel.DarkGrey;
        activeColor = Pixel.VeryDarkGrey;
    }
    public ButtonUI(Vector p, float sx, float sy, string text) : this(p.x, p.y, sx, sy, text) { }
    public ButtonUI(float px, float py, Vector s, string text) : this(px, py, s.x, s.y, text) { }
    public ButtonUI(Vector p, Vector s, string text) : this(p.x, p.y, s.x, s.y, text) { }
    public void Render(FormGameEngine fge)
    {
        clicked = false;
        hover = false;
        if (fge.ScreenMousePos().InsideBounds(pos, pos + size)) 
        {
            hover = true;
            if (Input.KeyHeld(MouseButtons.Left))
            {
                fge.FillRectangle(pos, size, activeColor);
                fge.DrawString(pos + size / 2, text, textColor, textSize, PositionMode.Center);
            }
            else
            {
                fge.FillRectangle(pos, size, hoverColor);
                fge.DrawString(pos + size / 2, text, textColor, textSize, PositionMode.Center);
            }
            if (Input.KeyReleased(MouseButtons.Left))
                clicked = true;
        }
        else
        {
            fge.FillRectangle(pos, size, idleColor);
            fge.DrawString(pos + size / 2, text, textColor, textSize, PositionMode.Center);
        }
    }
}
public class InputUI
{
    public Vector pos;
    public Vector size;
    public string value;
    public bool focused;
    public int textSize;
    public Pixel textColor;
    public Pixel idleColor;
    public Pixel focusColor;
    static readonly List<(Keys, char)> inputKeys = new List<(Keys, char)>() { (Keys.D1,'1'), (Keys.D2, '2'), (Keys.D3, '3'), (Keys.D4, '4'), 
        (Keys.D5, '5') , (Keys.D6, '6'), (Keys.D7,'7'), (Keys.D8,'8'), (Keys.D9,'9'), (Keys.D0,'0'), (Keys.NumPad1,'1'), (Keys.NumPad2, '2'), 
        (Keys.NumPad3, '3'),(Keys.NumPad4, '4'), (Keys.NumPad5, '5') , (Keys.NumPad6, '6'),(Keys.NumPad7,'7'), (Keys.NumPad8,'8'), 
        (Keys.NumPad9,'9'), (Keys.NumPad0,'0'), (Keys.Q,'q'), (Keys.Q,'q'), (Keys.W,'w'), (Keys.E,'e'), (Keys.R,'r'),(Keys.T,'t'), 
        (Keys.Y,'y'), (Keys.U,'u'), (Keys.I,'i'), (Keys.O,'o'), (Keys.P,'p'), (Keys.A,'a'), (Keys.S,'s'), (Keys.D,'d'), (Keys.F,'f'), 
        (Keys.G,'g'), (Keys.H,'h'), (Keys.J,'j'), (Keys.K,'k'), (Keys.L,'l'), (Keys.Z,'z'), (Keys.X,'x'), (Keys.C,'c'), (Keys.V,'v'),
        (Keys.B,'b'), (Keys.N,'n'), (Keys.M,'m'), (Keys.Space, ' '), (Keys.Back, '\0')};
    public InputUI(float px, float py, float sx, float sy)
    {
        pos = new Vector(px, py);
        size = new Vector(sx, sy);
        value = "";
        textSize = 1;
        textColor = Pixel.Black;
        idleColor = Pixel.Grey;
        focusColor = Pixel.DarkGrey;
    }
    public InputUI(Vector p, float sx, float sy) : this(p.x, p.y, sx, sy) { }
    public InputUI(float px, float py, Vector s) : this(px, py, s.x, s.y) { }
    public InputUI(Vector p, Vector s) : this(p.x, p.y, s.x, s.y) { }
    public void Render(FormGameEngine fge)
    {
        if (Input.KeyPressed(MouseButtons.Left))
        {
            if (fge.ScreenMousePos().InsideBounds(pos, pos + size)) 
                focused = true;
            else
                focused = false;
        }
        if (focused)
        {
            foreach (var keyPair in inputKeys)
            {
                if (Input.KeyPressed(keyPair.Item1))
                {
                    if (keyPair.Item1 == Keys.Back)
                    {
                        if (value.Length > 0)
                            value = value.Remove(value.Length - 1);
                    }
                    else
                    {
                        if (keyPair.Item2 >= 'a' && keyPair.Item2 <= 'z' && Input.KeyHeld(Keys.ShiftKey))
                            value += (char)(keyPair.Item2 - 32);
                        else
                            value += keyPair.Item2;
                    }
                }
            }
            fge.FillRectangle(pos, size, focusColor);
            fge.DrawString(pos.x, pos.y + size.y / 2, value, textColor, textSize, PositionMode.CenterY);
        }
        else
        {
            fge.FillRectangle(pos, size, idleColor);
            fge.DrawString(pos.x, pos.y + size.y / 2, value, textColor, textSize, PositionMode.CenterY);
        }
    }
}
public static class Utility
{
    public static uint seed = (uint)FormGameEngine.rnd.Next(int.MaxValue);
    public static uint SeededRandom(uint? rootSeed = null)
    {
        if (rootSeed != null) seed = (uint)rootSeed;
        seed += 0xe120fc15;
        ulong tmp = (ulong)seed * 0x4a39b70d;
        uint m1 = (uint)((tmp >> 32) ^ tmp);
        tmp = (ulong)m1 * 0x12fad5c9;
        uint m2 = (uint)((tmp >> 32) ^ tmp);
        return m2;
    }
    public static int SeededRandomInt(int min, int max, uint? seed = null) => 
        (int)((max - min) * (SeededRandom(seed) / (float)uint.MaxValue) + min);
    public static float SeededRandomFloat(float min, float max, uint? seed = null) => 
        (max - min) * (SeededRandom(seed) / (float)uint.MaxValue) + min;
    public static float[] SimpleNoise1D(float[] seed, int octaves = 8, float bias = 2f, bool wrap = false)
    {
        int nCount = seed.Length;
        float[] fOutput = new float[nCount];
        for (int x = 0; x < nCount; x++)
        {
            float fNoise = 0f;
            float fScaleAcc = 0f;
            float fScale = 1f;
            for (int o = 0; o < octaves; o++)
            {
                int nPitch = nCount >> o;
                int nSample1 = (x / nPitch) * nPitch;
                int ns = nSample1 + nPitch;
                int nSample2 = (ns < nCount) ? nSample1 + nPitch : (wrap ? ns % nCount : nCount - 1);
                float fBlend = (x - nSample1) / (float)nPitch;
                float fSample = (1.0f - fBlend) * seed[nSample1] + fBlend * seed[nSample2];
                fScaleAcc += fScale;
                fNoise += fSample * fScale;
                fScale /= bias;
            }
            fOutput[x] = fNoise / fScaleAcc;
        }
        return fOutput;
    }
    public static float[,] SimpleNoise2D(float[,] seed, int octaves = 8, float bias = 2f, bool wrap = false)
    {
        int nWidth = seed.GetLength(0);
        int nHeight = seed.GetLength(1);
        float[,] fOutput = new float[nWidth, nHeight];
        for (int x = 0; x < nWidth; x++)
            for (int y = 0; y < nHeight; y++)
            {
                float fNoise = 0.0f;
                float fScaleAcc = 0.0f;
                float fScale = 1.0f;
                for (int o = 0; o < octaves; o++)
                {
                    int nPitchX = nWidth >> o;
                    int nPitchY = nHeight >> o;
                    int nSampleX1 = (x / nPitchX) * nPitchX;
                    int nSampleY1 = (y / nPitchY) * nPitchY;
                    int nsx = nSampleX1 + nPitchX;
                    int nsy = nSampleY1 + nPitchY;
                    int nSampleX2 = (nsx < nWidth) ? nsx : (wrap ? nsx % nWidth : nWidth - 1);
                    int nSampleY2 = (nsy < nHeight) ? nsy : (wrap ? nsy % nHeight : nHeight - 1);
                    float fBlendX = (x - nSampleX1) / (float)nPitchX;
                    float fBlendY = (y - nSampleY1) / (float)nPitchY;
                    float fSampleT = (1.0f - fBlendX) * seed[nSampleX1, nSampleY1] + fBlendX * seed[nSampleX2, nSampleY1];
                    float fSampleB = (1.0f - fBlendX) * seed[nSampleX1, nSampleY2] + fBlendX * seed[nSampleX2, nSampleY2];
                    fScaleAcc += fScale;
                    fNoise += (fBlendY * (fSampleB - fSampleT) + fSampleT) * fScale;
                    fScale /= bias;
                }
                fOutput[x, y] = fNoise / fScaleAcc;
            }
        return fOutput;
    }
    public static float Map(float value, float valueLowLimit, float valueHighLimit, float mapLowLimit, float mapHighLimit)
    {
        if (value >= valueHighLimit) return mapHighLimit;
        if (value <= valueLowLimit) return mapLowLimit;
        float procent = (value - valueLowLimit) / (valueHighLimit - valueLowLimit);
        float limitLen = mapHighLimit - mapLowLimit;
        return mapLowLimit + procent * limitLen;
    }
    public static void Swap<T>(ref T a, ref T b)
    {
        T t = b;
        b = a;
        a = t;
    }
    public static float Degrees(float radians) => (float)(180f / Math.PI * radians);
    public static float Radians(float degrees) => (float)(degrees * Math.PI / 180f);
    public static Vector CollisionSAT(Vector[] pa, Vector[] pb)
    {
        float overlap = float.MaxValue;
        Vector resolve = new Vector();
        for (int s = 0; s < 2; s++)
        {
            if (s == 1) Swap(ref pa, ref pb);
            for (int p = 0; p < pa.Length; p++)
            {
                Vector axp = new Vector(-(pa[(p + 1) % pa.Length].y - pa[p].y), pa[(p + 1) % pa.Length].x - pa[p].x).Normalize();
                float min_r1 = float.MaxValue;
                float max_r1 = float.MinValue;
                for (int pt = 0; pt < pa.Length; pt++)
                {
                    float dot = pa[pt].Dot(axp);
                    min_r1 = Math.Min(min_r1, dot);
                    max_r1 = Math.Max(max_r1, dot);
                }
                float min_r2 = float.MaxValue;
                float max_r2 = float.MinValue;
                for (int pt = 0; pt < pb.Length; pt++)
                {
                    float dot = pb[pt].Dot(axp);
                    min_r2 = Math.Min(min_r2, dot);
                    max_r2 = Math.Max(max_r2, dot);
                }
                float co = Math.Min(max_r1, max_r2) - Math.Max(min_r1, min_r2);
                if (co < overlap)
                {
                    if (co < 0f) return new Vector();
                    overlap = co;
                    resolve = axp;
                }
            }
        }
        Vector mida = new Vector();
        for (int i = 0; i < pa.Length; i++) mida += pa[i];
        mida /= pa.Length;
        Vector midb = new Vector();
        for (int i = 0; i < pb.Length; i++) midb += pb[i];
        midb /= pb.Length;
        Vector cd = (mida - midb).Normalize();
        if (cd.Dot(resolve) < 0f) resolve *= -1f;
        return resolve * -overlap;
    }
}
public static class Time
{
    public static Stopwatch s;
    private static TimeSpan t;
    private static double t1;
    private static double t2;
    public static float fElapsedTime;
    public static float fTotalTime;
    public static void Start()
    {
        t1 = 0;
        t2 = 0;
        s = new Stopwatch();
        s.Start();
        Calculate();
    }
    public static void Calculate()
    {
        t = s.Elapsed;
        t1 = t.TotalSeconds;
        fElapsedTime = (float)(t1 - t2);
        t2 = t1;
        fTotalTime += fElapsedTime;
    }
}
public class Vector
{
    public static Vector Zero = new Vector(0, 0, 0);
    public static Vector Up = new Vector(0, 1, 0);
    public static Vector Down = new Vector(0, -1, 0);
    public static Vector Right = new Vector(1, 0, 0);
    public static Vector Left = new Vector(-1, 0, 0);
    public static Vector Forward = new Vector(0, 0, 1);
    public static Vector Back = new Vector(0, 0, -1);
    public float x;
    public float y;
    public float z;
    public Vector(float x = 0f, float y = 0f, float z = 0f)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    public Vector(PointF p)
    {
        x = p.X;
        y = p.Y;
        z = 0f;
    }
    public Vector(SizeF s)
    {
        x = s.Width;
        y = s.Height;
        z = 0f;
    }
    public static Vector RandomVector() => VectorFromAngle((float)(FormGameEngine.rnd.NextDouble() * Math.PI) * 2f);
    public Vector Clone() => new Vector(x, y, z);
    public void Reset()
    {
        x = 0f;
        y = 0f;
        z = 0f;
    }
    public float Dot(Vector v) => (x * v.x + y * v.y + z * v.z);
    public Vector Cross(Vector v) => new Vector(y * v.z - z * v.y, z * v.x - x * v.z, x * v.y - y * v.x);
    public float Magnitude() => (float)Math.Sqrt(x * x + y * y + z * z);
    public float MagnitudeSquared() => (x * x + y * y + z * z);
    public Vector SetMagnitude(float value)
    {
        Vector v = Normalize() * value;
        x = v.x;
        y = v.y;
        z = v.z;
        return Clone();
    }
    public Vector Normalize()
    {
        float d = Magnitude();
        if (d != 0f)
        {
            x /= d;
            y /= d;
            z /= d;
        }
        return Clone();
    }
    public Vector Limit(float value)
    {
        if (Magnitude() > value)
        {
            Vector v = Normalize() * value;
            x = v.x;
            y = v.y;
            z = v.z;
        }
        return Clone();
    }
    public Vector Rotate(float angle, Vector pivot = null)
    {
        if (pivot == null) pivot = new Vector();
        float x_ = (float)(Math.Cos(angle) * (x - pivot.x) - Math.Sin(angle) * (y - pivot.y)) + pivot.x;
        float y_ = (float)(Math.Cos(angle) * (y - pivot.y) + Math.Sin(angle) * (x - pivot.x)) + pivot.y;
        x = x_;
        y = y_;
        return Clone();
    }
    public Vector RotateFixed(float angle, Vector pivot = null)
    {
        if (pivot == null) pivot = new Vector();
        float ang = (this - pivot).AngleFromVector();
        return Rotate(angle - ang, pivot);
    }
    public Vector Lerp(Vector v) => (this + v) * 0.5f;
    public Vector IntVals()
    {
        x = (int)x;
        y = (int)y;
        z = (int)z;
        return Clone();
    }
    public static Vector VectorFromAngle(float angle) => new Vector((float)Math.Cos(angle), (float)Math.Sin(angle));
    public float AngleFromVector() => (float)Math.Atan2(y, x);
    public float AngleBetween(Vector v, Vector pivot = null)
    {
        if (pivot == null) pivot = new Vector();
        Vector v1 = this - pivot;
        Vector v2 = v - pivot;
        float mag = v1.Magnitude() * v2.Magnitude();
        float dot = v1.Dot(v2);
        float val = dot / mag;
        if (mag == 0f) val = 0f;
        if (val < -1f) val = -1f;
        if (val > 1f) val = 1f;
        return (float)Math.Acos(val);
    }
    public bool InsideBounds(float x1, float y1, float x2, float y2)
    {
        if (x1 > x2) Utility.Swap(ref x1, ref x2);
        if (y1 > y2) Utility.Swap(ref y1, ref y2);
        if (x >= x1 && y >= y1 && x <= x2 && y <= y2) return true;
        return false;
    }
    public bool InsideBounds(Vector v1, float x2, float y2) => InsideBounds(v1.x, v1.y, x2, y2);
    public bool InsideBounds(float x1, float y1, Vector v2) => InsideBounds(x1, y1, v2.x, v2.y);
    public bool InsideBounds(Vector v1, Vector v2) => InsideBounds(v1.x, v1.y, v2.x, v2.y);
    public void KeepInBounds(float x1, float y1, float x2, float y2, bool warp = false)
    {
        if (x1 > x2) Utility.Swap(ref x1, ref x2);
        if (y1 > y2) Utility.Swap(ref y1, ref y2);
        if (warp)
        {
            if (x < x1) x = x2;
            if (y < y1) y = y2;
            if (x > x2) x = x1;
            if (y > y2) y = y1;
        }
        else
        {
            if (x < x1) x = x1;
            if (y < y1) y = y1;
            if (x > x2) x = x2;
            if (y > y2) y = y2;
        }
    }
    public void KeepInBounds(Vector v1, float x2, float y2, bool wrap = false) => KeepInBounds(v1.x, v1.y, x2, y2, wrap);
    public void KeepInBounds(float x1, float y1, Vector v2, bool wrap = false) => KeepInBounds(x1, y1, v2.x, v2.y, wrap);
    public void KeepInBounds(Vector v1, Vector v2, bool wrap = false) => KeepInBounds(v1.x, v1.y, v2.x, v2.y, wrap);
    public static PointF[] AsPoints(Vector[] v)
    {
        PointF[] p = new PointF[v.Length];
        for (int i = 0; i < p.Length; i++) p[i] = v[i].AsPoint();
        return p;
    }
    public PointF AsPoint() => new PointF(x, y);
    public override string ToString() => $"x: {x}, y: {y}, z: {z}";
    public static Vector operator *(Vector v1, float l) => new Vector(v1.x * l, v1.y * l, v1.z * l);
    public static Vector operator *(float l, Vector v1) => new Vector(v1.x * l, v1.y * l, v1.z * l);
    public static Vector operator /(Vector v1, float l) => new Vector(v1.x / l, v1.y / l, v1.z / l);
    public static Vector operator /(float l, Vector v1) => new Vector(v1.x / l, v1.y / l, v1.z / l);
    public static Vector operator +(Vector v1, Vector v2) => new Vector(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
    public static Vector operator -(Vector v1, Vector v2) => new Vector(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
    public static Vector operator *(Vector v1, Vector v2) => new Vector(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
    public static Vector operator /(Vector v1, Vector v2) => new Vector(v1.x / v2.x, v1.y / v2.y, v1.z / v2.z);
}
public class Matrix
{
    public float[,] matrix;
    public int rows;
    public int cols;
    public Matrix(int n, int m)
    {
        rows = n;
        cols = m;
        matrix = new float[rows, cols];
        for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
                matrix[i, j] = 0f;
    }
    public Matrix(float[,] sample)
    {
        rows = sample.GetLength(0);
        cols = sample.GetLength(1);
        matrix = new float[rows, cols];
        for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
                matrix[i, j] = sample[i, j];
    }
    public static Matrix Identity(int n, int m)
    {
        Matrix mat = new Matrix(n, m);
        for (int i = 0; i < n; i++)
            mat.matrix[i, i] = 1f;
        return mat;
    }
    public static Matrix Incremented(int n, int m, int startvalue = 1)
    {
        Matrix mat = new Matrix(n, m);
        for (int i = 0; i < n; i++)
            for (int j = 0; j < m; j++)
                mat.matrix[i, j] = startvalue++;
        return mat;
    }
    public static Matrix RotationX(float a)
    {
        Matrix mat = new Matrix(3, 3);
        mat.matrix[0, 0] = 1f;
        mat.matrix[1, 1] = (float)Math.Cos(a);
        mat.matrix[1, 2] = -(float)Math.Sin(a);
        mat.matrix[2, 1] = (float)Math.Sin(a);
        mat.matrix[2, 2] = (float)Math.Cos(a);
        return mat;
    }
    public static Matrix RotationY(float a)
    {
        Matrix mat = new Matrix(3, 3);
        mat.matrix[0, 0] = (float)Math.Cos(a);
        mat.matrix[0, 2] = (float)Math.Sin(a);
        mat.matrix[1, 1] = 1f;
        mat.matrix[2, 0] = -(float)Math.Sin(a);
        mat.matrix[2, 2] = (float)Math.Cos(a);
        return mat;
    }
    public static Matrix RotationZ(float a)
    {
        Matrix mat = new Matrix(3, 3);
        mat.matrix[0, 0] = (float)Math.Cos(a);
        mat.matrix[0, 1] = -(float)Math.Sin(a);
        mat.matrix[1, 0] = (float)Math.Sin(a);
        mat.matrix[1, 1] = (float)Math.Cos(a);
        mat.matrix[2, 2] = 1f;
        return mat;
    }
    public static Matrix RotationXYZ(float ax, float ay, float az)
    {
        Matrix mat = new Matrix(3, 3);
        mat.matrix[0, 0] = (float)(Math.Cos(ay) * Math.Cos(az));
        mat.matrix[0, 1] = (float)(-Math.Cos(ay) * Math.Sin(az));
        mat.matrix[0, 2] = (float)Math.Sin(ay);
        mat.matrix[1, 0] = (float)(Math.Cos(ax) * Math.Sin(az) + Math.Sin(ax) * Math.Sin(ay) * Math.Cos(az));
        mat.matrix[1, 1] = (float)(Math.Cos(ax) * Math.Cos(az) - Math.Sin(ax) * Math.Sin(ay) * Math.Sin(az));
        mat.matrix[1, 2] = (float)(-Math.Sin(ax) * Math.Cos(ay));
        mat.matrix[2, 0] = (float)(Math.Sin(ax) * Math.Sin(az) - Math.Cos(ax) * Math.Sin(ay) * Math.Cos(az));
        mat.matrix[2, 1] = (float)(Math.Sin(ax) * Math.Cos(az) - Math.Cos(ax) * Math.Sin(ay) * Math.Sin(az));
        mat.matrix[2, 2] = (float)(Math.Cos(ax) * Math.Cos(ay));
        return mat;
    }
    public static Matrix RotationGivenAxis(Vector u, float a)
    {
        u.Normalize();
        Matrix mat = new Matrix(3, 3);
        mat.matrix[0, 0] = (float)(Math.Cos(a) + u.x * u.x * (1 - Math.Cos(a)));
        mat.matrix[0, 1] = (float)(u.x * u.y * (1 - Math.Cos(a)) - u.z * Math.Sin(a));
        mat.matrix[0, 2] = (float)(u.x * u.z * (1 - Math.Cos(a)) + u.y * Math.Sin(a));
        mat.matrix[1, 0] = (float)(u.y * u.x * (1 - Math.Cos(a)) + u.z * Math.Sin(a));
        mat.matrix[1, 1] = (float)(Math.Cos(a) + u.y * u.y * (1 - Math.Cos(a)));
        mat.matrix[1, 2] = (float)(u.y * u.z * (1 - Math.Cos(a)) - u.x * Math.Sin(a));
        mat.matrix[2, 0] = (float)(u.z * u.x * (1 - Math.Cos(a)) - u.y * Math.Sin(a));
        mat.matrix[2, 1] = (float)(u.z * u.y * (1 - Math.Cos(a)) + u.x * Math.Sin(a));
        mat.matrix[2, 2] = (float)(Math.Cos(a) + u.z * u.z * (1 - Math.Cos(a)));
        return mat;
    }
    public static Matrix RandomMatrix(int rows, int cols, int minval = 0, int maxval = 9)
    {
        Matrix mat = new Matrix(rows, cols);
        for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
                mat.matrix[i, j] = FormGameEngine.rnd.Next(minval, maxval + 1);
        return mat;
    }
    public Matrix Clone()
    {
        Matrix mat = new Matrix(rows, cols);
        for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
                mat.matrix[i, j] = matrix[i, j];
        return mat;
    }
    public Matrix Transpuse()
    {
        Matrix mat = new Matrix(rows, cols);
        for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
                mat.matrix[j, i] = matrix[i, j];
        return mat;
    }
    public Matrix RemoveLines(int row, int col)
    {
        Matrix mat = new Matrix(row >= 0 && row < rows ? rows - 1 : rows,
                                col >= 0 && col < cols ? cols - 1 : cols);
        for (int i = 0, j = 0; i < rows; i++)
        {
            if (i == row) continue;
            for (int k = 0, u = 0; k < cols; k++)
            {
                if (k == col) continue;
                mat.matrix[j, u] = matrix[i, k];
                u++;
            }
            j++;
        }
        return mat;
    }
    public Matrix Multiply(Matrix m)
    {
        if (cols == m.rows)
        {
            Matrix mat = new Matrix(rows, m.cols);
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < m.cols; j++)
                {
                    float sum = 0f;
                    for (int k = 0; k < cols; k++)
                        sum += matrix[i, k] * m.matrix[k, j];
                    mat.matrix[i, j] = sum;
                }
            return mat;
        }
        else
            throw new Exception("Can not mulitiply those");
    }
    public Vector Multiply(Vector v, bool vectorfirst = false)
    {
        if (vectorfirst)
        {
            Matrix m = new Matrix(new float[,] { { v.x, v.y, v.z } });
            Matrix mm = m.Multiply(this);
            return new Vector(mm.matrix[0, 0], mm.matrix[0, 1], mm.matrix[0, 2]);
        }
        else
        {
            Matrix m = new Matrix(new float[,] { { v.x }, { v.y }, { v.z } });
            Matrix mm = Multiply(m);
            return new Vector(mm.matrix[0, 0], mm.matrix[1, 0], mm.matrix[2, 0]);
        }
    }
    public float Determinant()
    {
        if (cols == rows)
        {
            int n = rows;
            if (n == 1)
                return matrix[0, 0];
            if (n == 2)
                return matrix[0, 0] * matrix[1, 1] - matrix[1, 0] * matrix[0, 1];
            if (n == 3)
            {
                return matrix[0, 0] * matrix[1, 1] * matrix[2, 2] +
                       matrix[1, 0] * matrix[2, 1] * matrix[0, 2] +
                       matrix[0, 1] * matrix[1, 2] * matrix[2, 0] -
                       matrix[2, 0] * matrix[1, 1] * matrix[0, 2] -
                       matrix[1, 0] * matrix[0, 1] * matrix[2, 2] -
                       matrix[2, 1] * matrix[1, 2] * matrix[0, 0];
            }
            else
            {
                float sum = 0;
                for (int i = 0; i < n; i++)
                    sum += (i % 2 == 0 ? matrix[0, i] : -matrix[0, i]) * RemoveLines(0, i).Determinant();
                return sum;
            }
        }
        else throw new Exception("Matrix needs to be square");
    }
    public Matrix Inverse()
    {
        float det = Determinant();
        if (det != 0f)
        {
            Matrix tr = Transpuse();
            Matrix mat = new Matrix(rows, cols);
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                {
                    float val = (i + j) % 2 == 0 ? 1 : -1;
                    mat.matrix[i, j] = val * tr.RemoveLines(i, j).Determinant();
                }
            return mat / det;
        }
        else throw new Exception("Matrix is not inverible");
    }
    public override string ToString()
    {
        string value = "";
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
                value += matrix[i, j] + " ";
            value += "\n";
        }
        return value;
    }
    public static Matrix operator /(Matrix m, float l)
    {
        Matrix mat = new Matrix(m.rows, m.cols);
        for (int i = 0; i < m.rows; i++)
            for (int j = 0; j < m.cols; j++)
                mat.matrix[i, j] = m.matrix[i, j] / l;
        return mat;
    }
    public static Matrix operator /(float l, Matrix m)
    {
        Matrix mat = new Matrix(m.rows, m.cols);
        for (int i = 0; i < m.rows; i++)
            for (int j = 0; j < m.cols; j++)
                mat.matrix[i, j] = m.matrix[i, j] / l;
        return mat;
    }
    public static Matrix operator *(Matrix m, float l)
    {
        Matrix mat = new Matrix(m.rows, m.cols);
        for (int i = 0; i < m.rows; i++)
            for (int j = 0; j < m.cols; j++)
                mat.matrix[i, j] = m.matrix[i, j] * l;
        return mat;
    }
    public static Matrix operator *(float l, Matrix m)
    {
        Matrix mat = new Matrix(m.rows, m.cols);
        for (int i = 0; i < m.rows; i++)
            for (int j = 0; j < m.cols; j++)
                mat.matrix[i, j] = m.matrix[i, j] * l;
        return mat;
    }
    public static Matrix operator +(Matrix m1, Matrix m2)
    {
        if (m1.cols == m2.cols && m1.rows == m2.rows)
        {
            Matrix mat = new Matrix(m1.rows, m1.cols);
            for (int i = 0; i < m1.rows; i++)
                for (int j = 0; j < m1.cols; j++)
                    mat.matrix[i, j] = m1.matrix[i, j] + m2.matrix[i, j];
            return mat;
        }
        else
            throw new Exception("The matrices do not have similar sizes");
    }
    public static Matrix operator -(Matrix m1, Matrix m2)
    {
        if (m1.cols == m2.cols && m1.rows == m2.rows)
        {
            Matrix mat = new Matrix(m1.rows, m1.cols);
            for (int i = 0; i < m1.rows; i++)
                for (int j = 0; j < m1.cols; j++)
                    mat.matrix[i, j] = m1.matrix[i, j] - m2.matrix[i, j];
            return mat;
        }
        else
            throw new Exception("The matrices do not have similar sizes");
    }
}
public class Line
{
    public Vector sp;
    public Vector ep;
    public Line(float spx, float spy, float epx, float epy)
    {
        sp = new Vector(spx, spy);
        ep = new Vector(epx, epy);
    }
    public Line(Vector sp, float epx, float epy) : this(sp.x, sp.y, epx, epy) { }
    public Line(float spx, float spy, Vector ep) : this(spx, spy, ep.x, ep.y) { }
    public Line(Vector sp, Vector ep) : this(sp.x, sp.y, ep.x, ep.y) { }
    public Line Clone() => new Line(sp, ep);
    public Line SwapEnds()
    {
        Vector t = sp;
        sp = ep.Clone();
        ep = t.Clone();
        return Clone();
    }
    public float DistPointToLine(Vector v)
    {
        float l2 = Length() * Length();
        if (l2 == 0f) return (v - sp).Magnitude();
        float t = Math.Max(0f, Math.Min(1f, (v - sp).Dot(ep - sp) / l2));
        Vector projection = sp + t * (ep - sp);
        return (v - projection).Magnitude();
    }
    public Vector ProjPointOnLine(Vector v)
    {
        float m = (sp.y - ep.y) / (sp.x - ep.x);
        float n = sp.y - m * sp.x;
        float x = (v.x + m * v.y - m * n) / (m * m + 1);
        float y = m * x + n;
        Vector rez = new Vector(x, y);
        if (DistPointToLine(rez) > 0.1f)
        {
            if ((sp - rez).Magnitude() < (ep - rez).Magnitude()) rez = sp.Clone();
            else rez = ep.Clone();
        }
        return rez;
    }
    public Vector IntersectionPoint(Line l, bool direct = true)
    {
        float x1 = sp.x;
        float y1 = sp.y;
        float x2 = ep.x;
        float y2 = ep.y;
        float x3 = l.sp.x;
        float y3 = l.sp.y;
        float x4 = l.ep.x;
        float y4 = l.ep.y;
        float t = ((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)) / ((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4));
        float u = -((x1 - x2) * (y1 - y3) - (y1 - y2) * (x1 - x3)) / ((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4));
        if (t >= 0f && u >= 0f && u <= 1f)
        {
            if (direct)
            {
                if (t <= 1f)
                    return new Vector(x1 + t * (x2 - x1), y1 + t * (y2 - y1));
            }
            else 
                return new Vector(x1 + t * (x2 - x1), y1 + t * (y2 - y1));
        }
        return null;
    }
    public float AngleLine(Line l)
    {
        float err = 0.000001f;
        float dm = (sp.x - ep.x);
        dm = Math.Abs(dm) < err ? err : dm;
        float m1 = (sp.y - ep.y) / dm;
        dm = (l.sp.x - l.ep.x);
        dm = Math.Abs(dm) < err ? err : dm;
        float m2 = (l.sp.y - l.ep.y) / dm;
        return (float)Math.Atan((m1 - m2) / (1f + m1 * m2));
    }
    public Line Rotate(float angle, Vector pivot = null)
    {
        sp.Rotate(angle, pivot);
        ep.Rotate(angle, pivot);
        return Clone();
    }
    public Line RotateFixed(float angle, Vector pivot = null)
    {
        sp.RotateFixed(angle, pivot);
        ep.RotateFixed(angle, pivot);
        return Clone();
    }
    public Line SetLength(float length)
    {
        Vector norm = (ep - sp).Normalize();
        ep = sp + length * norm;
        return Clone();
    }
    public float Length() => (ep - sp).Magnitude();
    public Vector Heading() => (ep - sp).Normalize();
    public override string ToString() => $"SP({sp.x}, {sp.y}, {sp.z}), EP({ep.x}, {ep.y}, {ep.z})";
    public static Line operator +(Line l, Vector v) => new Line(l.sp + v, l.ep + v);
    public static Line operator +(Vector v, Line l) => new Line(l.sp + v, l.ep + v);
    public static Line operator -(Line l, Vector v) => new Line(l.sp - v, l.ep - v);
    public static Line operator -(Vector v, Line l) => new Line(l.sp - v, l.ep - v);
}
public static class FileRead
{
    private static readonly List<char> ListDividers = new List<char>() { ' ', ',', ';', '\t' };
    private static readonly List<string> FileUnchanged = new List<string>();
    private static string FileChanged = "";
    private static int CurrentIndex = 0;
    public static void ProcessFile(string filepath)
    {
        FileUnchanged.Clear();
        FileChanged = "";
        CurrentIndex = 0;
        StringBuilder sb = new StringBuilder();
        using (var reader = new StreamReader(filepath))
        {
            string buffer;
            while ((buffer = reader.ReadLine()) != null)
            {
                FileUnchanged.Add(buffer);
                sb.Append(buffer);
                sb.Append(" ");
            }
        }
        FileChanged = sb.ToString();
    }
    private static bool IsDivider(char value)
    {
        foreach (char c in ListDividers)
            if (c == value) return true;
        return false;
    }
    public static string GetNextAsText()
    {
        string word = "";
        bool Working = true;
        while (CurrentIndex < FileChanged.Length && Working)
        {
            if (!IsDivider(FileChanged[CurrentIndex]))
                word += FileChanged[CurrentIndex];
            else
                if (word.Length > 0)
                Working = false;
            CurrentIndex++;
        }
        return word;
    }
    public static dynamic GetNextAsValue()
    {
        string word = GetNextAsText();
        if (int.TryParse(word, out int valueint))
            return valueint;
        if (float.TryParse(word, out float valuefloat))
            return valuefloat;
        if (word.Length == 1) return word[0];
        if (word.Length == 0) return null;
        return word;
    }
    public static bool EndOfFile()
    {
        int LastIndex = CurrentIndex;
        string word = GetNextAsText();
        if (word == "") return true;
        else
        {
            CurrentIndex = LastIndex;
            return false;
        }
    }
    public static void ReplaceDividers(params char[] dividers)
    {
        ListDividers.Clear();
        for (int i = 0; i < dividers.Length; i++)
            ListDividers.Add(dividers[i]);
    }
    public static string ViewFile(int line = -1)
    {
        string ret;
        if (line >= 0)
            ret = FileUnchanged[line < FileUnchanged.Count ? line : FileUnchanged.Count - 1];
        else
        {
            StringBuilder sb = new StringBuilder();
            foreach (string s in FileUnchanged) sb.Append(s);
            ret = sb.ToString();
        }
        return ret;
    }
}
public static class Input
{
    private static Hashtable kb_prev;
    private static Hashtable kb_now;
    private static bool ScrollUp;
    private static bool ScrollDown;
    private static Vector mouse;
    private static bool showKeysPressed;
    private static List<(Keys, bool)> keysState;
    private static List<(MouseButtons, bool)> buttonsState;
    public static void Link(Window window)
    {
        keysState = new List<(Keys, bool)>();
        buttonsState = new List<(MouseButtons, bool)>();
        kb_prev = new Hashtable();
        kb_now = new Hashtable();
        mouse = new Vector();
        showKeysPressed = false;
        window.KeyPreview = true;
        window.KeyDown += EventKeyDown;
        window.KeyUp += EventKeyUp;
        window.MouseDown += EventMouseDown;
        window.MouseUp += EventMouseUp;
        window.MouseMove += EventMouseMove;
        window.MouseWheel += EventMouseWheel;
    }
    public static void ToggleShowKeysPressed() => showKeysPressed = !showKeysPressed;
    private static void EventMouseWheel(object sender, MouseEventArgs e)
    {
        if (e.Delta > 0)
        {
            ScrollUp = true;
            ScrollDown = false;
            if (showKeysPressed)
                Console.WriteLine("Scroll Up");
        }
        else if (e.Delta < 0)
        {
            ScrollUp = false;
            ScrollDown = true;
            if (showKeysPressed)
                Console.WriteLine("Scroll Down");
        }
    }
    private static void EventMouseMove(object sender, MouseEventArgs e)
    {
        mouse.x = e.X;
        mouse.y = e.Y;
    }
    private static void EventMouseDown(object sender, MouseEventArgs e)
    {
        if (showKeysPressed) 
            Console.WriteLine(e.Button);
        buttonsState.Add((e.Button, true));
    }
    private static void EventMouseUp(object sender, MouseEventArgs e) => buttonsState.Add((e.Button, false));
    private static void EventKeyDown(object sender, KeyEventArgs e)
    {
        if (showKeysPressed)
            Console.WriteLine(e.KeyCode);
        keysState.Add((e.KeyCode, true));
    }
    private static void EventKeyUp(object sender, KeyEventArgs e) => keysState.Add((e.KeyCode, false));
    public static bool WheelScrollUp()
    {
        bool state = ScrollUp;
        ScrollUp = false;
        return state;
    }
    public static bool WheelScrollDown()
    {
        bool state = ScrollDown;
        ScrollDown = false;
        return state;
    }
    public static bool KeyPressed(MouseButtons key)
    {
        if (kb_now[key] != null)
        {
            if (!(bool)kb_prev[key])
                return (bool)kb_now[key];
            else
                return false;
        }
        else
            return false;
    }
    public static bool KeyReleased(MouseButtons key)
    {
        if (kb_now[key] != null)
        {
            if ((bool)kb_prev[key])
                return !(bool)kb_now[key];
            else
                return false;
        }
        else
            return false;
    }
    public static bool KeyHeld(MouseButtons key)
    {
        if (kb_now[key] == null)
            return false;
        return (bool)kb_now[key];
    }
    public static bool KeyPressed(Keys key)
    {
        if (kb_now[key] != null)
        {
            if (!(bool)kb_prev[key])
                return (bool)kb_now[key];
            else
                return false;
        }
        else
            return false;
    }
    public static bool KeyReleased(Keys key)
    {
        if (kb_now[key] != null)
        {
            if ((bool)kb_prev[key])
                return !(bool)kb_now[key];
            else
                return false;
        }
        else
            return false;
    }
    public static bool KeyHeld(Keys key)
    {
        if (kb_now[key] == null)
            return false;
        return (bool)kb_now[key];
    }
    public static Vector MousePos() => mouse.Clone();
    public static Vector PlaneMovement()
    {
        Vector v = new Vector();
        if (KeyHeld(Keys.W) || KeyHeld(Keys.Up)) v.y += -1;
        if (KeyHeld(Keys.D) || KeyHeld(Keys.Right)) v.x += 1;
        if (KeyHeld(Keys.S) || KeyHeld(Keys.Down)) v.y += 1;
        if (KeyHeld(Keys.A) || KeyHeld(Keys.Left)) v.x += -1;
        return v.Normalize();
    }
    public static Keys AsKey(string keyname) => (Keys)Enum.Parse(typeof(Keys), keyname, true);
    public static void UpdateCurrentKeys()
    {
        try
        {
            foreach ((Keys, bool) key in keysState)
                UpdateState(key.Item1, key.Item2);
            foreach ((MouseButtons, bool) button in buttonsState)
                UpdateState(button.Item1, button.Item2);
            keysState.Clear();
            buttonsState.Clear();
        }
        catch { Console.WriteLine("Keys crashed"); }
    }
    public static void UpdatePreviousKeys()
    {
        try
        {
            foreach (DictionaryEntry key in kb_now)
                kb_prev[key.Key] = (bool)kb_now[key.Key];
        }
        catch { Console.WriteLine("Keys crashed 2"); }
    }
    private static void UpdateState(Keys key, bool state)
    {
        kb_now[key] = state;
        if (kb_prev[key] == null)
            kb_prev[key] = false;
    }
    private static void UpdateState(MouseButtons key, bool state)
    {
        kb_now[key] = state;
        if (kb_prev[key] == null)
            kb_prev[key] = false;
    }
}