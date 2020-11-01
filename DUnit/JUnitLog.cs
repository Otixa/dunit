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
			public string Name { get; set; }
			public string Message { get; set; }
			public bool Success { get; set; }
			public TimeSpan Duration { get; set; }
        }
		private List<Entry> Entries;
		public JUnitLog()
        {
			Entries = new List<Entry>();
        }
		public void AddSuccess(string name, TimeSpan duration)
        {
			Entries.Add(new Entry() { Name = name, Success = true, Duration = duration});
        }
		public void AddFailure(string name, string message, TimeSpan duration)
        {
			Entries.Add(new Entry() { Name = name, Success = false, Message = message, Duration = duration});
		}

		public string Serialize()
        {
			var ts = new Testsuites();
			foreach (var entry in Entries)
            {
				ts.Testsuite.Add(
					new Testsuite()
					{
						Name = entry.Name,
						Errors = entry.Success ? "0" : "1",
						Failures = entry.Success ? "0" : "1",
						Tests = "1",
						Testcase = new List<Testcase>()
                        {
							new Testcase(){
								Classname="DUnit", 
								Name = entry.Name,
								Failure = entry.Success ? null : new Failure(){Message=entry.Message, Text="Test Failed" }, 
								Time = $"{entry.Duration.TotalSeconds}"}
                        }

					}
				);
            }

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
