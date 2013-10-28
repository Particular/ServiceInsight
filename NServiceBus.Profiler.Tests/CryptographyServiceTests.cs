using NServiceBus.Profiler.Desktop.Core.Licensing;
using NUnit.Framework;
using Shouldly;

namespace NServiceBus.Profiler.Tests
{
    [TestFixture]
    public class CryptographyServiceTests
    {
        private CryptographyService Crypto;
        private const string ClearString = "This is a string to encrypt";

        [SetUp]
        public void TestInitialize()
        {
            Crypto = new CryptographyService();
        }

        [Test]
        public void should_be_able_to_encrypted_string()
        {
            var encryptedString = Crypto.Encrypt(ClearString);

            encryptedString.ShouldNotBeEmpty();
            encryptedString.ShouldNotBe(ClearString);
        }

        [Test]
        public void should_be_able_to_decrypt_the_string()
        {
            var encryptedString = Crypto.Encrypt(ClearString);
            var decryptedString = Crypto.Decrypt(encryptedString);

            decryptedString.ShouldNotBeEmpty();
            decryptedString.ShouldBe(ClearString);
        }
    }
}