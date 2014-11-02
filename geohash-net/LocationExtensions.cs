using System;

namespace sharonjl.utils
{
	public static class LocationExtensions
	{
		public static double ToRadian(this double angle)
		{
			return Math.PI * angle / 180.0;
		}
	}
}

