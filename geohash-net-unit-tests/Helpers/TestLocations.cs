using System;
using sharonjl.utils;
using System.Collections.Generic;

namespace geohashnetunittests
{
	public static class TestLocations
	{
		public static List<Location> Locations = new List<Location>();

		public static Dictionary<string, Location> KeyedLocations = new Dictionary<string, Location> ();

		static TestLocations() 
		{
			KeyedLocations.Add ("Carpenter Island", new Location ( -72.652478, -98.111406 ));
			KeyedLocations.Add ("Codfish Island", new Location ( -46.772263, 167.629602 ));
			KeyedLocations.Add ("Uluru", new Location ( -25.344871, 131.034503 ));
			KeyedLocations.Add ("Taj Mahal", new Location ( 27.175273, 78.042134 ));
			KeyedLocations.Add ("Pic Blanc", new Location ( 45.125107, 6.127332 ));
			KeyedLocations.Add ("Buttes-Chaumont", new Location ( 48.880916, 2.382847 ));
			KeyedLocations.Add ("Tara", new Location ( 53.578996, -6.611670 ));
			KeyedLocations.Add ("Reykjavic", new Location ( 64.103743, -21.891493 ));
			KeyedLocations.Add ("Amsterdamøya", new Location ( 79.754484, 10.819219 ));

			foreach (var loc in KeyedLocations) {
				Locations.Add (loc.Value);
			}

			Locations.Add (new Location (0.0, 0.0));
		}
	}
}

