using Akka.Configuration;
using System;

using LogLevel = Akka.Event.LogLevel;

namespace Akka.Reactive.Tests
{
	/// <summary>
	///		Akka configurations for test.
	/// </summary>
	public static class TestConfigurations
	{
		/// <summary>
		///		The default configuration for use in unit tests.
		/// </summary>
		public static readonly Config Default = Log();

		/// <summary>
		///		Configuration with logging.
		/// </summary>
		public static Config Log(LogLevel logLevel = LogLevel.InfoLevel)
		{
			return ConfigurationFactory.ParseString(
				String.Format(@"
					akka {{
						loglevel = {0}

						suppress-json-serializer-warning = on
					}}
				",
				GetLogLevelName(logLevel))
			);
		}

		/// <summary>
		///		Get the name (for configuration) that corresponds to the specified Akka log level.
		/// </summary>
		/// <param name="logLevel">
		///		The log level.
		/// </param>
		/// <returns>
		///		The log level name.
		/// </returns>
		static string GetLogLevelName(LogLevel logLevel)
		{
			switch (logLevel)
			{
				case LogLevel.DebugLevel:
				{
					return "DEBUG";
				}
				case LogLevel.InfoLevel:
				{
					return "INFO";
				}
				case LogLevel.WarningLevel:
				{
					return "WARNING";
				}
				case LogLevel.ErrorLevel:
				{
					return "ERROR";
				}
				default:
				{
					throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, "Invalid log level.");
				}
			}
		}
	}
}
