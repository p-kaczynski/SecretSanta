using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using NLog;
using SecretSanta.Common.Interface;
using SecretSanta.Domain.Models;
using SecretSanta.Domain.SecurityModels;

namespace SecretSanta.Security
{
    public class SantaUserManager : UserManager<SantaSecurityUser>, ISantaAdminProvider
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        // TODO: Move somewhere else
        public const string AdminRole = "Admin";
        public const string UserRole = "User";

        private readonly ISantaUserStore _santaStore;
        private readonly IEncryptionProvider _encryptionProvider;

        public SantaUserManager(ISantaUserStore store, IEncryptionProvider encryptionProvider) :
            base(store)
        {
            _santaStore = store;
            _encryptionProvider = encryptionProvider;
        }

        public override async Task<IdentityResult> CreateAsync(SantaSecurityUser user)
        {
            try
            {
                await Store.CreateAsync(user);
                return IdentityResult.Success;
            }
            catch (Exception exception)
            {
                Log.Error(exception, $"{nameof(CreateAsync)}: Error while creating admin");
                return IdentityResult.Failed(exception.Message);
            }
        }

        public override async Task<IdentityResult> UpdateAsync(SantaSecurityUser user)
        {
            try
            {
                await Store.UpdateAsync(user);
                return IdentityResult.Success;
            }
            catch (Exception exception)
            {
                Log.Error(exception, $"{nameof(CreateAsync)}: Error while updating admin");
                return IdentityResult.Failed(exception.Message);
            }
        }

        public override Task<IdentityResult> CreateAsync(SantaSecurityUser user, string password)
        {
            throw new NotSupportedException();
        }

        public override Task<bool> CheckPasswordAsync(SantaSecurityUser user, string password)
            => Task.Factory.StartNew(() =>
                user != null && _encryptionProvider.VerifyPasswordHash(password, user.PasswordHash));

        /// <inheritdoc />
        public override Task<bool> HasPasswordAsync(string userId)
            => Task.FromResult(true);

        public override Task<IdentityResult> AddPasswordAsync(string userId, string password)
        {
            throw new NotSupportedException();
        }

