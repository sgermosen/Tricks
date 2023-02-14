using Application.Core;
using CommonTasks.Data;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Models;
using Web.Services;

namespace Api.Controllers
{
    [Route("api/patients")]
    [Authorize(AuthenticationSchemes = AppConstants.JWTScheme)]
    public class ApiPatientsController : BaseApiController
    {
        private readonly PatientService _patientService;
        private IPhotoAccessor _photoAccessor;
        protected IPhotoAccessor PhotoAccessor => _photoAccessor ??= HttpContext.RequestServices.GetService<IPhotoAccessor>();

        private IMediator _mediator;
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();
        private DataContext _db;
        protected DataContext Db => _db ??= HttpContext.RequestServices.GetService<DataContext>();

        private UserManager<ApplicationUser> _userManager;
        protected UserManager<ApplicationUser> UserManager => _userManager ??= HttpContext.RequestServices.GetService<UserManager<ApplicationUser>>();

        private TokenService _tokenService;
        protected TokenService TokenService => _tokenService ??= HttpContext.RequestServices.GetService<TokenService>();

        protected ActionResult HandleResult<T>(Result<T> result)
        {
            if (result == null)
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result);
            if (result.IsSuccess && result.Value == null)
                return NotFound();
            return BadRequest(result.Error);
        }

