using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util.Math;
using System.Security.Cryptography;
using JohnLambe.Util.Encoding;

namespace JohnLambe.Util.Security
{
    public class HashedPassword
    {
        public virtual string Value
        {
            set // assign a new password:
            {
                Salt = GenerateSalt();              // generate a new salt
                Hash = HashPassword(value);         // hash with the new salt
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="password"></param>
        /// <returns>true if <paramref name="password"/>is the stored password.</returns>
        public virtual bool TestPassword(string password)
        {
            return HashPassword(Salt,password).Equals(Hash);   // compare the existing hash to that of the given password with the same salt
        }

        /// <summary>
        /// Change the password only if the old one is correct.
        /// </summary>
        /// <param name="oldPassword">Password to compare to the existing password.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns>true iff <paramref name="oldPassword"/> is correct.</returns>
        public virtual bool ChangePassword(string oldPassword, string newPassword)
        {
            if (TestPassword(oldPassword))
            {
                Value = newPassword;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Hash a password with the salt of the current one.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        protected virtual string HashPassword(string password)
        {
            return HashPassword(GenerateSalt(),password);
        }

        protected virtual string HashPassword(long salt, string password)
        {
            return HashString(password
                    + HashSeparator + Salt.ToString("X16")
                    + HashSeparator + Pepper);
        }

        protected virtual string HashString(string value)
        {
            return HexConverter.ToHex(SHA512.Create().ComputeHash(System.Text.Encoding.UTF8.GetBytes(value)));
        }

        protected virtual string Hash { get; set; }
        protected virtual long Salt { get; set; }

        /// <summary>
        /// Opaque string representation of the hash and salt.
        /// This can be used for storing the value (e.g. in a database) and reconstructing this object on retrieving it.
        /// </summary>
        public virtual string EncodedValue
        { 
            get { return EncodingType + Separator + Salt.ToString("X16") + Separator + Hash; }
            set
            {
                string encodingType, salt, hash;
                value.SplitToVars(Separator, out encodingType, out salt, out hash);
                if (encodingType != EncodingType)
                    throw new System.IO.InvalidDataException("Unsupported encoding type: " + encodingType);
                Salt = Convert.ToInt64(salt, 16);
                Hash = hash;
            }
        }

        public virtual long GenerateSalt()
        {
            return RandomUtils.RandomPositiveLong() ^ DateTime.Now.Ticks;   // random value XORed with the time
        }

        protected static Random Random { get; } = new Random();

        public static string Pepper { protected get; set; } = "";

        protected const string EncodingType = "1";
        protected const char Separator = ' ';
        protected const string HashSeparator = "\t";
    }
    //TODO:
    // Make Hashed value binary until returning EncodedValue.
    // KDF.
}
