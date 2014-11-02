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
			GeocodeDecodeLocationError (TestLocations.KeyedLocations ["Reykjavic"], 5, 2670);
		}

		[Test]
		public void Reykjavik_GeohashDecodeLocationError_6Bits ()
		{
			GeocodeDecodeLocationError (TestLocations.KeyedLocations ["Reykjavic"], 6, 405);
		}

		[Test]
		public void Reykjavik_GeohashDecodeLocationError_7Bits ()
		{
			GeocodeDecodeLocationError (TestLocations.KeyedLocations ["Reykjavic"], 7, 85);
		}

		[Test]
		public void Reykjavik_GeohashDecodeLocationError_8Bits ()
		{
			GeocodeDecodeLocationError (TestLocations.KeyedLocations ["Reykjavic"], 8, 13);
		}

		[Test]
		public void Reykjavik_GeohashDecodeLocationError_9Bits ()
		{
			GeocodeDecodeLocationError (TestLocations.KeyedLocations ["Reykjavic"], 9, 2.6);
		}

		[Test]
		public void Reykjavik_GeohashDecodeLocationError_10Bits ()
		{
			GeocodeDecodeLocationError (TestLocations.KeyedLocations ["Reykjavic"], 10, 0.45);
		}

		[Test]
		public void Reykjavik_GeohashDecodeLocationError_12Bits ()
		{
			GeocodeDecodeLocationError (TestLocations.KeyedLocations ["Reykjavic"], 12, 0.015);
		}

		[Test]
		public void PicBlanc_GeohashDecodeLocationError_10Bits ()
		{
			GeocodeDecodeLocationError (TestLocations.KeyedLocations ["Pic Blanc"], 10, 0.45);
		}



		[Test]
		public void PicBlanc_GeohashDecodeLocationError_12Bits ()
		{
			GeocodeDecodeLocationError (TestLocations.KeyedLocations ["Pic Blanc"], 12, 0.015);
		}


		[Test]
		public void Tara_GeohashDecodeLocationError_12Bits ()
		{
			GeocodeDecodeLocationError (TestLocations.KeyedLocations ["Tara"], 12, 0.015);
		}


		/// <summary>
		/// Check the distance between the original location and the decoded geohash 
		/// to get an estimate of the expected accuracy at different bit lengths
		/// The test uses the location as a centre point and spirals out measuring the error at many points 
		/// in an area up to 3.5 times the angular size of the bounding box at that latitude
		/// </summary>
		/// <param name="loc">Location.</param>
		/// <param name="bits">Bits.</param>
		/// <param name="expectedLargestError">Expected largest error.</param>
		private void GeocodeDecodeLocationError(Location loc, int bits, double expectedLargestError)
		{
			// TODO: the largest error should be expressed in percentage - based on the bounding box size

			var steps = 14000;
			var rotations = 4;

			var hash = Geohash.Encode (loc, bits);
			var size = Geohash.CalculateAngularSize (hash);
			var step = size[0] * 3 / steps;

			var largestError = Double.MinValue;

			for (var j = steps ; j > 0 ; j--){

				var newLoc = new Location {
					Latitude = loc.Latitude + (step * j * Math.Sin (rotations * j / steps)),
					Longitude = loc.Longitude + (step * j * Math.Cos (rotations * j / steps))
				};

				var geohash = Geohash.Encode (newLoc, bits);

				var unencodedGeohash = Geohash.Decode (geohash);

				var haversineDiff = Geohash.HaversineDistance (newLoc, unencodedGeohash);

				largestError = haversineDiff > largestError
					? haversineDiff
					: largestError;
			}

			Assert.That (largestError , Is.LessThan (expectedLargestError));
		}

	}
}

