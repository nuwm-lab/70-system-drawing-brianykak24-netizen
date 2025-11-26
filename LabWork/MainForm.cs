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

        // UI елементи
        private CheckBox _chkScatterMode;
        private Panel _controlPanel;

        public MainForm()
        {
            // Налаштування форми
            this.Text = "Лабораторна робота: Графік";
            this.Size = new Size(800, 600);

            // 1. Увімкнення подвійної буферизації (вимога проти мерехтіння)
            this.DoubleBuffered = true;

            // Ініціалізація компонентів
            InitializeCustomComponents();

            // Ініціалізація логіки
            _calculator = new FunctionCalculator(2.5, 9, 0.8);
            _renderer = new GraphRenderer();

            // Завантаження даних (Model)
            LoadData();

            // Підписка на події
            this.Resize += (s, e) => this.Invalidate(); // Перемальовуємо при зміні розміру
            this.Paint += MainForm_Paint;
        }

        private void InitializeCustomComponents()
        {
            // Панель для кнопок знизу
            _controlPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                BackColor = Color.WhiteSmoke
            };

            // Чекбокс для перемикання режиму (Scatter vs Line)
            _chkScatterMode = new CheckBox
            {
                Text = "Точковий режим (Scatter Plot)",
                AutoSize = true,
                Location = new Point(20, 15)
            };

            // При зміні чекбокса просто викликаємо перемалювання
            _chkScatterMode.CheckedChanged += (s, e) => this.Invalidate();

            _controlPanel.Controls.Add(_chkScatterMode);
            this.Controls.Add(_controlPanel);
        }

        private void LoadData()
        {
            _dataPoints = _calculator.GetPoints();
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            // Обчислюємо область для малювання (все вікно мінус панель керування)
            RectangleF plotArea = new RectangleF(0, 0, this.ClientSize.Width, this.ClientSize.Height - _controlPanel.Height);

            // Делегуємо малювання рендереру
            // Передаємо стан чекбокса (Checked) для вибору типу графіка
            _renderer.Draw(e.Graphics, plotArea, _dataPoints, _chkScatterMode.Checked);
        }

        // Очищення ресурсів при закритті
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _renderer.Dispose();
            base.OnFormClosed(e);
        }
    }
}