using Microsoft.EntityFrameworkCore.Storage;

namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// Generates sequential parameter names in the form <c>__p_n</c>.
    /// </summary>
    public sealed class SequentialParameterNameGeneratorFactory : IParameterNameGeneratorFactory
    {
        public ParameterNameGenerator Create() => new SequentialParameterNameGenerator();

        private sealed class SequentialParameterNameGenerator : ParameterNameGenerator
        {
            private int _count;

            public override string GenerateNext() => $"__p_{_count++}";

            public override void Reset() => _count = 0;
        }
    }
}
