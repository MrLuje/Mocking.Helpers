using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Mocking.Helpers.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mocking.Helpers.NSubstitute
{
    public class NSubstituteProvider : BaseMockingProvider
    {
        public override string AssemblyName => "NSubstitute";
        public override string MockingMethodName => "For";
        public override string MockingWildcardMethod => "Arg.Any<{0}>()";
    }
}
