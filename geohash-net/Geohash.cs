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
		#region Direction enum

		public enum Direction
		{
			Top = 0,
			Right = 1,
			Bottom = 2,
			Left = 3 
		}

		#endregion

		private const string Base32 = "0123456789bcdefghjkmnpqrstuvwxyz";
		private static readonly int[] Bits = new[] {16, 8, 4, 2, 1};

		private static readonly string[][] Neighbors = {
			new[] {
				"p0r21436x8zb9dcf5h7kjnmqesgutwvy", // Top
				"bc01fg45238967deuvhjyznpkmstqrwx", // Right
				"14365h7k9dcfesgujnmqp0r2twvyx8zb", // Bottom
				"238967debc01fg45kmstqrwxuvhjyznp", // Left
			}, new[] {
				"bc01fg45238967deuvhjyznpkmstqrwx", // Top
				"p0r21436x8zb9dcf5h7kjnmqesgutwvy", // Right
				"238967debc01fg45kmstqrwxuvhjyznp", // Bottom
				"14365h7k9dcfesgujnmqp0r2twvyx8zb", // Left
			}
		};

		private static readonly string[][] Borders = {
			new[] { "prxz", "bcfguvyz", "028b", "0145hjnp" },
			new[] { "bcfguvyz", "prxz", "0145hjnp", "028b" }
		};

		public static String CalculateAdjacent(String hash, Direction direction)
		{
			hash = hash.ToLower();

			char lastChr = hash[hash.Length - 1];
			int type = hash.Length%2;
			var dir = (int) direction;
			string nHash = hash.Substring(0, hash.Length - 1);

			if (Borders[type][dir].IndexOf(lastChr) != -1)
			{
				nHash = CalculateAdjacent(nHash, (Direction) dir);
			}
			return nHash + Base32[Neighbors[type][dir].IndexOf(lastChr)];
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

		public static double[] Decode(String geohash)
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

			return new[] {(lat[0] + lat[1])/2, (lon[0] + lon[1])/2};
		}

		public static String Encode(double latitude, double longitude, int precision = 12)
		{
			bool even = true;
			int bit = 0;
			int ch = 0;
			string geohash = "";

			double[] lat = {-90.0, 90.0};
			double[] lon = {-180.0, 180.0};

			if (precision < 1 || precision > 20) precision = 12;

			while (geohash.Length < precision)
			{
				double mid;

				if (even)
				{
					mid = (lon[0] + lon[1])/2;
					if (longitude > mid)
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
					if (latitude > mid)
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

			var angularH = 180 / Math.Pow (2.0, power);
			var angularW = 180 / Math.Pow (2.0, power - 1);

			return new [] { angularH, angularW };
		}

		/// <summary>
		/// Calculates the approximate physical size of the geohash
		/// </summary>
		/// <returns>The size.</returns>
		public static double[] CalculateSize(string geoHash){

			var geoPos = Decode (geoHash);
			var La = geoPos [0];

			var LLa = 111132.954
				- (559.882 * Math.Cos (DegreeToRadian (2 * La)))
				+ (1.175 * Math.Cos (DegreeToRadian (4 * La)));

			var LLd = (Math.PI / 180) * 6378137 * Math.Cos (DegreeToRadian (La));

			return new [] { LLa, LLd };
		}

		private static double DegreeToRadian(double angle)
		{
			return Math.PI * angle / 180.0;
		}
	}
}