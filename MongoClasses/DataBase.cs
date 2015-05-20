using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using GeoJSON;
using GeoJSON.Net.Geometry;
using GeoJSON.Net.Converters;
using MongoDB.Driver.GeoJsonObjectModel;

namespace Classes
{
    public class DataBase
    {
        public string connectionString;
        public MongoClient client;
        public MongoServer server;
        public MongoDatabase database;
        public MongoCollection<Entity> collection;

        public DataBase() // A DataBase constructor which sets the main parameters to connect to MongoDB
        {
            this.connectionString = "mongodb://127.0.0.1";
            this.client = new MongoClient(connectionString);
            this.server = client.GetServer();
            this.database = server.GetDatabase("DatosAereos");
            this.collection = database.GetCollection<Entity>("log");
        }

        public void AddLog(String address, double value, double latitude, double longitude) // Adds a single log to the DB
        {
            // We initialize the database
            DataBase db = new DataBase();

            // This builds a new instance
            Entity e = new Entity();
            e.Address = address;
            e.Value = value;
            e.Position = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(longitude, latitude));
            e.Time = DateTime.Now.AddHours(1);
            e.SearchTime = (e.Time.Hour - 1) * 3600 + e.Time.Minute * 60 + e.Time.Second;

            // We insert the entity into the DB's collection
            db.collection.Insert(e);
            var id = e.Id;
        }

        public void AddLogs(List<Entity> LogList) // Adds multiple logs to the DB
        {
            // We initialize the database
            DataBase db = new DataBase();

            // This loops through the list entered so that every Entity is added
            for (int i = 0; i < LogList.Count; i++)
            {
                // This builds a new instance
                Entity e = new Entity();
                e.Address = LogList.ElementAt(i).Address;
                e.Value = LogList.ElementAt(i).Value;
                e.Position = LogList.ElementAt(i).Position;
                e.Time = LogList.ElementAt(i).Time;

                // This adds it to MongoDB
                db.collection.Insert(e);
                var id = e.Id;
            }
        }

        public List<Entity> SearchByTime(double LowTime, double HighTime)
        {
            // We initialize the database
            DataBase db = new DataBase();

            // This creates the query to search for addresses within the desired time interval
            var query = Query.And(Query.GTE("SearchTime", LowTime), Query.LTE("SearchTime", HighTime));
            var resultsCursor = db.collection.Find(query).SetSortOrder("SearchTime");

            // This sends the results to a list
            var results = resultsCursor.ToList();

            // This returns the list
            return results;

        }

        public List<Entity> SearchByAddress(string address)
        {
            // We initialize the database
            DataBase db = new DataBase();

            // This creates the query to search for the log(s) with the desired address
            var query = Query.EQ("Address", address);
            var resultsCursor = db.collection.Find(query).SetSortOrder("Value");

            // This sends the results to a list
            var results = resultsCursor.ToList();

            // This returns the list
            return results;
        }

        public void RemoveAll()
        {
            // We initialize the database
            DataBase db = new DataBase();

            // This deletes all the content inside the collection
            db.collection.RemoveAll();
        }

        public List<Entity> NearQuery(double Lat, double Long)
        {
            // We initialize the database
            DataBase db = new DataBase();

            double distance = 1000;
            var g = new GeoJson2DGeographicCoordinates(Long, Lat);

            var query = Query.Near<GeoJson2DGeographicCoordinates>("Position", new GeoJsonPoint<GeoJson2DGeographicCoordinates>(g), distance);
            var resultsCursor = db.collection.Find(query);

            // This sends the results to a list
            var results = resultsCursor.ToList();
            return results;
        }

        public List<Entity> SearchImageByTime(int option, int lowtime, int hightime)
        {
            // We initialize the database
            DataBase db = new DataBase();

            // Depending on the option chosen, we will have three cases:

            // 1. User wants all images created later than a specified time
            if (option == 1)
            {
                var query = Query.GTE("ImageName", lowtime);
                var resultsCursor = db.collection.Find(query).SetSortOrder("ImageName");

                // This sends the results to a list
                var results = resultsCursor.ToList();

                // This returns the list
                return results;
            }

            // 2. User wants all images prior to a specified time
            else if (option == 2)
            {
                var query = Query.LTE("ImageName", hightime);
                var resultsCursor = db.collection.Find(query).SetSortOrder("ImageName");

                // This sends the results to a list
                var results = resultsCursor.ToList();

                // This returns the list
                return results;
            }

            // 3. User wants all images created between two times
            else
            {
                var query = Query.And(Query.GTE("ImageName", lowtime), Query.LTE("ImageName", hightime));
                var resultsCursor = db.collection.Find(query).SetSortOrder("ImageName");

                // This sends the results to a list
                var results = resultsCursor.ToList();

                // This returns the list
                return results;
            }
        }

    }
}
