using System;

namespace sharonjl.utils
{
	public class Location
	{
		public Location(){}

		public Location(double latitude, double longitude){
			Latitude = latitude;
			Longitude = longitude;
		}

		public double Latitude {get;set;}

		public double Longitude {get;set;}
	}
}

