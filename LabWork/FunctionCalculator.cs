using System;
using System.Collections.Generic;
using System.Drawing;
namespace LabWork
{
    // Відповідає ТІЛЬКИ за математику
    public class FunctionCalculator
    {
        private readonly double _startX;
        private readonly double _endX;
        private readonly double _step;

        public FunctionCalculator(double startX, double endX, double step)
        {
            _startX = startX;
            _endX = endX;
            _step = step;
        }

        // y = (1.5x - ln(2x)) / (3x + 1)
        private double CalculateY(double x)
        {
            // Уникаємо логарифму від від'ємних чисел або нуля
            if (x <= 0) return 0;
            return (1.5 * x - Math.Log(2 * x)) / (3 * x + 1);
        }

        public List<PointF> GetPoints()
        {
            var points = new List<PointF>();
            for (double x = _startX; x <= _endX; x += _step)
            {
                double y = CalculateY(x);
                points.Add(new PointF((float)x, (float)y));
            }
            return points;
        }
    }
}