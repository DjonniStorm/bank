using AutoMapper;
using BankContracts.DataModels;
using BankContracts.Exceptions;
using BankContracts.StorageContracts;
using BankDatabase.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace BankDatabase.Implementations;

internal class DepositStorageContract : IDepositStorageContract
{
    private readonly BankDbContext _dbContext;
    private readonly Mapper _mapper;

    public DepositStorageContract(BankDbContext dbContext)
    {
        _dbContext = dbContext;
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Clerk, ClerkDataModel>();
            cfg.CreateMap<Deposit, DepositDataModel>()
                .ForMember(dest => dest.Currencies, opt => opt.MapFrom(src => src.DepositCurrencies));
            cfg.CreateMap<DepositDataModel, Deposit>()
                .ForMember(dest => dest.DepositCurrencies, opt => opt.MapFrom(src => src.Currencies));
            cfg.CreateMap<DepositCurrency, DepositCurrencyDataModel>()
                .ForMember(dest => dest.DepositId, opt => opt.MapFrom(src => src.DepositId))
                .ForMember(dest => dest.CurrencyId, opt => opt.MapFrom(src => src.CurrencyId));
            cfg.CreateMap<DepositCurrencyDataModel, DepositCurrency>()
                .ForMember(dest => dest.DepositId, opt => opt.MapFrom(src => src.DepositId))
                .ForMember(dest => dest.CurrencyId, opt => opt.MapFrom(src => src.CurrencyId))
                .ForMember(dest => dest.Deposit, opt => opt.Ignore())
                .ForMember(dest => dest.Currency, opt => opt.Ignore());
            cfg.CreateMap<Replenishment, ReplenishmentDataModel>();
        });
        _mapper = new Mapper(config);
    }
    public List<DepositDataModel> GetList(string? clerkId = null)
    {
        try
        {
            var query = _dbContext.Deposits
                .Include(x => x.Clerk)
                .Include(x => x.DepositCurrencies)
                .AsQueryable();
            if (clerkId is not null)
            {
                query = query.Where(x => x.ClerkId == clerkId);
            }
            return [.. query.Select(x => _mapper.Map<DepositDataModel>(x))];
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }

    public async Task<List<DepositDataModel>> GetListAsync(DateTime startDate, DateTime endDate, CancellationToken ct)
    {
        try
        {
            var query = _dbContext.Deposits.Include(x => x.Clerk).AsQueryable();
            // Например: query = query.Where(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate);

            var deposits = await query.ToListAsync(ct);
            return deposits.Select(x => _mapper.Map<DepositDataModel>(x)).ToList();
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }

    public DepositDataModel? GetElementById(string id)
    {
        try
        {
            return _mapper.Map<DepositDataModel>(_dbContext.Deposits.FirstOrDefault(x => x.Id == id));
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }

    public DepositDataModel? GetElementByInterestRate(float interestRate)
    {
        try
        {
            return _mapper.Map<DepositDataModel>(_dbContext.Deposits.Include(x => x.Clerk).FirstOrDefault(x => x.InterestRate == interestRate));
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }

    public void AddElement(DepositDataModel depositDataModel)
    {
        try
        {
            _dbContext.Deposits.Add(_mapper.Map<Deposit>(depositDataModel));
            _dbContext.SaveChanges();
        }
        catch (InvalidOperationException ex) when (ex.TargetSite?.Name == "ThrowIdentityConflict")
        {
            _dbContext.ChangeTracker.Clear();
            throw new ElementExistsException($"Id {depositDataModel.Id}");
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { ConstraintName: "IX_Deposits_InterestRate" })
        {
            _dbContext.ChangeTracker.Clear();
            throw new ElementExistsException($"InterestRate {depositDataModel.InterestRate}");
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }

    public void UpdElement(DepositDataModel depositDataModel)
    {
        try
        {
            using var transaction = _dbContext.Database.BeginTransaction();
            try
            {
                // Загружаем существующий вклад со связями
                var existingDeposit = _dbContext.Deposits
                    .Include(d => d.DepositCurrencies)
                    .FirstOrDefault(d => d.Id == depositDataModel.Id);

                if (existingDeposit == null)
                {
                    throw new ElementNotFoundException(depositDataModel.Id);
                }

                // Обновляем основные поля вклада
                existingDeposit.InterestRate = depositDataModel.InterestRate;
                existingDeposit.Cost = depositDataModel.Cost;
                existingDeposit.Period = depositDataModel.Period;
                existingDeposit.ClerkId = depositDataModel.ClerkId;

                // Обновляем связи с валютами, если они переданы
                if (depositDataModel.Currencies != null)
                {
                    // Удаляем все существующие связи
                    if (existingDeposit.DepositCurrencies != null)
                    {
                        _dbContext.DepositCurrencies.RemoveRange(existingDeposit.DepositCurrencies);
                    }

                    // Сохраняем изменения для применения удаления
                    _dbContext.SaveChanges();

                    // Создаем новые связи
                    existingDeposit.DepositCurrencies = depositDataModel.Currencies.Select(c =>
                        new DepositCurrency
                        {
                            DepositId = existingDeposit.Id,
                            CurrencyId = c.CurrencyId
                        }).ToList();
                }

                // Сохраняем все изменения
                _dbContext.SaveChanges();
                transaction.Commit();

                // Выводим отладочную информацию
                System.Console.WriteLine($"Updated deposit {existingDeposit.Id} with {existingDeposit.DepositCurrencies?.Count ?? 0} currency relations");
                foreach (var relation in existingDeposit.DepositCurrencies ?? Enumerable.Empty<DepositCurrency>())
                {
                    System.Console.WriteLine($"Currency relation: DepositId={relation.DepositId}, CurrencyId={relation.CurrencyId}");
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                System.Console.WriteLine($"Error in transaction: {ex.Message}");
                if (ex is ElementNotFoundException)
                    throw;
                throw new StorageException(ex.Message);
            }
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { ConstraintName: "IX_Deposits_InterestRate" })
        {
            _dbContext.ChangeTracker.Clear();
            throw new ElementExistsException($"InterestRate {depositDataModel.InterestRate}");
        }
        catch (ElementNotFoundException)
        {
            _dbContext.ChangeTracker.Clear();
            throw;
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }

    private Deposit? GetDepositById(string id) => _dbContext.Deposits.FirstOrDefault(x => x.Id == id);
}
