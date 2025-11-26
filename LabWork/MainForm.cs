using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace LabWork
{
    public partial class MainForm : Form
    {
        private readonly FunctionCalculator _calculator;
        private readonly GraphRenderer _renderer;
        private List<PointF> _dataPoints;

        private CheckBox _chkScatterMode;
        private Panel _controlPanel;

        public MainForm()
        {
            this.Text = "Лабораторна робота: Графік";
            this.Size = new Size(800, 600);
            this.DoubleBuffered = true;

            InitializeCustomComponents();

            _calculator = new FunctionCalculator(2.5, 9, 0.8);
            _renderer = new GraphRenderer();

            LoadData();

            // Важливо: початковий розрахунок координат
            UpdateGraphLayout();
        }

        private void InitializeCustomComponents()
        {
            _controlPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                BackColor = Color.WhiteSmoke
            };

            _chkScatterMode = new CheckBox
            {
                Text = "Точковий режим (Scatter Plot)",
                AutoSize = true,
                Location = new Point(20, 15)
            };

            _chkScatterMode.CheckedChanged += (s, e) => this.Invalidate(); // Тут тільки перемальовуємо, перераховувати координати не треба

            _controlPanel.Controls.Add(_chkScatterMode);
            this.Controls.Add(_controlPanel);
        }

        private void LoadData()
        {
            _dataPoints = _calculator.GetPoints();
        }

        // Цей метод відповідає за важкі математичні перетворення
        private void UpdateGraphLayout()
        {
            if (_dataPoints == null) return;

            // Обчислюємо доступну область
            RectangleF plotArea = new RectangleF(0, 0, this.ClientSize.Width, this.ClientSize.Height - _controlPanel.Height);

            // Кешуємо координати точок
            _renderer.Recalculate(plotArea, _dataPoints);
        }

        // Обробник зміни розміру
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // 1. Спочатку перераховуємо точки (математика)
            UpdateGraphLayout();

            // 2. Потім викликаємо перемалювання (графіка)
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            // Тут просто малюємо вже готові дані. Ніяких обчислень.
            _renderer.Draw(e.Graphics, _chkScatterMode.Checked);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _renderer.Dispose();
            base.OnFormClosed(e);
        }
    }
}