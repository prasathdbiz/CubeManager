using CubeServer.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using CubeServer.Data;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.DataProtection;

namespace CubeServer.Authentication
{
    public class CustomAuthenticationStateProvider : ServerAuthenticationStateProvider/* AuthenticationStateProvider*/
    {
        public const string DevelopmentAdminUserId = "admin";
        public const string DevelopmentAdminPassword = "Admin123!";

        //[Inject]
        //protected IDatabase db { get; set; }

        public ClaimsPrincipal curUserClaims;
        public bool newLogin;

        public CustomAuthenticationStateProvider()
        {

        }

        public static bool IsDevelopmentEnvironment()
        {
            string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "";
            return string.Equals(env, "Development", StringComparison.OrdinalIgnoreCase);
        }

        ClaimsPrincipal CreateDevelopmentAdminPrincipal()
        {
            ClaimsIdentity devIdentity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, DevelopmentAdminUserId),
                new Claim(ClaimTypes.Role, "Administrator")
            }, "Development");

            return new ClaimsPrincipal(devIdentity);
        }

        public void AuthorizeDevelopmentAdmin()
        {
            curUserClaims = CreateDevelopmentAdminPrincipal();
            Global.userName = DevelopmentAdminUserId;
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(curUserClaims)));
        }

        public bool IsDevelopmentAdminCredential(string userId, string password)
        {
            return IsDevelopmentEnvironment()
                && string.Equals(userId, DevelopmentAdminUserId, StringComparison.OrdinalIgnoreCase)
                && string.Equals(password, DevelopmentAdminPassword, StringComparison.Ordinal);
        }

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (IsDevelopmentEnvironment())
            {
                curUserClaims = CreateDevelopmentAdminPrincipal();
                Global.userName = DevelopmentAdminUserId;
                return new AuthenticationState(curUserClaims);
            }

            var authState = await base.GetAuthenticationStateAsync();
            var authUser = authState?.User ?? new ClaimsPrincipal(new ClaimsIdentity());
            if (authUser.Identity != null && authUser.Identity.IsAuthenticated)
            {
                try
                {
                    string winId = authUser.Identity.Name;
                    User winuser = Global.db?.GetWinUser(winId);
                    if (winuser == null)
                    {
                        if (IsDevelopmentEnvironment())
                        {
                            curUserClaims = CreateDevelopmentAdminPrincipal();
                            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(curUserClaims)));
                            return new AuthenticationState(curUserClaims);
                        }

                        return new AuthenticationState(authUser);
                    }

                    ClaimsIdentity identity;
                    identity = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, winuser.userId),
                        new Claim(ClaimTypes.Role, winuser.userPrivilegeStr)
                    }, "Role");

                    List<ClaimsIdentity> list = (List<ClaimsIdentity>)authUser.Identities;
                    if (list.Count == 1)
                    {
                        authUser.AddIdentity(identity);
                        newLogin = true;
                    }
                    else
                    {
                        newLogin = false;
                    }
                    curUserClaims = new ClaimsPrincipal(authUser.Identities);

                    NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(curUserClaims)));

                    return new AuthenticationState(authUser);
                }
                catch (Exception ex)
                {
                    Global.logger?.LogMessageEx("Error", "Error resolving authentication state: {0}", ex.Message);
                    if (IsDevelopmentEnvironment())
                    {
                        curUserClaims = CreateDevelopmentAdminPrincipal();
                        return new AuthenticationState(curUserClaims);
                    }
                    return new AuthenticationState(authUser);
                }
            }

            return new AuthenticationState(authUser);
        }

        // Authenticate password only
        public void AuthenticateUser(Database db, string userId, string password)
        {
            if (IsDevelopmentAdminCredential(userId, password))
            {
                AuthorizeDevelopmentAdmin();
                return;
            }

            User user = db.GetUser(userId);
            if (user == null) return;

            ClaimsIdentity identity;
            if (user.password == Util.EncryptPassword(password, user.privilege))
            {
                identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.userId),
                    new Claim(ClaimTypes.Role, user.userPrivilegeStr)
                }, "Role");

                if (!Global.db.UpdateUserLogin(user))
                {
                    identity = new ClaimsIdentity();
                    Global.logger.LogMessageEx("Error", "Error updating user last login. Authentication failed.");
                }
            }
            else
            {
                identity = new ClaimsIdentity();
            }
            
            curUserClaims = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(curUserClaims)));
        }

        // 2-Stage authentication: password
        public bool AuthenticateUserPassword(Database db, string userId, string password)
        {
            if (IsDevelopmentAdminCredential(userId, password))
            {
                return true;
            }

            User user = db.GetUser(userId);
            if (user == null) return false;

            if (user.password == Util.EncryptPassword(password, user.privilege))
            {
                return true;
            }

            return false;
        }

        public bool FirstTimeLogin(Database db, string userId)
        {
            User user = db.GetUser(userId);
            if (user == null) return false;

            if (user.lastLogin == null)
            {
                return true;
            }

            return false;
        }

        public bool HasSecret(Database db, string userId)
        {
            User user = db.GetUser(userId);
            if (user == null) return false;

            if (user.secret != null && user.secret.Length > 0)
            {
                return true;
            }

            return false;
        }

        public string SetupQRCodeForOTP(Database db, string userId)
        {
            bool status;

            User user = db.GetUser(userId);
            if (user == null) return null;

            string secret = Global.twoFA.CreateSecret(160);
            status = db.UpdateUserOTPSecret(user, secret, "system");
            if (!status) return null;

            string dataUri = Global.twoFA.GetQrCodeImageAsDataUri(user.userId, secret);
            return dataUri;
        }

        // 2-Stage authentication: OTP
        public bool AuthenticateUserOTP(Database db, string userId, string otp)
        {
            bool verified = false;
            ClaimsIdentity identity;

            User user = db.GetUser(userId);
            if (user == null) return false;

            string decSecret = null;
            if (user.secret.Length > 0)
            {
                byte[] bytes = Convert.FromBase64String(user.secret);

                decSecret = AesEncryption.AesDecrypt(bytes, Global.aesKey, Global.aesIV);

                verified = Global.twoFA.VerifyCode(decSecret, otp);
                if (verified)
                {
                    identity = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, user.userId),
                        new Claim(ClaimTypes.Role, user.userPrivilegeStr)
                    }, "Role");

                    if (!Global.db.UpdateUserLogin(user))
                    {
                        identity = new ClaimsIdentity();
                        Global.logger.LogMessageEx("Error", "Error updating user last login. Authentication failed.");
                    }
                }
                else
                {
                    identity = new ClaimsIdentity();
                }

                curUserClaims = new ClaimsPrincipal(identity);

                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(curUserClaims)));
            }

            return verified;
        }

        public void AuthorizeUser(Database db, string userId)
        {
            if (IsDevelopmentEnvironment())
            {
                AuthorizeDevelopmentAdmin();
                return;
            }

            User user = db.GetUser(userId);
            if (user == null) return;

            ClaimsIdentity identity;
            identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.userId),
                new Claim(ClaimTypes.Role, user.userPrivilegeStr)
            }, "Role");


            curUserClaims = new ClaimsPrincipal(identity);
            Global.userName = user.userId;

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(curUserClaims)));
        }

        public async Task<User> AuthorizeWinUser(Database db)
        {
            var authState = await base.GetAuthenticationStateAsync();
            var authUser = authState.User;
            if (authUser.Identity.IsAuthenticated)
            {
                string winId = authUser.Identity.Name;

                User user = db.GetWinUser(winId);
                if (user == null) return null;

                ClaimsIdentity identity;
                identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.userId),
                    new Claim(ClaimTypes.Role, user.userPrivilegeStr)
                }, "Role");

                curUserClaims = new ClaimsPrincipal(identity);

                authUser.AddIdentity(identity);

                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(curUserClaims)));

                return user;
            }

            return null;
        }

        public void LogoutUser(Database db, string userid)
        {
            ClaimsIdentity identity = new ClaimsIdentity();
            curUserClaims = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(curUserClaims)));
        }

    }
}
