using ApplicationCore.DtoMapper;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationCore.DI;
public static class DtoMapperDI
{
	public static void AddDtoMapper(this IServiceCollection services)
	{
		IMapper mapper = MappingConfig.CreateConfiguration().CreateMapper();
		services.AddSingleton(mapper);
	}
}
