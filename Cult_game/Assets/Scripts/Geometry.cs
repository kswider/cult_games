using System;
using UnityEngine;

public static class Geometry
{
    public static float DistanceFromCoordinates(Vector2 coord1, Vector2 coord2)
    {
        //sauce: https://stackoverflow.com/questions/639695/how-to-convert-latitude-or-longitude-to-meters
        const double R = 6378.137; // Radius of earth in KM
        double dLat = coord2.x * Math.PI / 180 - coord1.x * Math.PI / 180;
        double dLon = coord2.y * Math.PI / 180 - coord1.y * Math.PI / 180;
        double a = Math.Sin(dLat/2) * Math.Sin(dLat/2) +
                   Math.Cos(coord1.x * Math.PI / 180) * Math.Cos(coord2.x * Math.PI / 180) *
                   Math.Sin(dLon/2) * Math.Sin(dLon/2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1-a));
        double d = R * c;
        return (float)d * 1000; // meters
    }
    
    public static float AngleFromCoordinate(Vector2 coord1, Vector2 coord2)
    {
        //sauce: https://stackoverflow.com/questions/3932502/calculate-angle-between-two-latitude-longitude-points
        double dLon = coord2.y - coord1.y;

        double y = Math.Sin(dLon) * Math.Cos(coord2.x);
        double x = Math.Cos(coord1.x) * Math.Sin(coord2.x) - Math.Sin(coord1.x)
                   * Math.Cos(coord2.x) * Math.Cos(dLon);

        double brng = Math.Atan2(y, x);

        brng = brng * 180 / Math.PI;
        brng = (brng + 360) % 360;
        brng = 360 - brng; // count degrees counter-clockwise - remove to make clockwise

        return (float)brng;
    }
}
