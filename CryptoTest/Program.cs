using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Pkcs;
using System.IO;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Xml;

namespace CryptoTest {
  class KarismaLicenseSigner {
    public static byte[] Sign(byte[] data, byte[] pvkBuffer, string pvkPassword) {
      var pvk = new PvkFile(pvkBuffer);

      var password = Encoding.ASCII.GetBytes(pvkPassword);
      var saltedPassword = new byte[pvk.SaltLength + password.Length];
      Array.Copy(pvk.Salt, saltedPassword, pvk.SaltLength);
      Array.Copy(password, 0, saltedPassword, pvk.SaltLength, password.Length);

      // pinvoke
      var prov = IntPtr.Zero;
      var hash = IntPtr.Zero;
      var sessionKey = IntPtr.Zero;
      var privateKey = IntPtr.Zero;

      if (!Wcrypt2.CryptAcquireContext(out prov, null, null, Wcrypt2.PROV_RSA_FULL, 0)) {
        if (!Wcrypt2.CryptAcquireContext(out prov, null, null, Wcrypt2.PROV_RSA_FULL, Wcrypt2.CRYPT_NEWKEYSET))
          throw new Win32Exception();
      }

      #region key management
      if (!Wcrypt2.CryptCreateHash(prov, Wcrypt2.CALG_SHA1, IntPtr.Zero, 0, out hash))
        throw new Win32Exception();
      if (!Wcrypt2.CryptHashData(hash, saltedPassword, saltedPassword.Length, 0))
        throw new Win32Exception();
      if (!Wcrypt2.CryptDeriveKey(prov, Wcrypt2.CALG_RC4, hash, 0, out sessionKey))
        throw new Win32Exception();
      if (!Wcrypt2.CryptDestroyHash(hash))
        throw new Win32Exception();
      hash = IntPtr.Zero;

      if (!Wcrypt2.CryptImportKey(prov, pvk.Key, pvk.KeyLength, sessionKey, 0, out privateKey))
        throw new Win32Exception();
      #endregion

      #region signing
      byte[] hashBytes = null;
      int hashLength = 0;
      if (!Wcrypt2.CryptCreateHash(prov, Wcrypt2.CALG_SHA1, IntPtr.Zero, 0, out hash))
        throw new Win32Exception();
      if (!Wcrypt2.CryptHashData(hash, data, data.Length, 0))
        throw new Win32Exception();
      Wcrypt2.CryptSignHash(hash, Wcrypt2.AT_SIGNATURE, null, 0, hashBytes, ref hashLength);
      hashBytes = new byte[hashLength];
      if (!Wcrypt2.CryptSignHash(hash, Wcrypt2.AT_SIGNATURE, null, 0, hashBytes, ref hashLength))
        throw new Win32Exception();
      if (!Wcrypt2.CryptDestroyHash(hash))
        throw new Win32Exception();
      hash = IntPtr.Zero;
      #endregion

      if (!Wcrypt2.CryptAcquireContext(out prov, null, null, Wcrypt2.PROV_RSA_FULL, Wcrypt2.CRYPT_DELETEKEYSET))
        throw new Win32Exception();

      return hashBytes;
    }
    class PvkFile {
      public PvkFile(byte[] data) {
        Magic = data[0] | data[1] << 8 | data[2] << 16 | data[3] << 24;
        Reserved = data[4] | data[5] << 8 | data[6] << 16 | data[7] << 24;
        KeyType = data[8] | data[9] << 8 | data[10] << 16 | data[11] << 24;
        Encrypted = data[12] | data[13] << 8 | data[14] << 16 | data[15] << 24;
        SaltLength = data[16] | data[17] << 8 | data[18] << 16 | data[19] << 24;
        KeyLength = data[20] | data[21] << 8 | data[22] << 16 | data[23] << 24;

        Salt = new byte[SaltLength];
        Array.Copy(data, 24, Salt, 0, SaltLength);
        Key = new byte[KeyLength];
        Array.Copy(data, 24 + SaltLength, Key, 0, KeyLength);
      }
      public int Magic { get; internal set; }
      public int Reserved { get; internal set; }
      public int KeyType { get; internal set; }
      public int Encrypted { get; internal set; }
      public int SaltLength { get; internal set; }
      public int KeyLength { get; internal set; }
      public byte[] Salt { get; internal set; }
      public byte[] Key { get; internal set; }
    }
    static class Wcrypt2 {
      public static readonly uint PROV_RSA_FULL = 1;

      public static readonly uint CRYPT_VERIFYCONTEXT = 0xF0000000;
      public static readonly uint CRYPT_NEWKEYSET = 0x8;
      public static readonly uint CRYPT_DELETEKEYSET = 0x10;
      public static readonly uint CRYPT_MACHINE_KEYSET = 0x20;

      public static readonly uint CALG_SHA1 = 0x8004;
      public static readonly uint CALG_RC4 = 0x6801;

      public static readonly uint AT_SIGNATURE = 0x2;

      [DllImport("advapi32.dll", SetLastError = true)]
      public static extern bool CryptAcquireContext(out IntPtr phProv, string pszContainer, string pszProvider, uint dwProvType, uint dwFlags);
      [DllImport("advapi32.dll", SetLastError = true)]
      public static extern bool CryptCreateHash(IntPtr hProv, uint Algid, IntPtr hKey, uint dwFlags, out IntPtr phHash);
      [DllImport("advapi32.dll", SetLastError = true)]
      public static extern bool CryptDestroyHash(IntPtr hHash);
      [DllImport("advapi32.dll", SetLastError = true)]
      public static extern bool CryptHashData(IntPtr hHash, byte[] pbData, int dwDataLen, uint dwFlags);
      [DllImport("advapi32.dll", SetLastError = true)]
      public static extern bool CryptDeriveKey(IntPtr hProv, uint Algid, IntPtr hBaseData, uint dwFlags, out IntPtr phKey);
      [DllImport("advapi32.dll", SetLastError = true)]
      public static extern bool CryptImportKey(IntPtr hProv, byte[] pbData, int dwDataLen, IntPtr hPubKey, uint dwFlags, out IntPtr phKey);
      [DllImport("advapi32.dll", SetLastError = true)]
      public static extern bool CryptSignHash(IntPtr hHash, uint dwKeySpec, string sDescription, uint dwFlags, [Out] byte[] pbSignature, [In, Out] ref int pdwSignLen);
    }
  }
  class Program {
    static void Main(string[] args) {
      var signed1 = new XmlDocument();
      signed1.Load(@"\\gavinm\incoming\karisma\signed.xml");
      var signature1 = signed1.SelectSingleNode("/Certificate/Signature").InnerText;
      
      var hashBytes = KarismaLicenseSigner.Sign(
        File.ReadAllBytes(@"\\gavinm\incoming\karisma\raw.xml"),
        File.ReadAllBytes(@"\\gavinm\incoming\karisma\private.pvk"), 
        "jatrAtru7ecr"
      );
      var signature2 = Convert.ToBase64String(hashBytes);

      Console.WriteLine(signature1 == signature2 ? "PASS" : "FAIL");
      Console.ReadLine();
    }
  }
}
