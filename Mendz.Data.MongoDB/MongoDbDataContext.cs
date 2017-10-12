using Mendz.Data.Common;
using MongoDB.Driver;
using System;

namespace Mendz.Data.MongoDB
{
    /// <summary>
    /// Provides the database context for a MongoDB database.
    /// </summary>
    public class MongoDbDataContext : GenericDbDataContextBase<IMongoDatabase>, IDisposable
    {
        protected override IMongoDatabase BuildContext()
        {
            var client = new MongoClient(DataSettingOptions.ConnectionStrings[MongoDbDataSettingOption.Client]);
            return client.GetDatabase(DataSettingOptions.ConnectionStrings[MongoDbDataSettingOption.Context]);
        }

        #region IDisposable Support
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (_context != null)
                    {
                        //_context.Dispose();
                        _context = null;
                    }
                }
                disposed = true;
            }
        }

        public void Dispose() => Dispose(true);
        #endregion
    }
}
