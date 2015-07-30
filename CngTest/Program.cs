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

        //var key1 = NCrypt.CreateKey(CngProvider.MicrosoftSoftwareKeyStorageProvider, "foofoo");

        var sender = "ifr";
        var recipient = "rg";

        var provider = CngProvider.MicrosoftSoftwareKeyStorageProvider;
        var alg = CngAlgorithm.Rsa;

        var createParams = new CngKeyCreationParameters()
        {
          Provider = provider,
          KeyCreationOptions = CngKeyCreationOptions.OverwriteExistingKey
        };
        createParams.Parameters.Add(new CngProperty("Length", BitConverter.GetBytes(4096), CngPropertyOptions.None));

        var exists = CngKey.Exists("testkey1", provider, CngKeyOpenOptions.None);
        CngKey key = //exists ? CngKey.Open("testkey1", provider) :
          CngKey.Create(alg, "testkey1", createParams);

        var plainText = Encoding.UTF8.GetBytes("Hello crypto");

        var rsa = new RSACng(key);
        var cipherText = rsa.Encrypt(plainText, RSAEncryptionPadding.OaepSHA256);



        Console.WriteLine("Success");
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
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
