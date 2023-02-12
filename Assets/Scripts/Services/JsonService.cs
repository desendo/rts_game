#nullable enable
using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Services
{
    public interface IJsonService
    {
        bool FromJson<T>(string json, out T data) where T : new();
        bool FromJson(string json, Type type, out object? data);
        string ToJson(object obj);
    }

    public class VectorConverter : JsonConverter
    {

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var t = serializer.Deserialize(reader);
            var obj = JsonConvert.DeserializeObject<Vector3>(t?.ToString() ?? "");
            return obj;
        }
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            Vector3 v = (Vector3)(value ?? Vector3.zero);

            writer.WriteStartObject();
            writer.WritePropertyName("x");
            writer.WriteValue(v.x);
            writer.WritePropertyName("y");
            writer.WriteValue(v.y);
            writer.WritePropertyName("z");
            writer.WriteValue(v.z);
            writer.WriteEndObject();
        }


        public override bool CanConvert(Type type)
        {
            return type  == typeof(Vector3);
        }
    }


    public class JsonService : IJsonService
    {
        private JsonSerializerSettings _settings;

        public JsonService()
        {
            _settings = new JsonSerializerSettings();
            _settings.Converters.Add(new VectorConverter());
        }

        public bool FromJson<T>(string json, out T data) where T : new()
        {
            try
            {
                data = JsonConvert.DeserializeObject<T>(json, _settings) ?? new T();
                return true;

            }
            catch (Exception e)
            {
                data = new T();
                Debug.LogError($"DeserializeObject {typeof(T)} Error: {e.Message}");
                return false;
            }

        }

        public bool FromJson(string json, Type type, out object? data)
        {
            try
            {

                data = JsonConvert.DeserializeObject(json, type, _settings);
                return true;

            }
            catch (Exception e)
            {
                data = null;
                Debug.LogError($"DeserializeObject {type.Name} Error: {e.Message}");
                return false;
            }
        }

        public string ToJson(object obj)
        {

            return JsonConvert.SerializeObject(obj, _settings);
        }
    }
}