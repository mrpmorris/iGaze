using System;
using System.Drawing;

namespace iGaze.GazeSources
{
	public abstract class GazeSource : IDisposable
	{
		public Point DataPoint { get; private set; }
		public DateTime DataTimeStamp { get; private set; }
		public bool UseCalibration { get; set; } = true;
		public bool UseSmoothing { get; set; } = true;
		public bool IsDisposed { get; private set; }

		private KalmanFilter KalmanX;
		private KalmanFilter KalmanY;
		private static PointF[] CalibrationPoints;

		static GazeSource()
		{
			CalibrationPoints = new PointF[]
			{
				new PointF(0.01f, 0.01f),
				new PointF(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width, 0.01f),
				new PointF(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width,
					System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height),
				new PointF(0.01f, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height)
			};
		}

		public void SetCalibrationPoints(PointF topLeft, PointF topRight, PointF bottomRight, PointF bottomLeft)
		{
			CalibrationPoints[0] = topLeft;
			CalibrationPoints[1] = topRight;
			CalibrationPoints[2] = bottomRight;
			CalibrationPoints[3] = bottomLeft;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				if (disposing)
				{
				}
				IsDisposed = true;
			}
		}

		public virtual void Dispose()
		{
			Dispose(true);
		}

		protected void OnGazeDataReceived(double x, double y)
		{
			System.Diagnostics.Debug.Write($"{x},{y}\t\t");
			if (UseSmoothing)
				ApplyKalmanFilters(ref x, ref y);
			if (UseCalibration)
				ApplyCalibration(ref x, ref y);
			DataTimeStamp = DateTime.UtcNow;
			DataPoint = new Point((int)x, (int)y);
			//System.Windows.Forms.Cursor.Position = DataPoint;
		}

		private void ApplyKalmanFilters(ref double x, ref double y)
		{
			if (KalmanX == null)
			{
				KalmanX = new KalmanFilter(x, 0.5, 5, 1000);
				KalmanY = new KalmanFilter(y, 0.5, 5, 1000);
			}
			x = KalmanX.Update(x);
			y = KalmanY.Update(y);
		}

		private void ApplyCalibration(ref double x, ref double y)
		{
			var point = new PointF((float)x, (float)y);
			PointF calibratedPoint = InverseBiLinear.Calculate(point, CalibrationPoints[0], CalibrationPoints[1], CalibrationPoints[2], CalibrationPoints[3]);
			x = calibratedPoint.X * System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
			y = calibratedPoint.Y * System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
		}
	}
}
