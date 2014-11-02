/**
 *  Copyright (C) 2011 by Sharon Lourduraj
 *  
 *  Permission is hereby granted, free of charge, to any person obtaining a copy
 *  of this software and associated documentation files (the "Software"), to deal
 *  in the Software without restriction, including without limitation the rights
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *  copies of the Software, and to permit persons to whom the Software is
 *  furnished to do so, subject to the following conditions:
 *
 *  The above copyright notice and this permission notice shall be included in
 *  all copies or substantial portions of the Software.
 *
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 *  THE SOFTWARE.
 *  ------------------------------------------------------------------------------
 *  
 *  This code is a direct derivation from:
 *	  GeoHash Routines for Javascript 2008 (c) David Troy. 
 *  The source of which can be found at: 
 *	  https://github.com/davetroy/geohash-js
 */
using System;

namespace sharonjl.utils
{
	public static class Geohash
	{


		private const string Base32 = "0123456789bcdefghjkmnpqrstuvwxyz";
		private static readonly int[] Bits = new[] {16, 8, 4, 2, 1};
		private static readonly int[] OddnessLookup = {0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1};

		private static readonly string[][] Neighbors = {
			// even
			new[] {
				"p0r21436x8zb9dcf5h7kjnmqesgutwvy", // Top
				"bc01fg45238967deuvhjyznpkmstqrwx", // Right
				"14365h7k9dcfesgujnmqp0r2twvyx8zb", // Bottom
				"238967debc01fg45kmstqrwxuvhjyznp", // Left
			}, 
			// odd
			new[] {
				"bc01fg45238967deuvhjyznpkmstqrwx", // Top
				"p0r21436x8zb9dcf5h7kjnmqesgutwvy", // Right
				"238967debc01fg45kmstqrwxuvhjyznp", // Bottom
				"14365h7k9dcfesgujnmqp0r2twvyx8zb", // Left
			}
		};

		private static readonly string[][] Borders = {
			// even
			new[] { "prxz", "bcfguvyz", "028b", "0145hjnp" },
			// odd
			new[] { "bcfguvyz", "prxz", "0145hjnp", "028b" }
		};

		public static String CalculateAdjacent(string hash, Direction direction)
		{
			hash = hash.ToLower();

			char lastChr = hash[hash.Length - 1];
			var oddness = OddnessLookup[hash.Length]; // 0 = even, 1 = odd
			var dir = (int) direction;
			var nHash = hash.Substring(0, hash.Length - 1);

			if (Borders[oddness][dir].IndexOf(lastChr) != -1)
			{
				nHash = CalculateAdjacent(nHash, direction);
			}

			return nHash + Base32[Neighbors[oddness][dir].IndexOf(lastChr)];
		}

		public static void RefineInterval(ref double[] interval, int cd, int mask)
		{
			if ((cd & mask) != 0)
			{
				interval[0] = (interval[0] + interval[1])/2;
			}
			else
			{
				interval[1] = (interval[0] + interval[1])/2;
			}
		}

		public static Location Decode(String geohash)
		{
			bool even = true;
			double[] lat = {-90.0, 90.0};
			double[] lon = {-180.0, 180.0};

			foreach (char c in geohash)
			{
				int cd = Base32.IndexOf(c);
				for (int j = 0; j < 5; j++)
				{
					int mask = Bits[j];
					if (even)
					{
						RefineInterval(ref lon, cd, mask);
					}
					else
					{
						RefineInterval(ref lat, cd, mask);
					}
					even = !even;
				}
			}

			return new Location {
				Latitude = (lat[0] + lat[1])/2, 
				Longitude = (lon[0] + lon[1])/2
			};
		}

