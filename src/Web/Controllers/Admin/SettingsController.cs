using ApplicationCore.Services;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Views;
using ApplicationCore.Helpers;
using AutoMapper;
using Microsoft.Extensions.Options;
using ApplicationCore.Settings;
using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Specifications;

namespace Web.Controllers.Admin;

public class SettingsController : BaseAdminController
{
	private readonly IDataService _dataService;
	private readonly IDefaultRepository<Recruit> _recruitsRepository;
	private readonly IDefaultRepository<Subject> _subjectsRepository;
	private readonly IDefaultRepository<Term> _termsRepository;
	private readonly RootSubjectSettings _rootSubjectSettings;
	private readonly IMapper _mapper;

	public SettingsController(IOptions<RootSubjectSettings> rootSubjectSettings, IDataService dataService, IDefaultRepository<Recruit> recruitsRepository,
		IDefaultRepository<Subject> subjectsRepository, IDefaultRepository<Term> termsRepository, IMapper mapper)
	{
		_dataService = dataService;
		_recruitsRepository = recruitsRepository;
		_subjectsRepository = subjectsRepository;
		_termsRepository = termsRepository;
		_rootSubjectSettings = rootSubjectSettings.Value;
		_mapper = mapper;
	}

	[HttpGet("exam")]
	public async Task<ActionResult> FindExamSettings(int subject)
	{
		var rootSubject = await _subjectsRepository.FindSubjectLoadSubItemsAsync(subject);
		if (rootSubject == null) return NotFound();
		if (rootSubject.ParentId > 0)
		{
			ModelState.AddModelError("subject", "錯誤的科目");
			return BadRequest(ModelState);
		}

		var model = await _dataService.FindExamSettingsAsync(subject);
		if (model == null) return NotFound();

		var rootSubjectView = rootSubject.MapViewModel(_mapper);
		var subjectViews = rootSubject.SubItems!.MapViewModelList(_mapper);

		var allTerms = await _termsRepository.ListAsync(new TermsBySubjectSpecification(rootSubject.GetSubIds().ToList()));
		var termViews = allTerms.MapViewModelList(_mapper);

		model.Subject = rootSubjectView;
		foreach (var part in model.Parts!)
		{
			foreach (var subjectsSettings in part.Subjects)
			{
				subjectsSettings.Subject = subjectViews.FirstOrDefault(x => x.Id == subjectsSettings.SubjectId)!;
				foreach (var tqSettings in subjectsSettings.TermQuestions)
				{
					tqSettings.Term = termViews.FirstOrDefault(x => x.Id == tqSettings.TermId)!;
					foreach (var item in tqSettings.SubItems)
					{
						item.Term = termViews.FirstOrDefault(x => x.Id == item.TermId)!;
					}
				}
			}
		}


		return Ok(model);
	}

	[HttpPost("exam")]
	public async Task<ActionResult> SaveExamSettings([FromBody] ExamSettingsViewModel model)
	{
		var subjectId = model.SubjectId;
		model.Subject = null;
		model.Recruit = null;

		//移除object資料
		foreach (var part in model.Parts!)
		{
			foreach (var subjectsSettings in part.Subjects)
			{
				subjectsSettings.Subject = null;
				foreach (var tqSettings in subjectsSettings.TermQuestions)
				{
					tqSettings.Term = null;
					foreach (var item in tqSettings.SubItems)
					{
						item.Term = null;
					}
				}
			}
		}

		await _dataService.SaveExamSettingsAsync(subjectId, model);

		return Ok();
	}

}
