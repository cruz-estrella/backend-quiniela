using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using UseCases.Questions;
using ROP;

namespace Quiniela.Controllers
{
    [ApiController]
    [Route("questions")]
    public class QuestionsController(QuestionsUseCases _questions) : Controller
    {
        //[HasPermissionOnAction(Constants.Actions.ReadForms)]
        [HttpGet("")]
        public async Task<ActionResult<GenericResponse<List<Question>>>> GetAllForms()
        {
            var result = await _questions.GetAllQuestions.Execute("tenant-id");
            return result
                .ToGenericResponse()
                .ToActionResult();
        }

    }
}
