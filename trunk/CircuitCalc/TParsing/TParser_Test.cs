﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace CircuitCalc.TParsing
{
	[TestFixture]
	public class TParser_Test
	{
		private TParser parser = new TParser();

		[Test]
		public void ParseStuff()
		{
			var matrices = parser.ParseFuel(new TStream("2202202201010220101022022010102201010"));
			foreach(var m in matrices)
			{
				Console.Write(m.ToString());
			}
			Console.WriteLine();
		}

		[Test]
		public void ParseCar()
		{
			var chambers = parser.ParseChambers(new TStream("220 2211 0 10 11 12 0 1 12 2211 0 10 11 12 0 2210 0 10 11"));
			foreach(var chamber in chambers) Console.WriteLine(chamber);
		}

		[Test]
		public void ParseFuel()
		{
			var ms = new TParser().ParseFuel(new TStream("1211222111"));
			foreach(var m in ms)
			{
				Console.WriteLine(m.ToString());
				Console.WriteLine();
			}
		}
	}
}
