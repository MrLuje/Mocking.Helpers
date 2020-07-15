using Mocking.Helpers.Interfaces;

namespace Mocking.Helpers.FakeItEasy
{
    public class FakeItEasyProvider : BaseMockingProvider
    {
        public override string[] MockingMethodNames { get; } = new[] { "CallTo" };
        public override string AssemblyName => "FakeItEasy";
        public override string MockingWildcardMethod => "A<{0}>._";
    }
}
