using ApplicationCore.Helpers;
using ApplicationCore.Views;
using ApplicationCore.Specifications;
using ApplicationCore.DataAccess;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using ApplicationCore.Models;
using Web.Models;
using ApplicationCore.Settings;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Web.Controllers.Admin;

public class ResolvesController : BaseAdminController
{
	private readonly IDefaultRepository<Resolve> _resolvesRepository;
	private readonly IDefaultRepository<Note> _notesRepository;
	private readonly IDefaultRepository<Term> _termsRepository;
	private readonly IDefaultRepository<ReviewRecord> _reviewRecordsRepository;
	private readonly IDefaultRepository<UploadFile> _attachmentsRepository;
	private readonly AdminSettings _adminSettings;
	private readonly IMapper _mapper;

	public ResolvesController(IDefaultRepository<Resolve> resolvesRepository, IDefaultRepository<ReviewRecord> reviewRecordsRepository,
		IDefaultRepository<Note> notesRepository, IDefaultRepository<Term> termsRepository,
		IDefaultRepository<UploadFile> attachmentsRepository, IOptions<AdminSettings> adminSettings, IMapper mapper)
	{
		_resolvesRepository = resolvesRepository;
		_reviewRecordsRepository = reviewRecordsRepository;
		_notesRepository = notesRepository;
		_termsRepository = termsRepository;
		_attachmentsRepository = attachmentsRepository;
		_adminSettings = adminSettings.Value;
		_mapper = mapper;
	}

	[HttpGet("")]
	public async Task<ActionResult> Index(int question = 0, int page = 1, int pageSize = 10)
	{
		var resolves = await _resolvesRepository.FetchAsync(question);

		List<UploadFile>? attachments = null;

		if (resolves.IsNullOrEmpty())
		{
			if (question > 0) return Ok(new List<ResolveViewModel>());
			else return Ok(resolves.GetPagedList(_mapper, attachments, page, pageSize));
		}

		var postIds = resolves.Select(x => x.Id).ToList();

		attachments = (await _attachmentsRepository.FetchAsync(PostType.Resolve, postIds)).ToList();

		if (question > 0)
		{
			var viewList = resolves.MapViewModelList(_mapper, attachments.ToList());
			foreach (var view in viewList)
			{
				foreach (var item in view.Sources)
				{
					await item.MapContentAsync(_notesRepository, _termsRepository);
				}
			}
			return Ok(viewList);
		}
		else
		{
			var pageList = resolves.GetPagedList(_mapper, attachments.ToList(), page, pageSize);
			return Ok(pageList);
		}
	}

	[HttpPost("")]
	public async Task<ActionResult> Store([FromBody] ResolveViewModel model)
	{
		ValidateRequestAsync(model);
		if (!ModelState.IsValid) return BadRequest(ModelState);

		var resolve = model.MapEntity(_mapper, CurrentUserId);

		resolve.Reviewed = true;
		resolve = await _resolvesRepository.AddAsync(resolve);

		if (model.Attachments.HasItems())
		{
			var attachments = model.Attachments.Select(item => item.MapEntity(_mapper, CurrentUserId)).ToList();
			foreach (var attachment in attachments)
			{
				attachment.PostType = PostType.Resolve;
				attachment.PostId = resolve.Id;
			}

			await _attachmentsRepository.AddRangeAsync(attachments);

			resolve.Attachments = attachments;
		}

		var reviewRecord = new ReviewRecord { Reviewed = true, Type = ReviewableType.Resolve, PostId = resolve.Id };
		reviewRecord.SetCreated(CurrentUserId);
		await _reviewRecordsRepository.AddAsync(reviewRecord);

		return Ok(resolve.MapViewModel(_mapper));
	}

	[HttpPut("{id}")]
	public async Task<ActionResult> Update(int id, [FromBody] ResolveViewModel model)
	{
		var resolve = await _resolvesRepository.GetByIdAsync(id);
		if (resolve == null) return NotFound();

		ValidateRequestAsync(model);
		if (!ModelState.IsValid) return BadRequest(ModelState);

		resolve = model.MapEntity(_mapper, CurrentUserId, resolve);
		resolve.Reviewed = true;

		await _resolvesRepository.UpdateAsync(resolve);

		if (model.Attachments.HasItems())
		{
			var attachments = model.Attachments.Select(item => item.MapEntity(_mapper, CurrentUserId)).ToList();
			foreach (var attachment in attachments)
			{
				attachment.PostType = PostType.Resolve;
				attachment.PostId = resolve.Id;
			}

			await _attachmentsRepository.SyncAttachmentsAsync(PostType.Resolve, resolve, attachments);

			resolve.Attachments = attachments;
		}
		else
		{
			await _attachmentsRepository.SyncAttachmentsAsync(PostType.Resolve, resolve, null);
		}

		var reviewRecord = new ReviewRecord { Reviewed = true, Type = ReviewableType.Resolve, PostId = resolve.Id };
		reviewRecord.SetCreated(CurrentUserId);
		await _reviewRecordsRepository.AddAsync(reviewRecord);

		return Ok();
	}

	[HttpDelete("{id}")]
	public async Task<ActionResult> Delete(int id)
	{
		var resolve = await _resolvesRepository.GetByIdAsync(id);
		if (resolve == null) return NotFound();

		resolve.Removed = true;
		resolve.SetUpdated(CurrentUserId);
		await _resolvesRepository.UpdateAsync(resolve);

		return Ok();
	}

	[HttpPost("admin")]
	public async Task<ActionResult> Admin(AdminRequest model)
	{
		ValidateRequest(model, _adminSettings);
		if (!ModelState.IsValid) return BadRequest(ModelState);

		//同步更新解析中的參考(因Note TermId可能變動)
		var resolves = await _resolvesRepository.FetchAsync();

		resolves = resolves.Where(x => !String.IsNullOrEmpty(x.Source)).ToList();

		var viewList = resolves.MapViewModelList(_mapper, new List<UploadFile>());
		foreach (var view in viewList)
		{
			foreach (var item in view.Sources)
			{
				await item.MapContentAsync(_notesRepository, _termsRepository);
			}

			var entity = resolves.FirstOrDefault(x => x.Id == view.Id);
			entity!.Source = view.Sources.HasItems() ? JsonConvert.SerializeObject(view.Sources) : "";

			await _resolvesRepository.UpdateAsync(entity);
		}

		return Ok();
	}


	async void ValidateRequestAsync(ResolveViewModel model)
	{
		if (model.Sources.HasItems())
		{
			foreach (var item in model.Sources)
			{
				if (item.NoteId > 0)
				{
					var note = await _notesRepository.GetByIdAsync(item.NoteId);
					if (note == null)
					{
						ModelState.AddModelError("sources", $"錯誤的參考. Note Id: ${item.NoteId}");
						return;
					}
				}
				else
				{
					var term = await _termsRepository.GetByIdAsync(item.TermId);
					if (term == null)
					{
						ModelState.AddModelError("sources", $"錯誤的參考. Term Id: ${item.TermId}");
						return;
					}
				}
			}
		}
		else
		{
			if (String.IsNullOrEmpty(model.Text) && model.Attachments.IsNullOrEmpty())
			{
				ModelState.AddModelError("text", "必須填寫內容");
				return;
			}
		}
	}

}
