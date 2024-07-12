namespace Fanex.Customer.API.Modules.PositionTaking.Models;

public class PositionTakingEntity
{
    public string Username { get; set; }

        public int CustId { get; set; }

        public bool Closed { get; set; }

        public byte ClosedReason { get; set; }

        public bool Enabled { get; set; }

        public bool Suspended { get; set; }

        public byte Locked { get; set; }

        public int RoleId { get; set; }

        public int SportType { get; set; }

        public int BetType { get; set; }

        public bool IsLive { get; set; }

        public decimal PositionTaking { get; set; }

        public int RowNumber { get; set; }
}