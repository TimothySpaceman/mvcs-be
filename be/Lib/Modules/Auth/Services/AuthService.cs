using Lib.Modules.Auth.DTOs;
using Lib.Modules.Users.DTOs;
using Lib.Modules.Users.Services;
using Lib.Shared.Exceptions;

namespace Lib.Modules.Auth.Services;

public class AuthService(
    IUserService userService,
    IJwtService jwtService,
    IUserCredentialsService credentialsService
) : IAuthService
{
    public async Task<TokenPairDto> LoginWithCredentialsAsync(LoginWithCredentialsDto loginDto)
    {
        var user = await userService.GetByEmailAsync(loginDto.EmailOrUsername) ??
                   await userService.GetByUsernameAsync(loginDto.EmailOrUsername);
        if (user is null) throw new InvalidCredentialsException("Invalid credentials");

        var isValid = await credentialsService.VerifyAsync(new UserCredentialsVerifyDto(user.Id, loginDto.Password));
        if (!isValid) throw new InvalidCredentialsException("Invalid credentials");

        var accessToken = jwtService.GenerateAccessToken(user);
        var refreshToken = jwtService.GenerateRefreshToken(user);

        return new TokenPairDto(accessToken, refreshToken);
    }

    public async Task<UserDto> RegisterAsync(RegisterDto registerDto)
    {
        var user = await userService.CreateAsync(new UserCreateDto(
            registerDto.Username,
            registerDto.DisplayName,
            registerDto.Email
        ));

        try
        {
            await credentialsService.CreateAsync(new UserCredentialsCreateDto(user.Id, registerDto.PlainPassword));
            return user;
        }
        catch
        {
            // TODO: Do not delete user if OAuth providers are connected
            await userService.DeleteByIdAsync(user.Id);
            throw;
        }
    }
}