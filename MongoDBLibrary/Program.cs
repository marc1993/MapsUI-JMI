using System;
using System.Collections.Generic;
using System.Linq;
using Classes;
using GeoJSON;
using GeoJSON.Net.Geometry;
using GeoJSON.Net.Converters;
using MongoDB.Driver.GeoJsonObjectModel;

namespace MongoDBLibrary
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Type the number corresponding to the option you are willing to use:");
            Console.WriteLine("Type 1 to use AddLog.");
            Console.WriteLine("Type 2 to use AddLogs.");
            Console.WriteLine("Type 3 to research by Time.");
            Console.WriteLine("Type 4 to research by Name.");
            Console.WriteLine("Type 5 to remove all data stored");
            Console.WriteLine("Type 6 to check proximity");

            int choice = Convert.ToInt32(Console.ReadLine());

            if (choice == 1)
            {
                Console.WriteLine("Type an address.");
                string address = Console.ReadLine();
                Console.WriteLine("Type a value.");
                double value = Convert.ToDouble(Console.ReadLine());
                Console.WriteLine("Enter the latitude.");
                double lat = Convert.ToDouble(Console.ReadLine());
                Console.WriteLine("Enter the longitude.");
                double lon = Convert.ToDouble(Console.ReadLine());

                // This creates the database and executes the AddLog function
                DataBase db = new DataBase();
                db.AddLog(address, value, lat, lon);
            }
            else if (choice == 2)
            {
                List<Entity> LogList = new List<Entity>();

                int confirmation = 1;

                Console.WriteLine("Add as much paired addresses and values as you want.");

                // This keeps asking the user for address and value until he wants to stop
                while (confirmation != 0)
                {
                    Entity e = new Entity();
                    Console.WriteLine("Type an address.");
                    string address = Console.ReadLine();
                    e.Address = address;
                    Console.WriteLine("Type a value.");
                    double value = Convert.ToDouble(Console.ReadLine());
                    e.Value = value;
                    Console.WriteLine("Enter the latitude.");
                    double lat = Convert.ToDouble(Console.ReadLine());
                    Console.WriteLine("Enter the longitude.");
                    double lon = Convert.ToDouble(Console.ReadLine());
                    e.Position = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(lon, lat));
                    e.Time = DateTime.Now.AddHours(1);
                    e.SearchTime = (e.Time.Hour - 1) * 3600 + e.Time.Minute * 60 + e.Time.Second;
                    e.ImageName = (e.Time.Day * 1000000000000) + (e.Time.Month * 10000000000) + (e.Time.Year * 1000000) + ((e.Time.Hour - 2) * 10000) + (e.Time.Minute * 100) + (e.Time.Second);
                    LogList.Add(e);

                    Console.WriteLine("Do you wish to continue? (Y=1/N=0)");
                    confirmation = Convert.ToInt32(Console.ReadLine());
                }

                // This creates the database and executes the AddLogs function
                DataBase db = new DataBase();
                db.AddLogs(LogList);
                Console.WriteLine("The logs have been added to the DataBase");

            }

            else if (choice == 3)
            {
                Console.WriteLine("Type the lower time limit, in the format of: hour/minute/second");
                string low = Convert.ToString(Console.ReadLine());

                string[] lowsum = low.Split('/');

                int lowlimit = Convert.ToInt32(lowsum[0]) * 3600 + Convert.ToInt32(lowsum[1]) * 60 + Convert.ToInt32(lowsum[2]);

                Console.WriteLine("Type the higher value limit, in the format of: hour/minute/second");
                string high = Convert.ToString(Console.ReadLine());

                string[] highsum = high.Split('/');

                int highlimit = Convert.ToInt32(highsum[0]) * 3600 + Convert.ToInt32(highsum[1]) * 60 + Convert.ToInt32(highsum[2]);

                // This creates the database and executes the SearchByTime function
                DataBase db = new DataBase();
                List<Entity> LogList = db.SearchByTime(lowlimit, highlimit);

                int count = LogList.Count;

                // This prints the results of the function in the console
                for (int i = 0; i < count; i++)
                {
                    Entity e = new Entity();
                    e.Address = LogList.ElementAt(i).Address;
                    e.Value = LogList.ElementAt(i).Value;
                    e.Position = LogList.ElementAt(i).Position;
                    e.Time = LogList.ElementAt(i).Time;
                    e.SearchTime = LogList.ElementAt(i).SearchTime;


                    string print1 = Convert.ToString(e.Address);
                    string print2 = Convert.ToString(e.Value);
                    string print3 = Convert.ToString(e.Position.Coordinates.Latitude);
                    string print4 = Convert.ToString(e.Position.Coordinates.Longitude);
                    string print5 = Convert.ToString(e.Time);
                    string print6 = Convert.ToString(e.SearchTime);

                    Console.WriteLine("Address: {0}, Value: {1}, Position: Latitude: {2} Longitude: {3}, Time: {4}, SearchTime: {5}", print1, print2, print3, print4, print5, print6);
                }
            }

            else if (choice == 4)
            {
                Console.WriteLine("Type the name you wish to search for");
                string name = Convert.ToString(Console.ReadLine());

                // This creates the database and executes the SearchByAddress function
                DataBase db = new DataBase();
                List<Entity> LogList = db.SearchByAddress(name);

                int count = LogList.Count;

                // This prints the results of the function in the console
                for (int i = 0; i < count; i++)
                {
                    Entity e = new Entity();
                    e.Address = LogList.ElementAt(i).Address;
                    e.Value = LogList.ElementAt(i).Value;
                    e.Position = LogList.ElementAt(i).Position;
                    e.Time = LogList.ElementAt(i).Time;
                    e.SearchTime = LogList.ElementAt(i).SearchTime;

                    string print1 = Convert.ToString(e.Address);
                    string print2 = Convert.ToString(e.Value);
                    string print3 = Convert.ToString(e.Position);
                    string print4 = Convert.ToString(e.Time);
                    string print5 = Convert.ToString(e.SearchTime);

                    Console.WriteLine("Address: {0}, Value: {1}, Position: {2}, Time: {3}, SearchTime: {4}", print1, print2, print3, print4, print5);
                }
            }

            else if (choice == 5)
            {
                // This creates the database and executes the RemoveAll function
                DataBase db = new DataBase();
                db.RemoveAll();
            }

            else if (choice == 6)
            {
                // The user enters the point to be checked for proximity
                Console.WriteLine("Insert the point you want to be checked:");
                Console.WriteLine("Enter its latitude");
                double latpoint = Convert.ToDouble(Console.ReadLine());
                Console.WriteLine("Enter its longitude");
                double longpoint = Convert.ToDouble(Console.ReadLine());

                // A database is initialized and the function to check the MongoDB is executed
                DataBase db = new DataBase();
                List<Entity> NearList = db.NearQuery(latpoint, longpoint);

                int count = NearList.Count;

                // This prints the results of the function in the console
                for (int i = 0; i < count; i++)
                {
                    Entity e = new Entity();
                    e.Address = NearList.ElementAt(i).Address;
                    e.Value = NearList.ElementAt(i).Value;
                    e.Position = NearList.ElementAt(i).Position;
                    e.Time = NearList.ElementAt(i).Time;
                    e.SearchTime = NearList.ElementAt(i).SearchTime;

                    string print1 = Convert.ToString(e.Address);
                    string print2 = Convert.ToString(e.Value);
                    string print3 = Convert.ToString(e.Position.Coordinates.Latitude);
                    string print4 = Convert.ToString(e.Position.Coordinates.Longitude);
                    string print5 = Convert.ToString(e.Time);
                    string print6 = Convert.ToString(e.SearchTime);

                    Console.WriteLine("The points within its established vicinity are:");
                    Console.WriteLine("Address: {0}, Value: {1}, Position: Latitude: {2} Longitude: {3}, Time: {4}, SearchTime: {5}", print1, print2, print3, print4, print5, print6);
                }
            }
        }
    }
}