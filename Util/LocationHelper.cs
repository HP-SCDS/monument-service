namespace MonumentService.Util
{
    public static class LocationHelper
    {
        private const double EarthRadiusInKilometers = 6371;

        public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            // convert latitude and longitude to radians
            lat1 = ConvertToRadians(lat1);
            lon1 = ConvertToRadians(lon1);
            lat2 = ConvertToRadians(lat2);
            lon2 = ConvertToRadians(lon2);

            // calculate differences between the coordinates
            double latDiff = lat2 - lat1;
            double lonDiff = lon2 - lon1;

            // calculate the Haversine formula
            double a = Math.Sin(latDiff / 2) * Math.Sin(latDiff / 2) +
                       Math.Cos(lat1) * Math.Cos(lat2) *
                       Math.Sin(lonDiff / 2) * Math.Sin(lonDiff / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            // calculate the distance in kilometers
            double distance = EarthRadiusInKilometers * c;

            return distance;
        }

        public static double ConvertToRadians(double value)
        {
            return Math.PI / 180 * value;
        }
    }
}
