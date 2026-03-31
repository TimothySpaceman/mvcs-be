using Lib.Modules.Auth.DTOs;
using Lib.Modules.Users.DTOs;

namespace Lib.Modules.Auth.Services;

public interface IAuthService
{
    public Task<TokenPairDto> LoginWithCredentialsAsync(
        LoginWithCredentialsDto loginDto,
        DeviceWithIpDto deviceDto
    );

    public Task<UserDto> RegisterAsync(RegisterDto registerDto);
    
    public Task<TokenPairDto> RefreshAsync(string refreshToken);
    
    public Task LogoutAsync(string refreshToken);
}