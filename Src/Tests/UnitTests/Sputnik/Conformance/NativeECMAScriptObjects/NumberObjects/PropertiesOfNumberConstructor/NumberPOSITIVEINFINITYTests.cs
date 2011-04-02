// <auto-generated />
namespace IronJS.Tests.UnitTests.Sputnik.Conformance.NativeECMAScriptObjects.NumberObjects.PropertiesOfNumberConstructor
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class NumberPOSITIVEINFINITYTests : SputnikTestFixture
    {
        public NumberPOSITIVEINFINITYTests()
            : base(@"Conformance\15_Native_ECMA_Script_Objects\15.7_Number_Objects\15.7.3_Properties_of_Number_Constructor\15.7.3.6_Number.POSITIVE_INFINITY")
        {
        }

        [Test]
        [Category("Sputnik Conformance")]
        [Category("ECMA 15.7.3.6")]
        [TestCase("S15.7.3.6_A1.js", Description = "Number.POSITIVE_INFINITY is +Infinity")]
        public void NumberPOSITIVE_INFINITYIsInfinity(string file)
        {
            RunFile(file);
        }

        [Test]
        [Category("Sputnik Conformance")]
        [Category("ECMA 15.7.3.6")]
        [TestCase("S15.7.3.6_A2.js", Description = "Number.POSITIVE_INFINITY is ReadOnly")]
        public void NumberPOSITIVE_INFINITYIsReadOnly(string file)
        {
            RunFile(file);
        }

        [Test]
        [Category("Sputnik Conformance")]
        [Category("ECMA 15.7.3.6")]
        [TestCase("S15.7.3.6_A3.js", Description = "Number.POSITIVE_INFINITY is DontDelete")]
        public void NumberPOSITIVE_INFINITYIsDontDelete(string file)
        {
            RunFile(file);
        }

        [Test]
        [Category("Sputnik Conformance")]
        [Category("ECMA 15.7.3.6")]
        [TestCase("S15.7.3.6_A4.js", Description = "Number.POSITIVE_INFINITY has the attribute DontEnum")]
        public void NumberPOSITIVE_INFINITYHasTheAttributeDontEnum(string file)
        {
            RunFile(file);
        }
    }
}