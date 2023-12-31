﻿//global using AA.BB.CC;
global using MediatR;
global using Microsoft.EntityFrameworkCore;
global using System.ComponentModel.DataAnnotations;
global using System.Linq.Expressions;
global using AutoMapper;
global using FluentValidation;
global using System.ComponentModel.DataAnnotations.Schema;

//Shared
global using SharedKernel.Base;
global using SharedCommon;
global using SharedCommon.Services;
global using SharedCommon.Helpers;

//Contracts
global using Contracts.Common;
global using Contracts.Types;
global using Contracts.Dtos;

//CommonCleanArch
global using CommonCleanArch;
global using CommonCleanArch.Base;
global using CommonCleanArch.Helpers;
global using CommonCleanArch.CustomExceptions;
global using CommonCleanArch.Domain.Events;
global using CommonCleanArch.Services;
global using CommonCleanArch.Infrastructure.ApiMiddleware;
global using CommonCleanArch.Infrastructure.Enums;
global using CommonCleanArch.Infrastructure.Persistence;
global using CommonCleanArch.Infrastructure.Search;

//Application
global using NetCoreBE.Api.Application;
global using NetCoreBE.Api.Application.Interfaces;
global using NetCoreBE.Api.Application.Features.Requests;
global using NetCoreBE.Api.Application.Features.Tickets;
global using NetCoreBE.Api.Application.Features.Examples;

//Domain
global using NetCoreBE.Api.Domain.Entities;
global using NetCoreBE.Api.Domain.Events;

//Infrastructure
global using NetCoreBE.Api.Infrastructure.Persistence;
global using NetCoreBE.Api.Infrastructure.Persistence.Repositories;
global using NetCoreBE.Api.Infrastructure.Search;
