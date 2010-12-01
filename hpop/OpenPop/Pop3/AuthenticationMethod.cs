namespace OpenPop.Pop3
{
	/// <summary>
	/// Describes the authentication method to use when authenticating towards a POP3 server.
	/// </summary>
	public enum AuthenticationMethod
	{
		/// <summary>
		/// Authenticate using the UsernameAndPassword method.<br/>
		/// This will pass the username and password to the server in cleartext.<br/>
		/// APOP is more secure but might not be supported on a server.<br/>
		/// This method is not recommended. Use TryBoth instead, which will use APOP if supported and
		/// fall back to UsernameAndPassword if not.<br/>
		/// <br/>
		/// If SSL is used, there is no loss of security by using this authentication method.
		/// </summary>
		UsernameAndPassword,

		/// <summary>
		/// Authenticate using the Authenticated Post Office Protocol method, which is more secure then UsernameAndPassword
		/// since it is a request-response protocol where server checks if the client knows a shared secret, which
		/// is the password, without the password itself being transmitted.<br/>
		/// <br/>
		/// This authentication method is not supported by many servers.<br/>
		/// Choose this option if you want maximum security.
		/// </summary>
		APOP,

		/// <summary>
		/// This is the recomended method to authenticate with.<br/>
		/// If APOP is supported by the server, APOP is used for authentication.<br/>
		/// If APOP is not supported, TryBoth will fall back to UsernameAndPassword authentication.
		/// </summary>
		TryBoth
	}
}