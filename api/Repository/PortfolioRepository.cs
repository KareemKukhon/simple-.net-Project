using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.data;
using api.interfaces;
using api.models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly ApplicationDBContext _context;
        public PortfolioRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Portfolio> CreateAsync(Portfolio portfolio)
        {
            await _context.Portfolios.AddAsync(portfolio);
            await _context.SaveChangesAsync();
            return portfolio;
        }

        public async Task<List<Stock>> GetUserPortfolio(AppUser user)
        {
            return await _context.Portfolios.Where(u => u.AppUserId == user.Id)
            .Select(stock => new Stock{
                Id = stock.StockId,
                Symbol = stock.stock.Symbol,
                CompanyName = stock.stock.CompanyName,
                Purchase = stock.stock.Purchase,
                LastDiv = stock.stock.LastDiv,
                Industry = stock.stock.Industry,
                MarketCap = stock.stock.MarketCap,
            }).ToListAsync();
        }
    }
}