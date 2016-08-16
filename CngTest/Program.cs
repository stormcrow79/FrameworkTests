using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CngTest
{
  class Program
  {
    static void Main(string[] args)
    {
      try
      {
        var cer = new X509Certificate2(@"C:\Incoming\mobile-certs\karisma-kestral.cer");
        var pfx = new X509Certificate2(@"C:\Incoming\mobile-certs\karisma-kestral.pfx", "number1!");

        var keyName = "test1";
        var key = RecreateKey(keyName, 4096); key.Delete(); key.Dispose();
        RSAEncrypt(keyName);

        Console.WriteLine("Success");
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
      Console.ReadLine();
    }

    static CngKey RecreateKey(string name, int length)
    {
      var createParams = new CngKeyCreationParameters()
      {
        KeyCreationOptions = CngKeyCreationOptions.OverwriteExistingKey,
        ExportPolicy = CngExportPolicies.AllowPlaintextExport
      };

      createParams.Parameters.Add(new CngProperty("Length", BitConverter.GetBytes(length), CngPropertyOptions.None));
      var exists = CngKey.Exists(name, CngProvider.MicrosoftSoftwareKeyStorageProvider, CngKeyOpenOptions.None);
      var key = CngKey.Create(CngAlgorithm.Rsa, name, createParams);

      File.WriteAllBytes(@"C:\Connect\key\" + name + "-private", key.Export(CngKeyBlobFormat.GenericPrivateBlob));
      File.WriteAllBytes(@"C:\Connect\key\" + name + "-public",  key.Export(CngKeyBlobFormat.GenericPublicBlob));

      return key;
    }

    private static void RSAEncrypt(string keyName)
    {
      var publicKey = CngKey.Import(File.ReadAllBytes(@"C:\Connect\Key\" + keyName + "-public"), CngKeyBlobFormat.GenericPublicBlob);
      var cipherText = Encrypt(publicKey, Encoding.UTF8.GetBytes("Hello crypto"));

      var privateKey = CngKey.Import(File.ReadAllBytes(@"C:\Connect\key\" + keyName + "-private"), CngKeyBlobFormat.GenericPrivateBlob);
      var plainText = Decrypt(privateKey, cipherText);
      Console.WriteLine(Encoding.UTF8.GetString(plainText));
    }

    static byte[] Encrypt(CngKey key, byte[] plainText)
    {
      var rsa = new RSACng(key);
      return rsa.Encrypt(plainText, RSAEncryptionPadding.Pkcs1);
    }

    static byte[] Decrypt(CngKey key, byte[] cipherText)
    {
      var rsa = new RSACng(key);
      return rsa.Decrypt(cipherText, RSAEncryptionPadding.Pkcs1);
    } 
  }

  static class BCrypt
  {
    [DllImport("bcrypt.dll", CharSet = CharSet.Auto)]
    public static extern int BCryptOpenAlgorithmProvider(out IntPtr phAlgorithm, string pszAlgId, string pszImplementation, int dwFlags);
    [DllImport("bcrypt.dll", CharSet = CharSet.Auto)]
    public static extern int BCryptCloseAlgorithmProvider(IntPtr hAlgorithm, int dwFlags);

    [DllImport("bcrypt.dll", CharSet = CharSet.Auto)]
    public static extern int BCryptImportKeyPair(IntPtr hAlgorithm, IntPtr hImportKey, string pszBlobType, out IntPtr phKey, byte[] pbInput, int cbInput, int dwFlags);

    [DllImport("bcrypt.dll", CharSet = CharSet.Auto)]
    public static extern int BCryptDestroyKey(IntPtr hKey);
  }

  static class NCrypt
  {
    public static CngKey CreateKey(CngProvider provider = null, string keyName = null)
    {
      var hProvider = IntPtr.Zero;
      var hKey = IntPtr.Zero;
      int err = 0;

      try
      {
        err = NCryptOpenStorageProvider(out hProvider, provider == null ? null : provider.Provider, 0);
        if (err != 0)
          throw new CryptographicException(err);

        err = NCryptCreatePersistedKey(hProvider, out hKey, "RSA", keyName, 0, 0);
        if (err != 0)
          throw new CryptographicException(err);

        err = NCryptSetProperty(hKey, "Length", BitConverter.GetBytes(3072), 4, 0);
        if (err != 0)
          throw new CryptographicException(err);

        err = NCryptFinalizeKey(hKey, 0);
        if (err != 0)
          throw new CryptographicException(err);

        return CngKey.Open(keyName, provider);
      }
      finally
      {
        if (hProvider != IntPtr.Zero)
          NCryptFreeObject(hProvider);
        if (hKey != IntPtr.Zero)
          NCryptFreeObject(hKey);
      }
    }

    [DllImport("ncrypt.dll", CharSet = CharSet.Auto)]
    public static extern int NCryptOpenStorageProvider(out IntPtr phProvider, string pszProviderName, int dwFlags);
    [DllImport("ncrypt.dll", CharSet = CharSet.Auto)]
    public static extern int NCryptCreatePersistedKey(IntPtr hProvider, out IntPtr phKey, string pszAlgId, string pszKeyName, int dwLegacyKeySpec, int dwFlags);
    [DllImport("ncrypt.dll", CharSet = CharSet.Auto)]
    public static extern int NCryptGetProperty(IntPtr hObject, string pszProperty, byte[] pbOutput, int cbOutput, out int pcbResult, int dwFlags);
    [DllImport("ncrypt.dll", CharSet = CharSet.Auto)]
    public static extern int NCryptSetProperty(IntPtr hObject, string pszProperty, byte[] pbInput, int cbInput, int dwFlags);
    [DllImport("ncrypt.dll", CharSet = CharSet.Auto)]
    public static extern int NCryptFinalizeKey(IntPtr hKey, int dwFlags);
    [DllImport("ncrypt.dll", CharSet = CharSet.Auto)]
    public static extern int NCryptFreeObject(IntPtr hObject);
  }
}
