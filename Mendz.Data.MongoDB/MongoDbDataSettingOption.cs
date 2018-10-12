using MongoDB.Driver;

namespace Mendz.Data.MongoDB
{
    /// <summary>
    /// Provides the data setting options for MongoDB access.
    /// </summary>
    public static class MongoDbDataSettingOption
    {
        /// <summary>
        /// Gets or sets the client settings. When set/valued, this overrides Client.
        /// </summary>
        /// <remarks>
        /// It is recommended that this property is set at application startup
        /// via class library/assembly that returns a MongoClientSettings instance.
        /// This can help hide security sensitive data from the application code.
        /// </remarks>
        public static MongoClientSettings ClientSettings { get; set; } = null;

        /// <summary>
        /// Gets or sets the client name.
        /// </summary>
        public static string Client { get; set;  } = "MongoDBClient";

        /// <summary>
        /// Gets or sets the context name.
        /// </summary>
        public static string Context { get; set; }  = "MongoDBContext";

        /// <summary>
        /// Gets or sets the database settings.
        /// </summary>
        /// <remarks>
        /// It is recommended that this property is set at application startup
        /// via class library/assembly that returns a MongoDatabaseSettings instance.
        /// This can help hide security sensitive data from the application code.
        /// </remarks>
        public static MongoDatabaseSettings DatabaseSettings { get; set; } = null;
    }
}
