using System;
using System.Collections.Generic;

// Token: 0x02000034 RID: 52
[Serializable]
public class ProtectedString : IDisposable, IEquatable<ProtectedString>
{
    // Token: 0x06000188 RID: 392 RVA: 0x0000239D File Offset: 0x0000059D
    public ProtectedString()
    {
    }

    // Token: 0x17000013 RID: 19
    // (get) Token: 0x0600018C RID: 396 RVA: 0x0001261C File Offset: 0x0001081C
    // (set) Token: 0x0600018D RID: 397 RVA: 0x0000351D File Offset: 0x0000171D
    private char[] _value_buff
    {
        get
        {
            if (this._value_buff_string != null && this._value_buff_string2 != this._value_buff_string)
            {
                this.__value_buff = this._value_buff_string.ToCharArray();
                this._value_buff_string2 = this._value_buff_string;
            }
            return this.__value_buff;
        }
        set
        {
            this.__value_buff = value;
        }
    }

    // Token: 0x0600018E RID: 398 RVA: 0x00003526 File Offset: 0x00001726
    private void Encrypt(string str)
    {
        if (str == null)
        {
            this._value_buff = null;
            return;
        }
        if (str.Length == 0)
        {
            this._value_buff = new char[0];
            return;
        }
        this.Encrypt(str.ToCharArray());
    }

    // Token: 0x0600018F RID: 399 RVA: 0x00012670 File Offset: 0x00010870
    private void Encrypt(char[] str)
    {
        if (str == null)
        {
            this._value_buff = null;
            return;
        }
        if (str.Length == 0)
        {
            this._value_buff = new char[0];
            return;
        }
        uint num = (uint)(1162040133 + this._secret);
        uint num2 = 2506450243u;
        this._value_buff = new char[str.Length];
        str.CopyTo(this._value_buff, 0);
        uint num3 = 13u;
        for (int i = 0; i < this._value_buff.Length; i++)
        {
            num = (num * 4343255u + num3 + 5235457u) % 4294967294u;
            num2 = (num2 * 5354354u + num3 + 22646641u) % 4294967294u;
            num3 = (uint)this._value_buff[i];
            this._value_buff[i] ^= (char)(num & 32767u);
            this._value_buff[i] += (char)(num2 & 8191u);
        }
    }

    // Token: 0x06000190 RID: 400 RVA: 0x00012750 File Offset: 0x00010950
    private string Decrypt()
    {
        if (this._value_buff == null)
        {
            return null;
        }
        if (this._value_buff.Length == 0)
        {
            return string.Empty;
        }
        uint num = (uint)(1162040133 + this._secret);
        uint num2 = 2506450243u;
        char[] array = new char[this._value_buff.Length];
        uint num3 = 13u;
        for (int i = 0; i < this._value_buff.Length; i++)
        {
            array[i] = this._value_buff[i];
            num = (num * 4343255u + num3 + 5235457u) % 4294967294u;
            num2 = (num2 * 5354354u + num3 + 22646641u) % 4294967294u;
            array[i] -= (char)(num2 & 8191u);
            array[i] ^= (char)(num & 32767u);
            num3 = (uint)array[i];
        }
        string result = new string(array);
        for (int j = 0; j < array.Length; j++)
        {
            array[j] = '\0';
        }
        return result;
    }

    // Token: 0x06000191 RID: 401 RVA: 0x00012844 File Offset: 0x00010A44
    private bool DecryptCompare(string str)
    {
        if (this._value_buff == null)
        {
            return str == null;
        }
        if (this._value_buff.Length == 0)
        {
            return str != null && str.Length == 0;
        }
        if (str.Length != this._value_buff.Length)
        {
            return false;
        }
        uint num = (uint)(1162040133 + this._secret);
        uint num2 = 2506450243u;
        uint num3 = 13u;
        for (int i = 0; i < this._value_buff.Length; i++)
        {
            char c = this._value_buff[i];
            num = (num * 4343255u + num3 + 5235457u) % 4294967294u;
            num2 = (num2 * 5354354u + num3 + 22646641u) % 4294967294u;
            c -= (char)(num2 & 8191u);
            c ^= (char)(num & 32767u);
            num3 = (uint)c;
            if (str[i] != c)
            {
                return false;
            }
        }
        return true;
    }

