namespace Aldaman.Services.Dtos.Account;

public record LoginResultDto(bool Succeeded, bool IsLockedOut, bool IsNotAllowed, bool RequiresTwoFactor);
