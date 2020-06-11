using System;
using LiteDB;
namespace nats_standard.Index.DataObjects
{
    public class Keyword
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string Text { get; set; }
    }
}
