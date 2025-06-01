using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Threading;

public class CSVReader {
	static string SPLIT_RE = ";";
	//static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
	//static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
	static string LINE_SPLIT_RE = @"\n|\r";
	static char[] TRIM_CHARS = { '\"' };
	static string space = "|";

	/// <summary>
	/// Read the specified data.
	/// </summary>
	public static List<Dictionary<string, string>> Read (TextAsset data) {
		CultureInfo ci = new CultureInfo ("en-us");
		Thread.CurrentThread.CurrentCulture = ci;
		Thread.CurrentThread.CurrentUICulture = ci;
		var list = new List<Dictionary<string, string>> ();
		if (data == null) {
			return list;
		}
		var lines = Regex.Split (data.text, LINE_SPLIT_RE);
		if (lines.Length <= 1)
			return list;

		var header = Regex.Split (lines[0], SPLIT_RE);
		for (var i = 1; i < lines.Length; i++) {

			var values = Regex.Split (lines[i], SPLIT_RE);
			if (values.Length == 0 || values[0] == "")
				continue;

			var entry = new Dictionary<string, string> ();
			for (var j = 0; j < header.Length && j < values.Length; j++) {
				string value = values[j];
				value = value.TrimStart (TRIM_CHARS).TrimEnd (TRIM_CHARS);
				entry[header[j]] = value;
			}
			list.Add (entry);
		}
		return list;
	}

	/// <summary>
	/// Reads the string data.
	/// </summary>
	public static List<Dictionary<string, string>> ReadStringData (string data) {
		CultureInfo ci = new CultureInfo ("en-us");
		Thread.CurrentThread.CurrentCulture = ci;
		Thread.CurrentThread.CurrentUICulture = ci;
		var list = new List<Dictionary<string, string>> ();
		if (string.IsNullOrEmpty (data)) {
			return list;
		}
		//		var lines = Regex.Split(data, LINE_SPLIT_RE);
		var lines = data.Split (new char[] { ' ', '\n', '\r' });
		if (lines.Length <= 1)
			return list;
		var header = Regex.Split (lines[0], SPLIT_RE);
		for (var i = 1; i < lines.Length; i++) {
			var values = Regex.Split (lines[i], SPLIT_RE);
			if (values.Length == 0 || values[0] == "")
				continue;
			var entry = new Dictionary<string, string> ();
			for (var j = 0; j < header.Length && j < values.Length; j++) {
				string value = values[j];
				value = value.TrimStart (TRIM_CHARS).TrimEnd (TRIM_CHARS);
				value = value.Replace (space, " ");
				entry[header[j]] = value;
			}
			list.Add (entry);
		}
		return list;
	}


	public static List<Dictionary<string, string>> ReadStringDataType (string data) {
		CultureInfo ci = new CultureInfo ("en-us");
		Thread.CurrentThread.CurrentCulture = ci;
		Thread.CurrentThread.CurrentUICulture = ci;
		var list = new List<Dictionary<string, string>> ();
		if (string.IsNullOrEmpty (data)) {
			return list;
		}
		var lines = Regex.Split (data, LINE_SPLIT_RE);
		if (lines.Length <= 1)
			return list;
		var header = Regex.Split (lines[0], SPLIT_RE);
		for (var i = 1; i < lines.Length; i++) {
			var values = Regex.Split (lines[i], SPLIT_RE);
			if (values.Length == 0 || values[0] == "")
				continue;
			var entry = new Dictionary<string, string> ();
			for (var j = 0; j < header.Length && j < values.Length; j++) {
				string value = values[j];
				value = value.TrimStart (TRIM_CHARS).TrimEnd (TRIM_CHARS);
				value = value.Replace (space, " ");
				entry[header[j]] = value;
			}
			list.Add (entry);
		}
		return list;
	}
	/// <summary>
	/// Read the specified file.
	/// </summary>
	/// <param name="file">path to the file in Resources directory.</param>
	public static List<Dictionary<string, string>> Read (string file) {
		CultureInfo ci = new CultureInfo ("en-us");
		Thread.CurrentThread.CurrentCulture = ci;
		Thread.CurrentThread.CurrentUICulture = ci;
		TextAsset data = Resources.Load (file) as TextAsset;
		var list = Read (data);
		return list;
	}

	/*
     * Read text return Dictionary with key - value
     */

	public static Dictionary<string, string> ReadKeyValue (string data) {
		CultureInfo ci = new CultureInfo ("en-us");
		Thread.CurrentThread.CurrentCulture = ci;
		Thread.CurrentThread.CurrentUICulture = ci;
		Dictionary<string, string> dic = new Dictionary<string, string> ();
		if (data == null) {
			return dic;
		}
		var lines = Regex.Split (data, LINE_SPLIT_RE);
		if (lines.Length <= 1)
			return dic;
		//        var header = Regex.Split(lines[0], SPLIT_RE);
		for (var i = 1; i < lines.Length; i++) {
			var values = Regex.Split (lines[i], SPLIT_RE);
			if (values.Length == 0 || values[0] == "")
				continue;
			string key = values[0];
			key = key.TrimStart (TRIM_CHARS).TrimEnd (TRIM_CHARS);
			key = key.Replace (space, " ");
			string value = values[1];
			value = value.TrimStart (TRIM_CHARS).TrimEnd (TRIM_CHARS);
			value = value.Replace (space, " ");
			dic[key] = value;
		}

		return dic;
	}

	public static Dictionary<string, string> ReadKeyValueType (string data) {
		CultureInfo ci = new CultureInfo ("en-us");
		Thread.CurrentThread.CurrentCulture = ci;
		Thread.CurrentThread.CurrentUICulture = ci;
		Dictionary<string, string> dic = new Dictionary<string, string> ();
		if (data == null) {
			return dic;
		}
		var lines = data.Split (new char[] { ' ', '\n', '\r' });
		if (lines.Length <= 1)
			return dic;
		//        var header = Regex.Split(lines[0], SPLIT_RE);
		for (var i = 1; i < lines.Length; i++) {
			var values = Regex.Split (lines[i], SPLIT_RE);
			if (values.Length == 0 || values[0] == "")
				continue;
			string key = values[0];
			key = key.TrimStart (TRIM_CHARS).TrimEnd (TRIM_CHARS);
			key = key.Replace (space, " ");
			string value = values[1];
			value = value.TrimStart (TRIM_CHARS).TrimEnd (TRIM_CHARS);
			value = value.Replace (space, " ");
			dic[key] = value;
		}

		return dic;
	}
}