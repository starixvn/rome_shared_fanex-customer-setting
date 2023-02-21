// using Alpha.CloneAccount.Models;
// using MediatR;

// namespace Alpha.CloneAccount.Api.Requests;

// public class CustomerPTInfoQuery : IRequest<CustomerPTInfo>
// {
//     public CustomerPTInfoQuery(int custId, byte custLevel)
//     {
//         CustId = custId;
//         CustLevel = custLevel;
//     }

//     public int CustId { get; }

//     public byte CustLevel { get; }

//     public bool IsValid()
//         => CustId > 0 && 1 < CustLevel && CustLevel < 5;
// }
