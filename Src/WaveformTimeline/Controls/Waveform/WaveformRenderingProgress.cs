using MoreLinq;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WaveformTimeline.Commons;

namespace WaveformTimeline.Controls.Waveform
{
    internal sealed class WaveformRenderingProgress
    {
        public WaveformRenderingProgress(WaveformDimensions waveformDimensions,
            WaveformSection waveformSection,
            Canvas mainCanvas,
            PolyLineSegment leftWaveformPolyLine, PolyLineSegment rightWaveformPolyLine)
        {
            this._waveformDimensions = waveformDimensions;
            this._mainCanvas = mainCanvas;
            this._leftWaveformPolyLine = leftWaveformPolyLine;
            this._rightWaveformPolyLine = rightWaveformPolyLine;
            this._pointThickness = this._waveformDimensions.Width() / (int)((waveformSection.End - waveformSection.Start) / 2.0d);
            this._centerHeight = this._mainCanvas.RenderSize.Height / 2.0d;
        }

        private readonly WaveformDimensions _waveformDimensions;
        private readonly Canvas _mainCanvas;
        private readonly PolyLineSegment _leftWaveformPolyLine;
        private readonly PolyLineSegment _rightWaveformPolyLine;
        private readonly object _drawingLock = new object();
        private double _xLocation;
        private readonly double _pointThickness;
        private double _pointsDrawn;
        private readonly double _centerHeight;

        public void DrawWfPointByPoint(IList<float> leftRight)
        {
            Enumerable.Range(0, leftRight.Count / 2).Select(x => x * 2).ForEach(x => this.DrawWfPointByPointIter(leftRight, x));
        }

        private void DrawWfPointByPointIter(IList<float> leftRight, int x)
        {
            (this._xLocation, this._pointsDrawn) = this.AddWfPoints(leftRight[x], leftRight[x + 1]);
        }

        private (double, double) AddWfPoints(float left, float right)
        {
            lock (this._drawingLock)
            {
                var height = this._mainCanvas.RenderSize.Height / 2.0d;
                var location = ((this._pointsDrawn / 2) * this._pointThickness) + this._waveformDimensions.LeftMargin(); // where to draw - increasing by the point thickness
                this._leftWaveformPolyLine.Points.Add(new Point(this._xLocation, this._centerHeight - left * height));
                this._rightWaveformPolyLine.Points.Add(new Point(this._xLocation, this._centerHeight + right * height));
                return (location, this._pointsDrawn + 2);
            }
        }

        public void CompleteWaveform()
        {
            this._leftWaveformPolyLine.Points.Add(new Point(this._xLocation, this._centerHeight));
            this._leftWaveformPolyLine.Points.Add(new Point(this._waveformDimensions.LeftMargin(), this._centerHeight));
            this._rightWaveformPolyLine.Points.Add(new Point(this._xLocation, this._centerHeight));
            this._rightWaveformPolyLine.Points.Add(new Point(this._waveformDimensions.LeftMargin(), this._centerHeight));
        }
    }
}
