﻿using System;
using System.Device.Location;
using System.Linq;
using POGOProtos.Data.Player;
using System.IO;
using Newtonsoft.Json;
using static POGOLib.Pokemon.Inventory;

namespace POGOLib.Pokemon
{
    public class Player
    {

        internal Player(GeoCoordinate coordinate)
        {
            Coordinate = coordinate;

            Inventory = new Inventory(this);
            Inventory.ItemChanged += InventoryOnItemChanged;
        }

        /// <summary>
        /// Gets the <see cref="GeoCoordinate"/> of the <see cref="Player"/>.
        /// Used for internal calculations.
        /// </summary>
        internal GeoCoordinate Coordinate { get; private set; }

        /// <summary>
        /// Gets the <see cref="Player"/> his current latitude.
        /// </summary>
        public double Latitude => Coordinate.Latitude;

        /// <summary>
        /// Gets the <see cref="Player"/> his current longitude.
        /// </summary>
        public double Longitude => Coordinate.Longitude;

        /// <summary>
        /// Gets the <see cref="Inventory"/> of the <see cref="Player"/>
        /// </summary>
        public Inventory Inventory { get; }

        /// <summary>
        /// Gets the <see cref="Stats"/> of the beautiful <see cref="Inventory"/> object by PokémonGo.
        /// </summary>
        public PlayerStats Stats { get; private set; }

        /// <summary>
        /// Sets the <see cref="GeoCoordinate"/> of the <see cref="Player"/>.
        /// </summary>
        /// <param name="latitude">The latitude of your location.</param>
        /// <param name="longitude">The longitutde of your location.</param>
        /// <param name="altitude">The altitude of your location.</param>
        public void SetCoordinates(double latitude, double longitude, double altitude = 100)
        {
            Coordinate = new GeoCoordinate(latitude, longitude, altitude);

            var fileName = Path.Combine(Environment.CurrentDirectory, "cache", $"location.json");

            File.WriteAllText(fileName, JsonConvert.SerializeObject(Coordinate, Formatting.Indented));
        }
        
        /// <summary>
        /// Calculates the distance between the <see cref="Player"/> his current <see cref="Coordinate"/> and the given coordinate.
        /// </summary>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        /// <returns>Returns distance in meters.</returns>
        public double DistanceTo(double latitude, double longitude)
        {
            return Coordinate.GetDistanceTo(new GeoCoordinate(latitude, longitude));
        }

        private void InventoryOnItemChanged(object sender, InventoryItemChangedEventArgs eventArgs)
        {
            if (eventArgs.type == InventoryItemType.PlayerStats)
            {
                Stats = eventArgs.delta.InventoryItemData.PlayerStats;
            }
        }

    }
}
