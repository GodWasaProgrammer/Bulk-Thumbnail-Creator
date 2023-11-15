using System.IO;
using System.Xml.Serialization;
using Bulk_Thumbnail_Creator.Enums;
using System.Drawing;

namespace Bulk_Thumbnail_Creator.Serialization
{
    public static class DictionarySerializer
    {
        public static void Serialize(TextWriter writer, SerializableDictionary<BoxType,Rectangle> dictionary)
        {
            XmlSerializer serializer = new(typeof(SerializableDictionary<BoxType, Rectangle>));
            serializer.Serialize(writer, dictionary);
        }

        public static void Deserialize(TextReader reader, SerializableDictionary<BoxType, Rectangle> dictionary)
        {
            XmlSerializer serializer = new(typeof(SerializableDictionary<BoxType, Rectangle>));
            SerializableDictionary<BoxType, Rectangle> deserializedDictionary = (SerializableDictionary<BoxType, Rectangle>)serializer.Deserialize(reader);

            dictionary.Entries.Clear();
            foreach (var entry in deserializedDictionary.Entries)
            {
                dictionary.Entries.Add(entry);
            }
        }

    }
        public class Entry
        {
            public object Key;
            public object Value;
            public Entry()
            {

            }

            public Entry(object key, object value)
            {
                Key = key;
                Value = value;
            }

        }

    }