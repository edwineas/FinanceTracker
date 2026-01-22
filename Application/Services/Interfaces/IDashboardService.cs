namespace FinanceTracker.Application.Services.Interfaces;

public interface IDashboardService
{
  Task<(decimal totalBalance, decimal totalIncome, decimal totalExpense, decimal netSavings)> getSummary(Guid userId);
}
