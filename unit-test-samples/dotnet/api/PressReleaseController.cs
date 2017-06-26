using System;
using AutoMapper;
using Bizwire.Api.ViewModels;
using Bizwire.Service.Dto;
using Bizwire.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bizwire.Api.Mappers;
using Dgm.Core.Enums;
using Bizwire.Common.Constants;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace Bizwire.Api.Controllers
{
    public class PressReleaseController : DgmController
    {
        private readonly IPressReleaseService _pressReleaseService;
        private readonly IMapper _mapper;

        public PressReleaseController(
            IPressReleaseService service,
            IMapper mapper)
        {
            _pressReleaseService = service;
            _mapper = mapper;
        }

        [HttpGet("query/{skip}/{take}/{sortByField?}/{sortDirection?}/{query?}")]
        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> Query(int skip, int take, string sortByField = "createdDate",
            SortDirection sortDirection = SortDirection.Descending, string query = null)
        {
            var results = await _pressReleaseService.Query(skip, take, sortByField, sortDirection, query);
            return Ok(_mapper.Map<QueryResultViewModel<PressReleaseViewModel>>(results));
        }

        [HttpGet("queryOrganizationPressReleases/{skip}/{take}/{sortByField?}/{sortDirection?}/{query?}")]
        [Authorize(Roles = Role.User)]
        public async Task<IActionResult> QueryOrganizationPressReleases(int skip, int take,
            string sortByField = "createdDate",
            SortDirection sortDirection = SortDirection.Descending, string query = null)
        {
            var results = await _pressReleaseService.QueryOrganizationPressReleases(skip, take, sortByField,
                sortDirection, query, OwnOrganizationId);
            return Ok(_mapper.Map<QueryResultViewModel<PressReleaseViewModel>>(results));
        }

        [HttpGet("queryPressReleaseRequest/{skip}/{take}/{sortByField?}/{sortDirection?}/{query?}")]
        [Authorize(Roles = Role.Journalist)]
        public async Task<IActionResult> QueryPressReleaseRequest(int skip, int take,
            string sortByField = "createdDate",
            SortDirection sortDirection = SortDirection.Descending, string query = null)
        {
            var results = await _pressReleaseService.QueryPressReleaseRequest(skip, take, sortByField,
                sortDirection, query, CurrentAccountId);
            return Ok(_mapper.Map<QueryResultViewModel<PressReleaseRequestViewModel>>(results));
        }

        [HttpGet("getOrganizationPressRelease")]
        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> GetOrganizationPressRelease()
        {
            var result = await _pressReleaseService.GetOrganizationPressRelease();
            return Ok(_mapper.Map<IEnumerable<OrganizationViewModel>>(result));
        }

        [HttpPost]
        [Authorize(Roles = Role.Admin + "," + Role.User)]
        public async Task<IActionResult> Create([FromBody] PressReleaseViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var dto = _mapper.Map<PressReleaseDto>(viewModel);
            if (CurrentRole == Role.User)
            {
                if (OwnOrganizationId == Guid.Empty)
                    throw new Exception("User must be in an organization");
                if (viewModel.Organization != null && viewModel.Organization.Id != OwnOrganizationId)
                    throw new Exception("Cannot create a press release with another organization");
                dto.OrganizationId = OwnOrganizationId;
            }
            dto.CreatorId = CurrentAccountId;
            var createdDto = await _pressReleaseService.Create(dto);
            return Ok(_mapper.Map<PressReleaseViewModel>(createdDto));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get(Guid id)
        {
            var pressReleaseDto = await _pressReleaseService.GetById(id);
            return Ok(_mapper.Map<PressReleaseViewModel>(pressReleaseDto));
        }

        [HttpGet("getPressReleaseRequest/{id}")]
        [Authorize(Roles = Role.Journalist)]
        public async Task<IActionResult> GetPressReleaseRequest(Guid id)
        {
            var pressReleaseRequestDto = await _pressReleaseService.GetPressReleaseRequest(id);
            return Ok(_mapper.Map<PressReleaseRequestViewModel>(pressReleaseRequestDto));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (!await _pressReleaseService.CheckUpdatePermission(id, CurrentRole, CurrentAccountId, OwnOrganizationId))
                throw new Exception("Only creator and owner can delete a press release");
            await _pressReleaseService.Delete(id);
            return Ok();
        }

        [HttpPut]
        [Authorize(Roles = Role.User + "," + Role.Admin)]
        public async Task<IActionResult> Update([FromBody] PressReleaseViewModel viewModel)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (CurrentRole == Role.User)
            {
                if (OwnOrganizationId == Guid.Empty)
                    throw new Exception("User must be in an organization");
                if (viewModel?.Organization?.Id != OwnOrganizationId)
                    throw new Exception("Cannot create a press release with another organization");
            }
            if (!await _pressReleaseService.CheckUpdatePermission(viewModel.Id, CurrentRole, CurrentAccountId, OwnOrganizationId))
                throw new Exception("Only creator and owner can update a press release");
            var dto = _mapper.Map<PressReleaseDto>(viewModel);
            var updateDto = await _pressReleaseService.Update(dto);
            return Ok(_mapper.Map<PressReleaseViewModel>(updateDto));
        }

        [HttpGet("suggestAllNewspapers/{pressReleaseId}/{skip}/{take}/{query?}")]
        [Authorize]
        public async Task<IActionResult> SuggestAll(Guid pressReleaseId, int skip, int take, string query)
        {
            var results = await _pressReleaseService.SuggestAllNewspapers(pressReleaseId, skip, take, query);
            return Ok(_mapper.Map<QueryResultViewModel<NewspaperViewModel>>(results));
        }

        [HttpGet("suggestNewspapersMatchCategory/{pressReleaseId}/{skip}/{take}/{query?}")]
        [Authorize]
        public async Task<IActionResult> SuggestMatchCategory(Guid pressReleaseId, int skip, int take, string query)
        {
            var results = await _pressReleaseService.SuggestNewspapersMatchCategory(pressReleaseId, skip, take, query);
            return Ok(_mapper.Map<QueryResultViewModel<NewspaperViewModel>>(results));
        }

        [HttpPost("sendRequest")]
        [Authorize]
        public async Task<IActionResult> SendRequestToNewspapers([FromBody] PressReleaseRequestViewModel viewModel)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!await _pressReleaseService.CheckUpdatePermission(viewModel.PressReleaseId, CurrentRole, CurrentAccountId, OwnOrganizationId))
                throw new Exception("Only creator and owner can send a press release request to newspapers");
            await _pressReleaseService.SendRequestToNewspapers(viewModel.PressReleaseId, _mapper.Map<IEnumerable<NewspaperDto>>(viewModel.Newspapers));
            return Ok();
        }

        [HttpGet("queryNewspapers/{pressReleaseId}/{skip}/{take}")]
        [Authorize]
        public async Task<IActionResult> QueryNewspapers(Guid pressReleaseId, int skip, int take)
        {
            var rs = await _pressReleaseService.QueryNewspapers(pressReleaseId, skip, take);
            return Ok(rs);
        }

        [HttpDelete("removeRequestForNewspaper/{pressReleaseNewspaperId}")]
        [Authorize]
        public async Task<IActionResult> RemoveRequestForNewspaper(Guid pressReleaseNewspaperId)
        {
            await _pressReleaseService.RemoveRequestForNewspaper(pressReleaseNewspaperId);
            return Ok();
        }

        [HttpPut("rejectRequest")]
        [Authorize(Roles = Role.Journalist)]
        public async Task<IActionResult> RejectRequest([FromBody] PressReleaseRequestUpdateViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            await _pressReleaseService.RejectRequest(viewModel.Id, viewModel.RejectReason);
            return Ok();
        }

        [HttpPut("publishRequest")]
        [Authorize(Roles = Role.Journalist)]
        public async Task<IActionResult> PublishRequest([FromBody] PressReleaseRequestUpdateViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            await _pressReleaseService.PublishRequest(viewModel.Id, viewModel.PublishedUrl);
            return Ok();
        }
    }
}