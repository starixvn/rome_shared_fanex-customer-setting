using System.Xml.Linq;

namespace Fanex.Customer.API.Modules.Customer.CopyCustomer;

public record LogAPIRequest(int SystemId, int ProductId, int UserId, string FunctionName, int CustId, int ActionId, XDocument Request);