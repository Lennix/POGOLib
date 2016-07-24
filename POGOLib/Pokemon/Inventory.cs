using System;
using Google.Protobuf.Collections;
using POGOProtos.Inventory;
using System.Linq;

namespace POGOLib.Pokemon
{
    /// <summary>
    /// A wrapper class for <see cref="Inventory"/> with helper methods.
    /// </summary>
    public class Inventory
    {       
        /// <summary>
        /// The <see cref="Player"/> of the <see cref="Inventory"/>.
        /// </summary>
        private readonly Player _player;

        /// <summary>
        /// Holds all <see cref="RepeatedField{InventoryItem}"/> of the Player
        /// </summary>
        private RepeatedField<InventoryItem> _inventoryItems;

        /// <summary>
        /// Holds the last received <see cref="RepeatedField{InventoryItem}"/> from PokémonGo.
        /// </summary>
        private RepeatedField<InventoryItem> _inventoryDelta;

        internal Inventory(Player player)
        {
            _player = player;
        }

        internal long LastInventoryTimestampMs;

        /// <summary>
        /// Contains the last received <see cref="RepeatedField{InventoryItem}"/> from PokémonGo.<br/>
        /// Only use this if you know what you are doing.
        /// </summary>
        public RepeatedField<InventoryItem> InventoryItems
        {
            get { return _inventoryItems; }
            internal set {
                _inventoryItems = value;
            }
        }

        /// <summary>
        /// Contains all items <see cref="RepeatedField{InventoryItem}"/> from PokémonGo.<br/>
        /// Only use this if you know what you are doing.
        /// </summary>
        public RepeatedField<InventoryItem> InventoryDelta
        {
            get { return _inventoryDelta; }
            internal set
            {
                _inventoryDelta = value;
                _player.Inventory.OnUpdate();
            }
        }

        internal void OnUpdate()
        {
            if (InventoryItems == null)
               InventoryItems = new RepeatedField<InventoryItem>();

            foreach (var deltaItem in InventoryDelta)
            {
                InventoryItem item = null;
                InventoryItemType type = InventoryItemType.PlayerStats;
                if (deltaItem.InventoryItemData.PlayerStats != null)
                {
                    item = InventoryItems.SingleOrDefault(x => x.InventoryItemData.PlayerStats != null);
                }
                else if (deltaItem.InventoryItemData.Item != null)
                {
                    item = InventoryItems.SingleOrDefault(x => x.InventoryItemData.Item != null && x.InventoryItemData.Item.ItemId == deltaItem.InventoryItemData.Item.ItemId);
                    type = InventoryItemType.Item;
                }
                else if (deltaItem.InventoryItemData.PokedexEntry != null)
                {
                    item = InventoryItems.SingleOrDefault(x => x.InventoryItemData.PokedexEntry != null && x.InventoryItemData.PokedexEntry.PokemonId == deltaItem.InventoryItemData.PokedexEntry.PokemonId);
                    type = InventoryItemType.PokedexEntry;
                }
                else if (deltaItem.InventoryItemData.PokemonData != null)
                {
                    item = InventoryItems.SingleOrDefault(x => x.InventoryItemData.PokemonData != null && x.InventoryItemData.PokemonData.Id == deltaItem.InventoryItemData.PokemonData.Id);
                    type = InventoryItemType.PokemonData;
                }
                else if (deltaItem.InventoryItemData.PokemonFamily != null)
                {
                    item = InventoryItems.SingleOrDefault(x => x.InventoryItemData.PokemonFamily != null && x.InventoryItemData.PokemonFamily.FamilyId == deltaItem.InventoryItemData.PokemonFamily.FamilyId);
                    type = InventoryItemType.PokemonFamily;
                }
                else if (deltaItem.InventoryItemData.PlayerCurrency != null)
                {
                    item = InventoryItems.SingleOrDefault(x => x.InventoryItemData.PlayerCurrency != null);
                    type = InventoryItemType.PlayerCurrency;
                }
                else if (deltaItem.InventoryItemData.EggIncubators != null)
                {
                    item = InventoryItems.SingleOrDefault(x => x.InventoryItemData.EggIncubators != null && x.InventoryItemData.EggIncubators.EggIncubator == deltaItem.InventoryItemData.EggIncubators.EggIncubator);
                    type = InventoryItemType.EggIncubators;
                }
                else if (deltaItem.InventoryItemData.AppliedItems != null)
                {
                    item = InventoryItems.SingleOrDefault(x => x.InventoryItemData.AppliedItems != null && x.InventoryItemData.AppliedItems.Item == deltaItem.InventoryItemData.AppliedItems.Item);
                    type = InventoryItemType.AppliedItems;
                }
                else
                    continue; // Not Yet Implemented

                ItemChanged?.Invoke(this, new InventoryItemChangedEventArgs(item, deltaItem, type));

                if (item == null)
                    InventoryItems.Add(deltaItem);
                else
                {
                    item.InventoryItemData = deltaItem.InventoryItemData;
                }
            }

            Update?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler<InventoryItemChangedEventArgs> ItemChanged;
        public event EventHandler<EventArgs> Update;

        public class InventoryItemChangedEventArgs : EventArgs
        {
            public InventoryItemChangedEventArgs(InventoryItem _old, InventoryItem _delta, InventoryItemType _type)
            {
                old = _old;
                delta = _delta;
                type = _type;
            }

            public InventoryItem old;
            public InventoryItem delta;
            public InventoryItemType type;
        }

        public enum InventoryItemType
        {
            PlayerStats,
            Item,
            PokedexEntry,
            PokemonData,
            PokemonFamily,
            PlayerCurrency,
            EggIncubators,
            AppliedItems,
        }
    }
}
