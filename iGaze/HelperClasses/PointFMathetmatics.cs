using System.Drawing;

namespace iGaze.HelperClasses
{
	public static class PointFMathetmatics
	{
		public static PointF Add(this PointF source, PointF value)
		{
			return new PointF(source.X + value.X, source.Y + value.Y);
		}

		public static PointF Subtract(this PointF source, PointF value)
		{
			return new PointF(source.X - value.X, source.Y - value.Y);
		}
	}
}
