using iGaze.HelperClasses;
using System;
using System.Drawing;

namespace iGaze
{
	public static class InverseBiLinear
	{
		static float Cross(PointF a, PointF b) => a.X * b.Y - a.Y * b.X;

		public static PointF Calculate(PointF point, PointF topLeft, PointF topRight, PointF bottomRight, PointF bottomLeft)
		{
			PointF e = topRight.Subtract(topLeft);
			PointF f = bottomLeft.Subtract(topLeft);
			PointF g = topLeft.Subtract(topRight).Add(bottomRight).Subtract(bottomLeft);
			PointF h = point.Subtract(topLeft);

			double k2 = Cross(g, f);
			double k1 = Cross(e, f) + Cross(h, g);
			double k0 = Cross(h, e);

			double w = k1 * k1 - 4f * k0 * k2;
			if (w < 0.0)
				return new PointF(-1f, -1f);
			w = (double)Math.Sqrt(w);

			double v1 = (-k1 - w) / (2f * k2);
			double u1 = (h.X - f.X * v1) / (e.X + g.X * v1);

			double v2 = (-k1 + w) / (2f * k2);
			double u2 = (h.X - f.X * v2) / (e.X + g.X * v2);

			double u = u1;
			double v = v1;

			if (v < 0.0 || v > 1.0 || u < 0.0 || u > 1.0) { u = u2; v = v2; }
			if (v < 0.0 || v > 1.0 || u < 0.0 || u > 1.0) { u = -1f; v = -1f; }

			return new PointF((float)u, (float)v);
		}
	}
}
