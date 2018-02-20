using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Mocking.Helpers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mocking.Helpers.Moq
{
    public class MoqProvider : BaseMockingProvider
    {
        public override string MockingMethodName => "Setup";
        public override string AssemblyName => "Moq";
        public override string MockingWildcardMethod => "It.IsAny<{0}>()";
    }
}
