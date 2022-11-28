namespace Fanex.Auth.API;

public record AuthRequest(int UserId, string Username, byte RoleId, int SubAccId);