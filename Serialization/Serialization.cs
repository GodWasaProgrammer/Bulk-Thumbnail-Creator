using Bulk_Thumbnail_Creator.PictureObjects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using YoutubeDLSharp;

namespace Bulk_Thumbnail_Creator.Serialization
{
    public class Serialization
    {
        // Serialize PictureData object to XML
        public static void SerializePictureData(TextWriter writer, PictureData pictureData)
        {
            try
            {
                XmlSerializer serializer = new(typeof(PictureData));
                serializer.Serialize(writer, pictureData);

                // Serialize ParamForTextCreation dictionary using your custom serializer
                DictionarySerializer.Serialize(writer, pictureData.ParamForTextCreation.BoxesProxy);
            }
            catch (Exception ex)
            {
                // Handle the exception, log it, or rethrow it as needed
                Console.WriteLine("Error during serialization: " + ex.Message);
            }
        }

        // Deserialize PictureData object from XML
        public static PictureData DeserializePictureData(TextReader reader)
        {
            try
            {
                XmlSerializer serializer = new(typeof(PictureData));
                PictureData pictureData = (PictureData)serializer.Deserialize(reader);

                // Deserialize ParamForTextCreation dictionary using your custom serializer
                Dictionary<string, Rectangle> boxes = new();
                DictionarySerializer.Deserialize(reader, pictureData.ParamForTextCreation.BoxesProxy);

                // Rest of your deserialization code

                return pictureData;
            }
            catch (Exception ex)
            {
                // Handle the exception, log it, or rethrow it as needed
                Console.WriteLine("Error during deserialization: " + ex.Message);
                return null; // You may want to return null or throw a custom exception
            }
        }

        static readonly XmlSerializer serializer = new(typeof(List<string>));

        /// <summary>
        /// General DeSerializer for List of Strings
        /// </summary>
        /// <param name="pathtoXMLToDeSerialize">The XML to be deserialized</param>
        /// <returns>The Deserialized List of Strings</returns>
        public static List<string> DeSerializeXMLToListOfStrings(string pathtoXMLToDeSerialize)
        {
            List<string> ListofStringsToDeSerialize = BTCSettings.DownloadedVideosList;

            if (File.Exists(pathtoXMLToDeSerialize))
            {
                using FileStream file = File.OpenRead(pathtoXMLToDeSerialize);
                ListofStringsToDeSerialize = (List<string>)serializer.Deserialize(file);

            }
            return ListofStringsToDeSerialize;
        }

        /// <summary>
        ///  General Serializer for list of strings
        /// </summary>
        /// <param name="PathToXML">Your Path to the XML to be written to</param>
        /// <param name="ListOfStringsToSerialize">The List of Strings to Serialize</param>
        public static void SerializeListOfStringsToXML(string PathToXML, List<string> ListOfStringsToSerialize)
        {
            using FileStream file = File.Create(PathToXML);
            serializer.Serialize(file, ListOfStringsToSerialize);
        }

        
    }
}
