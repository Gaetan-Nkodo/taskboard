using Xunit;

namespace TaskBoard.Api.Tests.Fixtures;

[CollectionDefinition("Api collection")]
public class ApiCollection : ICollectionFixture<ApiFactory>, ICollectionFixture<SqlServerContainerFixture>{}
