using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util.Math;
using System.Security.Cryptography;
using JohnLambe.Util.Encoding;
using System.IO;

namespace JohnLambe.Util.Security
{
    public class PasswordHashingAlgorithm
    {
        /// <summary>
        /// Pepper - a static value combined with all passwords.
        /// </summary>
        public virtual string Pepper { protected get; set; } = "a26d0f9ed7364c8684034e8eb80f76a1";

        /// <summary>
        /// Number of iterations of the key derivation function to do.
        /// </summary>
        public virtual int KdfIterations { get; set; } = 1500;

        /// <summary>
        /// Hash size in bytes
        /// </summary>
        public virtual int HashSize { get; set; } = 384 / 8; // 384 bits

        /// <summary>
        /// Indentifies this algorithm/encoding.
        /// Future versions of this library may use simple integer IDs.
        /// Third party software (subclasses or systems that may store a password in the same field as one from this library)
        /// should use globally-unique values in one of the following formats:
        /// a standard GUID, represented as hexadecimal digits only, in lower case;
        /// a name that would be valid as a conventional Java namespace or class name (backwards domain name which is owned by the creator of the implementation (or is used with permission of the domain owner));
        /// any of the above immediately followed by ":", ";" or "|", followed by any string.
        /// </summary>
        protected const string EncodingType = "1";

        protected const char Separator = ' ';
        protected const string HashSeparator = "\t";

        /// <summary>
        /// Returns how long to delay responding when a wrong password is entered.
        /// </summary>
        /// <param name="failedAttempts">Number of consecutive failed attempts, including the current one.
        /// If the current attempt is successful, the number of previous failed attempts should be supplied.
        /// 0 if the current and previous attempts were successful.
        /// </param>
        /// <returns>the time to delay, in milliseconds.</returns>
        public virtual int GetWrongPasswordDelay(int failedAttempts)
        {
            return System.Math.Min(failedAttempts * 500, 10000);
        }

        /// <summary>
        /// Returns a random salt value.
        /// </summary>
        /// <returns></returns>
        public virtual byte[] GenerateSalt()
        {
            return BinaryConverter.FromLong(RandomUtils.RandomPositiveLong() ^ DateTime.Now.Ticks);   // random value XORed with the time
        }

        /// <summary>
        /// Parses the encoding value.
        /// </summary>
        /// <param name="encoding"></param>
        /// <param name="iterations"></param>
        /// <returns></returns>
        protected string ParseEncoding(string encoding, out int iterations)
        {
            string encodingType, iterationsString;
            encoding.SplitToVars(';', out encodingType, out iterationsString);
            if (encodingType != EncodingType)
                throw new InvalidDataException("Unsupported password encoding: " + encodingType);

            if (iterationsString != null)
                int.TryParse(iterationsString, out iterations);
            else
                iterations = 0;
            if (iterations == 0)
                iterations = KdfIterations;

            return encodingType;
        }

        /// <summary>
        /// Throws an exception if the given encoding is invalid or unsupported.
        /// </summary>
        /// <param name="encoding"></param>
        protected virtual void ValidateEncoding(string encoding)
        {
            int iterations;
            ParseEncoding(encoding, out iterations);
        }

        public virtual byte[] HashPassword(string encoding, byte[] salt, string password)
        {
            int iterations;
            ParseEncoding(encoding, out iterations);

            return new Rfc2898DeriveBytes(HashString(password), salt, iterations)
                    .GetBytes(HashSize);
            // The KDF is applied last since it can provide an arbitrary (configurable) output size.
            // The encoding of the string to bytes is done in HashString.
            // We don't use the character encoding feature of Rfc2898DeriveBytes since
            // its XML comments do not document what encoding it uses
            // (needed for interoperability if other (possibly non .NET) systems share passwords with this).
        }

        /// <summary>
        /// Hashes a string to a byte array, applying the pepper, but not the salt.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        protected virtual byte[] HashString(string password)
        {
            return GetHashAlgorithm().ComputeHash(System.Text.Encoding.UTF8.GetBytes(password + HashSeparator + Pepper));
        }

