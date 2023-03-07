using System.Collections;
using System.Data;
using Microsoft.Extensions.Options;

namespace Fanex.Customer.API.Modules.CopyCustomer;

public interface ICopyMemberSpParamsBuilder
{
    Hashtable BuildSpParams(DataRow sportbooksSetting, CopyCustomerRequest param, string hashPassword);
}

public class CopyMemberSpParamsBuilder : ICopyMemberSpParamsBuilder
{
    private readonly AppSettings appSettings;

    public CopyMemberSpParamsBuilder(IOptions<AppSettings> appSettings)
    {
        this.appSettings = appSettings.Value;
    }

    public Hashtable BuildSpParams(DataRow sportbooksSetting, CopyCustomerRequest param, string hashPassword)
    {
        var spParams = new Hashtable();

        AddAccountInfoSpParams(sportbooksSetting, param, hashPassword, spParams);
        AddBettingLimitSpParams(sportbooksSetting, spParams);
        AddPositionTakingSpParams(sportbooksSetting, spParams);

        return spParams;
    }

    private void AddAccountInfoSpParams(DataRow sportbooksSetting, CopyCustomerRequest param, string hashPassword, Hashtable spParams)
    {
        spParams.Add("custid", 0);
        spParams.Add("username", param.UserName);
        spParams.Add("userpwd", hashPassword);
        spParams.Add("firstname", string.Empty);
        spParams.Add("lastname", string.Empty);
        spParams.Add("phone", string.Empty);
        spParams.Add("mobilephone", string.Empty);
        spParams.Add("fax", string.Empty);
        spParams.Add("credit", param.Credit);
        spParams.Add("copyfromcustid", param.SourceCustId);
        spParams.Add("systemid", param.SystemId);
        spParams.Add("CreatedMode", 1);
        spParams.Add("playerdiscount", sportbooksSetting["playerdiscount"]);
        spParams.Add("playerdiscountcs", sportbooksSetting["playerdiscountcs"]);
        spParams.Add("playerdiscount1x2", sportbooksSetting["playerdiscount1x2"]);
        spParams.Add("playerdiscountmyanmarodds", sportbooksSetting["playerdiscountmyanmarodds"]);
        spParams.Add("recommend", param.ActorId);
        spParams.Add("ugroup", sportbooksSetting["ugroup"]);
        spParams.Add("closed", sportbooksSetting["closed"]);
        spParams.Add("issearchandcopy", 1);
        spParams.Add("disablenumber", sportbooksSetting["disablenumber"]);
        spParams.Add("language", param.LanguageKey);
        spParams.Add("ip", param.IP);
        spParams.Add("creator", param.ActorId);
        spParams.Add("subaccid", param.ActorSubAccId);
        spParams.Add("settingstatus", appSettings.SettingStatus);
        spParams.Add("errcode", 0);
        spParams.Add("errmsg", string.Empty);
    }

    private static void AddBettingLimitSpParams(DataRow sportbooksSetting, Hashtable spParams)
    {
        var sportTypes = new[] { "1", "1mp", "1saba", "2", "2saba", "3", "5", "8", "10", "11", "99", "99mp", "161", "164" };
        var parameters = new[] { "minbetv{0}", "maxbetv{0}", "maxpermatchv{0}", "maxpopermatchv{0}" };

        foreach (string item in sportTypes)
        {
            if (item == "1")
            {
                spParams.Add("minbet", sportbooksSetting["minbet"]);
                spParams.Add("maxbet", sportbooksSetting["maxbet"]);
                spParams.Add("maxpermatch", sportbooksSetting["maxpermatch"]);
                spParams.Add("maxpopermatch", sportbooksSetting["maxpayoutpermatch"]);
            }
            else
            {
                if (item == "161")
                {
                    spParams.Add("maxperballv161", sportbooksSetting["maxperballv161"]);
                }

                foreach (string param in parameters)
                {
                    var formatParam = string.Format(param, item);

                    if (!spParams.ContainsKey(formatParam))
                    {
                        if (param == "maxpopermatchv{0}" && item != "161" && item != "164")
                        {
                            spParams.Add(formatParam, sportbooksSetting[string.Format("maxpayoutpermatchv{0}", item)]);
                        }

                        if (param != "maxpopermatchv{0}")
                        {
                            spParams.Add(formatParam, sportbooksSetting[formatParam]);
                        }
                    }
                }
            }
        }
    }

    private static void AddPositionTakingSpParams(DataRow sportbooksSetting, Hashtable spParams)
    {
        var sportTypesBetTypes = GetSportTypesBetTypes();

        foreach (string key in sportTypesBetTypes.Keys)
        {
            var betTypes = (string[])sportTypesBetTypes[key];

            string parameter;
            string parameter_max;
            switch (key)
            {
                case "1":
                    parameter = "apositiontaking_{0}";
                    parameter_max = "maxpos_{0}";
                    break;

                case "1_live":
                    parameter = "alivepositiontaking_{0}";
                    parameter_max = "maxlpos_{0}";
                    break;

                case "161_live":
                    parameter = "v161alivepositiontaking_{0}";
                    parameter_max = "maxv161lpos_{0}";
                    break;

                default:
                    parameter = string.Format("v{0}apositiontaking_", key) + "{0}";
                    parameter_max = string.Format("maxv{0}pos_", key) + "{0}";
                    break;
            }

            foreach (string bettype in betTypes)
            {
                var param = string.Format(parameter, bettype);
                var param_max = string.Format(parameter_max, bettype);

                double min = Math.Min(Convert.ToDouble(sportbooksSetting[param]), Convert.ToDouble(sportbooksSetting[param_max]));
                spParams.Add(param, min);
            }
        }
    }

    private static Hashtable GetSportTypesBetTypes()
    {
        var sportTypesBetTypes = new Hashtable
        {
            { "1", new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "9999" } },
            { "1_live", new string[] { "1", "3", "7", "8" } },
            { "2", new string[] { "1", "2", "3", "7", "8", "20", "10", "9999" } },
            { "3", new string[] { "1", "2", "3", "20", "10" } },

            { "5", new string[] { "1", "2", "3", "20", "9999", "10" } },

            { "8", new string[] { "1", "3", "20", "10" } },
            { "10", new string[] { "1", "2", "3", "20", "10" } },
            { "11", new string[] { "1", "20", "10" } },
            { "99", new string[] { "1", "2", "3", "9", "20", "10", "9999" } },
            { "161", new string[] { "81", "83", "86", "88" } },
            { "161_live", new string[] { "85", "86", "87", "89", "90" } }
        };

        return sportTypesBetTypes;
    }
}