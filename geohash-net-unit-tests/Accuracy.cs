using NUnit.Framework;
using sharonjl.utils;
using System;

namespace geohashnetunittests
{
	[TestFixture]
	public class AccuracyTests
	{
		[Test]
		public void GeohashDecodeLocationError_5Bits ()
		{
			GeocodeDecodeLocationError (TestLocations.KeyedLocations ["Reykjavic"], 5);
		}

		[Test]
		public void Reykjavik_GeohashDecodeLocationError_6Bits ()
		{
			GeocodeDecodeLocationError (TestLocations.KeyedLocations ["Reykjavic"], 6);
		}

		[Test]
		public void Reykjavik_GeohashDecodeLocationError_7Bits ()
		{
			GeocodeDecodeLocationError (TestLocations.KeyedLocations ["Reykjavic"], 7);
		}

		[Test]
		public void Reykjavik_GeohashDecodeLocationError_8Bits ()
		{
			GeocodeDecodeLocationError (TestLocations.KeyedLocations ["Reykjavic"], 8);
		}

		[Test]
		public void Reykjavik_GeohashDecodeLocationError_9Bits ()
		{
			GeocodeDecodeLocationError (TestLocations.KeyedLocations ["Reykjavic"], 9);
		}

		[Test]
		public void Reykjavik_GeohashDecodeLocationError_10Bits ()
		{
			GeocodeDecodeLocationError (TestLocations.KeyedLocations ["Reykjavic"], 10);
		}

		[Test]
		public void Reykjavik_GeohashDecodeLocationError_12Bits ()
		{
			GeocodeDecodeLocationError (TestLocations.KeyedLocations ["Reykjavic"], 12);
		}

		[Test]
		public void PicBlanc_GeohashDecodeLocationError_10Bits ()
		{
			GeocodeDecodeLocationError (TestLocations.KeyedLocations ["Pic Blanc"], 10);
		}



		[Test]
		public void PicBlanc_GeohashDecodeLocationError_12Bits ()
		{
			GeocodeDecodeLocationError (TestLocations.KeyedLocations ["Pic Blanc"], 12);
		}


		[Test]
		public void Tara_GeohashDecodeLocationError_12Bits ()
		{
			GeocodeDecodeLocationError (TestLocations.KeyedLocations ["Tara"], 12);
		}


		/// <summary>
		/// Check the distance between the original location and the decoded geohash 
		/// to get an estimate of the expected accuracy at different bit lengths
		/// The test uses the location as a centre point and spirals out measuring the error at many points 
		/// in an area up to 3.5 times the angular size of the bounding box at that latitude
		/// </summary>
		/// <param name="loc">Location.</param>
		/// <param name="bits">Bits.</param>
		private void GeocodeDecodeLocationError(Location loc, int bits)
		{
			var steps = 5000;
			var rotations = 4;
			var areaMultiplier = 2;

			var hash = Geohash.Encode (loc, bits);
			var angularSize = Geohash.CalculateAngularSize (hash);
			var step = angularSize[0] * areaMultiplier / steps;

			var largestErrorLatitude = Double.MinValue;
			var largestErrorLongitude = Double.MinValue;

			for (var j = steps ; j > 0 ; j--){

				var newLoc = new Location {
					Latitude = loc.Latitude + (step * j * Math.Sin (rotations * j / steps)),
					Longitude = loc.Longitude + (step * j * Math.Cos (rotations * j / steps))
				};

				var geohash = Geohash.Encode (newLoc, bits);

				var unencodedGeohash = Geohash.Decode (geohash);

				var latDiff = Math.Abs (newLoc.Latitude - unencodedGeohash.Latitude);
				var lonDiff = Math.Abs (newLoc.Longitude - unencodedGeohash.Longitude);

				largestErrorLatitude = latDiff > largestErrorLatitude
					? latDiff
					: largestErrorLatitude;

				largestErrorLongitude = lonDiff > largestErrorLongitude
					? lonDiff
					: largestErrorLongitude;
			}
				
			Assert.That (largestErrorLatitude , Is.LessThan (angularSize [0]));
			Assert.That (largestErrorLongitude , Is.LessThan (angularSize [1]));
		}

	}
}

