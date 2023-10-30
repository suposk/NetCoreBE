//global using AA.BB.CC;
global using MediatR;
global using Microsoft.EntityFrameworkCore;
global using System.ComponentModel.DataAnnotations;
global using System.Linq.Expressions;
global using AutoMapper;
global using FluentValidation;

global using SharedKernel.Base;

//Contracts
global using Contracts.Common;
global using Contracts.Types;
global using Contracts.Dtos;

//CommonBE
global using CommonBE;
global using CommonBE.Base;
global using CommonBE.Helpers;
global using CommonBE.CustomExceptions;
global using CommonBE.Domain.Events;
global using CommonBE.Services;
global using CommonBE.Infrastructure.ApiMiddleware;
global using CommonBE.Infrastructure.Persistence;
global using CommonBE.Infrastructure.Search;

//Application
global using NetCoreBE.Api.Application;
global using NetCoreBE.Api.Application.Interfaces;
global using NetCoreBE.Api.Application.Features.Requests;
global using NetCoreBE.Api.Application.Features.Tickets;

//Domain
global using NetCoreBE.Api.Domain.Entities;
global using NetCoreBE.Api.Domain.Events;

//Infrastructure
global using NetCoreBE.Api.Infrastructure.Persistence;
global using NetCoreBE.Api.Infrastructure.Persistence.Repositories;
global using NetCoreBE.Api.Infrastructure.Search;
