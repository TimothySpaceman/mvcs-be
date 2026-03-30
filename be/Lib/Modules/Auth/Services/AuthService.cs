using Lib.Modules.Auth.DTOs;
using Lib.Modules.Auth.Entities;
using Lib.Modules.Users.DTOs;
using Lib.Modules.Users.Services;
using Lib.Shared.Exceptions;

namespace Lib.Modules.Auth.Services;

public class AuthService(
    IUserService userService,
    ISessionService sessionService,
    IUserCredentialsService credentialsService
) : IAuthService
{
    public async Task<TokenPairDto> LoginWithCredentialsAsync(
        LoginWithCredentialsDto loginDto,
        DeviceWithIpDto deviceDto
    )
    {
        var user = await userService.GetByEmailAsync(loginDto.EmailOrUsername) ??
                   await userService.GetByUsernameAsync(loginDto.EmailOrUsername);
        if (user is null) throw new InvalidCredentialsException("Invalid credentials");

        var isValid = await credentialsService.VerifyAsync(new UserCredentialsVerifyDto(user.Id, loginDto.Password));
        if (!isValid) throw new InvalidCredentialsException("Invalid credentials");

        return await sessionService.CreateAsync(new SessionCreateDto(
            user.Id,
            DeviceInfo.Create(
                deviceDto.UserAgent,
                deviceDto.Device,
                deviceDto.OS,
                deviceDto.Browser
            ),
            deviceDto.Ip
        ));
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