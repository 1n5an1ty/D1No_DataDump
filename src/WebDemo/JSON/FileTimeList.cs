using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDemo.JSON
{
    /// <summary>
    /// Custom DateTime JSON serializer/deserializer
    /// </summary>
    public class FileTimeList : JsonConverter
    {
        const ulong FUTURE_FILE_TIME = 1UL << 61;
        const ulong PAST_FILE_TIME = 1UL << 16;

        /// <summary>
        /// Writes value to JSON
        /// </summary>
        /// <param name="writer">JSON writer</param>
        /// <param name="value">Value to be written</param>
        /// <param name="serializer">JSON serializer</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reads value from JSON
        /// </summary>
        /// <param name="reader">JSON reader</param>
        /// <param name="objectType">Target type</param>
        /// <param name="existingValue">Existing value</param>
        /// <param name="serializer">JSON serialized</param>
        /// <returns>Deserialized DateTime</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            if (token.Type == JTokenType.Array)
            {
                var children = token.Children();
                var results = new List<DateTime>();
                foreach (var item in children)
                {
                    results.Add(DateTime.FromFileTimeUtc((long)item));
                }

                return results;
            }
            // some other format we're not expecting
            throw new JsonSerializationException("Unexpected JSON format encountered in MaterialArrayConverter: " + token.ToString());
        }

        public override bool CanConvert(Type objectType)
        {
            // CanConvert is not called when [JsonConverter] attribute is used
            return false;
        }
    }
}