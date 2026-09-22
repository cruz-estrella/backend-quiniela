using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Repository;
using Models.Entities;
using UseCases.Shared;

namespace UseCases.Questions
{
    public class GetAllQuestions(IMongoRepository<Question> _repo)
    {
        public async Task<Result<List<Question>>> Execute(string tenantId)
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                return Result<List<Question>>.Failure("TenantId is required");

            var items = await _repo.FilterByAsync(q => q.TenantId == tenantId && q.IsActive).ConfigureAwait(false);
            var list = items?.ToList() ?? new List<Question>();

            return Result<List<Question>>.Success(list);
        }
    }
}
