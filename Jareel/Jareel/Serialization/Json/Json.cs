
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
