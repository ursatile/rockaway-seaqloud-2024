using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rockaway.WebApp.Services;
using Shouldly;

namespace Rockaway.WebApp.Tests.Services {
	public class StatusTests {
		[Fact]
		public void StatusReporter_Has_Correct_Hostname() {
			var hostname = Environment.MachineName;
			var status = new StatusReporter().GetStatus();
			status.Hostname.ShouldBe(hostname);
		}
	}
}
