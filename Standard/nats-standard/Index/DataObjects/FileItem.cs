using System;
using LiteDB;
using System.Collections.Generic;

namespace nats_standard.Index.DataObjects
{

    public class FileItem
    {
        [BsonId]
        public Int64 Id { get; set; }

        public DateTime LastModified { get; set; }

        public string Name { get; set; }

        [BsonRef("Keywords")]
        public IList<Keyword> keywords { get; set; }

    }
}