    // Token: 0x06000192 RID: 402 RVA: 0x00012928 File Offset: 0x00010B28
    private char DecryptIndex(int index)
    {
        if (this._value_buff == null || this._value_buff.Length == 0)
        {
            return '\0';
        }
        uint num = (uint)(1162040133 + this._secret);
        uint num2 = 2506450243u;
        uint num3 = 13u;
        for (int i = 0; i < this._value_buff.Length; i++)
        {
            char c = this._value_buff[i];
            num = (num * 4343255u + num3 + 5235457u) % 4294967294u;
            num2 = (num2 * 5354354u + num3 + 22646641u) % 4294967294u;
            c -= (char)(num2 & 8191u);
            c ^= (char)(num & 32767u);
            num3 = (uint)c;
            if (index == i)
            {
                return c;
            }
        }
        return '\0';
    }

    // Token: 0x06000193 RID: 403 RVA: 0x000129D8 File Offset: 0x00010BD8
    internal static ProtectedString FromEncryptedRaw(int secred_val, char[] buff_val)
    {
        return new ProtectedString
        {
            _secret = secred_val,
            _value_buff = buff_val
        };
    }

    // Token: 0x06000194 RID: 404 RVA: 0x000129FC File Offset: 0x00010BFC
    internal static ProtectedString FromEncryptedRaw(int secred_val, string buff_val)
    {
        return new ProtectedString
        {
            _secret = secred_val,
            _value_buff = buff_val.ToCharArray()
        };
    }

    // Token: 0x06000195 RID: 405 RVA: 0x0000355A File Offset: 0x0000175A
    internal void ToEncryptedRaw(out int secred_val, out char[] buff_val)
    {
        secred_val = this._secret;
        buff_val = null;
        if (this._value_buff != null)
        {
            buff_val = new char[this._value_buff.Length];
            this._value_buff.CopyTo(buff_val, 0);
        }
    }

    // Token: 0x06000196 RID: 406 RVA: 0x0000358F File Offset: 0x0000178F
    internal void ToEncryptedRaw(out int secred_val, out string buff_val)
    {
        secred_val = this._secret;
        buff_val = null;
        if (this._value_buff != null)
        {
            buff_val = new string(this._value_buff);
        }
    }

    // Token: 0x17000014 RID: 20
    // (get) Token: 0x06000197 RID: 407 RVA: 0x000035B4 File Offset: 0x000017B4
    // (set) Token: 0x06000198 RID: 408 RVA: 0x000035BC File Offset: 0x000017BC
    internal string Value
    {
        get
        {
            return this.Decrypt();
        }
        set
        {
            this.Encrypt(value);
        }
    }

    // Token: 0x06000199 RID: 409 RVA: 0x00012A24 File Offset: 0x00010C24
    public void Dispose()
    {
        this._secret = 0;
        if (this._value_buff != null)
        {
            for (int i = 0; i < this._value_buff.Length; i++)
            {
                this._value_buff[i] = '\0';
            }
        }
        this._value_buff = null;
        this._value_buff_string = null;
        this._value_buff_string2 = null;
    }

    // Token: 0x0600019A RID: 410 RVA: 0x000035C5 File Offset: 0x000017C5
    public override bool Equals(object obj)
    {
        return this.EqualsInternal(obj);
    }

    // Token: 0x0600019B RID: 411 RVA: 0x00012A7C File Offset: 0x00010C7C
    public bool EqualsInternal(object obj)
    {
        if (obj == null)
        {
            return false;
        }
        if (!(obj is ProtectedString))
        {
            string str = (!(obj is string)) ? obj.ToString() : ((string)obj);
            return this.DecryptCompare(str);
        }
        ProtectedString protectedString = (ProtectedString)obj;
        if (this.Length != protectedString.Length)
        {
            return false;
        }
        for (int i = 0; i < this.Length; i++)
        {
            if (this.DecryptIndex(i) != protectedString.DecryptIndex(i))
            {
                return false;
            }
        }
        return true;
    }

