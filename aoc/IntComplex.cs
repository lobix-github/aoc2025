using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public readonly struct IntComplex
    : IEquatable<IntComplex>,
      IFormattable,
      IUtf8SpanFormattable
{
    private const NumberStyles DefaultNumberStyle = NumberStyles.Float | NumberStyles.AllowThousands;

    private const NumberStyles InvalidNumberStyles = ~(NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite
                                                     | NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingSign
                                                     | NumberStyles.AllowParentheses | NumberStyles.AllowDecimalPoint
                                                     | NumberStyles.AllowThousands | NumberStyles.AllowExponent
                                                     | NumberStyles.AllowCurrencySymbol | NumberStyles.AllowHexSpecifier);

    public static readonly IntComplex Zero = new IntComplex(0, 0);
    public static readonly IntComplex One = new IntComplex(1, 0);
    public static readonly IntComplex ImaginaryOne = new IntComplex(0, 1);
    public static readonly IntComplex Infinity = new IntComplex(int.MaxValue, int.MaxValue);

    private const double InverseOfLog10 = 0.43429448190325; // 1 / Log(10)

    // This is the largest x for which (Hypot(x,x) + x) will not overflow. It is used for branching inside Sqrt.
    private static readonly double s_sqrtRescaleThreshold = double.MaxValue / (Math.Sqrt(2.0) + 1.0);

    // This is the largest x for which 2 x^2 will not overflow. It is used for branching inside Asin and Acos.
    private static readonly double s_asinOverflowThreshold = Math.Sqrt(double.MaxValue) / 2.0;

    // This value is used inside Asin and Acos.
    private static readonly double s_log2 = Math.Log(2.0);

    // Do not rename, these fields are needed for binary serialization
    private readonly int m_real; // Do not rename (binary serialization)
    private readonly int m_imaginary; // Do not rename (binary serialization)

    public IntComplex(int real, int imaginary)
    {
        m_real = real;
        m_imaginary = imaginary;
    }

	public IntComplex(IntComplex number)
	{
		m_real = number.m_real;
		m_imaginary = number.m_imaginary;
	}

	public int Real { get { return m_real; } }
    public int Imaginary { get { return m_imaginary; } }

    public static IntComplex operator -(IntComplex value)  /* Unary negation of a complex number */
    {
        return new IntComplex(-value.m_real, -value.m_imaginary);
    }

    public static IntComplex operator +(IntComplex left, IntComplex right)
    {
        return new IntComplex(left.m_real + right.m_real, left.m_imaginary + right.m_imaginary);
    }

    public static IntComplex operator +(IntComplex left, int right)
    {
        return new IntComplex(left.m_real + right, left.m_imaginary);
    }

    public static IntComplex operator +(int left, IntComplex right)
    {
        return new IntComplex(left + right.m_real, right.m_imaginary);
    }

    public static IntComplex operator -(IntComplex left, IntComplex right)
    {
        return new IntComplex(left.m_real - right.m_real, left.m_imaginary - right.m_imaginary);
    }

    public static IntComplex operator -(IntComplex left, int right)
    {
        return new IntComplex(left.m_real - right, left.m_imaginary);
    }

    public static IntComplex operator -(int left, IntComplex right)
    {
        return new IntComplex(left - right.m_real, -right.m_imaginary);
    }

    public static IntComplex operator *(IntComplex left, IntComplex right)
    {
        // Multiplication:  (a + bi)(c + di) = (ac -bd) + (bc + ad)i
        int result_realpart = (left.m_real * right.m_real) - (left.m_imaginary * right.m_imaginary);
        int result_imaginarypart = (left.m_imaginary * right.m_real) + (left.m_real * right.m_imaginary);
        return new IntComplex(result_realpart, result_imaginarypart);
    }

    public static bool operator ==(IntComplex left, IntComplex right)
    {
        return left.m_real == right.m_real && left.m_imaginary == right.m_imaginary;
    }

    public static bool operator !=(IntComplex left, IntComplex right)
    {
        return left.m_real != right.m_real || left.m_imaginary != right.m_imaginary;
    }

    public override bool Equals([NotNullWhen(true)] object obj)
    {
        return obj is IntComplex other && Equals(other);
    }

    public bool Equals(IntComplex value)
    {
        return m_real.Equals(value.m_real) && m_imaginary.Equals(value.m_imaginary);
    }

    public override int GetHashCode() => HashCode.Combine(m_real, m_imaginary);

    public override string ToString() => ToString(null, null);

    public string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string format) => ToString(format, null);

    public string ToString(IFormatProvider provider) => ToString(null, provider);

    public string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string format, IFormatProvider provider)
    {
        // $"<{m_real.ToString(format, provider)}; {m_imaginary.ToString(format, provider)}>";
        var handler = new DefaultInterpolatedStringHandler(4, 2, provider, stackalloc char[512]);
        handler.AppendLiteral("<");
        handler.AppendFormatted(m_real, format);
        handler.AppendLiteral("; ");
        handler.AppendFormatted(m_imaginary, format);
        handler.AppendLiteral(">");
        return handler.ToStringAndClear();
    }

    //
    // Explicit Conversions To Complex
    //

    public static explicit operator IntComplex(decimal value)
    {
        return new IntComplex((int)value, 0);
    }

    /// <summary>Explicitly converts a <see cref="Int128" /> value to a double-precision complex number.</summary>
    /// <param name="value">The value to convert.</param>
    /// <returns><paramref name="value" /> converted to a double-precision complex number.</returns>
    public static explicit operator IntComplex(Int128 value)
    {
        return new IntComplex((int)value, 0);
    }

    public static explicit operator IntComplex(BigInteger value)
    {
        return new IntComplex((int)value, 0);
    }

    /// <summary>Explicitly converts a <see cref="UInt128" /> value to a double-precision complex number.</summary>
    /// <param name="value">The value to convert.</param>
    /// <returns><paramref name="value" /> converted to a double-precision complex number.</returns>
    public static explicit operator IntComplex(UInt128 value)
    {
        return new IntComplex((int)value, 0);
    }

    //
    // Implicit Conversions To Complex
    //

    public static implicit operator IntComplex(byte value)
    {
        return new IntComplex(value, 0);
    }

    /// <summary>Implicitly converts a <see cref="char" /> value to a double-precision complex number.</summary>
    /// <param name="value">The value to convert.</param>
    /// <returns><paramref name="value" /> converted to a double-precision complex number.</returns>
    public static implicit operator IntComplex(char value)
    {
        return new IntComplex(value, 0);
    }

    public static implicit operator IntComplex(double value)
    {
        return new IntComplex((int)value, 0);
    }

    /// <summary>Implicitly converts a <see cref="Half" /> value to a double-precision complex number.</summary>
    /// <param name="value">The value to convert.</param>
    /// <returns><paramref name="value" /> converted to a double-precision complex number.</returns>
    public static implicit operator IntComplex(Half value)
    {
        return new IntComplex((int)value, 0);
    }

    public static implicit operator IntComplex(short value)
    {
        return new IntComplex(value, 0);
    }

	public static implicit operator IntComplex(int value)
	{
		return new IntComplex(value, 0);
	}

	public static implicit operator IntComplex((int, int) value)
	{
		return new IntComplex(value.Item1, value.Item2);
	}

	public static implicit operator IntComplex(long value)
    {
        return new IntComplex((int)value, 0);
    }

    /// <summary>Implicitly converts a <see cref="IntPtr" /> value to a double-precision complex number.</summary>
    /// <param name="value">The value to convert.</param>
    /// <returns><paramref name="value" /> converted to a double-precision complex number.</returns>
    public static implicit operator IntComplex(nint value)
    {
        return new IntComplex((int)value, 0);
    }

    public static implicit operator IntComplex(sbyte value)
    {
        return new IntComplex(value, 0);
    }

    public static implicit operator IntComplex(float value)
    {
        return new IntComplex((int)value, 0);
    }

    public static implicit operator IntComplex(ushort value)
    {
        return new IntComplex(value, 0);
    }

    public static implicit operator IntComplex(uint value)
    {
        return new IntComplex((int)value, 0);
    }

    public static implicit operator IntComplex(ulong value)
    {
        return new IntComplex((int)value, 0);
    }

    /// <summary>Implicitly converts a <see cref="UIntPtr" /> value to a double-precision complex number.</summary>
    /// <param name="value">The value to convert.</param>
    /// <returns><paramref name="value" /> converted to a double-precision complex number.</returns>
    public static implicit operator IntComplex(nuint value)
    {
        return new IntComplex((int)value, 0);
    }

    //
    // IDecrementOperators
    //

    /// <inheritdoc cref="IDecrementOperators{TSelf}.op_Decrement(TSelf)" />
    public static IntComplex operator --(IntComplex value) => value - One;

    //
    // IIncrementOperators
    //

    /// <inheritdoc cref="IIncrementOperators{TSelf}.op_Increment(TSelf)" />
    public static IntComplex operator ++(IntComplex value) => value + One;

    /// <inheritdoc cref="INumberBase{TSelf}.IsComplexNumber(TSelf)" />
    public static bool IsComplexNumber(IntComplex value) => (value.m_real != 0) && (value.m_imaginary != 0);

    /// <inheritdoc cref="INumberBase{TSelf}.IsEvenInteger(TSelf)" />
    public static bool IsEvenInteger(IntComplex value) => (value.m_imaginary == 0) && double.IsEvenInteger(value.m_real);

    /// <inheritdoc cref="INumberBase{TSelf}.IsImaginaryNumber(TSelf)" />
    public static bool IsImaginaryNumber(IntComplex value) => (value.m_real == 0) && double.IsRealNumber(value.m_imaginary);

    /// <inheritdoc cref="INumberBase{TSelf}.IsInteger(TSelf)" />
    public static bool IsInteger(IntComplex value) => (value.m_imaginary == 0) && double.IsInteger(value.m_real);

    /// <inheritdoc cref="INumberBase{TSelf}.IsNegative(TSelf)" />
    public static bool IsNegative(IntComplex value)
    {
        // since complex numbers do not have a well-defined concept of
        // negative we report false if this value has an imaginary part

        return (value.m_imaginary == 0) && double.IsNegative(value.m_real);
    }

    /// <inheritdoc cref="INumberBase{TSelf}.IsNegativeInfinity(TSelf)" />
    public static bool IsNegativeInfinity(IntComplex value)
    {
        // since complex numbers do not have a well-defined concept of
        // negative we report false if this value has an imaginary part

        return (value.m_imaginary == 0) && double.IsNegativeInfinity(value.m_real);
    }

    /// <inheritdoc cref="INumberBase{TSelf}.IsNormal(TSelf)" />
    public static bool IsNormal(IntComplex value)
    {
        // much as IsFinite requires both part to be finite, we require both
        // part to be "normal" (finite, non-zero, and non-subnormal) to be true

        return double.IsNormal(value.m_real)
            && ((value.m_imaginary == 0) || double.IsNormal(value.m_imaginary));
    }

    /// <inheritdoc cref="INumberBase{TSelf}.IsOddInteger(TSelf)" />
    public static bool IsOddInteger(IntComplex value) => (value.m_imaginary == 0) && double.IsOddInteger(value.m_real);

    /// <inheritdoc cref="INumberBase{TSelf}.IsPositive(TSelf)" />
    public static bool IsPositive(IntComplex value)
    {
        // since complex numbers do not have a well-defined concept of
        // negative we report false if this value has an imaginary part

        return (value.m_imaginary == 0) && double.IsPositive(value.m_real);
    }

    /// <inheritdoc cref="INumberBase{TSelf}.IsPositiveInfinity(TSelf)" />
    public static bool IsPositiveInfinity(IntComplex value)
    {
        // since complex numbers do not have a well-defined concept of
        // positive we report false if this value has an imaginary part

        return (value.m_imaginary == 0) && double.IsPositiveInfinity(value.m_real);
    }

    /// <inheritdoc cref="INumberBase{TSelf}.IsRealNumber(TSelf)" />
    public static bool IsRealNumber(IntComplex value) => (value.m_imaginary == 0) && double.IsRealNumber(value.m_real);

    /// <inheritdoc cref="INumberBase{TSelf}.IsSubnormal(TSelf)" />
    public static bool IsSubnormal(IntComplex value)
    {
        // much as IsInfinite allows either part to be infinite, we allow either
        // part to be "subnormal" (finite, non-zero, and non-normal) to be true

        return double.IsSubnormal(value.m_real) || double.IsSubnormal(value.m_imaginary);
    }

    /// <inheritdoc cref="INumberBase{TSelf}.Parse(ReadOnlySpan{char}, NumberStyles, IFormatProvider?)" />
    public static IntComplex Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider provider)
    {
        if (!TryParse(s, style, provider, out IntComplex result))
        {
            throw new OverflowException();
        }
        return result;
    }

    /// <inheritdoc cref="INumberBase{TSelf}.Parse(string, NumberStyles, IFormatProvider?)" />
    public static IntComplex Parse(string s, NumberStyles style, IFormatProvider provider)
    {
        ArgumentNullException.ThrowIfNull(s);
        return Parse(s.AsSpan(), style, provider);
    }

    private static bool TryConvertFrom<TOther>(TOther value, out IntComplex result)
        where TOther : INumberBase<TOther>
    {
        // We don't want to defer to `double.Create*(value)` because some type might have its own
        // `TOther.ConvertTo*(value, out Complex result)` handling that would end up bypassed.

        if (typeof(TOther) == typeof(byte))
        {
            byte actualValue = (byte)(object)value;
            result = actualValue;
            return true;
        }
        else if (typeof(TOther) == typeof(char))
        {
            char actualValue = (char)(object)value;
            result = actualValue;
            return true;
        }
        else if (typeof(TOther) == typeof(decimal))
        {
            decimal actualValue = (decimal)(object)value;
            result = (IntComplex)actualValue;
            return true;
        }
        else if (typeof(TOther) == typeof(double))
        {
            double actualValue = (double)(object)value;
            result = actualValue;
            return true;
        }
        else if (typeof(TOther) == typeof(Half))
        {
            Half actualValue = (Half)(object)value;
            result = actualValue;
            return true;
        }
        else if (typeof(TOther) == typeof(short))
        {
            short actualValue = (short)(object)value;
            result = actualValue;
            return true;
        }
        else if (typeof(TOther) == typeof(int))
        {
            int actualValue = (int)(object)value;
            result = actualValue;
            return true;
        }
        else if (typeof(TOther) == typeof(long))
        {
            long actualValue = (long)(object)value;
            result = actualValue;
            return true;
        }
        else if (typeof(TOther) == typeof(Int128))
        {
            Int128 actualValue = (Int128)(object)value;
            result = (IntComplex)actualValue;
            return true;
        }
        else if (typeof(TOther) == typeof(nint))
        {
            nint actualValue = (nint)(object)value;
            result = actualValue;
            return true;
        }
        else if (typeof(TOther) == typeof(sbyte))
        {
            sbyte actualValue = (sbyte)(object)value;
            result = actualValue;
            return true;
        }
        else if (typeof(TOther) == typeof(float))
        {
            float actualValue = (float)(object)value;
            result = actualValue;
            return true;
        }
        else if (typeof(TOther) == typeof(ushort))
        {
            ushort actualValue = (ushort)(object)value;
            result = actualValue;
            return true;
        }
        else if (typeof(TOther) == typeof(uint))
        {
            uint actualValue = (uint)(object)value;
            result = actualValue;
            return true;
        }
        else if (typeof(TOther) == typeof(ulong))
        {
            ulong actualValue = (ulong)(object)value;
            result = actualValue;
            return true;
        }
        else if (typeof(TOther) == typeof(UInt128))
        {
            UInt128 actualValue = (UInt128)(object)value;
            result = (IntComplex)actualValue;
            return true;
        }
        else if (typeof(TOther) == typeof(nuint))
        {
            nuint actualValue = (nuint)(object)value;
            result = actualValue;
            return true;
        }
        else
        {
            result = default;
            return false;
        }
    }

    /// <inheritdoc cref="INumberBase{TSelf}.TryParse(ReadOnlySpan{char}, NumberStyles, IFormatProvider?, out TSelf)" />
    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider provider, out IntComplex result)
    {
        ValidateParseStyleFloatingPoint(style);

        int openBracket = s.IndexOf('<');
        int semicolon = s.IndexOf(';');
        int closeBracket = s.IndexOf('>');

        if ((s.Length < 5) || (openBracket == -1) || (semicolon == -1) || (closeBracket == -1) || (openBracket > semicolon) || (openBracket > closeBracket) || (semicolon > closeBracket))
        {
            // We need at least 5 characters for `<0;0>`
            // We also expect a to find an open bracket, a semicolon, and a closing bracket in that order

            result = default;
            return false;
        }

        if ((openBracket != 0) && (((style & NumberStyles.AllowLeadingWhite) == 0) || !s.Slice(0, openBracket).IsWhiteSpace()))
        {
            // The opening bracket wasn't the first and we either didn't allow leading whitespace
            // or one of the leading characters wasn't whitespace at all.

            result = default;
            return false;
        }

        if (!int.TryParse(s.Slice(openBracket + 1, semicolon - openBracket - 1), style, provider, out int real))
        {
            result = default;
            return false;
        }

        if (char.IsWhiteSpace(s[semicolon + 1]))
        {
            // We allow a single whitespace after the semicolon regardless of style, this is so that
            // the output of `ToString` can be correctly parsed by default and values will roundtrip.
            semicolon += 1;
        }

        if (!int.TryParse(s.Slice(semicolon + 1, closeBracket - semicolon - 1), style, provider, out int imaginary))
        {
            result = default;
            return false;
        }

        if ((closeBracket != (s.Length - 1)) && (((style & NumberStyles.AllowTrailingWhite) == 0) || !s.Slice(closeBracket).IsWhiteSpace()))
        {
            // The closing bracket wasn't the last and we either didn't allow trailing whitespace
            // or one of the trailing characters wasn't whitespace at all.

            result = default;
            return false;
        }

        result = new IntComplex(real, imaginary);
        return true;

        static void ValidateParseStyleFloatingPoint(NumberStyles style)
        {
            // Check for undefined flags or hex number
            if ((style & (InvalidNumberStyles | NumberStyles.AllowHexSpecifier)) != 0)
            {
                ThrowInvalid(style);

                static void ThrowInvalid(NumberStyles value)
                {
                    if ((value & InvalidNumberStyles) != 0)
                    {
                        throw new ArgumentException();
                    }

                    throw new ArgumentException();
                }
            }
        }
    }

    /// <inheritdoc cref="INumberBase{TSelf}.TryParse(string, NumberStyles, IFormatProvider?, out TSelf)" />
    public static bool TryParse([NotNullWhen(true)] string s, NumberStyles style, IFormatProvider provider, out IntComplex result)
    {
        if (s is null)
        {
            result = default;
            return false;
        }
        return TryParse(s.AsSpan(), style, provider, out result);
    }

    //
    // IParsable
    //

    /// <inheritdoc cref="IParsable{TSelf}.Parse(string, IFormatProvider?)" />
    public static IntComplex Parse(string s, IFormatProvider provider) => Parse(s, DefaultNumberStyle, provider);

    /// <inheritdoc cref="IParsable{TSelf}.TryParse(string?, IFormatProvider?, out TSelf)" />
    public static bool TryParse([NotNullWhen(true)] string s, IFormatProvider provider, out IntComplex result) => TryParse(s, DefaultNumberStyle, provider, out result);

    //
    // ISpanFormattable
    //

    /// <inheritdoc cref="ISpanFormattable.TryFormat(Span{char}, out int, ReadOnlySpan{char}, IFormatProvider)" />
    public bool TryFormat(Span<char> destination, out int charsWritten, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider provider = null) =>
        TryFormatCore(destination, out charsWritten, format, provider);

    public bool TryFormat(Span<byte> utf8Destination, out int bytesWritten, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider provider = null) =>
        TryFormatCore(utf8Destination, out bytesWritten, format, provider);

    private bool TryFormatCore<TChar>(Span<TChar> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider provider) where TChar : unmanaged, IBinaryInteger<TChar>
    {
        Debug.Assert(typeof(TChar) == typeof(char) || typeof(TChar) == typeof(byte));

        // We have at least 6 more characters for: <0; 0>
        if (destination.Length >= 6)
        {
            int realChars;
            if (typeof(TChar) == typeof(char) ?
                m_real.TryFormat(MemoryMarshal.Cast<TChar, char>(destination.Slice(1)), out realChars, format, provider) :
                m_real.TryFormat(MemoryMarshal.Cast<TChar, byte>(destination.Slice(1)), out realChars, format, provider))
            {
                destination[0] = TChar.CreateTruncating('<');
                destination = destination.Slice(1 + realChars); // + 1 for <

                // We have at least 4 more characters for: ; 0>
                if (destination.Length >= 4)
                {
                    int imaginaryChars;
                    if (typeof(TChar) == typeof(char) ?
                        m_imaginary.TryFormat(MemoryMarshal.Cast<TChar, char>(destination.Slice(2)), out imaginaryChars, format, provider) :
                        m_imaginary.TryFormat(MemoryMarshal.Cast<TChar, byte>(destination.Slice(2)), out imaginaryChars, format, provider))
                    {
                        // We have 1 more character for: >
                        if ((uint)(2 + imaginaryChars) < (uint)destination.Length)
                        {
                            destination[0] = TChar.CreateTruncating(';');
                            destination[1] = TChar.CreateTruncating(' ');
                            destination[2 + imaginaryChars] = TChar.CreateTruncating('>');

                            charsWritten = realChars + imaginaryChars + 4;
                            return true;
                        }
                    }
                }
            }
        }

        charsWritten = 0;
        return false;
    }

    //
    // ISpanParsable
    //

    /// <inheritdoc cref="ISpanParsable{TSelf}.Parse(ReadOnlySpan{char}, IFormatProvider?)" />
    public static IntComplex Parse(ReadOnlySpan<char> s, IFormatProvider provider) => Parse(s, DefaultNumberStyle, provider);

    /// <inheritdoc cref="ISpanParsable{TSelf}.TryParse(ReadOnlySpan{char}, IFormatProvider?, out TSelf)" />
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider provider, out IntComplex result) => TryParse(s, DefaultNumberStyle, provider, out result);

    //
    // IUnaryPlusOperators
    //

    /// <inheritdoc cref="IUnaryPlusOperators{TSelf, TResult}.op_UnaryPlus(TSelf)" />
    public static IntComplex operator +(IntComplex value) => value;
}
