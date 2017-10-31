using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util.Math;
using System.Security.Cryptography;
using JohnLambe.Util.Encoding;
using System.IO;
using JohnLambe.Util.Types;

namespace JohnLambe.Util.Security
{
    /// <summary>
    /// Holds a password hash and salt.
    /// </summary>
    public class HashedPassword
    {
        /// <summary>
        /// Initialise with a blank password.
        /// </summary>
        /// <param name="algorithm">Hashing algorithm to use. null for default.</param>
        public HashedPassword(PasswordHashingAlgorithm algorithm = null)
        {
            this.Algorithm = algorithm ?? PasswordHashingAlgorithm.DefaultInstance;
            Encoding = Algorithm.DefaultEncoding;
            Value = null;   // initialize to blank password
        }

        /// <summary>
        /// The cleartext password.
        /// </summary>
        public virtual string Value
        {
            set // assign a new password:
            {
                SaltInternal = Algorithm.GenerateSalt();    // generate a new salt
                HashInternal = HashPassword(value);         // hash with the new salt
            }
        }

        /// <summary>
        /// Tests whether a given password is correct.
        /// There is a delay in responding if this and/or previous attempts were unsuccessful.
        /// Note that the delay can be circumvented in code by using a new instance for each test.
        /// (The delay is for use when the password being tested comes from a user or a system accessing this system through an API).
        /// </summary>
        /// <param name="password"></param>
        /// <returns>true if <paramref name="password"/> matches the stored password.</returns>
        public virtual bool TestPassword(string password)
        {
            return TestHashInternal(HashPassword(password));     // compare the existing hash to that of the given password with the same salt
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
        /// Hash a password, with the salt of the current one.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        protected virtual byte[] HashPassword(string password)
        {
            return Algorithm.HashPassword(Encoding, SaltInternal, password);
        }

        /// <summary>
        /// Compare the hash to the given value.
        /// </summary>
        /// <param name="value">Hash value to compare to.</param>
        /// <returns>true iff the hash matches <paramref name="value"/>.</returns>
        protected virtual bool TestHashInternal(byte[] value)
        {
            bool result = true;
            for (int n = 0; n < HashInternal.Length; n++)
            {
                if (value[n] != HashInternal[n])
                {
                    result = false;    // different
                    break;
                }
            }

            int previousFailedAttempts = FailedAttempts;
            if (result)
                _failedAttempts = 0;
            else
                _failedAttempts++;
            System.Threading.Thread.Sleep(Algorithm.GetWrongPasswordDelay(System.Math.Max(FailedAttempts, previousFailedAttempts)));
            // Even if the password is correct this time, we have a delay if it was previously wrong, to make it more difficult to determine that is was wrong by the fact that there is a delay.
            // Though the delay does increase immediately on a wrong password.

            return result;
//            return Hash.Equals(value);
        }

        protected virtual string Encoding { get; set; }
        protected virtual byte[] HashInternal { get; set; }
        protected virtual byte[] SaltInternal { get; set; }

        /// <summary>
        /// The number of consecutive failed attempt to test a password.
        /// The can be set to a higher value (where the code using this is aware of failed attempts that didn't use this instance),
        /// but not to a lower one.
        /// </summary>
        public virtual int FailedAttempts
        {
            get { return _failedAttempts; }
            protected set
            {
                if (value > _failedAttempts)
                    _failedAttempts = value;
            }
        }
        protected int _failedAttempts = 0;

        /// <summary>
        /// Opaque string representation of the hash, salt and encoding type.
        /// This can be used for storing the value (e.g. in a database) and reconstructing this object on retrieving it.
        /// <para>Setting this to null sets a blank password. (Reading it after setting null, will not necessarily return null.)</para>
        /// </summary>
        public virtual string EncodedValue
        { 
            get
            {
                if (Encoding == null)
                    return null;
                else
                    return Algorithm.StringEncode(Encoding, SaltInternal, HashInternal);
            }
            set
            {
                if (value == null)
                {
                    Value = null;
                }
                else
                {
                    string encoding;
                    byte[] salt, hash;
                    Algorithm.StringDecode(value, out encoding, out salt, out hash);
                    Encoding = encoding;
                    SaltInternal = salt;
                    HashInternal = hash;
                }
            }
        }

        /// <summary>
        /// Opaque binary representation of the hash, salt and encoding type.
        /// This can be used for storing the value (e.g. in a database) and reconstructing this object on retrieving it.
        /// </summary>
        public virtual byte[] BinaryValue
        {
            get
            {
                return System.Text.Encoding.UTF8.GetBytes(Encoding + '\0').Concat(SaltInternal.Concat(HashInternal)).ToArray();
            }
            set
            {
                //TODO: set
                throw new NotImplementedException();
            }
        }

        protected readonly PasswordHashingAlgorithm Algorithm;
    }

}
