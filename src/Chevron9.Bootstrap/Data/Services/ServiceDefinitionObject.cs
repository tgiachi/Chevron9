namespace Chevron9.Bootstrap.Data.Services;

public record struct ServiceDefinitionObject(Type ServiceType, Type ImplementationType, int Priority = 0);
