using ApplicationCore.Views;
using ApplicationCore.Models;
using ApplicationCore.Paging;
using AutoMapper;
using Newtonsoft.Json;
using ApplicationCore.DataAccess;
using ApplicationCore.Specifications;

namespace ApplicationCore.Helpers;

public static class ResolvesHelpers
{
	public static async Task<IEnumerable<Resolve>> FetchAsync(this IDefaultRepository<Resolve> resolvesRepository, int questionId = 0)
	{
		if (questionId > 0) return await resolvesRepository.ListAsync(new ResolvesSpecification(questionId));
		return await resolvesRepository.ListAsync(new ResolvesSpecification());
	}
	public static async Task<IEnumerable<Resolve>> FetchByExamAsync(this IDefaultRepository<Resolve> resolvesRepository, Exam exam)
		=> await resolvesRepository.ListAsync(new ResolvesSpecification(exam));
	public static ResolveViewModel MapViewModel(this Resolve resolve, IMapper mapper, ICollection<UploadFile>? attachmentsList = null)
	{
		if (attachmentsList!.HasItems()) resolve.LoadAttachments(attachmentsList!);

		var model = mapper.Map<ResolveViewModel>(resolve);

		if (!String.IsNullOrEmpty(model.Highlight)) model.Highlights =  JsonConvert.DeserializeObject<ICollection<string>>(model.Highlight)!;
		if (!String.IsNullOrEmpty(model.Source)) model.Sources = JsonConvert.DeserializeObject<ICollection<SourceViewModel>>(model.Source)!;

		return model;
	}

	public static List<ResolveViewModel> MapViewModelList(this IEnumerable<Resolve> resolves, IMapper mapper, ICollection<UploadFile>? attachmentsList = null)
		=> resolves.Select(item => MapViewModel(item, mapper, attachmentsList)).ToList();

	public static Resolve MapEntity(this ResolveViewModel model, IMapper mapper, string currentUserId, Resolve? entity = null)
	{
		if (entity == null) entity = mapper.Map<ResolveViewModel, Resolve>(model);
		else entity = mapper.Map<ResolveViewModel, Resolve>(model, entity);

		if (model.Id == 0) entity.SetCreated(currentUserId);
		entity.SetUpdated(currentUserId);

		if(!entity.Text!.HasHtmlTag()) entity.Text = entity.Text!.ReplaceNewLine();

		entity.Highlight = model.Highlights.HasItems() ? JsonConvert.SerializeObject(model.Highlights) : "";
		entity.Source = model.Sources.HasItems() ? JsonConvert.SerializeObject(model.Sources) : "";

		return entity;
	}

	public static PagedList<Resolve, ResolveViewModel> GetPagedList(this IEnumerable<Resolve> resolves, IMapper mapper,
		ICollection<UploadFile>? attachmentsList = null, int page = 1, int pageSize = 999)
	{
		var pageList = new PagedList<Resolve, ResolveViewModel>(resolves, page, pageSize);

		pageList.SetViewList(pageList.List.MapViewModelList(mapper, attachmentsList));

		return pageList;
	}
}
