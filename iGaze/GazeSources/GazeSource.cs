using System;
using System.Drawing;

namespace iGaze.GazeSources
{
	public abstract class GazeSource : IDisposable
	{
		public Point DataPoint { get; private set; }
		public DateTime DataTimeStamp { get; private set; }
		public bool IsDisposed { get; private set; }

		private KalmanFilter KalmanX;
		private KalmanFilter KalmanY;

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
			ApplyKalmanFilters(ref x, ref y);
			DataTimeStamp = DateTime.UtcNow;
			DataPoint = new Point((int)x, (int)y);
		}

		private void ApplyKalmanFilters(ref double x, ref double y)
		{
			if (KalmanX == null)
			{
				KalmanX = new KalmanFilter(x, 0.5, 1.5, 1000);
				KalmanY = new KalmanFilter(y, 0.5, 1.5, 1000);
			}
			x = KalmanX.Update(x);
			y = KalmanY.Update(y);
		}
	}
}
