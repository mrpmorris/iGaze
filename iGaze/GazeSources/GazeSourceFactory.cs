namespace iGaze.GazeSources
{
	public static class GazeSourceFactory
	{
		public static GazeSource Create()
		{
			return new Tobii.TobiiGazeSource();
		}
	}
}