		public static string Encode(Location location, int precision = 16)
		{
			bool even = true;
			int bit = 0;
			int ch = 0;
			string geohash = "";

			double[] lat = {-90.0, 90.0};
			double[] lon = {-180.0, 180.0};

			// hashes over 22 bits seem invalid, and long hashes only seem useful near poles
			if (precision < 1 || precision > 22) precision = 22;

			while (geohash.Length < precision)
			{
				double mid;

				if (even)
				{
					mid = (lon[0] + lon[1])/2;
					if (location.Longitude > mid)
					{
						ch |= Bits[bit];
						lon[0] = mid;
					}
					else
						lon[1] = mid;
				}
				else
				{
					mid = (lat[0] + lat[1])/2;
					if (location.Latitude > mid)
					{
						ch |= Bits[bit];
						lat[0] = mid;
					}
					else
						lat[1] = mid;
				}

				even = !even;
				if (bit < 4)
					bit++;
				else
				{
					geohash += Base32[ch];
					bit = 0;
					ch = 0;
				}
			}
			return geohash;
		}

		// https://docs.google.com/spreadsheets/d/1HSsZAetPBlFgbGFbm0a4C8M587xpFC9Xkn3BIvDiJDs/edit?pli=1



		// for X bits of geohash 
		// box height degrees
		// Hdbx =180 / (2^((5*X+(X%2))/2))
		// box width degrees
		// Wdbx = 180 / (2^(((5*X+(X%2))/2)-1))

		// latitudewise length of single degree in metre at latitude La
		// LLa =111132.954 - 559.882*cos(radians(2*La))+1.175*cos(radians(4*La))
		// longitudewise length of single degree in meter at latitude La
		// LLd =(pi()/180)*6378137*cos(radians(La))

		// height in meters
		// Hmbx = Hdbx * LLa
		// width in meters
		// Wmbx = Wdbx * LLd



		/// <summary>
		/// Calculates the angular size of the geohash
		/// </summary>
		/// <returns>The angular size.</returns>
		public static double[] CalculateAngularSize(string geoHash){
			var bits = geoHash.Length;
			var power = (5 * bits + (bits % 2)) / 2;

			var angularH = 180.0 / Math.Pow(2.0, power);
			var angularW = 180.0 / Math.Pow(2.0,power - 1);

			return new [] { angularH, angularW };
		}

		/// <summary>
		/// Calculates the approximate physical size of the geohash
		/// </summary>
		/// <returns>The size.</returns>
		public static double[] CalculateSize(string geoHash){

			var geoPos = Decode (geoHash);

			var angularSize = CalculateAngularSize (geoHash);

			return CalculateSize (geoPos, angularSize);
		}

		private static double[] CalculateSize(Location geoPos, double[] angSize){

			// first calculate the size of a degree at the selected latitude
			var LLa = 111132.954
				- (559.882 * Math.Cos ((2.0 * geoPos.Latitude).ToRadian()))
				+ (1.175 * Math.Cos ((4.0 * geoPos.Latitude).ToRadian()));

			var LLd = (Math.PI / 180) * 6378137 * Math.Cos ( geoPos.Latitude.ToRadian());

			// now return the size of a degree by the angular size
			return new [] { LLa * angSize[0], LLd * angSize[1] };
		}

		/// <summary>
		/// Returns the surface distance in meters between two points on a globe, rather than the straight
		/// line distance (which probably cuts below the surface)
		/// Note: this is only an approximation due to calculating on an assumed sphere
		/// Better to replace with a combination of the short and long line methods listed here:
		/// https://en.wikipedia.org/wiki/Geographical_distance
		/// </summary>
		/// <returns>The distance.</returns>
		/// <param name="locA">The first point</param>
		/// <param name="locB">The second point</param>
		public static double HaversineDistance(Location locA, Location locB)
		{
			const double R = 6371;

			var lat = (locB.Latitude - locA.Latitude).ToRadian();
			var lng = (locB.Longitude - locA.Longitude).ToRadian();
			var h1 = (Math.Sin(lat / 2) * Math.Sin(lat / 2)) 
				+ (Math.Cos(locA.Latitude.ToRadian()) * Math.Cos(locB.Latitude.ToRadian()) *
					Math.Sin(lng / 2) * Math.Sin(lng / 2));
			var h2 = 2 * Math.Asin(Math.Min(1, Math.Sqrt(h1)));

			return R * h2 * 1000;
		}
	}
}