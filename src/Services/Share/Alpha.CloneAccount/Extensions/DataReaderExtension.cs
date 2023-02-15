using System.Data;

namespace Alpha.CloneAccount.Extensions;

public static class DataReaderExtension
{
    public static int ToInt(this IDataReader reader, string name)
    {
        if (reader[name] != DBNull.Value)
        {
            bool check = int.TryParse(reader[name].ToString(), out int result);
            if (check)
            {
                return result;
            }
        }

        return 0;
    }

    public static string ToString(this IDataReader reader, string name)
    {
        if (reader[name] != DBNull.Value)
        {
            return reader[name].ToString();
        }

        return string.Empty;
    }

    public static bool ToBool(this IDataReader reader, string name)
    {
        return ToBool(reader, name, false);
    }

    public static bool ToBool(this IDataReader reader, string name, bool defaultValue)
    {
        if (reader[name] != DBNull.Value)
        {
            bool check = bool.TryParse(reader[name].ToString(), out bool result);
            if (check)
            {
                return result;
            }
        }

        return defaultValue;
    }

    public static double ToDouble(this IDataReader reader, string name)
    {
        if (reader[name] != DBNull.Value)
        {
            bool check = double.TryParse(reader[name].ToString(), out double result);
            if (check)
            {
                return result;
            }
        }

        return 0;
    }

}