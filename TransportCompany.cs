using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportCompany
{
    class Driver
    {
        public static List<Driver> drivers = new List<Driver>();

        public int id;
        public string lastName;
        public string firstName;
        public DateTime birthDate;
        public DateTime hireDate;
        public string phone;
        public DriveCategory driveCategory;

        public override string ToString()
        {
            return $"{lastName} {firstName}";
        }
    }

    class Auto
    {
        public static List<Auto> autos = new List<Auto>();

        public int id;
        public string number;
        public string brand;
        public string model;
        public int year;
        public int placeCount;
        public decimal consumption;

        public override string ToString()
        {
            return $"{brand} {model} ({number})";
        }
    }

    class Route
    {
        public static List<Route> routes = new List<Route>();

        public int id;
        public Driver driver;
        public Auto auto;
        public DateTime dateTime;
        public string departurePoint;
        public string arrivalPoint;
    }

    [Flags]
    enum DriveCategory
    {
        None = 0,
        A = 1 << 0,
        B = 1 << 1,
        C = 1 << 2,
        D = 1 << 3,
        E = 1 << 4,
        M = 1 << 5,
    }
}
