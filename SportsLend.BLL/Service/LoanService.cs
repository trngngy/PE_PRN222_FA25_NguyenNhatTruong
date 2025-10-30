using SportsLend.BLL.Repository;
using SportsLend.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsLend.BLL.Service
{
    public class LoanService
    {
        private readonly LoanRepository _loanRepository;

        public LoanService(LoanRepository loanRepository)
        {
            _loanRepository = loanRepository;
        }

        public LoanService()
        {
            _loanRepository = new LoanRepository();
        }

        public async Task<List<Loan>> GetAllLoansAsync()
        {
            return await _loanRepository.GetAllLoansWithDetailsAsync();
        }

        public async Task<Loan?> GetLoanByIdAsync(int loanId)
        {
            return await _loanRepository.GetLoanByIdWithDetailsAsync(loanId);
        }

        public async Task<bool> CreateLoanAsync(Loan loan)
        {
            try
            {
                await _loanRepository.CreateAsync(loan);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ProcessReturnAsync(int loanId, DateOnly returnDate)
        {
            try
            {
                await _loanRepository.ProcessReturnAsync(loanId, returnDate);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Member>> GetAllMembersAsync()
        {
            return await _loanRepository.GetAllMembersAsync();
        }

        public async Task<List<Equipment>> GetAvailableEquipmentAsync()
        {
            return await _loanRepository.GetAvailableEquipmentAsync();
        }

        public decimal CalculateFee(DateOnly loanDate, DateOnly returnDate, decimal dailyFee)
        {
            int days = returnDate.DayNumber - loanDate.DayNumber;
            if (days < 1) days = 1; // Minimum 1 day
            return days * dailyFee;
        }
    }
}
