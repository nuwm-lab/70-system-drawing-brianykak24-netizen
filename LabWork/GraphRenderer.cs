using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace LabWork
{
    public class GraphRenderer : IDisposable
    {
        // Ресурси GDI+
        private readonly Pen _gridPen;
        private readonly Pen _axisPen;
        private readonly Pen _graphPen;
        private readonly Brush _pointBrush;
        private readonly Font _font;
        private readonly SolidBrush _textBrush; // Змінили тип на SolidBrush для коректного Dispose

        // Кешовані дані (щоб не рахувати в OnPaint)
        private PointF[] _cachedScreenPoints;
        private RectangleF _cachedDrawArea;

        // Зберігаємо межі даних для підписів осей
        private float _minX, _maxX, _minY, _maxY;

        public GraphRenderer()
        {
            _gridPen = new Pen(Color.LightGray, 1) { DashStyle = DashStyle.Dot };
            _axisPen = new Pen(Color.Black, 2); // Тепер будемо використовувати для рамки
            _graphPen = new Pen(Color.Blue, 2);
            _pointBrush = new SolidBrush(Color.Red);
            _textBrush = new SolidBrush(Color.Black); // Створюємо свій об'єкт, щоб чесно його звільнити
            _font = new Font("Arial", 8);
        }

        /// <summary>
        /// Перераховує координати точок під новий розмір вікна.
        /// Викликати ТІЛЬКИ при зміні даних або розміру вікна (Resize).
        /// </summary>
        public void Recalculate(RectangleF clientRect, List<PointF> dataPoints)
        {
            if (dataPoints == null || dataPoints.Count < 2)
            {
                _cachedScreenPoints = null;
                return;
            }

            // 1. Визначення меж значень (Data Bounds)
            _minX = dataPoints.Min(p => p.X);
            _maxX = dataPoints.Max(p => p.X);
            _minY = dataPoints.Min(p => p.Y);
            _maxY = dataPoints.Max(p => p.Y);

            // ЗАХИСТ: Обробка вироджених випадків (ділення на нуль)
            if (Math.Abs(_maxX - _minX) < 1e-6)
            {
                _maxX += 1.0f; // Штучно розширюємо діапазон X
                _minX -= 1.0f;
            }
            if (Math.Abs(_maxY - _minY) < 1e-6)
            {
                _maxY += 1.0f; // Штучно розширюємо діапазон Y
                _minY -= 1.0f;
            }

            // 2. Визначення області малювання з відступами
            float margin = 40f;

            // ЗАХИСТ: Якщо вікно занадто мале, не допускаємо від'ємних розмірів
            float width = Math.Max(1.0f, clientRect.Width - 2 * margin);
            float height = Math.Max(1.0f, clientRect.Height - 2 * margin);

            _cachedDrawArea = new RectangleF(
                clientRect.X + margin,
                clientRect.Y + margin,
                width,
                height);

            // 3. Коефіцієнти масштабування
            float scaleX = width / (_maxX - _minX);
            float scaleY = height / (_maxY - _minY);

            // 4. Трансформація точок (World -> Screen)
            _cachedScreenPoints = new PointF[dataPoints.Count];
            for (int i = 0; i < dataPoints.Count; i++)
            {
                float screenX = _cachedDrawArea.Left + (dataPoints[i].X - _minX) * scaleX;
                // Y інвертується
                float screenY = _cachedDrawArea.Bottom - (dataPoints[i].Y - _minY) * scaleY;
                _cachedScreenPoints[i] = new PointF(screenX, screenY);
            }
        }

        public void Draw(Graphics g, bool showAsScatter)
        {
            // Якщо немає розрахованих точок - нічого не малюємо
            if (_cachedScreenPoints == null || _cachedScreenPoints.Length < 2) return;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // 1. Малюємо сітку та рамку
            g.DrawRectangle(_gridPen, _cachedDrawArea.X, _cachedDrawArea.Y, _cachedDrawArea.Width, _cachedDrawArea.Height);

            // Використовуємо _axisPen для чіткіших меж (тепер ресурс використовується)
            g.DrawRectangle(_axisPen, Rectangle.Round(_cachedDrawArea));

            // 2. Малюємо підписи осей (використовуємо збережені min/max)
            g.DrawString($"{_minX:F1}", _font, _textBrush, _cachedDrawArea.Left, _cachedDrawArea.Bottom + 5);
            g.DrawString($"{_maxX:F1}", _font, _textBrush, _cachedDrawArea.Right - 30, _cachedDrawArea.Bottom + 5);
            g.DrawString($"{_minY:F2}", _font, _textBrush, _cachedDrawArea.Left - 35, _cachedDrawArea.Bottom - 10);
            g.DrawString($"{_maxY:F2}", _font, _textBrush, _cachedDrawArea.Left - 35, _cachedDrawArea.Top);

            // 3. Малюємо графік, використовуючи вже готові кешовані точки
            if (showAsScatter)
            {
                float pointSize = 6;
                foreach (var p in _cachedScreenPoints)
                {
                    g.FillEllipse(_pointBrush, p.X - pointSize / 2, p.Y - pointSize / 2, pointSize, pointSize);
                }
            }
            else
            {
                g.DrawLines(_graphPen, _cachedScreenPoints);
            }
        }

        public void Dispose()
        {
            _gridPen.Dispose();
            _axisPen.Dispose();
            _graphPen.Dispose();
            _pointBrush.Dispose();
            _textBrush.Dispose(); // Тепер це безпечно
            _font.Dispose();
        }
    }
}