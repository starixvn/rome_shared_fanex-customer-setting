using Fanex.Data;
using System.Data.SqlClient;

namespace Fanex.Customer.API.Helpers;

public class ShardDbSettingProvider : XmlDbSettingProvider
{
    private readonly IShardInfoAccessor _shardInfoAccessor;
    
    public ShardDbSettingProvider(IShardInfoAccessor shardInfoAccessor, string resourcePath)
        : base(resourcePath)
    {
        _shardInfoAccessor = shardInfoAccessor;
    }

    public override IDbSetting GetSetting(string key)
    {
        if (_shardInfoAccessor.TryUseShardDB(key, out string dbServer))
        {
            var originalShardSetting = base.GetSetting(key);
            var connectionStr = new SqlConnectionStringBuilder(originalShardSetting.ConnectionString)
            {
                DataSource = dbServer
            };

            var dbSetting = new DbSetting(
                key,
                originalShardSetting.DbName,
                connectionStr.ToString(),
                originalShardSetting.SpName,
                originalShardSetting.CommandTimeout,                
                originalShardSetting.ColumnMaps,
                originalShardSetting.FirmParameters);
            return dbSetting;
        }

        return base.GetSetting(key);
    }
}
