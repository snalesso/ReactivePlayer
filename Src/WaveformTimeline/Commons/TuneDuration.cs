﻿using JetBrains.Annotations;
using System;
using System.Diagnostics;
using WaveformTimeline.Contracts;
using WaveformTimeline.Primitives;

namespace WaveformTimeline.Commons
{
    /// <summary>
    /// Represents the portion of the track's duration in units to be displayed given a position and zoom levels.
    /// </summary>
    [DebuggerDisplay("TuneDuration | Start: {Start}, End: {End}, Zoom: {Zoom}")]
    public struct TuneDuration
    {
        /// <summary>
        /// Initialize with an instance of ITimedPlayback and assumed zoom=1
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public TuneDuration(ITimedPlayback tune) : this(tune, 1.0) { }

        /// <summary>
        /// Initialize with an instance of ITimedPlayback and an explicit zoom level
        /// </summary>
        public TuneDuration(ITimedPlayback tune, double zoom) : this(tune.CurrentTime().TotalSeconds, tune.TotalTime().TotalSeconds, zoom) { }

        /// <summary>
        /// Initialize with a set length, and assumed position=0 and zoom=1
        /// </summary>
        public TuneDuration(double length) : this(0, length, 1.0)
        {
        }

        /// <summary>
        /// Initialize with a set length, zoom, and assumed position=0
        /// </summary>
        public TuneDuration(double length, double zoom) : this(0, length, zoom)
        {
        }

        /// <summary>
        /// Initialize the instance with explicit position, length, and zoom.
        /// </summary>
        /// <param name="position">Position within the channel in units</param>
        /// <param name="length">Length of the channel in the same units</param>
        /// <param name="zoom">Zoom on the channel (zoom >= 1)</param>
        public TuneDuration(double position, double length, double zoom)
        {
            this.Length = Math.Max(0.0, new FiniteDouble(length).Value());
            this.Zoom = Math.Max(1.0, new FiniteDouble(zoom, 1.0).Value());
            double currentPosition = new FiniteDouble(this.Length <= 0.0 ? 0.0 : Math.Min(this.Length, position) / this.Length, 0.0d).Value();
            double fullArea = 1.0d / this.Zoom;
            double halfArea = fullArea / 2.0d;
            double windowStart = currentPosition - halfArea; // this can mean < 0
            double windowEnd = currentPosition + halfArea; // this can mean > 1

            // I want to preferably see the end
            if (1 - fullArea <= currentPosition)
            {
                windowStart = 1 - fullArea;
                windowEnd = 1;
            }
            if (windowStart < 0.0d)
            {
                windowStart = 0.0d;
                windowEnd = windowStart + fullArea;
            }
            if (windowEnd > 1.0d)
            {
                windowEnd = 1.0d;
                windowStart = windowEnd - fullArea;
            }

            var windowBetween0and1 = windowStart >= 0.0d && windowEnd <= 1.0d;
            var windowStartBeforeEnd = windowEnd > windowStart;
            if (!windowBetween0and1 || !windowStartBeforeEnd)
                throw new Exception($"Assertion failed: {windowStart} and {windowEnd} are suspect!");
            this.Start = windowStart;
            this.End = windowEnd;
        }

        private double Length { get; }

        /// <summary>
        /// Zoom level
        /// </summary>
        public double Zoom { get; }

        /// <summary>
        /// A double between 0-1
        /// </summary>
        private ZeroToOne Start { get; }

        /// <summary>
        /// A double between 0-1
        /// </summary>
        private ZeroToOne End { get; }

        /// <summary>
        /// Tells whether a given point (0 - 1) is visible on the currently displayed waveform given the zoom level.
        /// </summary>
        [Pure]
        public bool Includes(ZeroToOne point)
        {
            return point >= this.Start && point <= this.End;
        }

        /// <summary>
        /// Provides the starting position in the channel in units
        /// </summary>
        /// <returns></returns>
        [Pure]
        public double StartingPoint()
        {
            return this.Start * this.Length;
        }

        /// <summary>
        /// Given an arbitrary number, e.g. a width of a canvas in units, it returns the number of units
        /// of this canvas that precede the area covered by this instance
        /// </summary>
        /// <param name="total"></param>
        /// <returns></returns>
        [Pure]
        public double HiddenBefore(double total)
        {
            return this.Start * total;
        }

        /// <summary>
        /// Progress within the covered area (0-1)
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        [Pure]
        public ZeroToOne Progress(double position)
        {
            return new ZeroToOne((new FiniteDouble(position) - this.StartingPoint()) / this.Duration());
        }

        /// <summary>
        /// Actual position in the total channel, regardless of which section of the Tune is covered by this instance
        /// </summary>
        /// <param name="percOfCoveredArea">Position within the section of the Tune that is covered by this instance</param>
        /// <returns>Actual position in the total channel</returns>
        [Pure]
        public double ActualPosition(ZeroToOne percOfCoveredArea)
        {
            return this.StartingPoint() + percOfCoveredArea * this.Duration();
        }

        /// <summary>
        /// Duration of the channel within the covered area (100% when Zoom=1, less when Zoom > 1)
        /// </summary>
        /// <returns></returns>
        [Pure]
        public double Duration()
        {
            return this.Length * (this.End - this.Start);
        }

        /// <summary>
        /// Given a position, how much of this area's Length is remaining
        /// </summary>
        /// <param name="position">Position in the channel in units</param>
        /// <returns>Remaining length in units</returns>
        [Pure]
        public double Remaining(double position)
        {
            return this.Duration() - this.Progress(position) * this.Duration();
        }

        /// <summary>
        /// For equality comparisons, we take into account the total Duration, Start, and End. TODO: what about Zoom?
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return obj is TuneDuration area &&
                   this.Start.Equals(area.Start) &&
                   this.End.Equals(area.End) &&
                    this.Duration().Equals(area.Duration());
        }

        public bool Equals(TuneDuration other)
        {
            return this.Start.Equals(other.Start) && this.End.Equals(other.End) && this.Duration().Equals(other.Duration());
        }

        public static bool operator ==(TuneDuration td1, TuneDuration td2)
        {
            return td1.Equals(td2);
        }

        public static bool operator !=(TuneDuration td1, TuneDuration td2)
        {
            return !(td1 == td2);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (this.Start.GetHashCode() * 397) ^ this.End.GetHashCode();
            }
        }
    }
}