    // Token: 0x0600019C RID: 412 RVA: 0x000035C5 File Offset: 0x000017C5
    bool IEquatable<ProtectedString>.Equals(ProtectedString v)
    {
        return this.EqualsInternal(v);
    }

    // Token: 0x0600019D RID: 413 RVA: 0x000029C6 File Offset: 0x00000BC6
    public static bool operator ==(ProtectedString val1, ProtectedString val2)
    {
        return (val1 == null && val2 == null) || val1.Equals(val2);
    }

    // Token: 0x0600019E RID: 414 RVA: 0x000029DD File Offset: 0x00000BDD
    public static bool operator !=(ProtectedString val1, ProtectedString val2)
    {
        return (val1 != null || val2 != null) && !val1.Equals(val2);
    }

    // Token: 0x0600019F RID: 415 RVA: 0x000029C6 File Offset: 0x00000BC6
    public static bool operator ==(ProtectedString val1, string val2)
    {
        return (val1 == null && val2 == null) || val1.Equals(val2);
    }

    // Token: 0x060001A0 RID: 416 RVA: 0x000029DD File Offset: 0x00000BDD
    public static bool operator !=(ProtectedString val1, string val2)
    {
        return (val1 != null || val2 != null) && !val1.Equals(val2);
    }

    // Token: 0x060001A1 RID: 417 RVA: 0x000035CE File Offset: 0x000017CE
    public static bool operator ==(string val2, ProtectedString val1)
    {
        return (val1 == null && val2 == null) || val1.Equals(val2);
    }

    // Token: 0x060001A2 RID: 418 RVA: 0x000035E5 File Offset: 0x000017E5
    public static bool operator !=(string val2, ProtectedString val1)
    {
        return (val1 != null || val2 != null) && !val1.Equals(val2);
    }

    // Token: 0x060001A3 RID: 419 RVA: 0x000029C6 File Offset: 0x00000BC6
    public static bool operator ==(ProtectedString val1, object val2)
    {
        return (val1 == null && val2 == null) || val1.Equals(val2);
    }

    // Token: 0x060001A4 RID: 420 RVA: 0x000029DD File Offset: 0x00000BDD
    public static bool operator !=(ProtectedString val1, object val2)
    {
        return (val1 != null || val2 != null) && !val1.Equals(val2);
    }

    // Token: 0x060001A5 RID: 421 RVA: 0x000035FF File Offset: 0x000017FF
    public override int GetHashCode()
    {
        return this._value_buff.GetHashCode();
    }

    // Token: 0x17000015 RID: 21
    // (get) Token: 0x060001A6 RID: 422 RVA: 0x0000360C File Offset: 0x0000180C
    internal int Length
    {
        get
        {
            if (this._value_buff == null)
            {
                return 0;
            }
            return this._value_buff.Length;
        }
    }

    // Token: 0x17000016 RID: 22
    internal char this[int index]
    {
        get
        {
            return this.DecryptIndex(index);
        }
    }

    // Token: 0x060001A8 RID: 424 RVA: 0x0000362C File Offset: 0x0000182C
    public override string ToString()
    {
        return this.Value;
    }

    // Token: 0x060001AC RID: 428 RVA: 0x00003653 File Offset: 0x00001853
    public static implicit operator string(ProtectedString d)
    {
        return d.Value;
    }

    // Token: 0x06001501 RID: 5377
    public static ProtectedString FromEncryptedRaw1(int secred_val, string buff_val)
    {
        return new ProtectedString
        {
            _secret = secred_val,
            _value_buff = buff_val.ToCharArray()
        };
    }

    private char[] __value_buff;
    private string _value_buff_string;
    private string _value_buff_string2;
    private int _secret;
    public int a;
    public char[] b;
}
