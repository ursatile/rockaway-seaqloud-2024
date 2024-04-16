using System.Reflection;

namespace Rockaway.WebApp.Services;

public interface IStatusReporter {
	public ServerStatus GetStatus();

	public TimeSpan GetUptime();
	
}

public class StatusReporter : IStatusReporter {
	private static readonly Assembly assembly = Assembly.GetEntryAssembly()!; 


	public TimeSpan GetUptime() => DateTimeOffset.UtcNow - System.Diagnostics.Process.GetCurrentProcess().StartTime; 

	public ServerStatus GetStatus() => new() {
		Assembly = assembly.FullName ?? "Assembly.GetEntryAssembly() returned null",
		Modified = new DateTimeOffset(File.GetLastWriteTimeUtc(assembly.Location), TimeSpan.Zero).ToString("O"),
		Hostname = Environment.MachineName,
		DateTime = DateTimeOffset.UtcNow.ToString("O"),
		UptimeString = GetUptime().ToString(@"hh\:mm\:ss")
	};
}

//"142:28:46"

public class ServerStatus {
	public string Assembly { get; set; } = String.Empty;
	public string Modified { get; set; } = String.Empty;
	public string Hostname { get; set; } = String.Empty;
	public string DateTime { get; set; } = String.Empty;
	public string UptimeString { get; set; } = String.Empty;
}