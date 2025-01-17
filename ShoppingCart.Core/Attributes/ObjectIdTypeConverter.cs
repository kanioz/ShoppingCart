﻿using System;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ShoppingCart.Core.Attributes
{
    public class ObjectIdTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(ObjectId);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            return ObjectId.Parse(token.ToObject<string>());
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.ToString());
        }
    }
}
