using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Bizwire.Api.Controllers;
using Bizwire.Api.Mappers;
using Bizwire.Api.ViewModels;
using Bizwire.Service.Dto;
using Bizwire.Service.Interfaces;
using Bizwire.Tests.Utils;
using Dgm.Core.Constants;
using Dgm.Core.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using DgmClaims = Bizwire.Common.Constants.DgmClaims;
using AspNet.Security.OpenIdConnect.Primitives;
using Bizwire.Common.Constants;

namespace Bizwire.Tests.ApiTests
{
    public class PressReleaseControllerTest : IClassFixture<BizwireFixtures>
    {
        private readonly Mock<IPressReleaseService> _mockPresReleaseService;
        private readonly IMapper _mapper;
        private readonly PressReleaseController _pressReleaseController;

        private readonly Guid _mockOwnOrganizationId = Guid.NewGuid();
        private readonly string _mockCurrentRole = Role.User;
        private readonly string _mockCurrentAccountId = Guid.NewGuid().ToString();


        public PressReleaseControllerTest(BizwireFixtures fixtures)
        {
            _mockPresReleaseService = fixtures.MockPressReleaseService;
            _mapper = fixtures.Mapper;
            _pressReleaseController = new PressReleaseController(_mockPresReleaseService.Object, _mapper)

            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                        {
                            new Claim(DgmClaims.OwnOrganizationId, _mockOwnOrganizationId.ToString()),
                            new Claim(OpenIdConnectConstants.Claims.Role, _mockCurrentRole),
                            new Claim(OpenIdConnectConstants.Claims.Subject, _mockCurrentAccountId),
                        }))
                    }
                }
            };
        }

        [Fact]
        public async Task Query_ShouldPass()
        {
            var mockResult = MockObjectCreator.CreateMockQueryResult(MockObjectCreator.CreatePressReleaseDtoObject);
            _mockPresReleaseService.Setup(t => t.Query(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<SortDirection>(),
                It.IsAny<string>()
            )).ReturnsAsync(mockResult);
            var result = (OkObjectResult)await _pressReleaseController.Query(0, 10);
            result.Should().NotBeNull();
            _mockPresReleaseService.Verify(t => t.Query(
                    It.Is<int>(x => x == 0),
                    It.Is<int>(x => x == 10),
                    It.Is<string>(x => x == "createdDate"),
                    It.Is<SortDirection>(x => x == SortDirection.Descending),
                    It.Is<string>(x => x == null)
                ), Times.Once);
            var viewResult = (QueryResultViewModel<PressReleaseViewModel>)result.Value;
            viewResult.Should().NotBeNull();
            viewResult.Count.Should().Be(mockResult.Count);
            Assert.Equal(viewResult, _mapper.Map<QueryResultViewModel<PressReleaseViewModel>>(mockResult), new QueryResultViewModelCompare<PressReleaseViewModel>());
        }

        [Fact]
        public async Task Query_OrganizationPressReleases_ShouldPass()
        {
            var mockResult = MockObjectCreator.CreateMockQueryResult(MockObjectCreator.CreatePressReleaseDtoObject);
            _mockPresReleaseService.Setup(t => t.QueryOrganizationPressReleases(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<SortDirection>(),
                It.IsAny<string>(),
                It.IsAny<Guid>()
            )).ReturnsAsync(mockResult);

            var result = (OkObjectResult)await _pressReleaseController.QueryOrganizationPressReleases(0, 10);
            result.Should().NotBeNull();
            _mockPresReleaseService.Verify(t => t.QueryOrganizationPressReleases(
                It.Is<int>(x => x == 0),
                It.Is<int>(x => x == 10),
                It.Is<string>(x => x == "createdDate"),
                It.Is<SortDirection>(x => x == SortDirection.Descending),
                It.Is<string>(x => x == null),
                It.Is<Guid>(x => x == _mockOwnOrganizationId)
            ), Times.Once);
            var viewResult = (QueryResultViewModel<PressReleaseViewModel>)result.Value;
            viewResult.Should().NotBeNull();
            viewResult.Count.Should().Be(mockResult.Count);
        }

        [Fact]
        public async Task Query_PressReleaseRequest_ShoulPass()
        {
            var mockResult = MockObjectCreator.CreateMockQueryResult(MockObjectCreator.CreatePressReleaseRequestDtoObject);
            _mockPresReleaseService.Setup(t => t.QueryPressReleaseRequest(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<SortDirection>(),
                It.IsAny<string>(),
                It.IsAny<string>()
            )).ReturnsAsync(mockResult);

            var result = (OkObjectResult)await _pressReleaseController.QueryPressReleaseRequest(0, 10);
            result.Should().NotBeNull();
            _mockPresReleaseService.Verify(t => t.QueryPressReleaseRequest(
                It.Is<int>(x => x == 0),
                It.Is<int>(x => x == 10),
                It.Is<string>(x => x == "createdDate"),
                It.Is<SortDirection>(x => x == SortDirection.Descending),
                It.Is<string>(x => x == null),
                It.Is<string>(x => x == _mockCurrentAccountId)
            ), Times.Once);
            var viewResult = (QueryResultViewModel<PressReleaseRequestViewModel>)result.Value;
            viewResult.Should().NotBeNull();
            viewResult.Count.Should().Be(mockResult.Count);
            var mockList = mockResult.Items.ToList();
            var viewList = viewResult.Items.ToList();
            viewList.Count.Should().Be(mockList.Count);
        }

        [Fact]
        public async Task Create_ShouldPass()
        {
            var mockInput = MockObjectCreator.CreatePressReleaseViewModelObject(_mockOwnOrganizationId);
            var mockResult = _mapper.Map<PressReleaseDto>(mockInput);
            _mockPresReleaseService.Setup(t => t.Create(It.IsAny<PressReleaseDto>())).ReturnsAsync(mockResult);
            var result = (OkObjectResult)await _pressReleaseController.Create(mockInput);
            result.Should().NotBeNull();
            _mockPresReleaseService.Verify(t => t.Create(It.Is<PressReleaseDto>(x => x.Id == mockInput.Id)), Times.Once);
            var viewResult = (PressReleaseViewModel)result.Value;
            viewResult.Should().NotBeNull();
            viewResult.Id.Should().Be(mockResult.Id);
            viewResult.Title.Should().Be(mockResult.Title);
            viewResult.Content.Should().Be(mockResult.Content);
            viewResult.Category.Should().NotBeNull();
            viewResult.Category.Id.Should().Be(mockResult.Category.Id);
            viewResult.Category.Name.Should().Be(mockResult.Category.Name);
            viewResult.Organization.Should().NotBeNull();
            viewResult.Organization.Id.Should().Be(mockResult.Organization.Id);
            viewResult.Organization.Name.Should().Be(mockResult.Organization.Name);
        }

        [Fact]
        public async Task Update_ShouldPass()
        {
            var mockInput = MockObjectCreator.CreatePressReleaseViewModelObject(_mockOwnOrganizationId);
            var mockResult = _mapper.Map<PressReleaseDto>(mockInput);
            _mockPresReleaseService.Setup(t => t.Update(It.IsAny<PressReleaseDto>())).ReturnsAsync(mockResult);
            _mockPresReleaseService.Setup(t => t.CheckUpdatePermission(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Guid>()
            )).ReturnsAsync(true);
            var result = (OkObjectResult)await _pressReleaseController.Update(mockInput);
            result.Should().NotBeNull();
            _mockPresReleaseService.Verify(t => t.CheckUpdatePermission(
                It.Is<Guid>(x => x == mockInput.Id),
                It.Is<string>(x => x == _mockCurrentRole),
                It.Is<string>(x => x == _mockCurrentAccountId),
                It.Is<Guid>(x => x == _mockOwnOrganizationId)
            ), Times.Once);
            _mockPresReleaseService.Verify(t => t.Update(
                It.Is<PressReleaseDto>(x => x.Id == mockInput.Id)
            ), Times.Once);
            var viewResult = (PressReleaseViewModel)result.Value;
            viewResult.Should().NotBeNull();
            viewResult.Id.Should().Be(mockResult.Id);
            viewResult.Title.Should().Be(mockResult.Title);
            viewResult.Content.Should().Be(mockResult.Content);
            viewResult.Category.Should().NotBeNull();
            viewResult.Category.Id.Should().Be(mockResult.Category.Id);
            viewResult.Category.Name.Should().Be(mockResult.Category.Name);
            viewResult.Organization.Should().NotBeNull();
            viewResult.Organization.Id.Should().Be(mockResult.Organization.Id);
            viewResult.Organization.Name.Should().Be(mockResult.Organization.Name);
        }

        [Fact]
        public async Task Delete_ShouldPass()
        {
            var pressReleaseId = Guid.NewGuid();
            _mockPresReleaseService.Setup(t => t.CheckUpdatePermission(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Guid>()
            )).ReturnsAsync(true);
            await _pressReleaseController.Delete(pressReleaseId);
            _mockPresReleaseService.Verify(t => t.CheckUpdatePermission(
                It.Is<Guid>(x => x == pressReleaseId),
                It.Is<string>(x => x == _mockCurrentRole),
                It.Is<string>(x => x == _mockCurrentAccountId),
                It.Is<Guid>(x => x == _mockOwnOrganizationId)
            ), Times.Once);
            _mockPresReleaseService.Verify(t => t.Delete(It.Is<Guid>(x => x == pressReleaseId)));
        }

        [Fact]
        public async Task Get_ShouldPass()
        {
            var mockResult = MockObjectCreator.CreatePressReleaseDtoObject();
            _mockPresReleaseService.Setup(t => t.GetById(It.IsAny<Guid>())).ReturnsAsync(mockResult);
            var result = (OkObjectResult)await _pressReleaseController.Get(mockResult.Id);
            _mockPresReleaseService.Verify(t => t.GetById(It.Is<Guid>(x => x == mockResult.Id)), Times.Once);
            result.Should().NotBeNull();
            var viewResult = (PressReleaseViewModel)result.Value;
            viewResult.Should().NotBeNull();
            viewResult.Id.Should().Be(mockResult.Id);
            viewResult.Title.Should().Be(mockResult.Title);
            viewResult.Content.Should().Be(mockResult.Content);
            viewResult.Category.Should().NotBeNull();
            viewResult.Category.Id.Should().Be(mockResult.Category.Id);
            viewResult.Category.Name.Should().Be(mockResult.Category.Name);
            viewResult.Organization.Should().NotBeNull();
            viewResult.Organization.Id.Should().Be(mockResult.Organization.Id);
            viewResult.Organization.Name.Should().Be(mockResult.Organization.Name);
        }

        [Fact]
        public async Task GetPressReleaseRequest_ShouldPass()
        {
            var mockResult = MockObjectCreator.CreatePressReleaseRequestDtoObject();
            _mockPresReleaseService.Setup(t => t.GetPressReleaseRequest(It.IsAny<Guid>())).ReturnsAsync(mockResult);
            var result = (OkObjectResult)await _pressReleaseController.GetPressReleaseRequest(mockResult.Id);
            _mockPresReleaseService.Verify(t => t.GetPressReleaseRequest(It.Is<Guid>(x => x == mockResult.Id)), Times.Once);
            result.Should().NotBeNull();
            var viewResult = (PressReleaseRequestViewModel)result.Value;
            viewResult.Should().NotBeNull();
            viewResult.Id.Should().Be(mockResult.Id);
            viewResult.Status.Should().Be(mockResult.Status);
            viewResult.PublishedUrl.Should().Be(mockResult.PublishedUrl);
            viewResult.RejectReason.Should().Be(mockResult.RejectReason);
            viewResult.PressRelease.Should().NotBeNull();
            viewResult.PressRelease.Id.Should().Be(mockResult.PressRelease.Id);
            viewResult.PressRelease.Title.Should().Be(mockResult.PressRelease.Title);
            viewResult.PressRelease.Content.Should().Be(mockResult.PressRelease.Content);
            viewResult.PressRelease.Category.Should().NotBeNull();
            viewResult.PressRelease.Category.Id.Should().Be(mockResult.PressRelease.Category.Id);
            viewResult.PressRelease.Category.Name.Should().Be(mockResult.PressRelease.Category.Name);
            viewResult.PressRelease.Organization.Should().NotBeNull();
            viewResult.PressRelease.Organization.Id.Should().Be(mockResult.PressRelease.Organization.Id);
            viewResult.PressRelease.Organization.Name.Should().Be(mockResult.PressRelease.Organization.Name);
        }

        [Fact]
        public async Task SuggestAll_ShouldPass()
        {
            var pressReleaseId = Guid.NewGuid();
            var mockResult = MockObjectCreator.CreateMockQueryResult(MockObjectCreator.CreateNewspaperDtoObject);
            _mockPresReleaseService.Setup(t => t.SuggestAllNewspapers(
                It.IsAny<Guid>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>()
            )).ReturnsAsync(mockResult);
            var result = (OkObjectResult)await _pressReleaseController.SuggestAll(pressReleaseId, 0, 10, "");
            _mockPresReleaseService.Verify(t => t.SuggestAllNewspapers(
                It.Is<Guid>(x => x == pressReleaseId),
                It.Is<int>(x => x == 0),
                It.Is<int>(x => x == 10),
                It.Is<string>(x => x == string.Empty)
            ), Times.Once);
            result.Should().NotBeNull();
            var viewResult = (QueryResultViewModel<NewspaperViewModel>)result.Value;
            viewResult.Should().NotBeNull();
            viewResult.Count.Should().Be(mockResult.Count);
        }

        [Fact]
        public async Task SuggestMatchCategory_ShouldPass()
        {
            var pressReleaseId = Guid.NewGuid();
            var mockResult = MockObjectCreator.CreateMockQueryResult(MockObjectCreator.CreateNewspaperDtoObject);
            _mockPresReleaseService.Setup(t => t.SuggestNewspapersMatchCategory(
                It.IsAny<Guid>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>()
            )).ReturnsAsync(mockResult);
            var result = (OkObjectResult)await _pressReleaseController.SuggestMatchCategory(pressReleaseId, 0, 10, "");
            _mockPresReleaseService.Verify(t => t.SuggestNewspapersMatchCategory(
                It.Is<Guid>(x => x == pressReleaseId),
                It.Is<int>(x => x == 0),
                It.Is<int>(x => x == 10),
                It.Is<string>(x => x == string.Empty)
            ), Times.Once);
            result.Should().NotBeNull();
            var viewResult = (QueryResultViewModel<NewspaperViewModel>)result.Value;
            viewResult.Should().NotBeNull();
            viewResult.Count.Should().Be(mockResult.Count);
            viewResult.Items.Count().Should().Be(mockResult.Items.Count());
        }

        [Fact]
        public async Task SendRequestToNewspapers_ShouldPass()
        {
            var mockInput = MockObjectCreator.CreatePressReleaseRequestViewModelObject();
            _mockPresReleaseService.Setup(t => t.CheckUpdatePermission(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Guid>()
            )).ReturnsAsync(true);
            await _pressReleaseController.SendRequestToNewspapers(mockInput);
            _mockPresReleaseService.Verify(t => t.CheckUpdatePermission(
                It.Is<Guid>(x => x == mockInput.PressReleaseId),
                It.Is<string>(x => x == _mockCurrentRole),
                It.Is<string>(x => x == _mockCurrentAccountId),
                It.Is<Guid>(x => x == _mockOwnOrganizationId)
            ), Times.Once);
            _mockPresReleaseService.Verify(t => t.SendRequestToNewspapers(
                It.Is<Guid>(x => x == mockInput.PressReleaseId),
                It.Is<IEnumerable<NewspaperDto>>(x => x.All(x1 => mockInput.Newspapers.Any(x2 => x1.Id == x2.Id)))
            ), Times.Once);
        }

        [Fact]
        public async Task QueryNewspapers_ShouldPass()
        {
            var pressReleaseId = Guid.NewGuid();
            var mockResult = MockObjectCreator.CreateMockQueryResult(MockObjectCreator.CreatePressReleaseRequestDtoObject);
            _mockPresReleaseService.Setup(t => t.QueryNewspapers(
                It.IsAny<Guid>(),
                It.IsAny<int>(),
                It.IsAny<int>()
            )).ReturnsAsync(mockResult);
            var result = (OkObjectResult)await _pressReleaseController.QueryNewspapers(pressReleaseId, 0, 10);
            _mockPresReleaseService.Verify(t => t.QueryNewspapers(
                It.Is<Guid>(x => x == pressReleaseId),
                It.Is<int>(x => x == 0),
                It.Is<int>(x => x == 10)
            ), Times.Once);
            result.Should().NotBeNull();
            var dtoResult = (QueryResultDto<PressReleaseRequestDto>)result.Value;
            Assert.Equal(dtoResult, mockResult);
        }

        [Fact]
        public async Task RemoveRequestForNewspaper_ShouldPass()
        {
            var pressRequestId = Guid.NewGuid();
            await _pressReleaseController.RemoveRequestForNewspaper(pressRequestId);
            _mockPresReleaseService.Verify(t => t.RemoveRequestForNewspaper(It.Is<Guid>(x => x == pressRequestId)));
        }

        [Fact]
        public async Task RejectRequest_ShouldPass()
        {
            var mockInput = MockObjectCreator.CreatePressReleaseRequestUpdateViewModelObject();
            await _pressReleaseController.RejectRequest(mockInput);
            _mockPresReleaseService.Verify(t => t.RejectRequest(It.Is<Guid>(x => x == mockInput.Id), It.Is<string>(x => x == mockInput.RejectReason)), Times.Once);
        }

        [Fact]
        public async Task PublishRequest_ShouldPass()
        {
            var mockInput = MockObjectCreator.CreatePressReleaseRequestUpdateViewModelObject();
            await _pressReleaseController.PublishRequest(mockInput);
            _mockPresReleaseService.Verify(t => t.PublishRequest(It.Is<Guid>(x => x == mockInput.Id), It.Is<string>(x => x == mockInput.PublishedUrl)), Times.Once);
        }
    }
}