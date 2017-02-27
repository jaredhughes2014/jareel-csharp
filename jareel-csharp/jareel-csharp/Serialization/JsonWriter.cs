using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jareel
{
    /// <summary>
    /// Static wrapper class for converting a state data dictionary
    /// into a JSON string
    /// </summary>
    public static class JsonWriter
    {
        #region Constants

        /// <summary>
        /// Character denoting a string literal
        /// </summary>
        private const string STRING_LITERAL_FORMAT = "\"{0}\"";

        /// <summary>
        /// Character denoting a property assignment
        /// </summary>
        private const string OBJ_PROP = "{0}:{1}";

        /// <summary>
        /// Character denoting the beginning of an array
        /// </summary>
        private const char ARR_START = '[';

        /// <summary>
        /// Character denoting the end of an array
        /// </summary>
        private const char ARR_END = ']';

        /// <summary>
        /// Character denoting the start of an object
        /// </summary>
        private const char OBJ_START = '{';

        /// <summary>
        /// Character denoting the end of an object
        /// </summary>
        private const char OBJ_END = '}';

        /// <summary>
        /// JSON format for true
        /// </summary>
        private const string JSON_TRUE = "true";

        /// <summary>
        /// JSON format for false
        /// </summary>
        private const string JSON_FALSE = "false";

        #endregion

        #region Top-level

        /// <summary>
        /// Determines what the type of object wrapper is, then serializes that
        /// object into JSON. The object should be any one of the following types:
        /// 
        /// integer
        /// floating point number
        /// boolean
        /// string
        /// Dictionary with string keys and JSON serializable values
        /// Array or list of any of the previous types
        /// </summary>
        /// <param name="wrapper">Object to be written to JSON</param>
        /// <returns>JSON string representing the provided object</returns>
        public static string Write(object wrapper)
        {
            if (wrapper == null) {
                return "null";
            }
            else if (wrapper is Dictionary<string, object>) {
                return WriteObject((Dictionary<string, object>)wrapper);
            }
            else if (wrapper is string) {
                return WriteStringLiteral(wrapper.ToString());
            }
            else if (wrapper is int) {
                return ((int)wrapper).ToString();
            }
            else if (wrapper is float) {
                return WriteFloat((float)wrapper);
            }
            else if (wrapper is bool) {
                return WriteBoolean((bool)wrapper);
            }
            else if (wrapper is IEnumerable) {
                return WriteArray(((IEnumerable)wrapper).Cast<object>().ToArray());
            }
            throw new ArgumentException(string.Format("Cannot serialize type {0}", wrapper.GetType().Name));
        }

        #endregion

        #region Arrays

        /// <summary>
        /// Writes an array of JSON strings into a single JSON array string. The strings
        /// provided should be JSON data that has already been serialized
        /// </summary>
        /// <param name="array">Array of serialized JSON strings</param>
        /// <returns>Single string representing an array containing the given JSON items</returns>
        private static string WriteArray(object[] array)
        {
            string items = string.Format(GenerateFormatSeries(array.Length), array.Select(p => Write(p)).ToArray());
            return "[" + items + ']';
        }

        #endregion

        #region Objects

        /// <summary>
        /// Writes an object property. This will appear as a name, followed by a colon,
        /// followed by a value.
        /// </summary>
        /// <param name="name">The name of the property</param>
        /// <param name="value">The value of the property</param>
        /// <returns>String representing a JSON object property</returns>
        private static string WriteObjectProperty(string name, object value)
        {
            return string.Format(OBJ_PROP, WriteStringLiteral(name), Write(value));
        }

        /// <summary>
        /// Generates a format string which allows string.Format to insert
        /// an array of strings in a JSON friendly series. This is used for arrays and
        /// object properties
        /// </summary>
        /// <param name="propCount">The number of items to create a format token for</param>
        /// <returns>Format string used to format a list of items</returns>
        private static string GenerateFormatSeries(int propCount)
        {
            StringBuilder sb = new StringBuilder();

            if (propCount > 0) {
                for (int i = 0; i < propCount - 1; ++i) {
                    sb.Append('{').Append(i.ToString()).Append('}').Append(',');
                }

                sb.Append('{').Append((propCount - 1).ToString()).Append('}');
            }
            return sb.ToString();
        }

        /// <summary>
        /// Converts a dictionary mapping strings to objects into a single string representing
        /// a JSON object. All objects that are values in the dictionary should also be serializable
        /// to JSON
        /// </summary>
        /// <param name="wrapper">Dictionary which maps strings to objects</param>
        /// <returns>String representing a JSON object</returns>
        private static string WriteObject(Dictionary<string, object> wrapper)
        {
            var props = wrapper.Select(p => WriteObjectProperty(p.Key, p.Value)).ToArray();
            string propSeries = string.Format(GenerateFormatSeries(props.Length), props);

            return "{" + propSeries + '}';
        }

        #endregion

        #region Primitive Writers

        /// <summary>
        /// Writes a string inside a JSON string literal e.g. "String"
        /// </summary>
        /// <param name="str">The string to convert to a JSON string literal</param>
        /// <returns>The same string provided inside JSON double quotes</returns>
        private static string WriteStringLiteral(string str)
        {
            return string.Format(STRING_LITERAL_FORMAT, str);
        }

        /// <summary>
        /// Writes a string version of the given boolean value.
        /// </summary>
        /// <param name="boolean">A boolean value</param>
        /// <returns>Text equivalent of a true or false value</returns>
        private static string WriteBoolean(bool boolean)
        {
            return (boolean) ? JSON_TRUE : JSON_FALSE;
        }

        /// <summary>
        /// Writes a floating point number out to two decimal places
        /// </summary>
        /// <param name="value">The floating point number to write</param>
        /// <returns>String equivalent of the numerical value to two decimal places</returns>
        private static string WriteFloat(float value)
        {
            return value.ToString("n2");
        }

        #endregion
    }
}
