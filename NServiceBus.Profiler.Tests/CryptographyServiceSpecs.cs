using Machine.Specifications;
using NServiceBus.Profiler.Desktop.Core.Licensing;

namespace NServiceBus.Profiler.Tests.Cryptography
{
    [Subject("encryption")]
    public class with_encryption_service
    {
        protected static CryptographyService Crypto;
        protected static string ClearString = "This is a string to encrypt";
        protected static string EncryptedString = string.Empty;
        protected static string DecryptedString = string.Empty;

        Establish context = () => Crypto = new CryptographyService();
    }

    public class when_encrypting_a_string : with_encryption_service
    {
        Because of = () => EncryptedString = Crypto.Encrypt(ClearString);

        It should_have_encrypted_string = () => EncryptedString.ShouldNotBeEmpty();
        It should_not_be_the_same_as_clear_string = () => EncryptedString.ShouldNotEqual(ClearString);
    }

    public class when_decrypting_an_encrypted_string : with_encryption_service
    {
        Establish context = () => EncryptedString = Crypto.Encrypt(ClearString);

        Because of = () => DecryptedString = Crypto.Decrypt(EncryptedString);

        It should_decrypt_the_string = () => DecryptedString.ShouldNotBeEmpty();
        It should_decrypt_the_encrypted_string_back_to_original_string = () => DecryptedString.ShouldEqual(ClearString);
    }
}