        public override Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword,
            string newPassword)
        {
            throw new NotSupportedException();
        }

        public override Task<IdentityResult> RemovePasswordAsync(string userId)
        {
            throw new NotSupportedException();
        }

        protected override Task<IdentityResult> UpdatePassword(
            IUserPasswordStore<SantaSecurityUser, string> passwordStore, SantaSecurityUser user, string newPassword)
        {
            throw new NotSupportedException();
        }

        public override Task<string> GetSecurityStampAsync(string userId)
        {
            throw new NotSupportedException();
        }

        public override Task<IdentityResult> UpdateSecurityStampAsync(string userId)
        {
            throw new NotSupportedException();
        }

        public override Task<string> GeneratePasswordResetTokenAsync(string userId)
        {
            throw new NotSupportedException();
        }

        public override Task<IdentityResult> ResetPasswordAsync(string userId, string token, string newPassword)
        {
            throw new NotSupportedException();
        }

        public override Task<SantaSecurityUser> FindAsync(UserLoginInfo login)
        {
            throw new NotSupportedException();
        }

        public override Task<IdentityResult> RemoveLoginAsync(string userId, UserLoginInfo login)
        {
            throw new NotSupportedException();
        }

        public override Task<IdentityResult> AddLoginAsync(string userId, UserLoginInfo login)
        {
            throw new NotSupportedException();
        }

        public override Task<IList<UserLoginInfo>> GetLoginsAsync(string userId)
        {
            throw new NotSupportedException();
        }

        public override Task<IdentityResult> AddClaimAsync(string userId, Claim claim)
        {
            throw new NotSupportedException();
        }

        public override Task<IdentityResult> RemoveClaimAsync(string userId, Claim claim)
        {
            throw new NotSupportedException();
        }

        public override Task<IList<Claim>> GetClaimsAsync(string userId)
        {
            throw new NotSupportedException();
        }

        public override Task<IdentityResult> AddToRoleAsync(string userId, string role)
        {
            throw new NotSupportedException();
        }

        public override Task<IdentityResult> AddToRolesAsync(string userId, params string[] roles)
        {
            throw new NotSupportedException();
        }

        public override Task<IdentityResult> RemoveFromRolesAsync(string userId, params string[] roles)
        {
            throw new NotSupportedException();
        }

        public override Task<IdentityResult> RemoveFromRoleAsync(string userId, string role)
        {
            throw new NotSupportedException();
        }

        public override async Task<IList<string>> GetRolesAsync(string userId)
        {
            var user = await Store.FindByIdAsync(userId);
            return user.IsPrivileged ? new[] {AdminRole} : new[] {UserRole};
        }

        public override async Task<bool> IsInRoleAsync(string userId, string role)
        {
            var roles = await GetRolesAsync(userId);
            return roles.Contains(role);
        }

        public override bool SupportsUserRole { get; } = true;

        public override async Task<string> GetEmailAsync(string userId)
        {
            var user = await FindByIdAsync(userId);
            return user.IsPrivileged ? null : user.UserName;
        }

        public override Task<IdentityResult> SetEmailAsync(string userId, string email)
        {
            throw new NotSupportedException();
        }

        public override async Task<SantaSecurityUser> FindByEmailAsync(string email)
        {
            return await Store.FindByNameAsync(email);
        }

        public override Task<string> GenerateEmailConfirmationTokenAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public override Task<IdentityResult> ConfirmEmailAsync(string userId, string token)
        {
            throw new NotImplementedException();
        }

        public override Task<bool> IsEmailConfirmedAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public override Task<string> GetPhoneNumberAsync(string userId)
        {
            throw new NotSupportedException();
        }

        public override Task<IdentityResult> SetPhoneNumberAsync(string userId, string phoneNumber)
        {
            throw new NotSupportedException();
        }

        public override Task<IdentityResult> ChangePhoneNumberAsync(string userId, string phoneNumber, string token)
        {
            throw new NotSupportedException();
        }

        public override Task<bool> IsPhoneNumberConfirmedAsync(string userId)
        {
            throw new NotSupportedException();
        }

        public override Task<string> GenerateChangePhoneNumberTokenAsync(string userId, string phoneNumber)
        {
            throw new NotSupportedException();
        }

        public override Task<bool> VerifyChangePhoneNumberTokenAsync(string userId, string token, string phoneNumber)
        {
            throw new NotSupportedException();
        }

        public override Task<bool> VerifyUserTokenAsync(string userId, string purpose, string token)
        {
            throw new NotImplementedException();
        }

        public override Task<string> GenerateUserTokenAsync(string purpose, string userId)
        {
            throw new NotImplementedException();
        }

        public override void RegisterTwoFactorProvider(string twoFactorProvider, IUserTokenProvider<SantaSecurityUser, string> provider)
        {
            throw new NotSupportedException();
        }

        public override Task<IList<string>> GetValidTwoFactorProvidersAsync(string userId)
        {
            throw new NotSupportedException();
        }

        public override Task<bool> VerifyTwoFactorTokenAsync(string userId, string twoFactorProvider, string token)
        {
            throw new NotSupportedException();
        }

        public override Task<string> GenerateTwoFactorTokenAsync(string userId, string twoFactorProvider)
        {
            throw new NotSupportedException();
        }

        public override Task<IdentityResult> NotifyTwoFactorTokenAsync(string userId, string twoFactorProvider, string token)
        {
            throw new NotSupportedException();
        }

        public override Task<bool> GetTwoFactorEnabledAsync(string userId) => Task.FromResult(false);

        public override Task<IdentityResult> SetTwoFactorEnabledAsync(string userId, bool enabled)
        {
            throw new NotSupportedException();
        }

        public override Task SendEmailAsync(string userId, string subject, string body)
        {
            throw new NotImplementedException();
        }

        public override Task SendSmsAsync(string userId, string message)
        {
            throw new NotSupportedException();
        }

        public override Task<bool> IsLockedOutAsync(string userId) => Task.FromResult(false);

        public override Task<IdentityResult> SetLockoutEnabledAsync(string userId, bool enabled)
        {
            throw new NotSupportedException();
        }

        public override Task<bool> GetLockoutEnabledAsync(string userId) => Task.FromResult(false);

        public override Task<DateTimeOffset> GetLockoutEndDateAsync(string userId)
        {
            throw new NotSupportedException();
        }

        public override Task<IdentityResult> SetLockoutEndDateAsync(string userId, DateTimeOffset lockoutEnd)
        {
            throw new NotSupportedException();
        }

        public override Task<IdentityResult> AccessFailedAsync(string userId)
        {
            throw new NotSupportedException();
        }

        public override Task<IdentityResult> ResetAccessFailedCountAsync(string userId)
        {
            return Task.FromResult(IdentityResult.Success);
        }

        public override Task<int> GetAccessFailedCountAsync(string userId)
        {
            throw new NotSupportedException();
        }

        public override async Task<ClaimsIdentity> CreateIdentityAsync(SantaSecurityUser santaSecurityUser, string authenticationType)
        {
            var result = await base.CreateIdentityAsync(santaSecurityUser, authenticationType);
            result.AddClaim(new Claim(ClaimTypes.GivenName, santaSecurityUser.DisplayName));
            return result;
        }

        public IList<SantaAdmin> GetAllAdmins()
        {
            return _santaStore.GetAllAdmins();
        }
    }
}