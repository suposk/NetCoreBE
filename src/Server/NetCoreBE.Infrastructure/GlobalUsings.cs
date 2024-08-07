//global using Infrastructure;
global using MediatR;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
global using System.ComponentModel.DataAnnotations;
global using System.Linq.Expressions;
global using AutoMapper;
global using FluentValidation;
global using System.ComponentModel.DataAnnotations.Schema;
global using System.Net;
global using System.Text;
global using Microsoft.Extensions.Logging;

//Shared
global using SharedKernel.Base;
global using SharedCommon;
global using SharedCommon.Services;
global using SharedCommon.Helpers;

//Contracts
global using SharedContract.Common;
global using SharedContract.Types;
global using SharedContract.Dtos;

//CommonCleanArch
global using CommonCleanArch;
global using CommonCleanArch.Base;
global using CommonCleanArch.Application;
global using CommonCleanArch.Helpers;
global using CommonCleanArch.CustomExceptions;
global using CommonCleanArch.Domain.Events;
global using CommonCleanArch.Services;
global using CommonCleanArch.Infrastructure.ApiMiddleware;
global using CommonCleanArch.Infrastructure.Enums;
global using CommonCleanArch.Infrastructure.Persistence;
global using CommonCleanArch.Infrastructure.Search;

//Infrastructure
global using NetCoreBE.Infrastructure.Persistence;
global using NetCoreBE.Infrastructure.Persistence.Repositories;
global using NetCoreBE.Infrastructure.Search;

//Application
global using NetCoreBE.Application;
global using NetCoreBE.Application.Interfaces;
global using NetCoreBE.Application.Tickets;
global using NetCoreBE.Application.OldTickets;
global using NetCoreBE.Application.OutboxDomaintEvents;

//Domain
global using NetCoreBE.Domain.Entities;
global using NetCoreBE.Domain.Events;