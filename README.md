# Mendz.Data.MongoDB
Provides a generic Mendz.Data-aware context for MongoDB.
## Namespace
Mendz.Data.MongoDB
### Contents
Name | Description
---- | -----------
MongoDbDataContext | Provides the database context for MongoDB.
MongoDbDataSettingOption | Provides the data setting options for MongoDB.
#### MongoDbDataContext
Mendz.Data.Common defines an IDbDataContext interface, which is implemented as GenericDbDataContextBase.
MongoDbDataContext derives from GenericDbDataContextBase, which requires the abstract BuildContext() method to be implemented.
The internal implementation uses Mendz.Data.DataSettingOptions to build the data context.
MongoDbDataContext.BuildContext() will first look for MongoDbDataSettingOption.Client and MongoDbDataSettingOption.Context.

MongoDbDataContext assumes that appsettings.json contains an entry/section for DataSettings.
```JSON
{
    "DataSettings": {
        "ConnectionStrings": {
            "MongoDBClient" : "MongoDB client specification",
            "MongoDBContext" : "MongoDB context specification"
        }
    }
}
```
In the application startup or initialization routine, the DataSettings should be loaded into DataSettingOptions as follows:
```C#
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            DataSettingOptions.Initialize(Configuration.GetSection("DataSettings").Get<DataSettings>());
        }
```
Mendz.Data-aware repositories implement DbRepositoryBase, which expects a Mendz.Data-aware data context.
Using MongoDbDataContext, a repository skeleton can look like the following:
```C#
    public class TestRepository : DbRepositoryBase<MongoDbDataContext>
    {
        ...
    }
```
Using Mendz.Data can shield the application from "knowing" about the data context.
The application does not need to reference Mendz.Data.MongoDB.
The application can reference only Mendz.Data, and the models and repositories libraries.
## NuGet It...
https://www.nuget.org/packages/Mendz.Data.MongoDB/
