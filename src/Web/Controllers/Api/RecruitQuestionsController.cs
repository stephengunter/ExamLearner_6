using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Helpers;
using AutoMapper;
using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Views;
using ApplicationCore.Services;
using Web.Models;
using Web.Helpers;

namespace Web.Controllers.Api;

public class RecruitQuestionsController : BaseApiController
{
	private readonly IDataService _dataService;

	private readonly IDefaultRepository<Question> _questionsRepository;
	private readonly IDefaultRepository<UploadFile> _attachmentsRepository;
	private readonly IDefaultRepository<Subject> _subjectsRepository;

	private readonly IMapper _mapper;

	public RecruitQuestionsController(IDataService dataService, IDefaultRepository<Question> questionsRepository,
		IDefaultRepository<UploadFile> attachmentsRepository, IDefaultRepository<Subject> subjectsRepository, IMapper mapper)
	{
		_dataService = dataService;

		_questionsRepository = questionsRepository;
		_attachmentsRepository = attachmentsRepository;
		_subjectsRepository = subjectsRepository;
		_mapper = mapper;
	}

	[HttpGet("")]
	public async Task<ActionResult> Index(int mode, int recruit)
	{
		var yearRecruits = await _dataService.FetchYearRecruitsAsync();

		RQMode selectMode = mode.ToRQModeType();
		if (selectMode == RQMode.Unknown)
		{
			//初次載入頁面
			var rqIndexModel = new RQIndexViewModel();
			rqIndexModel.LoadModeOptions();

			rqIndexModel.YearRecruits = yearRecruits!.ToList();

			//考試科目
			var examSubjects = await _subjectsRepository.FetchExamSubjectsAsync();
			rqIndexModel.LoadSubjectOptions(examSubjects);

			return Ok(rqIndexModel);
		}


		if (selectMode != RQMode.Read)
		{
			ModelState.AddModelError("RQMode", "僅支援閱讀模式");
			return BadRequest(ModelState);
		}

		var recruitsViews = yearRecruits!.SelectMany(item => item.SubItems!);

		var selectedRecruitView = recruitsViews.FirstOrDefault(x => x.Id == recruit);
		if (selectedRecruitView == null)
		{
			ModelState.AddModelError("recruit", "年度不存在");
			return BadRequest(ModelState);
		}

		//取得題目與解析的附件
		var types = new List<PostType> { PostType.Question, PostType.Option, PostType.Resolve };
		var attachments = (await _attachmentsRepository.FetchByTypesAsync(types)).ToList();

		var model = new RQViewModel();

		var parts = selectedRecruitView.SubItems;
		if (parts!.HasItems())
		{
			foreach (var part in parts!)
			{
				var partView = await InitRQPartViewModelAsync(part, attachments);
				model.Parts.Add(partView);
			}

		}
		else
		{
			var partView = await InitRQPartViewModelAsync(selectedRecruitView, attachments);
			model.Parts.Add(partView);
		}

		model.LoadTitle();

		return Ok(model);

	}

	async Task<RQPartViewModel> InitRQPartViewModelAsync(RecruitViewModel recruitView, List<UploadFile> attachments)
	{
		var questions = await _questionsRepository.FetchByIdsAsync(recruitView.QuestionIds!);

		return new RQPartViewModel
		{
			Points = recruitView.Points,
			MultiAnswers = recruitView.MultiAnswers,
			Order = recruitView.Order,
			OptionCount = recruitView.OptionCount,
			Title = recruitView.Title,
			Questions = questions.MapViewModelList(_mapper, null, attachments, null)
		};
	}

}
