using FinanceTracker.Application.Services.Interfaces;
using FinanceTracker.Infrastructure.Repositories.Interfaces;

namespace FinanceTracker.Application.Services;

public class DashboardService : IDashboardService
{
  private readonly IAccountRepository _accountRepo;
  private readonly ITransactionRepository _transactionRepo;

  public DashboardService(IAccountRepository accountRepo, ITransactionRepository transactionRepo)
  {
    _accountRepo = accountRepo;
    _transactionRepo = transactionRepo;
  }
  public async Task<(decimal totalBalance, decimal totalIncome, decimal totalExpense, decimal netSavings)> getSummary(Guid useId)
  {
    var now = DateTime.UtcNow;
    var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

    var totalBalance = await _accountRepo.GetTotalBalanceAsync(useId);
    var totalIncome = await _transactionRepo.GetTotalIncomeAsync(useId, startOfMonth, now);
    var totalExpense = await _transactionRepo.GetTotalExpenseAsync(useId, startOfMonth, now);
    var netSaving = totalIncome - totalExpense;

    return (totalBalance, totalIncome, totalExpense, netSaving);
  }

}
