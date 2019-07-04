using UnityEngine;
using System.Security.Cryptography;
using System.Text;
using System.Collections;

public class Crypto {

    private static char ToHex(byte b) {
        if (b >= 0 && b <= 9) {
            return (char)('0' + b);
        } else {
            return (char)('a' + (b - 10));
        }
    }

    public static string MD5(string s) {
        var md5 = new MD5CryptoServiceProvider();
        byte[] bin = md5.ComputeHash(Encoding.UTF8.GetBytes(s));
        var buf = new char[bin.Length * 2];
        int i = 0;
        foreach (byte b in bin) {
            buf[i++] = ToHex((byte)(b >> 4));
            buf[i++] = ToHex((byte)(b & 0xf));
        }
        return new string(buf);
    }

}
