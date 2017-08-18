using System.Text.RegularExpressions;

namespace PDollarGestureRecognizer {
	public static class TextUtil {
		
		private static readonly Regex Relative = new Regex(@"[\w|\/]+(Assets.*)", RegexOptions.IgnoreCase);
		private static readonly Regex Filename = new Regex(@"[\w|\/]+\/(.+)\.asset", RegexOptions.IgnoreCase);
		
		public static string GetRelativePath(string path) {
			var match = Relative.Match(path);
			return match.Success ? match.Groups[1].Value : "";
		}

		public static string GetFilename(string path) {
			var match = Filename.Match(path);
			return match.Success ? match.Groups[1].Value : "";
		}
	}
}
