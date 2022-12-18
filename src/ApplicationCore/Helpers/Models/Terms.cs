using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using ApplicationCore.Views;
using AutoMapper;
using Newtonsoft.Json;

namespace ApplicationCore.Helpers;
public static class TermsHelpers
{
	public static async Task<IEnumerable<Term>> FetchAllAsync(this IDefaultRepository<Term> termsRepository)
		=> await termsRepository.ListAsync(new TermsSpecification());
	public static async Task<IEnumerable<Term>> FetchAsync(this IDefaultRepository<Term> termsRepository, Subject subject, int parentId = -1)
	{
		if (parentId >= 0) return await termsRepository.ListAsync(new TermsBySubjectSpecification(subject, parentId));
		return await termsRepository.ListAsync(new TermsBySubjectSpecification(subject));
	}
	public static async Task<Term?> FindTermLoadSubItemsAsync(this IDefaultRepository<Term> termsRepository, int id)
	{
		var term = await termsRepository.FirstOrDefaultAsync(new TermsSpecification(id));
		if (term != null) term.LoadSubItems(termsRepository.DbSet.AllSubItems());

		return term;
	}
	public static async Task<int> GetMaxOrderAsync(this IDefaultRepository<Term> termsRepository, Subject subject, int parentId)
	{
		var spec = new TermsBySubjectSpecification(subject, parentId);
		var list = await termsRepository.ListAsync(spec);

		if (list.IsNullOrEmpty()) return 0;
		return list.Max(item => item.Order);
	}
	public static void LoadSubItems(this IDefaultRepository<Term> termsRepository, IEnumerable<Term> terms)
	{
		var subItems = termsRepository.DbSet.AllSubItems();
		foreach (var entity in terms)
		{
			entity.LoadSubItems(subItems);
		}
	}
	public static void LoadSubItems(this IEnumerable<Term> terms, List<Term> subItems)
	{
		foreach (var entity in terms)
		{
			entity.LoadSubItems(subItems);
		}
	}
	public static IEnumerable<Term> GetOrdered(this IEnumerable<Term> terms)
		=> terms.OrderBy(item => item.Order);

	public static IEnumerable<Term> FilterByKeyword(this IEnumerable<Term> terms, ICollection<string> keywords)
		=> terms.Where(item => keywords.Any(item.Text!.Contains)).ToList();

	public static TermViewModel MapViewModel(this Term term, IMapper mapper)
	{
		var model = mapper.Map<TermViewModel>(term);
		if (!String.IsNullOrEmpty(model.Highlight)) model.Highlights = JsonConvert.DeserializeObject<ICollection<string>>(model.Highlight)!;
		if (!String.IsNullOrEmpty(model.Reference)) model.References = JsonConvert.DeserializeObject<ICollection<ReferenceViewModel>>(model.Reference)!;

		if (term.SubItems!.HasItems()) model.SubItems = term.SubItems!.Select(item => item.MapViewModel(mapper)).ToList();

		return model;
	}

	public static List<TermViewModel> MapViewModelList(this IEnumerable<Term> terms, IMapper mapper)
		=> terms.Select(item => MapViewModel(item, mapper)).ToList();

	public static Term MapEntity(this TermViewModel model, IMapper mapper, string currentUserId, Term? entity)
	{
		if (entity == null) entity = mapper.Map<TermViewModel, Term>(model);
		else entity = mapper.Map<TermViewModel, Term>(model, entity);

		if (entity.Text!.HasHtmlTag() == false) entity.Text = entity.Text!.ReplaceNewLine();

		entity.Highlight = model.Highlights.HasItems() ? JsonConvert.SerializeObject(model.Highlights) : "";
		entity.Reference = model.References.HasItems() ? JsonConvert.SerializeObject(model.References) : "";


		if (model.Id == 0) entity.SetCreated(currentUserId);
		entity.SetUpdated(currentUserId);

		return entity;
	}
}
