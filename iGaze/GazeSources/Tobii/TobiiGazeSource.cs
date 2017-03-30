using EyeXFramework;
using Tobii.EyeX.Framework;

namespace iGaze.GazeSources.Tobii
{
	public class TobiiGazeSource : GazeSource
	{
		private double LastTimeStamp = -1;
		private readonly EyeXHost EyeXHost;
		private readonly GazePointDataStream GazePointDataStream;

		public TobiiGazeSource()
		{
			EyeXHost = new EyeXHost();
			GazePointDataStream = EyeXHost.CreateGazePointDataStream(GazePointDataMode.Unfiltered);
			GazePointDataStream.Next += GazeData;
			EyeXHost.Start();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
				EyeXHost.Dispose();
			base.Dispose(disposing);
		}

		private void GazeData(object sender, GazePointEventArgs e)
		{
			if (e.Timestamp != LastTimeStamp)
			{
				LastTimeStamp = e.Timestamp;
				OnGazeDataReceived(e.X, e.Y);
			}
		}
	}
}
