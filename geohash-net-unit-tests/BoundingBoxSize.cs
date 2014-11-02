using NUnit.Framework;
using System;
using sharonjl.utils;
using System.Collections.Generic;
using System.Diagnostics;

namespace geohashnetunittests
{
	[TestFixture ()]
	public class Test
	{
		[Test]
		public void Reykjavic_HashBoundingBox_6bit(){

			var Amsterdamøya = TestLocations.KeyedLocations ["Reykjavic"];

			var geohash = Geohash.Encode (Amsterdamøya, 6);

			var boundBoxSize = Geohash.CalculateSize (geohash);

			Assert.That (boundBoxSize [0], Is.LessThan (620));
			Assert.That (boundBoxSize [1], Is.LessThan (540));
		}

		[Test]
		public void Amsterdamøya_HashBoundingBox_4bit(){
		
			var Amsterdamøya = TestLocations.KeyedLocations ["Amsterdamøya"];

			var geohash = Geohash.Encode (Amsterdamøya, 4);

			var boundBoxSize = Geohash.CalculateSize (geohash);

			Assert.That (boundBoxSize [0], Is.LessThan (20000));
			Assert.That (boundBoxSize [1], Is.LessThan (27000));
		}

		[Test]
		public void PicBlanc_HashBoundingBox_12bit(){

			var picBlanc = TestLocations.KeyedLocations ["Pic Blanc"];

			var geohash = Geohash.Encode (picBlanc, 12);

			var boundBoxSize = Geohash.CalculateSize (geohash);

			Assert.That (boundBoxSize [0], Is.LessThan (0.019)); // 19cm
			Assert.That (boundBoxSize [1], Is.LessThan (0.027));
		}

		[Test]
		public void Tara_HashBoundingBox_20bit(){

			var tara = TestLocations.KeyedLocations ["Tara"];

			var geohash = Geohash.Encode (tara, 20);

			var boundBoxSize = Geohash.CalculateSize (geohash);

			Assert.That (boundBoxSize [0], Is.LessThan (0.000000018)); // 180nanometres!
			Assert.That (boundBoxSize [1], Is.LessThan (0.000000022));
		}

		[Test]
		public void BoundingBoxAngularSize(){

			var geohash = Geohash.Encode (new Location(0.0,0.0), 1);

			var boundBoxSize = Geohash.CalculateAngularSize (geohash);

			Assert.That (boundBoxSize [0], Is.EqualTo (22.5));
			Assert.That (boundBoxSize [1], Is.EqualTo (45));
		}

	}
}

