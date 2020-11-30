namespace FactsGreet.Services.Data.Tests
{
    using System;
    using System.Reflection;

    using FactsGreet.Services.Mapping;
    using FactsGreet.Web.ViewModels;

    public abstract class Tests<T>
    {
        protected readonly Random Random;

        protected Tests()
        {
            this.Random = new Random(this.GetHashCode());
            AutoMapperConfig.RegisterMappings(
                typeof(ErrorViewModel).Assembly,
                Assembly.GetExecutingAssembly(),
                typeof(T).Assembly);
        }
    }
}
