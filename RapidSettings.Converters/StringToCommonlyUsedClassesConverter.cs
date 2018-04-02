using System;
using System.Net.Mail;

namespace RapidSettings.Converters
{
    /// <summary>
    /// Provides converting methods for commonly used (in settings) types: <see cref="Uri"/> and <see cref="MailAddress"/>.
    /// </summary>
    /// <remarks>
    /// String form of <see cref="MailAddress"/> should be like 'My Display Name#myemail@mail.com'.
    /// Display name (all before "#" and "#" itself) is optional.
    /// </remarks>
    public class StringToCommonlyUsedClassesConverter : RawSettingsConverterBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringToCommonlyUsedClassesConverter"/> class.
        /// </summary>
        public StringToCommonlyUsedClassesConverter()
        {
            AddSupportForTypes(typeof(string), typeof(Uri), (rawValue, type) =>
               rawValue == null
               ? throw new ArgumentNullException(nameof(rawValue))
               : System.Convert.ChangeType(new Uri((string)rawValue), typeof(Uri))
            );
            AddSupportForTypes(typeof(string), typeof(MailAddress), (rawValue, type) =>
                rawValue == null
                ? throw new ArgumentNullException(nameof(rawValue))
                : this.ConvertToMailAddress((string)rawValue)
            );
        }

        /// <summary>
        /// Converts mail address as string to <see cref="MailAddress"/>. 
        /// </summary>
        /// <param name="mailAddress"><see cref="MailAddress"/> as string in form like 'My Display Name#myemail@mail.com'. Display name (all before "#" and "#" itself) is optional.</param>
        /// <returns><paramref name="mailAddress"/> converted to <see cref="MailAddress"/>.</returns>
        protected virtual MailAddress ConvertToMailAddress(string mailAddress)
        {
            var mailAddressSplitted = mailAddress.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
            if (mailAddressSplitted.Length == 2)
            {
                return new MailAddress(mailAddressSplitted[1], mailAddressSplitted[0]);
            }
            else if (mailAddressSplitted.Length == 1)
            {
                return new MailAddress(mailAddressSplitted[0]);
            }

            return null;
        }
    }
}