        [Authorize]
        [HttpGet]
        [Route("getCurrentUser")]
        protected async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var user = await UserManager.Users//.Include(p => p.Photos)
             .FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));

            return CreateUserObject(user);
        }

        [Route("getLocalConectedUser")]
        protected async Task<ApplicationUser> GetLocalConectedUser()
        {
            return await Db.Users.Where(p => p.Email.ToLower() == User.FindFirstValue(ClaimTypes.Email).ToLower()).FirstOrDefaultAsync();
        }

        [Route("createUserObject")]
        public UserDto CreateUserObject(ApplicationUser user)
        {
            return new UserDto
            {
                DisplayName = user.DisplayName,
                Username = user.UserName,
                Token = TokenService.CreateToken(user),
                //Image = user?.Photos?.FirstOrDefault(x => x.IsMain)?.Url
            };
        }

        public ApiPatientsController(PatientService patientService)
        {
            _patientService = patientService;
        }
        [HttpGet]
        [Route("getPatients")]
        public async Task<IActionResult> GetPatients(string criteria, int page = 1, int pageSize = 15)
        {
            try
            {
                var user = await GetLocalConectedUser();
                var totalItems = await Db.People.CountAsync(p => p.AuthorId == user.AuthorId && p.StatusId == 1 && p.Patients.Any());

                var model = Db.People
                  .Where(p => p.AuthorId == user.AuthorId && p.StatusId == 1 && p.Patients.Any())
                  .Select(p => new
                  {
                      Person = p,
                      Patient = p.Patients.FirstOrDefault()
                  });

                if (!string.IsNullOrEmpty(criteria))
                {
                    var searchTerm = criteria.ToUpper();
                    model = model.Where(p => string.IsNullOrEmpty(criteria) ||
                                        p.Person.Record2.ToUpper().Contains(searchTerm) ||
                                        p.Person.Rnc.ToUpper().Contains(searchTerm) ||
                                        p.Person.Record3.ToUpper().Contains(searchTerm) ||
                                        p.Person.Name.ToUpper().Contains(searchTerm) ||
                                        p.Person.LastName.ToUpper().Contains(searchTerm));
                }

                model = model.AsNoTracking().OrderByDescending(p => p.Person.PersonId)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize);

                var query = from p in model
                            select new PatientRequest()
                            {
                                Name = p.Person.Name,
                                LastName = p.Person.LastName,
                                PersonId = p.Person.PersonId,
                                PatientId = p.Patient.PatientId,
                                IsExternal = p.Person.IsExternal,
                                Imagen = p.Person.Imagen,
                                Rnc = p.Person.Rnc,
                                Age = p.Patient.Age,
                                Email = p.Person.Email,
                                Nss = p.Patient.Nss,
                                CompanionRnc = p.Patient.CompanionRnc,
                                Companion = p.Patient.Companion,
                                InsuranceNumber = p.Patient.InsuranceNumber,
                                InsuranceName = p.Patient.Insurance == null ? "" : p.Patient.Insurance.Name,
                                BloodTypeName = p.Patient.BloodType == null ? "" : p.Patient.BloodType.Name,
                                Record2 = p.Person.Record2.ToUpper(),
                            };

                return HandleResult(Result<List<PatientRequest>>.Success(await query.AsNoTracking().ToListAsync(), totalItems, page, pageSize));

            }
            catch (Exception ex)
            {
                return HandleResult(Result<string>.Failure(ex.Message));
            }

        }

        [Route("getPatient/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetPatient(int id)
        {
            var user = await GetLocalConectedUser();

            var model = await Db.People
                .Include(p => p.UserUpdate)
                .Include(p => p.User)
                .Include(p => p.Author)
                .Include(p => p.Country)
                .Include(p => p.Gender)
                .Include(p => p.MaritalSituation)
                .Include(p => p.SchoolLevel)
                .Include(p => p.Ocupation)
                .Include(p => p.Religion)
                .Include(p => p.Status)
                .Include(p => p.Patients)
                .Include(p => p.Patients).ThenInclude(z => z.Imagings)
                .Include(p => p.Patients).ThenInclude(z => z.Appointments)
            .FirstOrDefaultAsync(p => p.AuthorId == user.AuthorId
                                && p.PersonId == id);

            if (model == null)
                return NotFound();

            var patient = model.Patients.FirstOrDefault(p => p.PersonId == id);
            var res = new PatientRequest();
            model.Transfer(ref res);
            patient.Transfer(ref res);
            return HandleResult(Result<PatientRequest>.Success(res));
        }

        
        [Route("postPatient")]
        [HttpPost]
        public async Task<IActionResult> PostPatient([FromForm] PatientRequest model)
        { 
            if (!ModelState.IsValid)
                return HandleResult(Result<ModelStateDictionary>.Failure(ModelState));

            var user = await GetLocalConectedUser();

            if (!string.IsNullOrEmpty(model.Rnc))
            {
                var pat = await Db.People.AnyAsync(p => p.Rnc.ToUpper() == model.Rnc.ToUpper()
                                                && p.AuthorId == user.AuthorId && p.StatusId == 1);
                if (pat)
                    return HandleResult(Result<string>.Failure("Esta cedula ya esta registrada"));
            }

            if (!string.IsNullOrEmpty(model.Email))
            {
                var pat = await Db.People.AnyAsync(p => p.Email.ToUpper() == model.Email.ToUpper()
                                                && p.AuthorId == user.AuthorId && p.StatusId == 1);
                if (pat)
                    return HandleResult(Result<string>.Failure("Este Email ya esta registrado"));
            }

            try
            { 
                if (model.File != null && model.File.Length > 0)
                {
                       var photoUploadResult = await PhotoAccessor.AddPhotoLargeFile(model.File);
                   
                    model.Imagen = photoUploadResult.Url;  
                    model.IsExternal = true;

                } 
                PatientView view = new PatientView
                {
                    Imagen = model.Imagen,
                    Rnc = model.Rnc,
                    Address = model.Address,
                    Age = model.Age,
                    BloodTypeId = model.BloodTypeId,
                    Nss = model.Nss,
                    InsuranceNumber = model.InsuranceNumber,
                    Name = model.Name,
                    BornDate = model.BornDate,
                    Cel = model.Cel,
                    Tel = model.Tel,
                    Email = model.Email,
                    GenderId = model.GenderId,
                    InsuranceId = model.InsuranceId,
                    LastName = model.LastName,
                    Record2 = model.Record2,
                    Companion = model.Companion,
                    CompanionRnc = model.CompanionRnc
                };

                var person = await _patientService.GeneratePerson(view, user.Id, user.AuthorId, "");

                return HandleResult(Result<Patient>.Success(person));
            }
            catch (Exception ex)
            {
                return HandleResult(Result<string>.Failure(ex.Message));
            }

        }

    }
}