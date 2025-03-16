//global using Infrastructure;
global using MediatR;
global using MassTransit;
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
global using CommonCleanArch.Domain.Base;
global using CommonCleanArch.Application;
global using CommonCleanArch.Domain.Events;
global using CommonCleanArch.Application.Services;
global using CommonCleanArch.Application.Search;
global using CommonCleanArch.Infrastructure.ApiMiddleware;
global using CommonCleanArch.Infrastructure.Enums;
global using CommonCleanArch.Infrastructure.Persistence;

//Infrastructure
global using NetCoreBE.Infrastructure.Persistence;
global using NetCoreBE.Infrastructure.Persistence.Repositories;
global using NetCoreBE.Infrastructure.Search;

//Application
global using NetCoreBE.Application;
global using NetCoreBE.Application.Interfaces;
global using NetCoreBE.Application.Tickets;
global using NetCoreBE.Application.CrudExamples;
global using NetCoreBE.Application.OutboxDomaintEvents;

//Domain
global using NetCoreBE.Domain.Entities;
global using NetCoreBE.Domain.Events;