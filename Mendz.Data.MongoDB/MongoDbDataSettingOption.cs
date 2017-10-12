namespace Mendz.Data.MongoDB
{
    /// <summary>
    /// Provides the data setting options for MongoDB access.
    /// </summary>
    public static class MongoDbDataSettingOption
    {
        /// <summary>
        /// Gets or sets the client name.
        /// </summary>
        public static string Client { get; set;  } = "MongoDBClient";

        /// <summary>
        /// Gets or sets the context name.
        /// </summary>
        public static string Context { get; set; }  = "MongoDBContext";
    }
}
