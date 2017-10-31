using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Security
{
    /// <summary>
    /// Hashed password which is entered on a client of the machine that validates it,
    /// and the client does the hashing.
    /// </summary>
    public class RemoteHashedPassword : HashedPassword
    {
        public RemoteHashedPassword(PasswordHashingAlgorithm algorithm = null) : base(algorithm)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">Binary hash value.</param>
        /// <returns>True iff the given hash matches that of the stored password.</returns>
        public bool TestHash(byte[] value)   // not virtual, bacause TestHashInternal should be overridden instead.
        {
            return TestHashInternal(value);
        }

        /// <summary>
        /// Test whether a given hash value matches the stored value (to test a password).
        /// </summary>
        /// <param name="value">Base-64 encoded hash value: A password hash creating using the same settings as the stored password, provided by <seealso cref="HashMetadata"/>.</param>
        /// <param name="fromClient"></param>
        /// <returns>True iff the given hash matches that of the stored password.</returns>
        public virtual bool TestHash(string value, bool fromClient = false)
        {
            return TestHashInternal(Convert.FromBase64String(value));
        }

        protected virtual bool TestHashInternal(byte[] hash, bool fromClient = false)
        {
            return base.TestHashInternal(PreprocessHash(hash, fromClient));
        }

        protected virtual byte[] PreprocessHash(byte[] hash, bool fromClient = false)
        {
            return hash;
        }

        protected void SetHashInternal(byte[] hash, bool fromClient = false)
        {
            HashInternal = PreprocessHash(hash, fromClient);
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
            set
            {
                string encoding;
                byte[] salt;
                Algorithm.StringDecode(value, out encoding, out salt);
                Encoding = encoding;
                SaltInternal = salt;
            }
        }

        /*
        public virtual bool ChangePasswordByHash(string oldPasswordHash, string newPassword)
        {
            return ChangePasswordByHash(Convert.FromBase64String(oldPasswordHash), Convert.FromBase64String(newPassword));
        }
        */

        /// <summary>
        /// Change the password, providing the hash of the current password and new encoded password value (in the format of <see cref="HashedPassword.EncodedValue"/>).
        /// </summary>
        /// <param name="oldPasswordHash">Hash of the current password.</param>
        /// <param name="newPasswordEncodedValue">new encoded password value (in the format of <see cref="HashedPassword.EncodedValue"/>).</param>
        /// <param name="fromClient"></param>
        /// <returns>true iff successful. false if the old password was wrong.</returns>
        /// <exception cref="PasswordRejectedException"/>
        public virtual bool ChangePasswordByHash(string oldPasswordHash, string newPasswordEncodedValue, bool fromClient = false)
        {
            if (!TestHash(oldPasswordHash,fromClient))
            {
                return false;
            }
            else
            {
                ValidatePasswordHash(newPasswordEncodedValue);
                SetEncodedValue(newPasswordEncodedValue,fromClient);
                return true;
            }
        }

        protected virtual void SetEncodedValue(string newPasswordEncodedValue, bool fromClient = false)
        {
            if (!fromClient)
            {
                EncodedValue = newPasswordEncodedValue;
            }
            else
            {
                string encoding;
                byte[] salt;
                byte[] hash;

                Algorithm.StringDecode(newPasswordEncodedValue, out encoding, out salt, out hash);
                Encoding = encoding;
                SaltInternal = salt;
                SetHashInternal(hash, fromClient);
            }
        }

        /// <summary>
        /// Change the password, providing the old and new hash values.
        /// </summary>
        /// <param name="oldPasswordHash">Hash of the current password.</param>
        /// <param name="newPassword">The new password in the format of <see cref="HashedPassword.BinaryValue"/>.</param>
        /// <param name="fromClient"></param>
        /// <returns>true iff successful. false if the old password was wrong.</returns>
        /// <exception cref="PasswordRejectedException"/>
        public virtual bool ChangePasswordByHash(byte[] oldPasswordHash, byte[] newPassword, bool fromClient = false)
        {
            if (!TestHash(oldPasswordHash))
            {
                return false;
            }
            else
            {
                ValidatePasswordHash(newPassword);
                BinaryValue = newPassword;
                return true;
            }
        }

        /// <summary>
        /// Validate a password hash and settings.
        /// </summary>
        /// <param name="passwordEncoded"></param>
        /// <exception cref="PasswordRejectedException">If the given password hash is not compatible with this instance, or does not meet security requirements.</exception>
        public virtual void ValidatePasswordHash(string passwordEncoded)
        {
            //TOOD: Validate compatibility and security

        }
        public virtual void ValidatePasswordHash(byte[] passwordEncoded)
        {
            //TOOD: Validate compatibility and security
        }

        /// <summary>
        /// Returns a hash of a the given password using the salt and settings of the current one.
        /// </summary>
        /// <param name="password">Cleartext password.</param>
        /// <returns>The calculated hash.</returns>
        public virtual byte[] GetPasswordHash(string password)
        {
            return Algorithm.GetHashAsBytes(HashMetadata, password);
            // same as: return HashPassword(password);
        }

        /// <summary>
        /// Returns a hash of a the given password using the salt and settings of the current one.
        /// </summary>
        /// <param name="password">Cleartext password.</param>
        /// <returns>The calculated hash.</returns>
        /// <seealso cref="GetPasswordHash"/>
        public virtual string GetPasswordHashString(string password)
        {
            return Algorithm.GetHashAsString(HashMetadata, password);
        }

        /*
        public virtual void SetPasswordByHash(string hashMetadata, byte[] hash)
        {
            string encoding;
            byte[] salt;
            Algorithm.StringDecode(hashMetadata, out encoding, out salt);
            HashInternal = hash;
            ...
        }
        */

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


    /// <summary>
    /// Class for use on a client of a server using <seealso cref="RemoteHashedPassword"/>.
    /// </summary>
    public class RemoteHashedPasswordClient
    {
        public RemoteHashedPasswordClient(PasswordHashingAlgorithm algorithm = null)
        {
            _hashedPassword = new RemoteHashedPassword(algorithm);
        }

        protected RemoteHashedPassword _hashedPassword;

        /// <summary>
        /// 
        /// </summary>
        /// <seealso cref="RemoteHashedPassword.HashMetadata"/>
        public virtual string HashMetadata
        {
            get { return _hashedPassword.HashMetadata; }
            set
            {
                _hashedPassword.HashMetadata = value;
                MetadataAssigned = value != null;
            }
        }

        /// <summary>
        /// true iff <seealso cref="HashMetadata"/> has been assigned.
        /// </summary>
        public virtual bool MetadataAssigned { get; protected set; } = false;

        /// <summary>
        /// Returns an encoded password that can be used for providing a new password to the server (to change the password).
        /// </summary>
        /// <param name="password">Cleartext new password.</param>
        /// <returns></returns>
        public virtual string GetNewPasswordString(string password)
        {
            EnsureInitialized();
            _hashedPassword.Value = password;
            return _hashedPassword.EncodedValue;
        }

        /// <summary>
        /// Returns an encoded password that can be used for providing a new password to the server (to change the password).
        /// </summary>
        /// <param name="password">Cleartext new password.</param>
        /// <returns></returns>
        public virtual byte[] GetNewPassword(string password)
        {
            EnsureInitialized();
            _hashedPassword.Value = password;
            return _hashedPassword.BinaryValue;
        }

        /// <summary>
        /// Returns a password hash string that can be used to test the password (e.g. for login) stored on the server.
        /// </summary>
        /// <param name="password">Cleartext password.</param>
        /// <returns></returns>
        public virtual byte[] GetTestPasswordHash(string password)
        {
            EnsureInitialized();
            return _hashedPassword.GetPasswordHash(password);
        }

        /// <summary>
        /// Returns a password hash string that can be used to test the password (e.g. for login) stored on the server.
        /// </summary>
        /// <param name="password">Cleartext password.</param>
        /// <returns></returns>
        public virtual string GetTestPasswordHashString(string password)
        {
            EnsureInitialized();
            return _hashedPassword.GetPasswordHashString(password);
        }

        protected virtual void EnsureInitialized()
        {
            if (!MetadataAssigned)
                throw new InvalidOperationException("HashMetadata not assigned");
        }
    }
}
