using ApplicationCore.Views;
using ApplicationCore.Models;
using ApplicationCore.Paging;
using AutoMapper;
using ApplicationCore.DataAccess;
using ApplicationCore.Specifications;
using Infrastructure.Entities;

namespace ApplicationCore.Helpers;

public static class AttachmentsHelpers
{
	public static async Task<List<UploadFile>> FetchAsync(this IDefaultRepository<UploadFile> attachmentsRepository, PostType postType, int postId = 0)
	{
		if (postId > 0) return await attachmentsRepository.ListAsync(new AttachmentsByTypesSpecification(postType, postId));
		return await attachmentsRepository.ListAsync(new AttachmentsByTypesSpecification(postType));

	}
	public static async Task<List<UploadFile>> FetchAsync(this IDefaultRepository<UploadFile> attachmentsRepository, PostType postType, IList<int> postIds)
	{
		return await attachmentsRepository.ListAsync(new AttachmentsByTypesSpecification(postType, postIds));
	}
	public static async Task<List<UploadFile>> FetchByTypesAsync(this IDefaultRepository<UploadFile> attachmentsRepository, ICollection<PostType> postTypes)
	{
		return await attachmentsRepository.ListAsync(new AttachmentsByTypesSpecification(postTypes));
	}
	public static async Task SyncAttachmentsAsync(this IDefaultRepository<UploadFile> attachmentsRepository, PostType type, EntityBase entity, ICollection<UploadFile>? latestList)
	{
		var existingList = await FetchAsync(attachmentsRepository, type, entity.Id);

		SyncAttachments(attachmentsRepository, existingList, latestList);
	}
	
	static void SyncAttachments(IDefaultRepository<UploadFile> attachmentsRepository, ICollection<UploadFile>? existingList, ICollection<UploadFile>? latestList)
	{
		if (existingList.IsNullOrEmpty()) existingList = new List<UploadFile>();
		if (latestList.IsNullOrEmpty()) latestList = new List<UploadFile>();

		foreach (var existingItem in existingList!)
		{
			if (latestList!.Any(item => item.Id == existingItem.Id) == false)
			{
				existingItem.Removed = true;
			}
		}

		foreach (var latestItem in latestList!)
		{
			var existingItem = existingList.Where(item => item.Id == latestItem.Id).FirstOrDefault();

			if (existingItem != null) attachmentsRepository.DbContext.Entry(existingItem).CurrentValues.SetValues(latestItem);
			else attachmentsRepository.DbSet.Add(latestItem);

		}

		attachmentsRepository.DbContext.SaveChanges();

	}
	public static async Task<UploadFile?> FindByNameAsync(this IDefaultRepository<UploadFile> attachmentsRepository, string name, PostType postType, int postId)
	{
		var attachments = await attachmentsRepository.FetchAsync(postType, postId);
		if (attachments.IsNullOrEmpty()) return null;

		return attachments.Where(a => a.Name == name).FirstOrDefault();
	}
	public static AttachmentViewModel MapViewModel(this UploadFile attachment, IMapper mapper)
		=> mapper.Map<AttachmentViewModel>(attachment);

	public static List<AttachmentViewModel> MapViewModelList(this IEnumerable<UploadFile> attachments, IMapper mapper)
		=> attachments.Select(item => MapViewModel(item, mapper)).ToList();

	public static PagedList<UploadFile, AttachmentViewModel> GetPagedList(this IEnumerable<UploadFile> attachments, IMapper mapper, int page = 1, int pageSize = 99)
	{
		var pageList = new PagedList<UploadFile, AttachmentViewModel>(attachments, page, pageSize);

		pageList.SetViewList(pageList.List.MapViewModelList(mapper));

		return pageList;
	}

	public static UploadFile MapEntity(this AttachmentViewModel model, IMapper mapper, string currentUserId)
	{
		var entity = mapper.Map<AttachmentViewModel, UploadFile>(model);

		if (model.Id == 0) entity.SetCreated(currentUserId);
		else entity.SetUpdated(currentUserId);

		return entity;
	}

	public static IEnumerable<UploadFile> GetOrdered(this IEnumerable<UploadFile> attachments)
		=> attachments.OrderBy(item => item.Order);

	
}
