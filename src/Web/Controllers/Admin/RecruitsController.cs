using ApplicationCore.Helpers;
using ApplicationCore.DataAccess;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using ApplicationCore.Models;
using ApplicationCore.Views;
using Infrastructure.Views;

namespace Web.Controllers.Admin;

public class RecruitsController : BaseAdminController
{
	private readonly IMapper _mapper;
	private readonly IDefaultRepository<Recruit> _recruitsRepository;
	private readonly IDefaultRepository<Subject> _subjectsRepository;

	public RecruitsController(IDefaultRepository<Recruit> recruitsRepository, IDefaultRepository<Subject> subjectsRepository, IMapper mapper)
	{
		_mapper = mapper;
		_recruitsRepository = recruitsRepository;
		_subjectsRepository = subjectsRepository;
	}

	[HttpGet("")]
	public async Task<ActionResult> Index(int parent = 0, int active = 1, int year = 0)
	{
		var recruits = await _recruitsRepository.FetchAsync(parent);

		if (active >= 0) recruits = recruits.Where(x => x.Active == active.ToBoolean());
		if (year > 0) recruits = recruits.Where(x => x.Year == year);
		
		if (recruits.HasItems())
		{
			recruits = recruits.GetOrdered();
			_recruitsRepository.LoadSubItems(recruits);
		}

		if (parent == 0)
		{
			var subjects = await _subjectsRepository.FetchAsync();
			_subjectsRepository.LoadSubItems(subjects);

			foreach (var recruit in recruits)
			{
				recruit.LoadParent(recruits);

				foreach (var subItem in recruit.SubItems!)
				{
					if (subItem.SubjectId > 0)
					{
						var subject = subjects.FirstOrDefault(x => x.Id == subItem.SubjectId);
						subject!.GetSubIds();

						subItem.Subject = subject;

						var subjectIds = new List<int> { subject.Id };
						subjectIds.AddRange(subject.SubIds!);
						subItem.SubjectIds = subjectIds;

					}
				}
			}
		}


		return Ok(recruits.MapViewModelList(_mapper));
	}

	[HttpGet("create")]
	public ActionResult Create()
	{
		var model = new RecruitEditForm();

		var subjects = _subjectsRepository.DbSet.AllRootItems();
		model.SubjectOptions = subjects.Select(item => new BaseOption<int>(item.Id, item.Title)).ToList();

		return Ok(model);
	}



	[HttpPost("")]
	public async Task<ActionResult> Store([FromBody] RecruitViewModel model)
	{
		await ValidateRequestAsync(model);
		if (!ModelState.IsValid) return BadRequest(ModelState);

		model.Order = model.Active ? 0 : -1;
		var recruit = model.MapEntity(_mapper, CurrentUserId);

		await _recruitsRepository.AddAsync(recruit);
		if (recruit.SubItems!.HasItems())
		{
			foreach (var item in recruit.SubItems!)
			{
				item.ParentId = recruit.Id;
			}
			await _recruitsRepository.AddRangeAsync(recruit.SubItems);
		}

		return Ok(recruit.Id);
	}

	[HttpGet("edit/{id}")]
	public async Task<ActionResult> Edit(int id)
	{
		var recruit = await _recruitsRepository.GetByIdAsync(id);
		if (recruit == null) return NotFound();

		_recruitsRepository.LoadSubItems(recruit);
		var model = new RecruitEditForm() { Recruit = recruit.MapViewModel(_mapper) };

		if (recruit.ParentId > 0)
		{
			var parent = await _recruitsRepository.GetByIdAsync(recruit.ParentId);
			if (parent != null) model.Recruit.Parent = parent.MapViewModel(_mapper);
		}
		else
		{
			var subjects = _subjectsRepository.DbSet.AllRootItems();
			model.SubjectOptions = subjects.Select(item => new BaseOption<int>(item.Id, item.Title)).ToList();
		}

		return Ok(model);
	}

	[HttpPut("{id}")]
	public async Task<ActionResult> Update(int id, [FromBody] RecruitViewModel model)
	{
		var recruit = await _recruitsRepository.GetByIdAsync(id);
		if (recruit == null) return NotFound();

		_recruitsRepository.LoadSubItems(recruit);

		await ValidateRequestAsync(model);
		if (!ModelState.IsValid) return BadRequest(ModelState);

		if (model.Active)
		{
			if (recruit.Active == false) model.Order = 0;
		}
		else
		{
			if (recruit.Active) model.Order = -1;
		}

		recruit = model.MapEntity(_mapper, CurrentUserId, recruit);
		var subItems = recruit.SubItems!.HasItems() ? recruit.SubItems : new List<Recruit>();

		await _recruitsRepository.UpdateAsync(recruit, subItems!);

		return Ok();
	}

	[HttpPost("order")]
	public async Task<ActionResult> Order([FromBody] OrderRequest model)
	{
		await _recruitsRepository.UpdateOrderAsync(model.TargetId, model.ReplaceId, model.Up);
		return Ok();
	}

	[HttpDelete("{id}")]
	public async Task<ActionResult> Delete(int id)
	{
		var recruit = await _recruitsRepository.GetByIdAsync(id);
		if (recruit == null) return NotFound();

		recruit.Removed = true;
		recruit.SetUpdated(CurrentUserId);

		await _recruitsRepository.UpdateAsync(recruit);

		return Ok();
	}

	async Task ValidateRequestAsync(RecruitViewModel model)
	{
		if (String.IsNullOrEmpty(model.Title)) ModelState.AddModelError("title", "請填寫標題");

		if (model.ParentId > 0)
		{
			var parent = await _recruitsRepository.GetByIdAsync(model.ParentId);
			if (parent == null) ModelState.AddModelError("parentId", "父項目錯誤");
			else
			{
				if (model.SubItems!.HasItems())
				{
					var points = model.SubItems!.Select(x => x.Points).ToList();
					if (points.Sum() != 100)
					{
						ModelState.AddModelError("points", "分數錯誤");
					}
				}

			}

		}
		else
		{
			if (model.Year <= 0) ModelState.AddModelError("year", "請填寫年度");
			if (model.SubItems.IsNullOrEmpty()) ModelState.AddModelError("subItems", "必須要有筆試項目");

			var subjectIds = model.SubItems!.Select(x => x.SubjectId).Distinct();
			if (subjectIds.Count() != model.SubItems!.Count())
			{
				ModelState.AddModelError("subItems", "筆試科目重複了");
			}
		}
	}

}
