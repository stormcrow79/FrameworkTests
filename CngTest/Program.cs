using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CngTest
{
  class Program
  {
    static void ExportStrongKey()
    {
      var rsa1 = new RSACng();
      var pri = rsa1.Key.Export(CngKeyBlobFormat.GenericPrivateBlob);
      var pub = rsa1.Key.Export(CngKeyBlobFormat.GenericPublicBlob);

      File.WriteAllBytes(@"C:\Connect\key\ifr_pri", pri);
      File.WriteAllBytes(@"C:\Connect\key\ifr_pub", pub);
    }
    static void Main(string[] args)
    {
      try
      {
        //RecreateKey("test1", 4096);

        var privateKey = CngKey.Import(File.ReadAllBytes(@"C:\Connect\key\test1-private"), CngKeyBlobFormat.GenericPrivateBlob);
        var cipherText = Encrypt(privateKey, Encoding.UTF8.GetBytes("Hello crypto"));

        var publicKey = CngKey.Import(File.ReadAllBytes(@"C:\Connect\Key\test1-public"), CngKeyBlobFormat.GenericPublicBlob);
        var plainText = Decrypt(publicKey, cipherText);
        Console.WriteLine(Encoding.UTF8.GetString(plainText));

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
    static extern int NCryptOpenStorageProvider(out IntPtr phProvider, string pszProviderName, int dwFlags);
    [DllImport("ncrypt.dll", CharSet = CharSet.Auto)]
    static extern int NCryptCreatePersistedKey(IntPtr hProvider, out IntPtr phKey, string pszAlgId, string pszKeyName, int dwLegacyKeySpec, int dwFlags);
    [DllImport("ncrypt.dll", CharSet = CharSet.Auto)]
    static extern int NCryptGetProperty(IntPtr hObject, string pszProperty, byte[] pbOutput, int cbOutput, out int pcbResult, int dwFlags);
    [DllImport("ncrypt.dll", CharSet = CharSet.Auto)]
    static extern int NCryptSetProperty(IntPtr hObject, string pszProperty, byte[] pbInput, int cbInput, int dwFlags);
    [DllImport("ncrypt.dll")]
    static extern int NCryptFinalizeKey(IntPtr hKey, int dwFlags);
    [DllImport("ncrypt.dll")]
    static extern int NCryptFreeObject(IntPtr hObject);
  }
}
