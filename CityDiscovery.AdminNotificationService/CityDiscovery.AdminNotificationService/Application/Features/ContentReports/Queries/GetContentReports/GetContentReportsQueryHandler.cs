using CityDiscovery.AdminNotificationService.Application.Common.Models;
using CityDiscovery.AdminNotificationService.Application.Features.ContentReports.DTOs;
using CityDiscovery.AdminNotificationService.Application.Interfaces.Repositories;
using MediatR;

namespace CityDiscovery.AdminNotificationService.Application.Features.ContentReports.Queries.GetContentReports
{
    public class GetContentReportsQueryHandler
        : IRequestHandler<GetContentReportsQuery, PagedResult<ContentReportDto>>
    {
        private readonly IContentReportRepository _repository;

        public GetContentReportsQueryHandler(IContentReportRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResult<ContentReportDto>> Handle(
            GetContentReportsQuery request,
            CancellationToken cancellationToken)
        {
            var (items, totalCount) = await _repository.GetPagedAsync(
                request.Page,
                request.PageSize,
                request.Status,
                cancellationToken);

            // Entity listesini DTO listesine çevir
            var dtos = items.Select(ContentReportDto.FromEntity).ToList();

            return new PagedResult<ContentReportDto>
            {
                Items = dtos,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }
    }
}