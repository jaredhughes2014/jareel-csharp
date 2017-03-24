
using System;
using System.Linq;

namespace Jareel
{
    /// <summary>
    /// Static library for reading and writing JSON
    /// </summary>
    public static class Json
    {
        /// <summary>
        /// Writes a JSON serializable object into a JSON string. This can write
        /// the following:
        /// 
        /// Numeric values (int/float)
        /// Strings
        /// Boolean values
        /// Dictionaries mapping strings to JSON serializable objects
        /// Enumerables of JSON serializable objects (always written to arrays)
        /// </summary>
        /// <param name="data">Valid object for JSON serialization</param>
        /// <returns>JSON string representing the given object</returns>
        public static string Write(object data)
        {
            return JsonWriter.Write(data);
        }

		/// <summary>
		/// Utility function to read a serialized JSON array and cast all read objects
		/// into the given T type. If the read objects are JSON objects or arrays, you may
		/// specify a casting function to convert the read objects into your desired type.
		/// 
		/// Objects will be Dictionaries of strings to objects and arrays will be arrays of objects
		/// </summary>
		/// <typeparam name="T">The type to cast the array elements to</typeparam>
		/// <param name="jsonArray">Serialized JSON array string</param>
		/// <param name="converter">Optional function used to convert parsed JSON data into your desired type</param>
		/// <returns>Array of cast elements from the deserialized JSON array</returns>
		public static T[] ReadArray<T>(string jsonArray, Func<object, T> converter = null)
		{
			if (converter == null) {
				converter = x => (T)x;
			}

			return ((object[])Read(jsonArray)).Select(p => converter(p)).ToArray();
		}

        /// <summary>
        /// Reads a JSON string and converts it into a deserialized value
        /// </summary>
        /// <param name="json">JSON string</param>
        /// <returns>Converted JSON Value</returns>
        public static object Read(string json)
        {
            return JsonReader.Read(json);
        }
    }
}
