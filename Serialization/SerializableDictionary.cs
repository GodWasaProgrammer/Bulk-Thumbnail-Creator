using System.Collections.Generic;
using System.Xml.Serialization;

namespace Bulk_Thumbnail_Creator.Serialization
{

    public class SerializableDictionary<TKey, TValue>
    {
        [XmlElement("Entry")]
        public List<Entry> Entries { get; set; } = new List<Entry>();

        public void Add(TKey key, TValue value)
        {
            Entries.Add(new Entry(key, value));
        }

        public void Clear()
        {
            Entries.Clear();
        }

        public List<Entry> GetEntries()
        {
            return Entries;
        }

        // Entry class for serialization
        public class Entry
        {
            public TKey Key { get; set; }
            public TValue Value { get; set; }

            public Entry()
            {
            }

            public Entry(TKey key, TValue value)
            {
                Key = key;
                Value = value;
            }
        }

    }

}