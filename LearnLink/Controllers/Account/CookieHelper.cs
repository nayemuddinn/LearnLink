using System;
using System.Web;
using System.Security.Cryptography;
using System.Text;

namespace LearnLink.Controllers.Account
{
   
    public static class CookieHelper
    {
      
        private const string USER_ID_COOKIE = "LearnLink_UserID";
        private const string USER_ROLE_COOKIE = "LearnLink_UserRole";
        private const string USER_NAME_COOKIE = "LearnLink_UserName";
        private const string USER_EMAIL_COOKIE = "LearnLink_UserEmail";

      
        private const int COOKIE_EXPIRATION_DAYS = 30;

      
        public static void SetLoginCookies(int userId, string userRole, string userName, string userEmail)
        {
            try
            {
                HttpResponse response = HttpContext.Current.Response;

                // Create cookies with encrypted sensitive data
                HttpCookie userIdCookie = new HttpCookie(USER_ID_COOKIE, userId.ToString())
                {
                    HttpOnly = true,
                    Secure = HttpContext.Current.Request.IsSecureConnection,
                    Expires = DateTime.Now.AddDays(COOKIE_EXPIRATION_DAYS),
                    SameSite = SameSiteMode.Lax,
                    Path = "/"
                };

                HttpCookie userRoleCookie = new HttpCookie(USER_ROLE_COOKIE, EncryptData(userRole))
                {
                    HttpOnly = true,
                    Secure = HttpContext.Current.Request.IsSecureConnection,
                    Expires = DateTime.Now.AddDays(COOKIE_EXPIRATION_DAYS),
                    SameSite = SameSiteMode.Lax,
                    Path = "/"
                };

                HttpCookie userNameCookie = new HttpCookie(USER_NAME_COOKIE, EncryptData(userName))
                {
                    HttpOnly = true,
                    Secure = HttpContext.Current.Request.IsSecureConnection,
                    Expires = DateTime.Now.AddDays(COOKIE_EXPIRATION_DAYS),
                    SameSite = SameSiteMode.Lax,
                    Path = "/"
                };

                HttpCookie userEmailCookie = new HttpCookie(USER_EMAIL_COOKIE, EncryptData(userEmail))
                {
                    HttpOnly = true,
                    Secure = HttpContext.Current.Request.IsSecureConnection,
                    Expires = DateTime.Now.AddDays(COOKIE_EXPIRATION_DAYS),
                    SameSite = SameSiteMode.Lax,
                    Path = "/"
                };

                response.Cookies.Add(userIdCookie);
                response.Cookies.Add(userRoleCookie);
                response.Cookies.Add(userNameCookie);
                response.Cookies.Add(userEmailCookie);

                System.Diagnostics.Debug.WriteLine($"✓ Login cookies set successfully for UserID={userId}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting login cookies: {ex.Message}");
            }
        }

  
        public static bool RestoreSessionFromCookies(HttpSessionStateBase session)
        {
            try
            {
                HttpRequest request = HttpContext.Current.Request;

               
                HttpCookie userIdCookie = request.Cookies[USER_ID_COOKIE];
                HttpCookie userRoleCookie = request.Cookies[USER_ROLE_COOKIE];
                HttpCookie userNameCookie = request.Cookies[USER_NAME_COOKIE];
                HttpCookie userEmailCookie = request.Cookies[USER_EMAIL_COOKIE];

                if (userIdCookie == null || userRoleCookie == null || userNameCookie == null || userEmailCookie == null)
                {
                    System.Diagnostics.Debug.WriteLine("One or more login cookies are missing");
                    return false;
                }

                if (string.IsNullOrEmpty(userIdCookie.Value) || 
                    string.IsNullOrEmpty(userRoleCookie.Value) || 
                    string.IsNullOrEmpty(userNameCookie.Value) || 
                    string.IsNullOrEmpty(userEmailCookie.Value))
                {
                    System.Diagnostics.Debug.WriteLine("One or more cookie values are empty");
                    return false;
                }

                // Decrypt and restore session data
                int userId;
                if (!int.TryParse(userIdCookie.Value, out userId))
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to parse UserID cookie: {userIdCookie.Value}");
                    return false;
                }

                string userRole = DecryptData(userRoleCookie.Value);
                string userName = DecryptData(userNameCookie.Value);
                string userEmail = DecryptData(userEmailCookie.Value);

                if (string.IsNullOrEmpty(userRole) || string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userEmail))
                {
                    System.Diagnostics.Debug.WriteLine("Failed to decrypt one or more cookie values");
                    return false;
                }

                session["UserID"] = userId;
                session["UserRole"] = userRole;
                session["UserName"] = userName;
                session["UserEmail"] = userEmail;

                System.Diagnostics.Debug.WriteLine($"✓ Successfully restored session from cookies: UserID={userId}, Role={userRole}");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error restoring session from cookies: {ex.GetType().Name} - {ex.Message}");
                return false;
            }
        }

      
        public static void ClearLoginCookies()
        {
            try
            {
                HttpResponse response = HttpContext.Current.Response;
                HttpRequest request = HttpContext.Current.Request;

                System.Diagnostics.Debug.WriteLine(">>> Starting ClearLoginCookies()");

            
                string[] cookieNames = { USER_ID_COOKIE, USER_ROLE_COOKIE, USER_NAME_COOKIE, USER_EMAIL_COOKIE };

                foreach (var cookieName in cookieNames)
                {
                   
                    if (request.Cookies[cookieName] != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"  - Clearing cookie: {cookieName}");
                    }

                 
                    HttpCookie cookie = new HttpCookie(cookieName)
                    {
                        Value = string.Empty,
                        Expires = DateTime.Now.AddDays(-1),
                        HttpOnly = true,
                        Secure = request.IsSecureConnection,
                        SameSite = SameSiteMode.Lax,
                        Path = "/"
                    };

                    response.Cookies.Add(cookie);
                    System.Diagnostics.Debug.WriteLine($"  ✓ Set-Cookie response header sent for: {cookieName}");
                }

                System.Diagnostics.Debug.WriteLine(">>> ClearLoginCookies() completed successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($">>> ERROR in ClearLoginCookies(): {ex.Message}");
            }
        }

        private static string EncryptData(string plainText)
        {
            try
            {
                byte[] dataToEncrypt = Encoding.UTF8.GetBytes(plainText);
                byte[] encryptedData = ProtectedData.Protect(dataToEncrypt, null, DataProtectionScope.CurrentUser);
                return Convert.ToBase64String(encryptedData);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error encrypting data: {ex.Message}");
                return plainText;
            }
        }

        
        private static string DecryptData(string encryptedText)
        {
            try
            {
                byte[] dataToDecrypt = Convert.FromBase64String(encryptedText);
                byte[] decryptedData = ProtectedData.Unprotect(dataToDecrypt, null, DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(decryptedData);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error decrypting data: {ex.Message}");
                return encryptedText; // Return encrypted text as fallback
            }
        }


        public static bool HasValidLoginCookies()
        {
            try
            {
                HttpRequest request = HttpContext.Current.Request;
                return request.Cookies[USER_ID_COOKIE] != null &&
                       request.Cookies[USER_ROLE_COOKIE] != null &&
                       request.Cookies[USER_NAME_COOKIE] != null &&
                       request.Cookies[USER_EMAIL_COOKIE] != null;
            }
            catch
            {
                return false;
            }
        }
    }
}
