using NUnit.Framework;
using System;
using sharonjl.utils;
using System.Collections.Generic;

namespace geohashnetunittests
{
	[TestFixture ()]
	public class Test
	{
		private List<double[]> locations = new List<double[]>{
			new [] { 53.348642, -6.278743 },
			new [] { 53.348939, -6.283747 },
			new [] { 64.103743, -21.891493 }
		};

		


		[Test ()]
		public void first ()
		{
			var latitude = locations[2][0];
			var longitude = locations[2][1];

			var geohash = Geohash.Encode (latitude, longitude, 20);

			var unencodedGeohash = Geohash.Decode (geohash);

			Assert.That (unencodedGeohash [0], Is.EqualTo (latitude));
		}
	}
}

