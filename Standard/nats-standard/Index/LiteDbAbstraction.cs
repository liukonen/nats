using System;
using LiteDB;
using nats_standard.Index.DataObjects;
using System.Linq;
using System.Collections.Generic;

namespace nats_standard.Index
{
    public class LiteDbAbstraction : IDisposable
    {

        const string FilesTable = "FileItem";
        const string KeywordsTable = "Keywords";

        private static LiteDbAbstraction response;
        private bool disposedValue;
        private LiteDatabase db = new LiteDatabase("natsLite.db");
        private object Threadkey = new object();


        public static LiteDbAbstraction Instance()
        {
            if (response == null) response = new LiteDbAbstraction();
            return response;
        }
        protected LiteDbAbstraction(){}


        public bool NeedsUpdating(System.IO.FileInfo file)
        {
            DateTime date = new DateTime();
            return (!(TryGetStoredLastModified(file, out date) && date <= file.LastWriteTime));
        }

        public string[] Inquire(string path, string[] keywords)
        {
            var FileCollection = db.GetCollection<FileItem>(FilesTable);

            string[] items = new string[] { };
            Boolean first = true;
            foreach (var words in keywords)
            {
                var Files2 = FileCollection.Query().Include(x => x.keywords).Where(x => x.Name.StartsWith(path)).ToArray();


                var Files = FileCollection.Query().Include(x => x.keywords).Where(x => x.Name.StartsWith(path) && (x.keywords.Where(y => y.Text == words)).Any()).Select(x => x.Name);
                if (first) { first = false; items = Files.ToArray(); }
                else { items = items.Intersect(Files.ToArray()).ToArray(); }
            }
            return items;

            

        }

        public void AddOrInsert(System.IO.FileInfo file, string[] keywords)
        {
            lock (Threadkey)
            {
                var KeywordCollection = db.GetCollection<Keyword>(KeywordsTable);
                var FileCollection = db.GetCollection<FileItem>(FilesTable);

                var existingKeywords = KeywordCollection.Query().Where(x => keywords.Contains(x.Text)).Select(x => x);

                List<Keyword> keywordsToAdd = existingKeywords.ToList();
                var KeywordsToCreate = keywords.Except(from Keyword key in keywordsToAdd select key.Text);
                List<Keyword> itemsToAdd = new List<Keyword>(from string item in KeywordsToCreate select new Keyword() { Text = item });
                KeywordCollection.InsertBulk(itemsToAdd);


                FileItem fil;
                fil = FileCollection.FindOne(x => x.Name == file.FullName);
                if (fil == null)
                {
                    fil = new FileItem() { keywords = keywordsToAdd.Union(itemsToAdd).ToList(), LastModified = file.LastWriteTime, Name = file.FullName };
                    FileCollection.Insert(fil);
                }
                else
                {
                    fil.keywords = keywordsToAdd.Union(itemsToAdd).ToList();
                    FileCollection.Update(fil);
                }
                FileCollection.EnsureIndex(x => x.Name);
                KeywordCollection.EnsureIndex(x => x.Text);
            }
        }


        private Boolean TryGetStoredLastModified(System.IO.FileInfo file, out DateTime dateTime)
        {
            var FilesColllection = db.GetCollection<FileItem>(FilesTable);
            if (FilesColllection.Count() > 0)
            {
                var fie = FilesColllection.Query().Where(x => x.Name == file.FullName).Select(x => x.LastModified);
                if (fie.Exists()) { dateTime = fie.First(); return true; }
            }
            dateTime = new DateTime(); return false;
        }



        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~LiteDbAbstraction()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
