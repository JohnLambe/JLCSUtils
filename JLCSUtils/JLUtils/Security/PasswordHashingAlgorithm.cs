using JohnLambe.Util.Encoding;
using JohnLambe.Util.Math;
using JohnLambe.Util.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Security
{
    public class PasswordHashingAlgorithm
    {
        public PasswordHashingAlgorithm()
        {
            KdfIterations = (int)(BaseInterationsCount * System.Math.Pow(AnnualIterationsMultiplier, System.Math.Max(0, DateTime.Now.Year - BaseYear)));
        }

        /// <summary>
        /// The year to which <see cref="BaseInterationsCount"/> applies.
        /// If the system year is earlier than this, the number of iterations for this year is used (to reduce the impact of the clock being wrong).
        /// </summary>
        public static int BaseYear { get; set; } = 2016;
        /// <summary>
        /// Number of interations to use in the year <see cref="BaseYear"/>.
        /// </summary>
        public static int BaseInterationsCount { get; set; } = 2000;   // this is low - chosen for speed
        /// <summary>
        /// Multiply the iterations count by this for each year after <see cref="BaseYear"/>.
        /// </summary>
        public static double AnnualIterationsMultiplier { get; set; } = 1.5;
        // This is low, in case the hardware isn't being upgraded at the same rate as technological advancement.
        // Moore's Law suggests a value of 1.58 (doubling every 18 months).

        /// <summary>
        /// Pepper - a static value combined with all passwords.
        /// <para>Consumers of this library can assign their own value system-wide.</para>
        /// </summary>
        public virtual string Pepper { protected get; set; } = "a26d0f9ed7364c8684034e8eb80f76a1";

        /// <summary>
        /// Number of iterations of the key derivation function to do, when generating new hashes, and the default assumed number of iterations
        /// for decoding, if the number of iterations is not specified.
        /// <para>The default set in the constructor increases each year, but is low (for speed). You should choose a value according to your security requirements.</para>
        /// </summary>
        public virtual int KdfIterations { get; set; }

        /// <summary>
        /// Hash size in bytes.
        /// </summary>
        public virtual int HashSize { get; set; } = 384 / 8; // 384 bits

        /// <summary>
        /// Indentifies this algorithm/encoding.
        /// Future versions of this library may use simple integer IDs.
        /// <para>
        /// Third party software (subclasses or systems that may store a password in the same field as one from this library)
        /// should use globally-unique values in one of the following formats:
        /// a standard GUID, represented as hexadecimal digits only, in lower case;<br/>
        /// a name that would be valid as a conventional Java namespace or class name (backwards domain name which is owned by the creator of the implementation
        /// (or is used with permission of the domain owner));<br/>
        /// any of the above immediately followed by ":", ";" or "|", followed by any string.<br/>
        /// Future versions of this library will not use values beginning with "#", so these can be used in a closed system (where these IDs are controlled)
        /// to ensure compatibility with future versions.
        /// </para>
        /// </summary>
        protected const string EncodingType = "1";

        /// <summary>
        /// Separator used in encoding all required values (Encdoing, Hash and Salt) into a string.
        /// </summary>
        protected const char Separator = ' ';
        /// <summary>
        /// Separator used within the value to be hashed, between the Password and Pepper.
        /// </summary>
        protected const string HashSeparator = "\t";

        /// <summary>
        /// Returns how long to delay responding when a wrong password is entered (to slow down dictionary or brute force attacks, or users manually guessing passwords).
        /// <para>
        /// A small delay (if it can't be cicrumvented) is enough to make attacks using a large dictionary or brute force infeasible.
        /// Long delays can be a denial of service vulnerability (an attacker could repeatedly make wrong attempts to delay/block the genuine user from logging in,
        /// or could make many concurrent attempts in order to consumer server resources).
        /// Servers could apply the wrong password account per source IP address.
        /// </para>
        /// <para>
        /// A random adjustment is used to make it difficult for an attacker to infer information based on how long it took to respond.
        /// For example, if a hash is compared byte-by-byte, the time to respond might otherwise give an indication of the point at which a difference was encountered.
        /// </para>
        /// <para>
        /// It is recommended that, unless an account is likely to have multiple workstations logged in to simultaneously, a server should not allow
        /// concurrent login attempts for the same account, or should not process them concurrently (so that an attacker cannot circumvent the delay by
        /// making concurrent requests, so that the delay does not reduce the frequency of login attempts).
        /// </para>
        /// </summary>
        /// <param name="failedAttempts">Number of consecutive failed attempts, including the current one.
        /// If the current attempt is successful, the number of previous failed attempts should be supplied
        /// (so that an attacker cannot tell, from the delay, that it will fail).
        /// Returns 0 if the current and previous attempts were successful.
        /// </param>
        /// <param name="client">true iff the delay is implemented on the client.
        /// The recommended delay is much lower when it is the server that delays, since it could cause a denial of service vulnerability
        /// (the server wouldn't be using additional processor time, but would use additional resources by keeping the connection open longer,
        /// and an attacker could make many concurrent requests).
        /// <para>Defaults to false.</para>
        /// </param>
        /// <returns>the time to delay, in milliseconds.</returns>
        /// <remarks>
        /// <para>
        /// A possible way of reducing the denial-of-service risk would be to require a client to reconnect after a specified time interval (given by the server in the response to the login attempt),
        /// to complete the login request, and find out whether their credentials were accepted. The first response would provide a token (such as a random GUID generated by the server) to be provided on the second request.
        /// The server would respond to such requests made too early, in the same way as it responds when the credentials are invalid.
        /// It would hold details of such valid pending requests until either the second call was made, or a short time after the time at which it is allowed.
        /// For failed requests, it would consume no additional resources (it wouldn't hold the details of the request).
        /// Since the client knows what the delay will be before waiting, the delay on a valid request (at least when the subsequent one was invalid)
        /// would have to be same as if that request was invalid (so that the client can't tell whether it is valid based on the delay). (This would require adjusting <paramref name="failedAttempts"/>.)
        /// </para>
        /// <para>
        /// Servers could expire passwords earlier if there are a lot of failed login attempts (so that if a dictionary or brute-force attack is 
        /// being done over a long period of time, the password would be likely to change before the attack reaches the correct one).
        /// </para>
        /// </remarks>
        public virtual int GetWrongPasswordDelay(int failedAttempts, bool client = false)
        {
            if (failedAttempts == 0)
                return 0;   // no delay
            else
                return System.Math.Min(500 + failedAttempts * (client ? 1000 : 500), (client ? 25000 : 5000)) + RandomUtil.RandomService.Next(500);
        }

        /// <summary>
        /// Returns a random salt value.
        /// </summary>
        /// <returns></returns>
        public virtual byte[] GenerateSalt()
        {
            return BinaryConverter.FromLong(RandomUtil.RandomService.NextLong() ^ DateTime.Now.Ticks);   // random value XORed with the time
        }

        /// <summary>
        /// Parses the encoding value.
        /// </summary>
        /// <param name="encoding">Specifies the algorithm/encoding and parameters to the algorithm. If null, <see cref="DefaultEncoding"/> is used.</param>
        /// <param name="iterations">The number of iterations of the KDF to be performed (according to <paramref name="encoding"/>).</param>
        /// <returns>The encoding ID (identifies the algorithm/encoding).</returns>
        protected virtual string ParseEncoding([Nullable] string encoding, out int iterations)
        {
            string encodingType, iterationsString;
            (encoding ?? DefaultEncoding).SplitToVars(';', out encodingType, out iterationsString);
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
            int iterations;   // dummy value
            ParseEncoding(encoding, out iterations);
        }

        /// <summary>
        /// Calculate a hash.
        /// </summary>
        /// <param name="encoding">Specifies the algorithm/encoding and parameters to the algorithm. If null, <see cref="DefaultEncoding"/> is used.</param>
        /// <param name="salt">The salt value.</param>
        /// <param name="password"></param>
        /// <returns>The hashed salted (with pepper applied) password.</returns>
        [return: NotNull]
        public virtual byte[] HashPassword([Nullable] string encoding, byte[] salt, [Nullable] string password)
        {
            int iterations;
            ParseEncoding(encoding, out iterations);

            return new Rfc2898DeriveBytes(
                GetHashAlgorithm().ComputeHash(StringToBinary(password)), salt, iterations)
                    .GetBytes(HashSize);
            // The KDF is applied last since it can provide an arbitrary (configurable) output size.
            // The encoding of the string to bytes is done in HashString.
            // We don't use the character encoding feature of Rfc2898DeriveBytes because
            // its XML comments do not document what encoding it uses
            // (needed for interoperability if other (possibly non .NET) systems share passwords with this).
        }

        /// <summary>
        /// Converts a string (password) to a byte array, applying the pepper, but not the salt.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        [return: NotNull]
        protected virtual byte[] StringToBinary([Nullable] string password)
        {
            return System.Text.Encoding.UTF8.GetBytes(password.NullToBlank() + HashSeparator + Pepper);
        }

        /// <summary>
        /// Returns an instance of the hash algorithm class.
        /// This is used before the KDF, and converts the cleartext password into binary.
        /// </summary>
        /// <returns></returns>
        [return: NotNull]
        protected virtual HashAlgorithm GetHashAlgorithm()
        {
            return SHA384.Create();
        }

        /// <summary>
        /// String representations of the hash value of a given password with the given metadata.
        /// </summary>
        /// <param name="hashMetadata">A value in the form returned by <see cref="RemoteHashedPassword.HashMetadata"/></param>
        /// <param name="password"></param>
        /// <returns>The password hash.</returns>
        /// <seealso cref="GetHashAsString(string, string)"/>
        [return: NotNull]
        public virtual byte[] GetHashAsBytes(string hashMetadata, [Nullable] string password)
        {
            string encoding;
            byte[] salt, hash;
            StringDecode(hashMetadata, out encoding, out salt, out hash);
            return HashPassword(encoding, salt, password);
        }

        /// <summary>
        /// String representations of the hash value (that would be returned by <see cref="GetHashAsBytes(string, string)"/>)
        /// of a given password with the given metadata.
        /// </summary>
        /// <param name="hashMetadata">The hash metadata, in the format of <seealso cref="RemoteHashedPassword.HashMetadata"/>.</param>
        /// <param name="password">The cleartext password to be hashed.</param>
        /// <returns>The string representation of the password hash.</returns>
        public virtual string GetHashAsString(string hashMetadata, [Nullable] string password)
        {
            return Convert.ToBase64String(GetHashAsBytes(hashMetadata, password));
        }

        /// <summary>
        /// Convert to the string encoding (as held in <see cref="HashedPassword.EncodedValue"/>).
        /// </summary>
        /// <param name="encoding">The encoding for this password hash.</param>
        /// <param name="salt">The salt value.</param>
        /// <param name="hash">The password hash value. null to encode only the metadata.</param>
        /// <returns>The string encoding of the given values.</returns>
        /// <seealso cref="StringDecode(string, out string, out byte[], out byte[])"/>
        public virtual string StringEncode(string encoding, byte[] salt, [Nullable] byte[] hash = null)
        {
            return (encoding ?? EncodingType)
                + Separator + Convert.ToBase64String(salt)
                + (hash != null ?
                    Separator + Convert.ToBase64String(hash)
                    : ""
                );
        }

        /// <summary>
        /// Convert from the string encoding (as held in <see cref="HashedPassword.EncodedValue"/>)
        /// to the three parts.
        /// </summary>
        /// <param name="value">The string to be decoded, in the format of <seealso cref="HashedPassword.EncodedValue"/>.</param>
        /// <param name="encoding">The encoding specified in <paramref name="value"/>.</param>
        /// <param name="salt">The salt value.</param>
        /// <param name="hash">The hash value. null if there was no hash in <paramref name="value"/>.</param>
        /// <seealso cref="StringEncode"/>
        public virtual void StringDecode(string value, out string encoding, out byte[] salt, out byte[] hash)
        {
            string saltString, hashString = null;
            value.SplitToVars(Separator, out encoding, out saltString, out hashString);
            ValidateEncoding(encoding);
            salt = Convert.FromBase64String(saltString);
            hash = hashString == null ? null : Convert.FromBase64String(hashString);
        }

        /// <summary>
        /// Convert from the string encoding (as held in <see cref="HashedPassword.EncodedValue"/>)
        /// to the three parts.
        /// </summary>
        /// <param name="value">The string to be decoded, in the format of <seealso cref="HashedPassword.EncodedValue"/>.</param>
        /// <param name="encoding">The encoding specified in <paramref name="value"/>.</param>
        /// <param name="salt">The salt value.</param>
        /// <seealso cref="StringDecode(string, out string, out byte[], out byte[])"/>
        public virtual void StringDecode(string value, out string encoding, out byte[] salt)
        {
            byte[] hashDummy;
            StringDecode(value, out encoding, out salt, out hashDummy);
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
}
