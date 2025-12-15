using Gym.Application.DTO.Auth;
using Gym.Application.Security;
using Gym.Domain.Entities;
using Gym.Domain.Enums;
using Gym.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly GymDbContext _db;
        private readonly IPasswordHasher _hasher;
        private readonly IJwtService _jwt;

        public AuthService(GymDbContext db, IPasswordHasher hasher, IJwtService jwt)
        {
            _db = db;
            _hasher = hasher;
            _jwt = jwt;
        }

        public async Task RegisterAsync(RegisterRequestDto dto)
        {
            if (await _db.Users.AnyAsync(x => x.NationalCode == dto.NationalCode))
                throw new Exception("کاربری با این کد ملی وجود دارد");

            var user = new User
            {
                TenantId = dto.TenantId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                NationalCode = dto.NationalCode,
                Mobile = dto.Mobile,
                Email = dto.Email,
                BirthDate = dto.BirthDate,
                PasswordHash = _hasher.Hash(dto.Password),
                Role = UserRole.Customer
            };

            user.Wallet = new Wallet { Balance = 0 };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
        {
            var user = await _db.Users
                .Include(x => x.RefreshTokens)
                .FirstOrDefaultAsync(x => x.NationalCode == dto.NationalCode);

            if (user == null || !_hasher.Verify(dto.Password, user.PasswordHash))
                throw new Exception("اطلاعات ورود نادرست است");

            var accessToken = _jwt.GenerateAccessToken(user);
            var refreshToken = _jwt.GenerateRefreshToken(user.Id);

            _db.RefreshTokens.Add(refreshToken);
            await _db.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                Role = user.Role.ToString()
            };
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
        {
            var token = await _db.RefreshTokens
                .Include(x => x.User)
                .FirstOrDefaultAsync(x =>
                    x.Token == refreshToken &&
                    !x.IsRevoked &&
                    x.ExpireAt > DateTime.UtcNow);

            if (token == null)
                throw new Exception("Refresh Token نامعتبر است");

            token.IsRevoked = true;

            var newAccess = _jwt.GenerateAccessToken(token.User);
            var newRefresh = _jwt.GenerateRefreshToken(token.UserId);

            _db.RefreshTokens.Add(newRefresh);
            await _db.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = newAccess,
                RefreshToken = newRefresh.Token,
                Role = token.User.Role.ToString()
            };
        }
    }
}
