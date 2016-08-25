--------------------------------------------------------------------
	Geolocation API
--------------------------------------------------------------------

	Die Geolocation API ermöglicht die Ermittlung von Positionen
	mittels bekannter Webdienste.

	Um die Api zu verwenden zu können, muss eine Instanz der
	Locator<T>-Klasse erzeugt werden. T ist hierbei die Angabe des
	Providers, der für die Auflösung verwendet werden soll.

	Vorhandene Provider sind:

	-> Google
		T -> MapsApiClient
	-> FreeGeoIp
		T -> FreeGeoIpClient

	Szenarien:
	
	-> Ermittlung des Standorts anhand der PLZ:

		var locator = new Locator<MapsApiClient>();
		Coordinate coord = locator.GetCoordinateFromStringExpression("44536");

	-> Ermittlung des Standorts anhand des Ortsnamens:

		var locator = new Locator<MapsApiClient>();
		Coordinate coord = locator.GetCoordinateFromStringExpression("Lünen");

	-> Kalkulieren von Abständen zweier Koordinaten:
		
		var locator = new Locator<MapsApiClient>();
		var location1 = new Coordinate();
		var location2 = new Coordinate();

		var distance = locator.GetLocationDifference(location1, location2);

	-> Ermittlung der nächstliegenden Orte innerhalb eines bestimmten Radius:

		Anmerkung: Noch nicht implementiert, folgt noch.

	Nur GeoIp:

	-> Bestimmung der aktuellen Position anhand einer IP-Adresse:

		var locator = new Locator<FreeGeoIpClient>();
		Coordinate c = locator.GetCoordinateFromStringExpression("88.208.188.154");		
		