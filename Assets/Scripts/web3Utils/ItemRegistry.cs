using System.Collections.Generic;
using UnityEngine;

namespace Bakeland
{
    /// Stores and retrieves metadata for collectible inventory items.
    public static class ItemRegistry
    {
        /// Represents a game inventory item.
        public class ItemData
        {
            public string Name { get; private set; }
            public int Id { get; private set; } /// @dev - This id should be passed in the api calls

            public ItemData(int id, string name)
            {
                Id = id;
                Name = name;
            }
        }

        /// Registered items mapped by ID
        private static readonly Dictionary<int, ItemData> itemsById = new()
        {
            { 1, new ItemData(1, "Amulet of Resurrection") },
            { 2, new ItemData(2, "Beranade") },
            { 3, new ItemData(3, "Fire Bud") },
            { 4, new ItemData(4, "Gold Kush") },
            { 5, new ItemData(5, "Honey Pot") },
            { 6, new ItemData(6, "Honeycomb") },
            { 7, new ItemData(7, "Mugwort") },
            { 8, new ItemData(8, "Purple Haze") },
            { 9, new ItemData(9, "Beradrome Pass") },
            { 10, new ItemData(10, "Speed Elixir") },
            { 11, new ItemData(11, "Weed") },
            { 12, new ItemData(12, "Grow Up Fertilizer") },
            { 13, new ItemData(13, "Quality Fertilizer") },
            { 14, new ItemData(14, "Ramen") },
            { 16, new ItemData(16, "Fire Bud Seeds") },
            { 18, new ItemData(18, "Mugwort Seeds") },
            { 19, new ItemData(19, "Purple Haze Seeds") },
            { 21, new ItemData(21, "Hoe") },
            { 22, new ItemData(22, "Bike") },
            { 23, new ItemData(23, "Chainsaw") },
            { 24, new ItemData(24, "Blue Wires") },
            { 25, new ItemData(25, "Red Wires") },
            { 26, new ItemData(26, "Yellow Wires") },
            { 27, new ItemData(27, "Wheat") },
            { 28, new ItemData(28, "Beranade Seeds") },
            { 29, new ItemData(29, "Gold Kush Seeds") },
            { 30, new ItemData(30, "Lettuce Seeds") },
            { 31, new ItemData(31, "Carrot Seeds") },
            { 32, new ItemData(32, "Carrot") },
            { 33, new ItemData(33, "Lettuce") },
            { 34, new ItemData(34, "Pumpkin") },
            { 36, new ItemData(36, "Strawberry") },
            { 37, new ItemData(37, "Water can") },
        };


        /// Name-based lookup for reverse access
        private static readonly Dictionary<string, ItemData> itemsByName = new Dictionary<string, ItemData>();

        static ItemRegistry()
        {
            foreach (var item in itemsById.Values)
            {
                itemsByName[item.Name.ToLowerInvariant()] = item;
            }
        }

        /// Get item data by ID.
        public static ItemData GetItemById(int id)
        {
            if (itemsById.TryGetValue(id, out var item))
                return item;

            Debug.LogWarning($"Item ID '{id}' not found.");
            return null;
        }

        /// Get item data by name (case-insensitive).
        public static ItemData GetItemByName(string name)
        {
            name = name.ToLowerInvariant();
            if (itemsByName.TryGetValue(name, out var item))
                return item;

            Debug.LogWarning($"Item name '{name}' not found.");
            return null;
        }

        /// Returns all item definitions.
        public static List<ItemData> GetAllItems()
        {
            return new List<ItemData>(itemsById.Values);
        }

        /// Check if item exists by ID.
        public static bool IsValidItemId(int id) => itemsById.ContainsKey(id);
    }
}
