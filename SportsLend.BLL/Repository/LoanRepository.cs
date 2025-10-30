using Microsoft.EntityFrameworkCore;
using SportsLend.BLL.Models;
using SportsLend.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsLend.BLL.Repository
{
    public class LoanRepository : GenericRepository<Loan>
    {
        public LoanRepository()
        {

        }

        public LoanRepository(SportsLendDBContext context)
        {
            _context = context;
        }

        public async Task<List<Loan>> GetAllLoansWithDetailsAsync()
        {
            return await _context.Loans
                .Include(l => l.Equipment)
                    .ThenInclude(e => e.Type)
                .Include(l => l.Member)
                .Include(l => l.CreatedByNavigation)
                .OrderByDescending(l => l.LoanDate)
                .ToListAsync();
        }

        public async Task<Loan?> GetLoanByIdWithDetailsAsync(int loanId)
        {
            return await _context.Loans
                .Include(l => l.Equipment)
                    .ThenInclude(e => e.Type)
                .Include(l => l.Member)
                .Include(l => l.CreatedByNavigation)
                .FirstOrDefaultAsync(l => l.LoanId == loanId);
        }

        public async Task ProcessReturnAsync(int loanId, DateOnly returnDate)
        {
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;

            var loan = await _context.Loans
               .Include(l => l.Equipment)
               .FirstOrDefaultAsync(l => l.LoanId == loanId);

            if (loan == null)
                throw new Exception("Loan not found");

            if (loan.ReturnDate != null)
                throw new Exception("Equipment already returned");

            // Set return date
            loan.ReturnDate = returnDate;

            // Increase stock
            loan.Equipment.InStock += 1;

            await _context.SaveChangesAsync();
        }

        public async Task<List<Member>> GetAllMembersAsync()
        {
            return await _context.Members
                .OrderBy(m => m.FullName)
                .ToListAsync();
        }

        public async Task<List<Equipment>> GetAvailableEquipmentAsync()
        {
            return await _context.Equipment
                .Include(e => e.Type)
                .Where(e => e.InStock > 0)
                .OrderBy(e => e.Name)
                .ToListAsync();
        }
    }
}
