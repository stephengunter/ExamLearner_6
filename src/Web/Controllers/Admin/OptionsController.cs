using ApplicationCore.Helpers;
using ApplicationCore.Views;
using ApplicationCore.Specifications;
using ApplicationCore.DataAccess;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using ApplicationCore.Models;

namespace Web.Controllers.Admin;

public class OptionsController : BaseAdminController
{
	private readonly IDefaultRepository<Question> _questionsRepository;
	private readonly IDefaultRepository<Option> _optionsRepository;
	private readonly IDefaultRepository<UploadFile> _attachmentsRepository;
	private readonly IMapper _mapper;

	public OptionsController(IDefaultRepository<Question> questionsRepository, IDefaultRepository<Option> optionssRepository,
		IDefaultRepository<UploadFile> attachmentsRepository, IMapper mapper)
	{
		_questionsRepository = questionsRepository;
		_optionsRepository = optionssRepository;
		_attachmentsRepository = attachmentsRepository;
		_mapper = mapper;
	}

	[HttpPost("")]
	public async Task<ActionResult> Store([FromBody] QuestionViewModel model)
	{
		var question = await _questionsRepository.FirstOrDefaultAsync(new QuestionsSpecification(model.Id));
		if (question == null)
		{
			ModelState.AddModelError("question", "錯誤的QuestionId");
			return BadRequest(ModelState);
		}

		var correctOptions = question.Options.Where(x => x.Correct);
		int minCorrectCounts = correctOptions.Count() == 0 ? 1 : 0;
		int maxCorrectCounts = question.MultiAnswers ? 999 : 1;

		ValidateRequest(model.Options, minCorrectCounts, maxCorrectCounts);
		if (!ModelState.IsValid) return BadRequest(ModelState);


		foreach (var optionModel in model.Options)
		{
			optionModel.QuestionId = question.Id;
			var option = optionModel.MapEntity(_mapper);
			option = await _optionsRepository.AddAsync(option);

			var mediaIds = optionModel.Attachments.Select(x => x.Id).ToList();
			var attachments = await _attachmentsRepository.ListAsync(new AttachmentsSpecification(mediaIds));
			foreach (var attachment in attachments)
			{
				attachment.PostType = PostType.Option;
				attachment.PostId = option.Id;
			}

			await _attachmentsRepository.UpdateRangeAsync(attachments);
		}

		return Ok();
	}


	void ValidateRequest(ICollection<OptionViewModel> models, int minCorrectCounts, int maxCorrectCounts)
	{
		if (models.IsNullOrEmpty())
		{
			ModelState.AddModelError("options", "正確選項數量錯誤");
		}
		else
		{
			var correctOptions = models.Where(item => item.Correct);
			if (correctOptions.Count() < minCorrectCounts || correctOptions.Count() > maxCorrectCounts)
			{
				ModelState.AddModelError("options", "正確選項數量錯誤");
			}
		}


	}

}
