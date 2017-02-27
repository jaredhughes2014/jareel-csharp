using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jareel
{
    /// <summary>
    /// Static JSON reading library
    /// </summary>
    public static class JsonReader
    {
        /// <summary>
        /// Serializes a JSON string into an object. The object returned
        /// is based on the provided JSON string:
        /// 
        /// Integer string: int
        /// Decimal string: float
        /// JSON string literal: string
        /// "true" or "false": bool
        /// JSON object: Dictionary string->object
        /// JSON array: List of read JSON data (each element is read with this function)
        /// 
        /// This will throw an argument exception if the JSON string is not properly formatted
        /// 
        /// </summary>
        /// <param name="json"></param>
        /// <returns>Deserialized object</returns>
        public static object Read(string json)
        {
            return ParseFromIterator(new StringIterator(json));
        }

        /// <summary>
        /// Parses a JSON data item from the current position in the string iterator.
        /// The iterator will end directly after the next token
        /// </summary>
        /// <param name="iter">Iterator used to parse JSON data</param>
        /// <returns>Data parsed from the iterator</returns>
        private static object ParseFromIterator(StringIterator iter)
        {
            if (iter.Current == '{' || iter.Current == 'n') {
                return ParseObject(iter);
            }
            else if (iter.Current == '[') {
                return ParseArray(iter);
            }
            else if (IsNumeric(iter.Current)) {
                return ParseNumeric(iter);
            }
            else if (iter.Current == '\"') {
                return ParseString(iter);
            }
            else if (iter.Current == 't' && SequenceMatches(iter, "true")) {
                return true;
            }
            else if (iter.Current == 'f' && SequenceMatches(iter, "false")) {
                return false;
            }

            throw new ArgumentException("Invalid JSON");
        }

        /// <summary>
        /// Parses a numeric string literal into a boxed numeric value
        /// </summary>
        /// <param name="iter">Iterator for the top-level string</param>
        /// <returns>Boxed numeric value</returns>
        private static object ParseNumeric(StringIterator iter)
        {
            StringBuilder sb = new StringBuilder();

            bool dec = false;
            while (!iter.IsFinished && IsNumeric(iter.Current)) {

                dec = dec || iter.Current == '.';
                sb.Append(iter.Next);
            }

            if (dec) {
                return float.Parse(sb.ToString());
            }
            else {
                return int.Parse(sb.ToString());
            }
        }

        /// <summary>
        /// Parses a string literal into an actual string from the given iterator. This
        /// will insure the iterator starts immediately after the string literal
        /// </summary>
        /// <param name="iter">Iterator for the JSON token</param>
        /// <returns>Parsed string</returns>
        private static object ParseString(StringIterator iter)
        {            
            StringBuilder sb = new StringBuilder();
            if (iter.Next == '\"') {

                while (!iter.IsFinished && iter.Current != '\"') {
                    sb.Append(iter.Next);
                }

                if (iter.Next == '\"') {
                    return sb.ToString();
                }
            }
            throw new ArgumentException("Invalid string format");
        }

        /// <summary>
        /// Parses an array string into an array of parsed JSON data
        /// </summary>
        /// <param name="iter">Iterator for the JSON string</param>
        /// <returns>Array where each element is a parsed JSON</returns>
        private static object ParseArray(StringIterator iter)
        {
            if (iter.Next == '[') {

                var objs = new List<object>();
                while (!iter.IsFinished && iter.Current != ']') {
                    objs.Add(TrimItem(iter));

                    if (iter.Current != ']' && iter.Next != ',') {
                        throw new ArgumentException("Array element not separated by comma");
                    }
                }
                if (iter.Next == ']') return objs.ToArray();
            }
            throw new ArgumentException("Invalid array format");
        }

        /// <summary>
        /// Parses a serialized JSON object into a Dictionary mapping strings
        /// to parsed JSON data
        /// </summary>
        /// <param name="iter">Iterator for the JSON data</param>
        /// <returns>Dictionary representing the parsed object</returns>
        private static object ParseObject(StringIterator iter)
        {
            if (iter.Current == '{') {
                iter.Advance();
                var obj = new Dictionary<string, object>();

                do {
                    if (iter.Current != '}') {
                        string key = (string)TrimItem(iter);

                        if (iter.Next != ':') {
                            throw new ArgumentException("Missing property value identifier in object literal");
                        }

                        object val = TrimItem(iter);
                        obj.Add(key, val);
                    }
                    else break;

                } while (!iter.IsFinished && iter.Current != '}' && iter.Next == ',');

                if (iter.Next == '}') return obj;
            }
            else if (iter.Current == 'n') {
                if (SequenceMatches(iter, "null")) {
                    return null;
                }
            }
            throw new ArgumentException("Improperly formatted object literal");
        }

        /// <summary>
        /// Advance the iterator until it is past any whitespace characters.
        /// Will not advance the iterator if it is not currently on a whitespace
        /// </summary>
        /// <param name="iter">Iterator which will be moved past any whitespace</param>
        private static void TrimWhitespace(StringIterator iter)
        {
            do {
                if (!IsWhitespace(iter.Current)) return;
            } while (IsWhitespace(iter.Next));
        }

        /// <summary>
        /// Trims all whitespace around the next token and returns the parsed item. The
        /// iterator will end after the last whitespace character after the token
        /// </summary>
        /// <param name="iter">Iterator used to parse JSON data</param>
        /// <returns>The data parsed from the next token, with all whitespace trimmed in front and behind it</returns>
        private static object TrimItem(StringIterator iter)
        {
            TrimWhitespace(iter);
            object val = ParseFromIterator(iter);
            TrimWhitespace(iter);

            return val;
        }

        /// <summary>
        /// Returns true if the iterator matches the given pattern at its current position. This
        /// will advance the iterator forward equal to the length of the pattern
        /// </summary>
        /// <param name="iter">JSON string iterator</param>
        /// <param name="pattern">The pattern being checked for a match</param>
        /// <returns>True if the next characters in the iterator matches the given pattern</returns>
        private static bool SequenceMatches(StringIterator iter, string pattern)
        {
            return pattern.ToCharArray().All(p => !iter.IsFinished && iter.Next == p);
        }

        /// <summary>
        /// Helper function to see if a character is part of a numeric string
        /// </summary>
        /// <param name="c">The character to test</param>
        /// <returns>True if the character is part of a numeric literal, false otherwise</returns>
        private static bool IsNumeric(char c)
        {
            return (c == '.' || (c >= '0' && c <= '9'));
        }

        /// <summary>
        /// Returns true if the given character is a whitespace character
        /// </summary>
        /// <param name="c">The character to test</param>
        /// <returns>True if the character is any whitespace character</returns>
        private static bool IsWhitespace(char c)
        {
            return (c == ' ' || c == '\t' || c == '\n');
        }
    }

    /// <summary>
    /// Utility class for iterating over a string
    /// </summary>
    internal class StringIterator
    {
        /// <summary>
        /// The string to be iterated
        /// </summary>
        private string m_str;

        /// <summary>
        /// The current index this iterator is on
        /// </summary>
        private int m_index;

        /// <summary>
        /// Get the next character in the iterator
        /// </summary>
        public char Next
        {
            get
            {
                char c = m_str[m_index];
                m_index += 1;
                return c;
            }
        }

        /// <summary>
        /// Gets the current character. Does not advance the iterator
        /// </summary>
        public char Current { get { return m_str[m_index]; } }

        /// <summary>
        /// If true, the iterator has iterated over the entire string
        /// </summary>
        public bool IsFinished { get { return m_index == m_str.Length; } }

        /// <summary>
        /// Moves the iterator forward one index. The character advanced past
        /// is returned
        /// </summary>
        public char Advance() { return Next; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        public StringIterator(string str)
        {
            m_str = str;
            m_index = 0;
        }
    }
}
