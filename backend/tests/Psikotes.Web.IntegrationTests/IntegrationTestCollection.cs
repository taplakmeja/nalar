using Xunit;

namespace Psikotes.Web.IntegrationTests;

[CollectionDefinition("Integration")]
public sealed class IntegrationTestCollection : ICollectionFixture<PsikotesWebApplicationFactory>;
