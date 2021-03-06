using ImageProcess.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace ImageProcess.Controls
{
    public class PatternDrawingControl : UIElement
    {
        private int stage = 0;
        private PatternDrawingParameters pattern;
        private SolidColorBrush slipperColor = new SolidColorBrush(Colors.Red);
        private SolidColorBrush dotColor = new SolidColorBrush(Colors.DarkRed);
        private Pen linePen = new Pen(new SolidColorBrush(Colors.Black), 5);
        private long startTime;
        private double dotSize = 15;
        private double halfDotSize = 8;
        private double slipperSize = 19;
        private double halfSlipperSize = 10;

        private double speed;
        private double patternStay = 400;
        private double[] dotTime;
        private double[] lineDuration;
        private double[] lines;
        private Vector[] lineVectors;

        public event EventHandler MoveBegin;
        public event EventHandler MoveEnded;

        private DispatcherTimer timer;

        public PatternDrawingControl()
        {
        }

        public void SetPattern(PatternDrawingParameters pattern, double[] lines, double speed)
        {
            this.pattern = pattern;
            this.speed = speed;
            this.lines = lines;

            Point[] points = pattern.Points;
            int lineCount = lines.Length;
            int timeCount = pattern.Points.Length * 2;
            dotTime = new double[timeCount];
            lineDuration = new double[lineCount];
            lineVectors = new Vector[lineCount];
            dotTime[0] = 0;
            dotTime[1] = 1000;
            double lastStarted = 1000;
            int currentDotIndex = 2;
            double lastX = points[0].X;
            double lastY = points[0].Y;
            double currentX;
            double currentY;
            for (int lineIndex = 0; lineIndex < lineCount; lineIndex++)
            {
                double lineLength = lines[lineIndex];
                double duration = lineLength / speed;
                double currentTimestamp = lastStarted + duration;
                lineDuration[lineIndex] = duration;

                currentX = points[lineIndex + 1].X;
                currentY = points[lineIndex + 1].Y;
                Vector lineVector = new Vector(currentX - lastX, currentY - lastY) / duration;
                lineVectors[lineIndex] = lineVector;
                lastX = currentX;
                lastY = currentY;

                dotTime[currentDotIndex] = currentTimestamp;
                dotTime[currentDotIndex + 1] = lastStarted = currentTimestamp + patternStay;
                currentDotIndex += 2;
            }
            dotTime[timeCount - 1] = dotTime[timeCount - 2] + 1000;
        }

        public void Start()
        {
            stage = 1;
            startTime = DateTime.Now.Ticks;
            MoveBegin?.Invoke(this, null);
            timer = new DispatcherTimer(
                interval: TimeSpan.FromMilliseconds(10),
                priority: DispatcherPriority.Send,
                callback: (o, e) => InvalidateVisual(),
                dispatcher: Dispatcher);
            timer.Start();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            DrawPattern(drawingContext);
            switch (stage)
            {
                case 0:
                    double firstX = pattern.Points[0].X;
                    double firstY = pattern.Points[0].Y;
                    //drawingContext.DrawEllipse(slipperColor, null, new Point(firstX, firstY), slipperSize, slipperSize);
                    //Rect rect = new Rect(firstX - slipperSize, firstY - slipperSize, 2 * slipperSize, 2 * slipperSize);
                    //drawingContext.DrawRectangle(slipperColor, null, rect);
                    //drawingContext.DrawLine(null, new Point(firstX, firstY - slipperSize), new Point(firstX - Math.Sqrt(3) /2 * slipperSize, firstY + 1/2 * slipperSize));
                    //drawingContext.DrawLine(null, new Point(firstX - Math.Sqrt(3) / 2 * slipperSize, firstY + 1 / 2 * slipperSize), new Point(firstX + Math.Sqrt(3) / 2 * slipperSize, firstY + 1 / 2 * slipperSize));
                    Point point1 = new Point(firstX - slipperSize, firstY - Math.Sqrt(3) / 3 * slipperSize);
                    Point point2 = new Point(firstX + slipperSize, firstY - Math.Sqrt(3) / 3 * slipperSize);
                    Point point3 = new Point(firstX, firstY + 2 * Math.Sqrt(3) / 3 *  slipperSize);
                    Point[] pntArr = { point1, point2, point3 };
                    StreamGeometry streamGeometry = new StreamGeometry();
                    using (StreamGeometryContext geometryContext = streamGeometry.Open())
                    {
                        geometryContext.BeginFigure(point1, true, true);
                        geometryContext.PolyLineTo(pntArr, true, true);
                    }
                    streamGeometry.Freeze();
                    drawingContext.DrawGeometry(Brushes.Red, new Pen(Brushes.Black, 3), streamGeometry);
                    break;
                case 1:
                    long currentDiff = DateTime.Now.Ticks - startTime;
                    DrawSlipper(currentDiff / 10000.0, drawingContext);
                    break;
                case 2:
                default:
                    int lastIndex = pattern.Points.Length - 1;
                    double lastX = pattern.Points[lastIndex].X - halfSlipperSize;
                    double lastY = pattern.Points[lastIndex].Y - halfSlipperSize;
                    //drawingContext.DrawEllipse(slipperColor, null, new Point(lastX, lastY), slipperSize, slipperSize);
                    //rect = new Rect(lastX - slipperSize, lastY - slipperSize, 2 * slipperSize, 2 * slipperSize);
                    //drawingContext.DrawRectangle(slipperColor, null, rect);
                    //drawingContext.DrawLine(null, new Point(lastX, lastY - slipperSize), new Point(lastX - Math.Sqrt(3) / 2 * slipperSize, lastY + 1 / 2 * slipperSize));
                    //drawingContext.DrawLine(null, new Point(lastX - Math.Sqrt(3) / 2 * slipperSize, lastY + 1 / 2 * slipperSize), new Point(lastX + Math.Sqrt(3) / 2 * slipperSize, lastY + 1 / 2 * slipperSize));
                    point1 = new Point(lastX - slipperSize, lastY - Math.Sqrt(3) / 3 * slipperSize);
                    point2 = new Point(lastX + slipperSize, lastY - Math.Sqrt(3) / 3 * slipperSize);
                    point3 = new Point(lastX, lastY + 2 * Math.Sqrt(3) / 3 * slipperSize);
                    Point[] pntArr1 = { point1, point2, point3 };
                    streamGeometry = new StreamGeometry();
                    using (StreamGeometryContext geometryContext = streamGeometry.Open())
                    {
                        geometryContext.BeginFigure(point1, true, true);
                        geometryContext.PolyLineTo(pntArr1, true, true);
                    }
                    streamGeometry.Freeze();
                    drawingContext.DrawGeometry(Brushes.Red, new Pen(Brushes.Black, 3), streamGeometry);
                    break;
            }
        }

        private void DrawPattern(DrawingContext drawingContext)
        {
            Point[] points = pattern.Points;
            int total = points.Length;
            double lastX = points[0].X;
            double lastY = points[0].Y;
            drawingContext.DrawEllipse(dotColor, null, new Point(lastX, lastY), halfDotSize, halfDotSize);
            double currentX;
            double currentY;
            for (int i = 1; i < total; i++)
            {
                currentX = points[i].X;
                currentY = points[i].Y;
                drawingContext.DrawEllipse(dotColor, null, new Point(currentX, currentY), halfDotSize, halfDotSize);
                drawingContext.DrawLine(linePen, new Point(lastX, lastY), new Point(currentX, currentY));
                lastX = currentX;
                lastY = currentY;
            }
        }

        private void DrawSlipper(double timeDiff, DrawingContext drawingContext)
        {
            int lineCount = lines.Length;
            int index = 0;
            int timeCount = dotTime.Length;
            while (index < timeCount && dotTime[index] <= timeDiff)
                index++;
            Point[] points = pattern.Points;
            if (index >= timeCount)
            {
                timer.Stop();
                timer = null;
                int lastIndex = points.Length - 1;
                double slipperX = points[lastIndex].X;
                double slipperY = points[lastIndex].Y;
                //Rect rect = new Rect(slipperX - slipperSize, slipperY - slipperSize, 2*slipperSize, 2*slipperSize);
                //drawingContext.DrawRectangle(slipperColor, null, rect);
                //drawingContext.DrawEllipse(slipperColor, null, new Point(slipperX, slipperY), slipperSize, slipperSize);
                //drawingContext.DrawLine(null, new Point(slipperX, slipperY - slipperSize), new Point(slipperX - Math.Sqrt(3) / 2 * slipperSize, slipperY + 1 / 2 * slipperSize));
                //drawingContext.DrawLine(null, new Point(slipperX - Math.Sqrt(3) / 2 * slipperSize, slipperY + 1 / 2 * slipperSize), new Point(slipperX + Math.Sqrt(3) / 2 * slipperSize, slipperY + 1 / 2 * slipperSize));

                Point point1 = new Point(slipperX - slipperSize, slipperY - Math.Sqrt(3) / 3 * slipperSize);
                Point point2 = new Point(slipperX + slipperSize, slipperY - Math.Sqrt(3) / 3 * slipperSize);
                Point point3 = new Point(slipperX, slipperY + 2 * Math.Sqrt(3) / 3 * slipperSize);
                Point[] pntArr = { point1, point2, point3 };
                StreamGeometry streamGeometry = new StreamGeometry();
                using (StreamGeometryContext geometryContext = streamGeometry.Open())
                {
                    geometryContext.BeginFigure(point1, true, true);
                    geometryContext.PolyLineTo(pntArr, true, true);
                }
                streamGeometry.Freeze();
                drawingContext.DrawGeometry(Brushes.Red, new Pen(Brushes.Black, 3), streamGeometry);
                stage = 2;
                MoveEnded?.Invoke(this, null);
            }
            else
            {
                index--;
                if (index % 2 == 0)
                {
                    int dotIndex = index / 2;
                    double slipperX = points[dotIndex].X;
                    double slipperY = points[dotIndex].Y;
                    //drawingContext.DrawEllipse(slipperColor, null, new Point(slipperX, slipperY), slipperSize, slipperSize);
                    //Rect rect = new Rect(slipperX - slipperSize, slipperY - slipperSize, 2 * slipperSize, 2 * slipperSize);
                    //drawingContext.DrawRectangle(slipperColor, null, rect);
                    Point point1 = new Point(slipperX - slipperSize, slipperY - Math.Sqrt(3) / 3 * slipperSize);
                    Point point2 = new Point(slipperX + slipperSize, slipperY - Math.Sqrt(3) / 3 * slipperSize);
                    Point point3 = new Point(slipperX, slipperY + 2 * Math.Sqrt(3) / 3 * slipperSize);
                    Point[] pntArr = { point1, point2, point3 };
                    StreamGeometry streamGeometry = new StreamGeometry();
                    using (StreamGeometryContext geometryContext = streamGeometry.Open())
                    {
                        geometryContext.BeginFigure(point1, true, true);
                        geometryContext.PolyLineTo(pntArr, true, true);
                    }
                    streamGeometry.Freeze();
                    drawingContext.DrawGeometry(Brushes.Red, new Pen(Brushes.Black, 3), streamGeometry);
                }
                else
                {
                    int lineIndex = index / 2;
                    double lineTimeDiff = timeDiff - dotTime[index];
                    Point point = pattern.Points[lineIndex] + lineVectors[lineIndex] * lineTimeDiff;
                    double slipperX = point.X;
                    double slipperY = point.Y;
                    //drawingContext.DrawEllipse(slipperColor, null, point, slipperSize, slipperSize); 
                    //Rect rect = new Rect(slipperX - slipperSize, slipperY - slipperSize, 2 * slipperSize, 2 * slipperSize);
                    //drawingContext.DrawRectangle(slipperColor, null, rect);
                    //drawingContext.DrawLine(null, new Point(slipperX, slipperY - slipperSize), new Point(slipperX - Math.Sqrt(3) / 2 * slipperSize, slipperY + 1 / 2 * slipperSize));
                    //drawingContext.DrawLine(null, new Point(slipperX - Math.Sqrt(3) / 2 * slipperSize, slipperY + 1 / 2 * slipperSize), new Point(slipperX + Math.Sqrt(3) / 2 * slipperSize, slipperY + 1 / 2 * slipperSize));
                    Point point1 = new Point(slipperX - slipperSize, slipperY - Math.Sqrt(3) / 3 * slipperSize);
                    Point point2 = new Point(slipperX + slipperSize, slipperY - Math.Sqrt(3) / 3 * slipperSize);
                    Point point3 = new Point(slipperX, slipperY + 2 * Math.Sqrt(3) / 3 * slipperSize);
                    Point[] pntArr = { point1, point2, point3 };
                    StreamGeometry streamGeometry = new StreamGeometry();
                    using (StreamGeometryContext geometryContext = streamGeometry.Open())
                    {
                        geometryContext.BeginFigure(point1, true, true);
                        geometryContext.PolyLineTo(pntArr, true, true);
                    }
                    streamGeometry.Freeze();
                    drawingContext.DrawGeometry(Brushes.Red, new Pen(Brushes.Black, 3), streamGeometry);
                }
            }
        }
    }
}
