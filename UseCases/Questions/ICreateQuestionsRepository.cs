using System.Threading.Tasks;
using Models.Entities;

namespace UseCases.Questions
{
    public interface ICreateQuestionsRepository
    {
        Task<bool> ExistsByTextAsync(string tenantId, string text);
        Task InsertAsync(Question question);
    }
}
