using ApplicationCore.Views;
using ApplicationCore.Models;
using AutoMapper;

namespace ApplicationCore.Helpers;

public static class OptionsHelpers
{
	public static OptionViewModel MapViewModel(this Option option, IMapper mapper)
		=> mapper.Map<OptionViewModel>(option);

	public static Option MapEntity(this OptionViewModel model, IMapper mapper)
		=> mapper.Map<OptionViewModel, Option>(model);
}
