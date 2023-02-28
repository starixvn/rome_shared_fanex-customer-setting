using System.Data;

namespace Fanex.Customer.API.Extensions;

public static class DataRecordExtension
{
    public static bool HasColumn(this IDataRecord dataRecord, string columnName)
    {
        for (int i = 0; i < dataRecord.FieldCount; i++)
        {
            if (dataRecord.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}