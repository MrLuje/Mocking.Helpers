using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Mocking.Helpers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mocking.Helpers.FakeItEasy
{
    public class FakeItEasyProvider : BaseMockingProvider
    {
        public override string MockingMethodName => "CallTo";
        public override string AssemblyName => "FakeItEasy";
        public override string MockingWildcardMethod => "A<{0}>._";
    }
}
