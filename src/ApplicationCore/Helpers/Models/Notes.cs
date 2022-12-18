using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using ApplicationCore.Paging;
using AutoMapper;
using Newtonsoft.Json;
using ApplicationCore.Views;

namespace ApplicationCore.Helpers;

public static class NotesHelpers
{
	public static async Task<IEnumerable<Note>> FetchAsync(this IDefaultRepository<Note> notesRepository, IList<int> termIds)
		=> await notesRepository.ListAsync(new NotesByTermsSpecification(termIds));
	public static async Task<Note?> FindNoteLoadSubItemsAsync(this IDefaultRepository<Note> notesRepository, int id)
	{
		var note = await notesRepository.GetByIdAsync(id);
		if (note != null) note!.LoadSubItems(notesRepository.DbSet.AllSubItems());

		return note;
	}
	public static async Task<int> GetMaxOrderAsync(this IDefaultRepository<Note> notesRepository, Term term, int parentId)
	{
		var spec = new NotesByTermsSpecification(term, parentId);
		var list = await notesRepository.ListAsync(spec);

		if (list.IsNullOrEmpty()) return 0;
		return list.Max(item => item.Order);
	}
	public static NoteViewModel MapViewModel(this Note note, IMapper mapper, ICollection<UploadFile>? attachmentsList = null)
	{
		if (attachmentsList!.HasItems()) note.LoadAttachments(attachmentsList!);

		var model = mapper.Map<NoteViewModel>(note);

		if (!String.IsNullOrEmpty(model.Highlight)) model.Highlights =  JsonConvert.DeserializeObject<ICollection<string>>(model.Highlight)!;
		if (!String.IsNullOrEmpty(model.Reference)) model.References = JsonConvert.DeserializeObject<ICollection<ReferenceViewModel>>(model.Reference)!;

		return model;
	}

	public static List<NoteViewModel> MapViewModelList(this IEnumerable<Note> notes, IMapper mapper, ICollection<UploadFile>? attachmentsList = null)
		=> notes.Select(item => MapViewModel(item, mapper, attachmentsList)).ToList();

	public static Note MapEntity(this NoteViewModel model, IMapper mapper, string currentUserId, Note? entity = null)
	{
		if (entity == null) entity = mapper.Map<NoteViewModel, Note>(model);
		else entity = mapper.Map<NoteViewModel, Note>(model, entity);

		if (!entity.Text.HasHtmlTag()) entity.Text = entity.Text.ReplaceNewLine();

		entity.Highlight = model.Highlights.HasItems() ? JsonConvert.SerializeObject(model.Highlights) : "";
		entity.Reference = model.References.HasItems() ? JsonConvert.SerializeObject(model.References) : "";

		if (model.Id == 0) entity.SetCreated(currentUserId);
		entity.SetUpdated(currentUserId);

		return entity;
	}

	public static PagedList<Note, NoteViewModel> GetPagedList(this IEnumerable<Note> notes, IMapper mapper,
		ICollection<UploadFile>? attachmentsList = null, int page = 1, int pageSize = 999)
	{
		var pageList = new PagedList<Note, NoteViewModel>(notes, page, pageSize);

		pageList.SetViewList(pageList.List.MapViewModelList(mapper, attachmentsList));

		return pageList;
	}

	public static IEnumerable<Note> GetOrdered(this IEnumerable<Note> notes)
		=> notes.OrderBy(item => item.Order);

	public static IEnumerable<Note> FilterByKeyword(this IEnumerable<Note> notes, ICollection<string> keywords)
		=> notes.Where(item => keywords.Any(item.HasKeyword)).ToList();

	
}
