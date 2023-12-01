﻿using Bulk_Thumbnail_Creator.PictureObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Bulk_Thumbnail_Creator.Serialization
{
    public class Serializing
    {
        //Serialize PictureData object to XML
        public static void SerializePictureData(TextWriter writer, PictureData pictureData)
        {
            try
            {
                XmlSerializer serializer = new(typeof(PictureData));
                serializer.Serialize(writer, pictureData);

                //// Serialize ParamForTextCreation dictionary
                //DictionarySerializer.Serialize(writer, pictureData.ParamForTextCreation.BoxesProxy);
            }
            catch (Exception ex)
            {
                // Handle the exception, log it
                Settings.LogService.LogError("Error during serialization: " + ex.Message);
            }

        }

        // Deserialize PictureData object from XML
        public static PictureData DeserializePictureData(TextReader reader)
        {
            try
            {
                XmlSerializer serializer = new(typeof(PictureData));
                PictureData pictureData = (PictureData)serializer.Deserialize(reader);

                return pictureData;
            }
            catch (Exception ex)
            {
                // Handle the exception, log it,
                Settings.LogService.LogError("Error during deserialization: " + ex.Message);
                return null; // return null or throw a custom exception
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
            List<string> ListofStringsToDeSerialize = Settings.DownloadedVideosList;

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
