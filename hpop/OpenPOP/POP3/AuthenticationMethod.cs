namespace OpenPOP.POP3
{
	/// <summary>
	/// Authentication method to use
	/// </summary>
	/// <remarks>
	/// TRYBOTH means code will first attempt by using APOP method as its more secure.
	/// In case of failure the code will fall back to USERPASS method.
	/// </remarks>
	public enum AuthenticationMethod
	{
		/// <summary>
		/// Authenticate using the USER/PASS method.
		/// APOP is more secure but might not be supported on a server.
		/// Recomended AuthenticationMethod is APOP, but it does not matter
		/// if SSL is used.
		/// </summary>
		USERPASS = 0,
		/// <summary>
		/// Authenticate using the APOP method, which is more secure.
		/// </summary>
		APOP = 1,
		/// <summary>
		/// Authenticate using APOP first, which is more secure.
		/// If APOP is not supported on the server, authenticate
		/// using USER/PASS.
		/// </summary>
		TRYBOTH = 2
	}
}