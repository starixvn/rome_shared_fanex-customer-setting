using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.UserNameSuggestion;

public class SelectAutoSuggestionCriteria : CriteriaBase
{
    public SelectAutoSuggestionCriteria(
        int actorId,
        string suggestionQuery,
        bool isInternal,
        bool directDownline,
        bool isAccInfo)
    {
        ActorId = actorId;
        SuggestionQuery = suggestionQuery;
        IsInternal = isInternal;

        IsAccInfo = isAccInfo;
        DirectDownline = directDownline;
    }

    public int UserId => ActorId;

    [SpParam(SqlParamName = "CustId")]
    public int ActorId { get; }

    [SpParam(SqlParamName = "username")]
    public string SuggestionQuery { get; }

    [SpParam(SqlParamName = "isInternalAgent")]
    public bool IsInternal { get; }

    public bool DirectDownline { get; }

    public bool IsAccInfo { get; }

    public override string GetSettingKey() => "Age_CustomerSelUserSearch";

    public override bool IsValid() => ActorId > 0 && !string.IsNullOrEmpty(SuggestionQuery);
}