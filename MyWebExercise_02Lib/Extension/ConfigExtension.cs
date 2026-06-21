using System.Runtime.InteropServices;
//using System.Security;
using System.Text;

namespace Microsoft.Extensions.Configuration
{
        public enum DeCodeType { NONE, Base64 }

    /// <summary>
    /// 提供Configuration 取得連線字串(Base64)
    /// </summary>
    /// <param name="ConnectionName">連線字串名稱</param>
    /// <returns>string</returns>
    public static class ConfigExtension
    {
        public static string? GetConnectionString(this IConfiguration configuration, string name, DeCodeType deCodeType = DeCodeType.NONE)
        {
            string? result;

            try
            {
                string connectionString = configuration?.GetConnectionString(name) ?? string.Empty;
                if (string.IsNullOrEmpty(connectionString)) 
                    result = string.Empty;

                //if (IsBase64String(connectionString))
                //    result = Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(Base64DeCode(connectionString, Encoding.UTF8)));
                //else
                    result = connectionString;
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// 用Base64進行解碼
        /// </summary>
        /// <param name="value"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        //private static SecureString Base64DeCode(string value, Encoding encode)
        //{
        //    byte[] bytes = Convert.FromBase64String(value);
        //    SecureString secureString = new SecureString();
        //    foreach (char ch in encode.GetString(bytes).ToCharArray())
        //        secureString.AppendChar(ch);
        //    secureString.MakeReadOnly();

        //    return secureString;
        //}

        /// <summary>
        /// 用Base64進行編碼
        /// </summary>
        /// <param name="value"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        private static string Base64EnCode(string value, Encoding encode)
        {
            byte[] bytes = encode.GetBytes(value);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// 判斷是否為Base64字串
        /// </summary>
        /// <param name="base64Text"></param>
        /// <returns></returns>
        public static bool IsBase64String(string base64Text)
        {
            if (string.IsNullOrWhiteSpace(base64Text)) return false;

            try
            {
                // Base64 的長度必須為 4 的倍數
                if (base64Text.Length % 4 != 0) return false;

                Convert.FromBase64String(base64Text);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
