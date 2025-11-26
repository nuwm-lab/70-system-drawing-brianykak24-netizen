using System.Collections.Generic;
using System.Drawing;
using System;
using System.Linq;

namespace LabWork
{
    public class GraphRenderer : IDisposable
    {
        private readonly Pen _gridPen;
        private readonly Pen _axisPen;
        private readonly Pen _graphPen;
        private readonly Brush _pointBrush;
        private readonly Font _font;
        private readonly Brush _textBrush;

        public GraphRenderer()
        {
            _gridPen = new Pen(Color.LightGray, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dot };
            _axisPen = new Pen(Color.Black, 2);
            _graphPen = new Pen(Color.Blue, 2);
            _pointBrush = new SolidBrush(Color.Red);
            _textBrush = Brushes.Black;
            _font = new Font("Arial", 8);
        }

        public void Draw(Graphics g, RectangleF clientRect, List<PointF> dataPoints, bool showAsScatter)
        {
            if (dataPoints == null || dataPoints.Count < 2) return;

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // 1. Визначення меж (World Coordinates)
            float minX = dataPoints.Min(p => p.X);
            float maxX = dataPoints.Max(p => p.X);
            float minY = dataPoints.Min(p => p.Y);
            float maxY = dataPoints.Max(p => p.Y);

            // Додаємо трохи "повітря" по краях
            float margin = 40f;
            RectangleF drawArea = new RectangleF(
                clientRect.X + margin,
                clientRect.Y + margin,
                clientRect.Width - 2 * margin,
                clientRect.Height - 2 * margin);

            // 2. Обчислення коефіцієнтів масштабування
            float scaleX = drawArea.Width / (maxX - minX);
            float scaleY = drawArea.Height / (maxY - minY);

            // Локальна функція для перетворення координат (World -> Screen)
            PointF ToScreen(PointF point)
            {
                float screenX = drawArea.Left + (point.X - minX) * scaleX;
                // Інвертуємо Y, бо в WinForms вісь Y йде вниз
                float screenY = drawArea.Bottom - (point.Y - minY) * scaleY;
                return new PointF(screenX, screenY);
            }

            // 3. Малювання сітки та осей
            g.DrawRectangle(_gridPen, drawArea.X, drawArea.Y, drawArea.Width, drawArea.Height);

            // Малюємо підписи меж
            g.DrawString($"X: {minX:F1}", _font, _textBrush, drawArea.Left, drawArea.Bottom + 5);
            g.DrawString($"X: {maxX:F1}", _font, _textBrush, drawArea.Right - 30, drawArea.Bottom + 5);
            g.DrawString($"Y: {minY:F2}", _font, _textBrush, drawArea.Left - 35, drawArea.Bottom - 10);
            g.DrawString($"Y: {maxY:F2}", _font, _textBrush, drawArea.Left - 35, drawArea.Top);

            // 4. Малювання графіка
            // Перетворюємо всі точки заздалегідь
            PointF[] screenPoints = dataPoints.Select(p => ToScreen(p)).ToArray();

            if (showAsScatter)
            {
                // Точковий графік (Scatter Plot)
                float pointSize = 6;
                foreach (var p in screenPoints)
                {
                    g.FillEllipse(_pointBrush, p.X - pointSize / 2, p.Y - pointSize / 2, pointSize, pointSize);
                }
            }
            else
            {
                // Лінійний графік (Line Plot)
                g.DrawLines(_graphPen, screenPoints);
            }
        }

        public void Dispose()
        {
            _gridPen.Dispose();
            _axisPen.Dispose();
            _graphPen.Dispose();
            _pointBrush.Dispose();
            _font.Dispose();
        }
    }
}