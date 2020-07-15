using Mocking.Helpers.Interfaces;

namespace Mocking.Helpers.NSubstitute
{
    public class NSubstituteProvider : BaseMockingProvider
    {
        public override string AssemblyName => "NSubstitute";
        public override string[] MockingMethodNames { get; } = new[] { "For" };
        public override string MockingWildcardMethod => "Arg.Any<{0}>()";
    }
}
