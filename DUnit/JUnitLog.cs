/* 
 Licensed under the Apache License, Version 2.0

 http://www.apache.org/licenses/LICENSE-2.0
 */
using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace DUnit
{
	[XmlRoot(ElementName = "testsuite")]
	public class Testsuite
	{
		[XmlAttribute(AttributeName = "name")]
		public string Name { get; set; }
		[XmlAttribute(AttributeName = "errors")]
		public string Errors { get; set; }
		[XmlAttribute(AttributeName = "tests")]
		public string Tests { get; set; }
		[XmlAttribute(AttributeName = "failures")]
		public string Failures { get; set; }
		[XmlAttribute(AttributeName = "time")]
		public string Time { get; set; }
		[XmlAttribute(AttributeName = "timestamp")]
		public string Timestamp { get; set; }
		[XmlElement(ElementName = "properties")]
		public Properties Properties { get; set; }
		[XmlElement(ElementName = "testcase")]
		public List<Testcase> Testcase { get; set; }
		[XmlAttribute(AttributeName = "skipped")]
		public string Skipped { get; set; }
	}

	[XmlRoot(ElementName = "property")]
	public class Property
	{
		[XmlAttribute(AttributeName = "name")]
		public string Name { get; set; }
		[XmlAttribute(AttributeName = "value")]
		public string Value { get; set; }
	}

	[XmlRoot(ElementName = "properties")]
	public class Properties
	{
		[XmlElement(ElementName = "property")]
		public List<Property> Property { get; set; }
	}

	[XmlRoot(ElementName = "failure")]
	public class Failure
	{
		[XmlAttribute(AttributeName = "message")]
		public string Message { get; set; }
		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "testcase")]
	public class Testcase
	{
		[XmlElement(ElementName = "failure")]
		public Failure Failure { get; set; }
		[XmlAttribute(AttributeName = "classname")]
		public string Classname { get; set; }
		[XmlAttribute(AttributeName = "name")]
		public string Name { get; set; }
		[XmlAttribute(AttributeName = "time")]
		public string Time { get; set; }
		//[XmlElement(ElementName = "skipped")]
		//public string Skipped { get; set; }
	}

	[XmlRoot(ElementName = "testsuites")]
	public class Testsuites
	{
		[XmlElement(ElementName = "testsuite")]
		public List<Testsuite> Testsuite { get; set; }
		public Testsuites()
        {
			Testsuite = new List<Testsuite>();
        }
	}

	public class JUnitLog
    {
		private class Entry
        {
			public string ScriptName { get; set; }
			public string TestName { get; set; }
			public string Message { get; set; }
			public bool Success { get; set; }
			public TimeSpan Duration { get; set; }
        }
		private Dictionary<string, Testsuite> Entries;
		public JUnitLog()
        {
			Entries = new Dictionary<string, Testsuite>();
		}
		public void AddSuccess(string scriptName, string testFileName, string testName, TimeSpan duration)
        {
			if (!Entries.ContainsKey(scriptName)) Entries[scriptName] = new Testsuite()
			{
				Name = scriptName,
				Errors = "0",
				Failures = "0",
				Tests = "0",
				Testcase = new List<Testcase>()
			};

			Entries[scriptName].Testcase.Add(
				new Testcase()
				{
					Classname = testFileName,
					Name = testName,
					Time = $"{duration.TotalSeconds}"
				}
			);

			Entries[scriptName].Tests = $"{int.Parse(Entries[scriptName].Tests) + 1}";
		}
		public void AddFailure(string scriptName, string testFileName, string testName, string message, TimeSpan duration)
        {
			if (!Entries.ContainsKey(scriptName)) Entries[scriptName] = new Testsuite()
			{
				Name = scriptName,
				Errors = "0",
				Failures = "0",
				Tests = "0",
				Testcase = new List<Testcase>()
			};

			Entries[scriptName].Testcase.Add(
				new Testcase()
				{
					Classname = testFileName,
					Name = testName,
					Time = $"{duration.TotalSeconds}",
					Failure = new Failure() { Message = message, Text = "Test Failed" },
				}
			);

			Entries[scriptName].Tests = $"{int.Parse(Entries[scriptName].Tests) + 1}";
			Entries[scriptName].Failures = $"{int.Parse(Entries[scriptName].Failures) + 1}";
			Entries[scriptName].Errors = $"{int.Parse(Entries[scriptName].Errors) + 1}";
		}

		public string Serialize()
        {
			var ts = new Testsuites();
			ts.Testsuite = Entries.Values.ToList();

			var slz = new XmlSerializer(typeof(Testsuites));
			using (var ms = new System.IO.MemoryStream())
            {
				slz.Serialize(ms, ts);
				ms.Position = 0;
				using (var sr = new System.IO.StreamReader(ms))
                {
					return sr.ReadToEnd();
				}
			}
			
		}
    }
}
