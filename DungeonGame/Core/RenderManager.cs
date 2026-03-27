using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame
{
    public struct Rect
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;

        public Rect(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public Rect Shrink(Thickness thickness)
        {
            int newX = X + thickness.Left;
            int newY = Y + thickness.Top;
            int newWidth = Width - thickness.Left - thickness.Right;
            int newHeight = Height - thickness.Top - thickness.Bottom;

            if (newWidth < 0) newWidth = 0;
            if (newHeight < 0) newHeight = 0;

            return new Rect(newX, newY, newWidth, newHeight);
        }
    }

    public struct Thickness
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public Thickness(int uniform)
        {
            Left = Top = Right = Bottom = uniform;
        }

        public Thickness(int horizontal, int vertical)
        {
            Left = Right = horizontal;
            Top = Bottom = vertical;
        }

        public Thickness(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }
    }

    public enum PanelType
    {
        Map,
        Status,
        Log
    }

    internal class RenderManager
    {
        private const int WINDOW_WIDTH = 120;
        private const int WINDOW_HEIGHT = 40;

        private const int STATUS_WIDTH = 60;
        private const int STATUS_HEIGHT = 100;

        private const int LOG_WIDTH = 200;
        private const int LOG_HEIGHT = 10;

        private readonly Dictionary<PanelType, Panel> _panels = new();

        public void Initialize()
        {
            Console.CursorVisible = false;

            Console.SetBufferSize(WINDOW_WIDTH, WINDOW_HEIGHT);
            Console.SetWindowSize(WINDOW_WIDTH, WINDOW_HEIGHT);

            _panels[PanelType.Map] = new Panel(
                new Rect(0, 0, WINDOW_WIDTH - STATUS_WIDTH, WINDOW_HEIGHT - LOG_HEIGHT),
                margin: new Thickness(0, 1, 0, 0),
                padding: new Thickness(1, 0, 1, 0),
                hasBorder: true);

            _panels[PanelType.Status] = new Panel(
                new Rect(WINDOW_WIDTH - STATUS_WIDTH, 0, STATUS_WIDTH, WINDOW_HEIGHT - LOG_HEIGHT),
                margin: new Thickness(0, 1, 2, 0),
                padding: new Thickness(1, 0, 1, 0),
                hasBorder: true);

            _panels[PanelType.Log] = new Panel(
                new Rect(0, WINDOW_HEIGHT - LOG_HEIGHT, WINDOW_WIDTH, LOG_HEIGHT),
                margin: new Thickness(0, 0, 2, 1),
                padding: new Thickness(1, 0, 1, 0),
                hasBorder: true);
        }

        public Panel GetPanel(PanelType panelType)
        {
            return _panels[panelType];
        }

        public void AllClearPanel()
        {
            foreach (var panelType in _panels.Keys)
            {
                ClearPanel(panelType);
            }
        }

        public void ClearPanel(PanelType panelType)
        {
            Rect rect = _panels[panelType].GetContentRect();

            string blank = new string(' ', rect.Width);

            for (int y = 0; y < rect.Height; y++)
            {
                Console.SetCursorPosition(rect.X, rect.Y + y);
                Console.Write(blank);
            }
        }

        public void DrawText(PanelType panelType, int localX, int localY, string text)
        {
            Rect rect = _panels[panelType].GetContentRect();

            if (localX < 0 || localY < 0) return;
            if (localY >= rect.Height) return;
            if (localX >= rect.Width) return;

            int maxLength = rect.Width - localX;
            if (maxLength <= 0) return;

            if (text.Length > maxLength)
                text = text.Substring(0, maxLength);

            Console.SetCursorPosition(rect.X + localX, rect.Y + localY);
            Console.Write(text);
        }

        public void DrawChar(PanelType type, int localX, int localY, char c)
        {
            Rect rect = _panels[type].GetContentRect();

            if (localX < 0 || localY < 0) return;
            if (localX >= rect.Width || localY >= rect.Height) return;

            Console.SetCursorPosition(rect.X + localX, rect.Y + localY);
            Console.Write(c);
        }

        public void DrawBorder(PanelType panelType)
        {
            Panel panel = _panels[panelType];
            Rect rect = panel.Bounds;

            if (rect.Width < 2 || rect.Height < 2)
                return;

            for (int x = 0; x < rect.Width; x++)
            {
                Console.SetCursorPosition(rect.X + x, rect.Y);
                Console.Write(x == 0 ? '┌' : x == rect.Width - 1 ? '┐' : '─');

                Console.SetCursorPosition(rect.X + x, rect.Y + rect.Height - 1);
                Console.Write(x == 0 ? '└' : x == rect.Width - 1 ? '┘' : '─');
            }

            for (int y = 1; y < rect.Height - 1; y++)
            {
                Console.SetCursorPosition(rect.X, rect.Y + y);
                Console.Write('│');

                Console.SetCursorPosition(rect.X + rect.Width - 1, rect.Y + y);
                Console.Write('│');
            }
        }
    }
}
