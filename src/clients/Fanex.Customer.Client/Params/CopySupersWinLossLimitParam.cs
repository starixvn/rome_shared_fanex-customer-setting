using System.Collections.Generic;
using Fanex.Customer.Client.Constants;

namespace Fanex.Customer.Client.Params
{
    public class CopySupersWinLossLimitParam
    {
        public CopySupersWinLossLimitParam(
           int sourceSuperId,
           string sourceUserName,
           string copiedSuperIds,
           string copiedUserNames,
           int actorId,
           byte systemId,
           string ip,
           int custDBGroupID,
           IEnumerable<int> sportTypeIds,
           string language = Languages.English)
        {
            SourceSuperId = sourceSuperId;
            SourceUserName = sourceUserName;
            CopiedSuperIds = copiedSuperIds;
            CopiedUserNames = copiedUserNames;
            ActorId = actorId;
            SystemId = systemId;
            IP = ip;
            CustDBGroupID = custDBGroupID;
            SportTypeIds = sportTypeIds;
            Language = language;
        }


        public int SourceSuperId { get; set; }

        public string SourceUserName { get; set; }

        public string CopiedSuperIds { get; set; }

        public string CopiedUserNames { get; set; }

        public int ActorId { get; set; }

        public byte SystemId { get; set; }

        public string IP { get; set; }

        public int CustDBGroupID { get; set; }

        public IEnumerable<int> SportTypeIds { get; set; }

        public string Language { get; set; }
    }
}