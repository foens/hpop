using System;
using System.Diagnostics;
using System.Text;
using OpenPOP.Shared.Logging;

namespace OpenPOP.MIME.Decode
{
	/// <summary>
	/// Utility class for dealing with Base64 encoded strings
	/// </summary>
	internal static class Base64
	{
		private static byte[] DecodeToBytes(string toDecode)
		{
			try
			{
				return Convert.FromBase64String(toDecode);
			} catch (FormatException e)
			{
				DefaultLogger.CreateLogger().LogError("Base64:DecodeToBytes: (FormatException) " + e.Message);
				throw;
			}
		}

		/// <summary>
		/// Decodes a Base64 encoded string using a specified <see cref="System.Text.Encoding"/> 
		/// </summary>
		/// <param name="base64Encoded">Source string to decode</param>
		/// <param name="encoding">The encoding to use for the output</param>
		/// <returns>A decoded string</returns>
		/// <exception cref="FormatException">Thrown if the <para>base64Encoded</para> string is not a valid base64 encoded string</exception>
		/// <exception cref="System.Text.DecoderFallbackException"> Thrown if the encoding cannot successfully map a byte sequence to a character</exception>
		public static string Decode(string base64Encoded, Encoding encoding)
		{
			return encoding.GetString(DecodeToBytes(base64Encoded));
		}

		/// <summary>
		/// Decodes a Base64 encoded string using the Default encoding of the system
		/// </summary>
		/// <param name="base64Encoded">Source string to decode</param>
		/// <returns>A decoded string</returns>
		/// <remarks>
		/// If the string cannot be decoded, it falls back to using <see cref="Decode(string, Encoding)"/> 
		/// with the <see cref="Encoding.ASCII"/> encoding.
		/// </remarks>
		/// <exception cref="FormatException">Thrown if the <para>base64Encoded</para> string is not a valid base64 encoded string</exception>
		/// <exception cref="System.Text.DecoderFallbackException"> Thrown if the encoding cannot successfully map a byte sequence to a character</exception>
		public static string Decode(string base64Encoded)
		{
			try
			{
				return Decode(base64Encoded, Encoding.Default);
			} catch (DecoderFallbackException e)
			{
				DefaultLogger.CreateLogger().LogError("Base64:Decode: (DecoderFallbackException) " + e.Message);
			}
			return Decode(base64Encoded, Encoding.ASCII);
		}

		/// <summary>
		/// Decodes a Base64 encoded string using a specified encoding
		/// </summary>
		/// <param name="base64Encoded">Source string to decode</param>
		/// <param name="nameOfEncoding">The name of the encoding to use</param>
		/// <returns>A decoded string</returns>
		/// <exception cref="FormatException">Thrown if the <para>base64Encoded</para> string is not a valid base64 encoded string</exception>
		/// <exception cref="System.Text.DecoderFallbackException"> Thrown if the encoding cannot successfully map a byte sequence to a character</exception>
		public static string Decode(string base64Encoded, string nameOfEncoding)
		{
			try
			{
				return Decode(base64Encoded, Encoding.GetEncoding(nameOfEncoding));
			} catch (DecoderFallbackException e)
			{
				DefaultLogger.CreateLogger().LogError("Base64:Decode: (DecoderFallbackException) " + e.Message);
			} catch (ArgumentException e)
			{
				DefaultLogger.CreateLogger().LogError("Base64:Decode: Invalid encoding specified! " + e.Message);
			}
			return Decode(base64Encoded);
		}
	}
}