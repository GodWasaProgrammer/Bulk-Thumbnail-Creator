using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Xml.Serialization;
using Bulk_Thumbnail_Creator.Enums;
using System.Drawing;

namespace Bulk_Thumbnail_Creator.Serialization
{
    public static class DictionarySerializer
    {
        public static void Serialize(TextWriter writer, SerializableDictionary<Box,Rectangle> dictionary)
        {
            XmlSerializer serializer = new(typeof(SerializableDictionary<Box, Rectangle>));
            serializer.Serialize(writer, dictionary);
        }

        public static void Deserialize(TextReader reader, SerializableDictionary<Box, Rectangle> dictionary)
        {
            XmlSerializer serializer = new(typeof(SerializableDictionary<Box, Rectangle>));
            SerializableDictionary<Box, Rectangle> deserializedDictionary = (SerializableDictionary<Box, Rectangle>)serializer.Deserialize(reader);

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
