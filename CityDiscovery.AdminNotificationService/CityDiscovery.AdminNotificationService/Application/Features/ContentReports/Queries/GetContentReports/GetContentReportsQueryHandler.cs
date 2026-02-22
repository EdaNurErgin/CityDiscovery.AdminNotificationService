using CityDiscovery.AdminNotificationService.Application.Common.Models;
using CityDiscovery.AdminNotificationService.Application.Features.ContentReports.DTOs;
using CityDiscovery.AdminNotificationService.Application.Interfaces.Repositories;
using CityDiscovery.AdminNotificationService.Application.Interfaces.External;
using MediatR;

namespace CityDiscovery.AdminNotificationService.Application.Features.ContentReports.Queries.GetContentReports
{
    public class GetContentReportsQueryHandler
        : IRequestHandler<GetContentReportsQuery, PagedResult<ContentReportDto>>
    {
        private readonly IContentReportRepository _repository;
        private readonly IIdentityServiceClient _identityService;

        public GetContentReportsQueryHandler(
            IContentReportRepository repository,
            IIdentityServiceClient identityService)
        {
            _repository = repository;
            _identityService = identityService;
        }

        public async Task<PagedResult<ContentReportDto>> Handle(
            GetContentReportsQuery request,
            CancellationToken cancellationToken)
        {
            var (items, totalCount) = await _repository.GetPagedAsync(
                request.Page, request.PageSize, request.Status, cancellationToken);

            // Toplu kullanıcı çek
            var userIds = items.Select(r => r.ReportingUserId).Distinct().ToList();
            var users = await _identityService.GetUsersAsync(userIds, cancellationToken);
            var userMap = users.ToDictionary(u => u.Id, u => u.UserName);

            var dtos = items.Select(r =>
            {
                var dto = ContentReportDto.FromEntity(r);
                dto.ReportingUserName = userMap.TryGetValue(r.ReportingUserId, out var name) ? name : null;
                return dto;
            }).ToList();

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