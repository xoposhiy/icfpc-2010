﻿using CircuitCalc.PeCalc;
using NUnit.Framework;

namespace CircuitCalc.AkCalc
{
	[TestFixture]
	public class CalcTest
	{
		[Test]
		public void Input()
		{
			var calc = new Calculator();
			calc.Run(keyCircuit, keyInput);
		}

		private const string keyCircuit = @"19L:
12R13R0#1R12R,
14R0L0#4R9L,
9R10R0#3L8L,
2L17R0#5L9R,
15R1L0#10R13R,
3L18R0#6L15L,
5L11R0#13L12L,
19R16R0#11R8R,
2R7R0#11L10L,
1R3R0#18L2L,
8R4L0#16L2R,
8L7L0#15R6R,
6R0R0#14L0L,
6L4R0#14R0R,
12L13L0#17L1L,
5R11L0#16R4L,
10L15L0#17R7R,
14L16L0#18R3R,
9L17L0#19R5R,
X18L0#X7L:
19L
";
		private const string keyInput = @"0,2,2,2,2,2,2,0,2,1,0,1,1,0,0,1,1";
	}
}