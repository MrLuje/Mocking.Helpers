using Mocking.Helpers.Interfaces;

namespace Mocking.Helpers.Moq
{
    public class MoqProvider : BaseMockingProvider
    {
        public override string[] MockingMethodNames { get; } = new[] { "Setup", "Verify" };
        public override string AssemblyName => "Moq";
        public override string MockingWildcardMethod => "It.IsAny<{0}>()";
    }
}
