﻿using AutoMapper;
using BankContracts.DataModels;
using BankContracts.Exceptions;
using BankContracts.StorageContracts;
using BankDatabase.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace BankDatabase.Implementations;

internal class ClientStorageContract : IClientStorageContract
{

    private readonly BankDbContext _dbContext;
    private readonly Mapper _mapper;

    public ClientStorageContract(BankDbContext dbContext)
    {
        _dbContext = dbContext;
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Clerk, ClerkDataModel>();
            cfg.CreateMap<Client, ClientDataModel>()
                .ForMember(dest => dest.DepositClients, opt => opt.MapFrom(src => src.DepositClients))
                .ForMember(dest => dest.CreditProgramClients, opt => opt.MapFrom(src => src.CreditProgramClients));
            cfg.CreateMap<ClientDataModel, Client>()
                .ForMember(dest => dest.DepositClients, opt => opt.MapFrom(src => src.DepositClients))
                .ForMember(dest => dest.CreditProgramClients, opt => opt.MapFrom(src => src.CreditProgramClients));
            cfg.CreateMap<DepositClient, DepositClientDataModel>()
                .ForMember(dest => dest.DepositId, opt => opt.MapFrom(src => src.DepositId))
                .ForMember(dest => dest.ClientId, opt => opt.MapFrom(src => src.ClientId));
            cfg.CreateMap<DepositClientDataModel, DepositClient>()
                .ForMember(dest => dest.DepositId, opt => opt.MapFrom(src => src.DepositId))
                .ForMember(dest => dest.ClientId, opt => opt.MapFrom(src => src.ClientId));
            cfg.CreateMap<Deposit, DepositClientDataModel>()
                .ForMember(dest => dest.DepositId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ClientId, opt => opt.Ignore());
            cfg.CreateMap<DepositClientDataModel, Deposit>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.DepositId));
            cfg.CreateMap<ClientCreditProgram, ClientCreditProgramDataModel>()
                .ForMember(dest => dest.ClientId, opt => opt.MapFrom(src => src.ClientId))
                .ForMember(dest => dest.CreditProgramId, opt => opt.MapFrom(src => src.CreditProgramId));
            cfg.CreateMap<ClientCreditProgramDataModel, ClientCreditProgram>()
                .ForMember(dest => dest.ClientId, opt => opt.MapFrom(src => src.ClientId))
                .ForMember(dest => dest.CreditProgramId, opt => opt.MapFrom(src => src.CreditProgramId));
            cfg.CreateMap<CreditProgram, ClientCreditProgramDataModel>()
                .ForMember(dest => dest.CreditProgramId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ClientId, opt => opt.Ignore());
            cfg.CreateMap<ClientCreditProgramDataModel, CreditProgram>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CreditProgramId));
            cfg.CreateMap<Replenishment, ReplenishmentDataModel>();
        });
        _mapper = new Mapper(config);
    }

    public List<ClientDataModel> GetList(string? clerkId = null)
    {
        try
        {
            var query = _dbContext.Clients
                .Include(x => x.Clerk)
                .Include(x => x.CreditProgramClients)
                .Include(x => x.DepositClients)
                .AsQueryable();
            if (clerkId is not null)
            {
                query = query.Where(x => x.ClerkId == clerkId);
            }
            return [.. query.Select(x => _mapper.Map<ClientDataModel>(x))];
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }

    public ClientDataModel? GetElementById(string id)
    {
        try
        {
            return _mapper.Map<ClientDataModel>(_dbContext.Clients.FirstOrDefault(x => x.Id == id));
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }

    public ClientDataModel? GetElementByName(string name)
    {
        try
        {
            return _mapper.Map<ClientDataModel>(_dbContext.Clients.Include(x => x.Clerk).FirstOrDefault(x => x.Name == name));
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }

    public ClientDataModel? GetElementBySurname(string surname)
    {
        try
        {
           return _mapper.Map<ClientDataModel>(_dbContext.Clients.Include(x => x.Clerk).FirstOrDefault(x => x.Surname == surname));
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }

    public void AddElement(ClientDataModel clientDataModel)
    {
        try
        {
            _dbContext.Clients.Add(_mapper.Map<Client>(clientDataModel));
            _dbContext.SaveChanges();
        }
        catch (InvalidOperationException ex) when (ex.TargetSite?.Name == "ThrowIdentityConflict")
        {
            _dbContext.ChangeTracker.Clear();
            throw new ElementExistsException($"Id {clientDataModel.Id}");
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { ConstraintName: "IX_Clients_Name" })
        {
            _dbContext.ChangeTracker.Clear();
            throw new ElementExistsException($"Name {clientDataModel.Name}");
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { ConstraintName: "IX_Clients_Surname" })
        {
            _dbContext.ChangeTracker.Clear();
            throw new ElementExistsException($"Surname {clientDataModel.Surname}");
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }

    public async Task<List<ClientDataModel>> GetListAsync(DateTime startDate, DateTime endDate, CancellationToken ct)
    {
        try
        {
            var clients = await _dbContext.Clients
                .Include(x => x.Clerk)
                .ToListAsync(ct);

            return clients.Select(x => _mapper.Map<ClientDataModel>(x)).ToList();
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }

    public void UpdElement(ClientDataModel clientDataModel)
    {
        try
        {
            var transaction = _dbContext.Database.BeginTransaction();
            try
            {
                var element = GetClientById(clientDataModel.Id) ?? throw new ElementNotFoundException(clientDataModel.Id);
                //проверь пожалуйста(не уверен)
                if (clientDataModel.DepositClients != null && clientDataModel.CreditProgramClients != null)
                {
                    if (element.DepositClients != null || element.DepositClients?.Count >= 0)
                    {
                        _dbContext.DepositClients.RemoveRange(element.DepositClients);
                    }

                    if (element.CreditProgramClients != null || element.CreditProgramClients?.Count >= 0)
                    {
                        _dbContext.CreditProgramClients.RemoveRange(element.CreditProgramClients);
                    }

                    element.DepositClients = _mapper.Map<List<DepositClient>>(clientDataModel.DepositClients);
                    element.CreditProgramClients = _mapper.Map<List<ClientCreditProgram>>(clientDataModel.CreditProgramClients);
                }
                _mapper.Map(element, clientDataModel);
                _dbContext.SaveChanges();
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (ElementNotFoundException)
        {
            _dbContext.ChangeTracker.Clear();
            throw;
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { ConstraintName: "IX_Clients_Name" })
        {
            _dbContext.ChangeTracker.Clear();
            throw new ElementExistsException($"Name {clientDataModel.Name}");
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { ConstraintName: "IX_Clients_Surname" })
        {
            _dbContext.ChangeTracker.Clear();
            throw new ElementExistsException($"Surname {clientDataModel.Surname}");
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }
    private Client? GetClientById(string id) => _dbContext.Clients.Include(client => client.DepositClients).Include(client => client.CreditProgramClients).FirstOrDefault(x => x.Id == id);
}
