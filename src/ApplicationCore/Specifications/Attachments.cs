using ApplicationCore.Models;
using Ardalis.Specification;

namespace ApplicationCore.Specifications;
public class AttachmentsSpecification : Specification<UploadFile>
{
	public AttachmentsSpecification()
	{
		Query.Where(item => !item.Removed);
	}
	public AttachmentsSpecification(IList<int> ids)
	{
		Query.Where(item => !item.Removed && ids.Contains(item.Id));
	}
}

public class AttachmentsByTypesSpecification : Specification<UploadFile>
{
	public AttachmentsByTypesSpecification(PostType postType)
	{
		Query.Where(item => !item.Removed && item.PostType == postType);
	}
	public AttachmentsByTypesSpecification(PostType postType, int postId)
	{
		Query.Where(item => !item.Removed && item.PostType == postType && item.PostId == postId);
	}
	public AttachmentsByTypesSpecification(PostType postType, IList<int> postIds)
	{
		Query.Where(item => !item.Removed && item.PostType == postType && postIds.Contains(item.PostId));
	}
	public AttachmentsByTypesSpecification(ICollection<PostType> postTypes)
	{
		Query.Where(item => !item.Removed && postTypes.Contains(item.PostType));
	}
}
