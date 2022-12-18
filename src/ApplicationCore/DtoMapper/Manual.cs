using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class ManualMappingProfile : Profile
{
	public ManualMappingProfile()
	{
		CreateMap<Manual, ManualViewModel>();

		CreateMap<ManualViewModel, Manual>();
	}
}

public class FeatureMappingProfile : Profile
{
	public FeatureMappingProfile()
	{
		CreateMap<Feature, FeatureViewModel>();

		CreateMap<FeatureViewModel, Feature>();
	}
}