        protected virtual HashAlgorithm GetHashAlgorithm()
        {
            return SHA384.Create();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hashMetadata">A value in the form returned by <see cref="RemoteHashedPassword.HashMetadata"/></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public virtual byte[] GetHashAsBytes(string hashMetadata, string password)
        {
            string encoding;
            byte[] salt, hash;
            StringDecode(hashMetadata, out encoding, out salt, out hash);
            return HashPassword(encoding, salt, password);
        }

        public virtual string GetHashAsString(string hashMetadata, string password)
        {
            return Convert.ToBase64String(GetHashAsBytes(hashMetadata, password));
        }

        public virtual string StringEncode(string encoding, byte[] Salt, byte[] hash = null)
        {
            return (encoding ?? EncodingType)
                + Separator + Convert.ToBase64String(Salt)
                + ( hash != null ?
                    Separator + Convert.ToBase64String(hash)
                    : ""
                );
        }

        public virtual void StringDecode(string value, out string encoding, out byte[] salt, out byte[] hash)
        {
            string saltString, hashString = null;
            value.SplitToVars(Separator, out encoding, out saltString, out hashString);
            ValidateEncoding(encoding);
            salt = Convert.FromBase64String(saltString);
            hash = hashString == null ? null : Convert.FromBase64String(hashString);
        }

        public virtual string DefaultEncoding => EncodingType + ";" + KdfIterations;

        /// <summary>
        /// Create a password hash instance.
        /// </summary>
        /// <returns></returns>
        public virtual HashedPassword Create()
        {
            return new HashedPassword(this);
        }

        public static PasswordHashingAlgorithm DefaultInstance => new PasswordHashingAlgorithm();
    }


    /// <summary>
    /// Holds a password hash and salt.
    /// </summary>
    public class HashedPassword
    {
        public HashedPassword(PasswordHashingAlgorithm algorithm = null)
        {
            this.Algorithm = algorithm ?? PasswordHashingAlgorithm.DefaultInstance;
            Encoding = Algorithm.DefaultEncoding;
        }

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
        /// (The delay is for use when the password being tested comes from a user).
        /// </summary>
        /// <param name="password"></param>
        /// <returns>true if <paramref name="password"/> matches the stored password.</returns>
        public virtual bool TestPassword(string password)
        {
            bool result = TestHashInternal(HashPassword(password));     // compare the existing hash to that of the given password with the same salt

            int previousFailedAttempts = FailedAttempts;
            if (result)
                _failedAttempts = 0;
            else
                _failedAttempts++;
            System.Threading.Thread.Sleep(Algorithm.GetWrongPasswordDelay(System.Math.Max(FailedAttempts,previousFailedAttempts)));
                // Even if the password is correct this time, we have a delay if it was previously wrong, to make it more difficult to determine that is was wrong by the fact that there is a delay.
                // Though the delay does increase immediately on a wrong password.

            return result;
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
        protected virtual byte[] HashPassword(string password)
        {
            return Algorithm.HashPassword(Encoding, SaltInternal, password);
        }

        protected virtual bool TestHashInternal(byte[] value)
        {
            for (int n = 0; n < HashInternal.Length; n++)
                if (value[n] != HashInternal[n])
                    return false;
            return true;
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
        /// </summary>
        public virtual string EncodedValue
        { 
            get
            {
                return Algorithm.StringEncode(Encoding, SaltInternal, HashInternal);
            }
            set
            {
                string encoding;
                byte[] salt, hash;
                Algorithm.StringDecode(value, out encoding, out salt, out hash);
                Encoding = encoding;
                SaltInternal = salt;
                HashInternal = hash;
            }
        }

        public virtual byte[] BinaryValue
        {
            get
            {
                return System.Text.Encoding.UTF8.GetBytes(Encoding + '\0').Concat(SaltInternal.Concat(HashInternal)).ToArray();
            }
            //TODO: set
        }

        protected readonly PasswordHashingAlgorithm Algorithm;
    }


    public class RemoteHashedPassword : HashedPassword
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">Binary hash value.</param>
        /// <returns>True iff the given hash matches that of the stored password.</returns>
        public bool TestHash(byte[] value)   // not virtual, since TestHashInternal should be overridden instead.
        {
            return TestHashInternal(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">Base-64 encoded hash value.</param>
        /// <returns>True iff the given hash matches that of the stored password.</returns>
        public virtual bool TestHash(string value)
        {
            return TestHashInternal(Convert.FromBase64String(value));
        }

        /// <summary>
        /// The information needed, in addition to the password, to generate the hash.
        /// This can be supplied to a client on which a user enters a password, so that
        /// the client does the hashing.
        /// </summary>
        public virtual string HashMetadata
        {
            get
            {
                return Algorithm.StringEncode(Encoding, SaltInternal);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldPasswordHash"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public virtual bool ChangePasswordByHash(string oldPasswordHash, string newPassword)
        {
            return ChangePasswordByHash(Convert.FromBase64String(oldPasswordHash), Convert.FromBase64String(newPassword));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldPassword">Hash of the current password.</param>
        /// <param name="newPassword">The new password in the format of <see cref="HashedPassword.BinaryValue"/>.</param>
        /// <returns></returns>
        public virtual bool ChangePasswordByHash(byte[] oldPassword, byte[] newPassword)
        {
            TestHash(oldPassword);

            throw new NotImplementedException();
        }

        /*
        public virtual byte[] Hash
        {
            get { return HashInternal; }
//            set { HashInternal = Hash; }
        }

        public virtual byte[] Salt
        {
            get { return SaltInternal; }
//            set { SaltInternal = Salt; }
        }
        */

    }
}
