using System.Xml.Linq;

namespace Alpha.CloneAccount.Params;

public record LogAPIRequestParam(int SystemId, int ProductId, int UserId, string FunctionName, int CustId, int ActionId, XDocument Request);