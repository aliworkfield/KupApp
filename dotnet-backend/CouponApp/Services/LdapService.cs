using System.DirectoryServices.Protocols;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace CouponApp.Services
{
    public class LdapService
    {
        private readonly IConfiguration _configuration;
        private readonly string _ldapServer;
        private readonly string _ldapBaseDn;
        private readonly int _ldapPort;
        private readonly bool _useSsl;

        public LdapService(IConfiguration configuration)
        {
            _configuration = configuration;
            _ldapServer = _configuration["Ldap:Server"] ?? "localhost";
            _ldapBaseDn = _configuration["Ldap:BaseDn"] ?? "dc=example,dc=com";
            _ldapPort = int.Parse(_configuration["Ldap:Port"] ?? "389");
            _useSsl = bool.Parse(_configuration["Ldap:UseSsl"] ?? "false");
        }

        public async Task<bool> AuthenticateUserAsync(string username, string password)
        {
            // If using domain\username format, extract just the username
            var parts = username.Split('\\');
            var userPart = parts.Length > 1 ? parts[1] : username;

            try
            {
                // Construct the user DN
                var userDn = $"cn={userPart},{_ldapBaseDn}";

                // Create connection
                var identifier = new LdapDirectoryIdentifier(_ldapServer, _ldapPort, true, false);
                var connection = new LdapConnection(identifier)
                {
                    AuthType = AuthType.Basic
                };

                // Bind with user credentials
                var credential = new NetworkCredential(userPart, password);
                await Task.Run(() => connection.Bind(credential));

                // If we get here, authentication was successful
                connection.Dispose();
                return true;
            }
            catch (LdapException)
            {
                // Authentication failed
                return false;
            }
            catch (Exception)
            {
                // Other error occurred
                return false;
            }
        }

        public async Task<LdapUserInfo?> GetUserInfoAsync(string username)
        {
            // If using domain\username format, extract just the username
            var parts = username.Split('\\');
            var userPart = parts.Length > 1 ? parts[1] : username;

            try
            {
                // Construct the user DN
                var userDn = $"cn={userPart},{_ldapBaseDn}";

                // Create connection
                var identifier = new LdapDirectoryIdentifier(_ldapServer, _ldapPort, true, false);
                var connection = new LdapConnection(identifier)
                {
                    AuthType = AuthType.Basic
                };

                // Bind with service account or anonymous
                // In a real implementation, you would use a service account
                await Task.Run(() => connection.Bind());

                // Search for user
                var searchRequest = new SearchRequest(
                    _ldapBaseDn,
                    $"(cn={userPart})",
                    SearchScope.Subtree,
                    "cn", "mail", "displayName");

                var response = (SearchResponse)await Task.Run(() => connection.SendRequest(searchRequest));

                if (response.Entries.Count > 0)
                {
                    var entry = response.Entries[0];
                    var userInfo = new LdapUserInfo
                    {
                        Username = entry.Attributes["cn"]?.GetValues(typeof(string))[0]?.ToString() ?? userPart,
                        Email = entry.Attributes["mail"]?.GetValues(typeof(string))[0]?.ToString() ?? $"{userPart}@company.com",
                        DisplayName = entry.Attributes["displayName"]?.GetValues(typeof(string))[0]?.ToString() ?? userPart
                    };

                    connection.Dispose();
                    return userInfo;
                }

                connection.Dispose();
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    public class LdapUserInfo
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
    }
}