using Data.Repository;
using Models.Entities;
using UseCases.Shared;

namespace UseCases.Questions
{
    public class CreateQuestions(IMongoRepository<Question> _repo) 
    {
        public Task<Result<string>> Execute(CreateQuestionRequest request)
        {
            // Start ROP pipeline
            return ValidateRequest(request)
                .Bind(req => EnsureUniqueQuestion(req))
                .Bind(validReq => CreateNewQuestion(validReq));
        }

        private Task<Result<CreateQuestionRequest>> ValidateRequest(CreateQuestionRequest request)
        {
            if (request == null) return Result<CreateQuestionRequest>.Failure("Request is null").ToTask();

            if (string.IsNullOrWhiteSpace(request.TenantId))
                return Result<CreateQuestionRequest>.Failure("TenantId is required").ToTask();

            if (string.IsNullOrWhiteSpace(request.Text))
                return Result<CreateQuestionRequest>.Failure("Question text is required").ToTask();

            if (request.Hints == null || request.Hints.Count != 3)
                return Result<CreateQuestionRequest>.Failure("Exactly 3 hints are required").ToTask();

            if (request.Options == null || request.Options.Count != 3)
                return Result<CreateQuestionRequest>.Failure("Exactly 3 options are required").ToTask();

            var correctCount = request.Options.FindAll(o => o.IsCorrect).Count;
            if (correctCount != 1)
                return Result<CreateQuestionRequest>.Failure("There must be exactly one correct option").ToTask();

            return Result<CreateQuestionRequest>.Success(request).ToTask();
        }

        private async Task<Result<CreateQuestionRequest>> EnsureUniqueQuestion(CreateQuestionRequest request)
        {
            bool exists = await _repo.ExistsAsync(q => ((Question)(object)q).Text == request.Text && ((Question)(object)q).TenantId == request.TenantId && ((Question)(object)q).IsActive);
            if (exists) return Result<CreateQuestionRequest>.Failure("A question with the same text already exists");
            return Result<CreateQuestionRequest>.Success(request);
        }

        private async Task<Result<string>> CreateNewQuestion(CreateQuestionRequest request)
        {
            var question = new Question
            {
                Text = request.Text,
                TenantId = request.TenantId,
                IsActive = true,
                Hints = request.Hints,
                Options = request.Options
            };

            await _repo.InsertOneAsync(question).ConfigureAwait(false);
            return Result<string>.Success(question.Id);
            //prubea gits
        }
    }
}
