using Mendz.Data.Common;
using MongoDB.Driver;
using System;
using System.Threading;

namespace Mendz.Data.MongoDB
{
    /// <summary>
    /// Provides the database context for a MongoDB database.
    /// </summary>
    public class MongoDbDataContext : GenericDbDataContextBase<IMongoDatabase>, IDisposable
    {
        protected override IMongoDatabase BuildContext()
        {
            MongoClient client;
            if (MongoDbDataSettingOption.ClientSettings != null)
            {
                client = new MongoClient(MongoDbDataSettingOption.ClientSettings);
            }
            else
            {
                client = new MongoClient(DataSettingOptions.ConnectionStrings[MongoDbDataSettingOption.Client]);
            }
            return client.GetDatabase(DataSettingOptions.ConnectionStrings[MongoDbDataSettingOption.Context], MongoDbDataSettingOption.DatabaseSettings);
        }

        #region Session/Transaction Support
        protected IClientSessionHandle _session = null;
        /// <summary>
        /// Gets the client session handle.
        /// </summary>
        /// <remarks>
        /// MongoDB's .Net client/driver is proprietary, with a unique session/transaction design.
        /// Intentionally, MongoDB's .Net client/driver is not ADO.Net compliant.
        /// Therefore, IDbDataTransaction is incompatible and cannot be implemented/used.
        /// This implemention basically aligns with MongoDB's APIs for sessions/transactions.
        /// However, note that the basic essence and intent of Mendz.Data contexts remains intact.
        /// </remarks>
        public IClientSessionHandle Session
        {
            get => _session;
        }

        /// <summary>
        /// Begins a transaction.
        /// </summary>
        /// <param name="clientSessionOptions">Optional client session options.</param>
        /// <param name="transactionOptions">Optional transaction options.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        public void BeginTransaction(ClientSessionOptions clientSessionOptions = null, TransactionOptions transactionOptions = null, CancellationToken cancellationToken = default)
        {
            if (_session == null)
            {
                CreateContext();
                _session = _context.Client.StartSession(clientSessionOptions, cancellationToken);
                _session.StartTransaction(transactionOptions);
            }
        }

        /// <summary>
        /// Ends a transaction.
        /// </summary>
        /// <param name="mode">The transaction mode to commit or rollback.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        public void EndTransaction(EndTransactionMode mode = EndTransactionMode.Commit, CancellationToken cancellationToken = default)
        {
            if (_session != null)
            {
                if (_session.IsInTransaction)
                {
                    if (mode == EndTransactionMode.Commit)
                    {
                        _session.CommitTransaction(cancellationToken);
                    }
                    else
                    {
                        _session.AbortTransaction(cancellationToken);
                    }
                }
                _session.Dispose();
                _session = null;
            }
        }

        /// <summary>
        /// Begins a transaction asynchronously.
        /// </summary>
        /// <param name="clientSessionOptions">Optional client session options.</param>
        /// <param name="transactionOptions">Optional transaction options.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        public async void BeginTransactionAsync(ClientSessionOptions clientSessionOptions = null, TransactionOptions transactionOptions = null, CancellationToken cancellationToken = default)
        {
            if (_session == null)
            {
                CreateContext();
                _session = await _context.Client.StartSessionAsync(clientSessionOptions, cancellationToken);
                _session.StartTransaction(transactionOptions);
            }
        }

        /// <summary>
        /// Ends a transaction asynchronously.
        /// </summary>
        /// <param name="mode">The transaction mode to commit or rollback.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        public async void EndTransactionAsync(EndTransactionMode mode = EndTransactionMode.Commit, CancellationToken cancellationToken = default)
        {
            if (_session != null)
            {
                if (_session.IsInTransaction)
                {
                    if (mode == EndTransactionMode.Commit)
                    {
                        await _session.CommitTransactionAsync(cancellationToken);
                    }
                    else
                    {
                        await _session.AbortTransactionAsync(cancellationToken);
                    }
                }
                _session.Dispose();
                _session = null;
            }
        }
        #endregion

        #region IDisposable Support
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (_session != null)
                    {
                        _session.Dispose();
                    }
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
