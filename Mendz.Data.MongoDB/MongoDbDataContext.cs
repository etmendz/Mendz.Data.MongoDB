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
            if (MongoDbDataSettingOption.ClientSettings == null)
            {
                client = new MongoClient(DataSettingOptions.ConnectionStrings[MongoDbDataSettingOption.Client]);
            }
            else
            {
                client = new MongoClient(MongoDbDataSettingOption.ClientSettings);
            }
            return client.GetDatabase(DataSettingOptions.ConnectionStrings[MongoDbDataSettingOption.Context], MongoDbDataSettingOption.DatabaseSettings);
        }

        #region Session/Transaction Support
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
        public IClientSessionHandle Session { get; protected set; }

        /// <summary>
        /// Begins a transaction.
        /// </summary>
        /// <param name="clientSessionOptions">Optional client session options.</param>
        /// <param name="transactionOptions">Optional transaction options.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        public void BeginTransaction(ClientSessionOptions clientSessionOptions = null, TransactionOptions transactionOptions = null, CancellationToken cancellationToken = default)
        {
            if (Session == null)
            {
                CreateContext();
                Session = Context.Client.StartSession(clientSessionOptions, cancellationToken);
                Session.StartTransaction(transactionOptions);
            }
        }

        /// <summary>
        /// Ends a transaction.
        /// </summary>
        /// <param name="mode">The transaction mode to commit or rollback.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        public void EndTransaction(EndTransactionMode mode = EndTransactionMode.Commit, CancellationToken cancellationToken = default)
        {
            if (Session != null)
            {
                if (Session.IsInTransaction)
                {
                    if (mode == EndTransactionMode.Commit)
                    {
                        Session.CommitTransaction(cancellationToken);
                    }
                    else
                    {
                        Session.AbortTransaction(cancellationToken);
                    }
                }
                Session.Dispose();
                Session = null;
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
            if (Session == null)
            {
                CreateContext();
                Session = await Context.Client.StartSessionAsync(clientSessionOptions, cancellationToken).ConfigureAwait(false);
                Session.StartTransaction(transactionOptions);
            }
        }

        /// <summary>
        /// Ends a transaction asynchronously.
        /// </summary>
        /// <param name="mode">The transaction mode to commit or rollback.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        public async void EndTransactionAsync(EndTransactionMode mode = EndTransactionMode.Commit, CancellationToken cancellationToken = default)
        {
            if (Session != null)
            {
                if (Session.IsInTransaction)
                {
                    if (mode == EndTransactionMode.Commit)
                    {
                        await Session.CommitTransactionAsync(cancellationToken).ConfigureAwait(false);
                    }
                    else
                    {
                        await Session.AbortTransactionAsync(cancellationToken).ConfigureAwait(false);
                    }
                }
                Session.Dispose();
                Session = null;
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
                    if (Session != null) Session.Dispose();
                    if (Context != null) Context = null;
                }
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
