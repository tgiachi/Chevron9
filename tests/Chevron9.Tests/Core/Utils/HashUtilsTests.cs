using System.Security.Cryptography;
using Chevron9.Core.Utils;

namespace Chevron9.Tests.Core.Utils;

[TestFixture]
public class HashUtilsTests
{
    [Test]
    public void ComputeSha256Hash_WithValidInput_ShouldReturnConsistentHash()
    {
        const string input = "Hello, World!";

        var hash1 = HashUtils.ComputeSha256Hash(input);
        var hash2 = HashUtils.ComputeSha256Hash(input);

        Assert.That(hash1, Is.EqualTo(hash2));
        Assert.That(hash1, Has.Length.EqualTo(64));
        Assert.That(hash1, Does.Match("^[a-f0-9]{64}$"));
    }

    [Test]
    public void ComputeSha256Hash_WithDifferentInputs_ShouldReturnDifferentHashes()
    {
        var hash1 = HashUtils.ComputeSha256Hash("input1");
        var hash2 = HashUtils.ComputeSha256Hash("input2");

        Assert.That(hash1, Is.Not.EqualTo(hash2));
    }

    [Test]
    public void HashPassword_ShouldReturnHashAndSalt()
    {
        const string password = "MySecurePassword123!";

        var (hash, salt) = HashUtils.HashPassword(password);

        Assert.That(hash, Is.Not.Null.And.Not.Empty);
        Assert.That(salt, Is.Not.Null.And.Not.Empty);

        var hashBytes = Convert.FromBase64String(hash);
        var saltBytes = Convert.FromBase64String(salt);

        Assert.That(hashBytes, Has.Length.EqualTo(32));
        Assert.That(saltBytes, Has.Length.EqualTo(16));
    }

    [Test]
    public void HashPassword_WithSamePassword_ShouldReturnDifferentResults()
    {
        const string password = "SamePassword";

        var (hash1, salt1) = HashUtils.HashPassword(password);
        var (hash2, salt2) = HashUtils.HashPassword(password);

        Assert.That(hash1, Is.Not.EqualTo(hash2));
        Assert.That(salt1, Is.Not.EqualTo(salt2));
    }

    [Test]
    public void CheckPasswordHash_WithCorrectPassword_ShouldReturnTrue()
    {
        const string password = "TestPassword123";

        var (hash, salt) = HashUtils.HashPassword(password);
        var isValid = HashUtils.CheckPasswordHash(password, hash, salt);

        Assert.That(isValid, Is.True);
    }

    [Test]
    public void CheckPasswordHash_WithIncorrectPassword_ShouldReturnFalse()
    {
        const string password = "TestPassword123";
        const string wrongPassword = "WrongPassword456";

        var (hash, salt) = HashUtils.HashPassword(password);
        var isValid = HashUtils.CheckPasswordHash(wrongPassword, hash, salt);

        Assert.That(isValid, Is.False);
    }

    [Test]
    public void CreatePassword_ShouldReturnFormattedString()
    {
        const string password = "TestPassword";

        var result = HashUtils.CreatePassword(password);

        Assert.That(result, Is.Not.Null.And.Not.Empty);
        Assert.That(result, Contains.Substring(":"));

        var parts = result.Split(':');
        Assert.That(parts, Has.Length.EqualTo(2));

        var hashBytes = Convert.FromBase64String(parts[0]);
        var saltBytes = Convert.FromBase64String(parts[1]);

        Assert.That(hashBytes, Has.Length.EqualTo(32));
        Assert.That(saltBytes, Has.Length.EqualTo(16));
    }

    [Test]
    public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue()
    {
        const string password = "TestPassword";

        var hashSaltCombined = HashUtils.CreatePassword(password);
        var isValid = HashUtils.VerifyPassword(password, hashSaltCombined);

        Assert.That(isValid, Is.True);
    }

    [Test]
    public void VerifyPassword_WithIncorrectPassword_ShouldReturnFalse()
    {
        const string password = "TestPassword";
        const string wrongPassword = "WrongPassword";

        var hashSaltCombined = HashUtils.CreatePassword(password);
        var isValid = HashUtils.VerifyPassword(wrongPassword, hashSaltCombined);

        Assert.That(isValid, Is.False);
    }

    [Test]
    public void VerifyPassword_WithInvalidFormat_ShouldThrowFormatException()
    {
        const string password = "TestPassword";
        const string invalidFormat = "invalid-format-string";

        Assert.Throws<FormatException>(() => HashUtils.VerifyPassword(password, invalidFormat));
    }

    [Test]
    public void GenerateRandomRefreshToken_ShouldReturnValidBase64()
    {
        var token = HashUtils.GenerateRandomRefreshToken();

        Assert.That(token, Is.Not.Null.And.Not.Empty);

        var bytes = Convert.FromBase64String(token);
        Assert.That(bytes, Has.Length.EqualTo(32));
    }

    [Test]
    public void GenerateRandomRefreshToken_ShouldReturnDifferentTokens()
    {
        var token1 = HashUtils.GenerateRandomRefreshToken();
        var token2 = HashUtils.GenerateRandomRefreshToken();

        Assert.That(token1, Is.Not.EqualTo(token2));
    }

    [Test]
    public void Encrypt_And_Decrypt_ShouldRoundTrip()
    {
        const string plaintext = "This is a secret message!";
        var key = Convert.FromBase64String(HashUtils.GenerateBase64Key());

        var encrypted = HashUtils.Encrypt(plaintext, key);
        var decrypted = HashUtils.Decrypt(encrypted, key);

        Assert.That(decrypted, Is.EqualTo(plaintext));
    }

    [Test]
    public void Encrypt_WithSameInputAndKey_ShouldReturnDifferentResults()
    {
        const string plaintext = "Same message";
        var key = Convert.FromBase64String(HashUtils.GenerateBase64Key());

        var encrypted1 = HashUtils.Encrypt(plaintext, key);
        var encrypted2 = HashUtils.Encrypt(plaintext, key);

        Assert.That(encrypted1, Is.Not.EqualTo(encrypted2));
    }

    [Test]
    public void Encrypt_ShouldIncludeIVInResult()
    {
        const string plaintext = "Test message";
        var key = Convert.FromBase64String(HashUtils.GenerateBase64Key());

        var encrypted = HashUtils.Encrypt(plaintext, key);

        Assert.That(encrypted.Length, Is.GreaterThan(16));
    }

    [TestCase(16)]
    [TestCase(24)]
    [TestCase(32)]
    public void Encrypt_WithDifferentKeySizes_ShouldWork(int keySize)
    {
        const string plaintext = "Test with different key sizes";
        var key = new byte[keySize];
        RandomNumberGenerator.Fill(key);

        var encrypted = HashUtils.Encrypt(plaintext, key);
        var decrypted = HashUtils.Decrypt(encrypted, key);

        Assert.That(decrypted, Is.EqualTo(plaintext));
    }
}
