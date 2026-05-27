using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using project_2.Data;
using project_2.Extenstions;
using project_2.Helpers.Email;
using project_2.Helpers.OTPHelper;
using project_2.Models;
using project_2.Models.Repositories;
using project_2.ViewModels;

namespace project_2.Controllers
{
    public class UserController : Controller
    {
        IRepository<Appointment> repo;
        IRepository<Dentist> repoDentist;
        IRepository<Schedule> repoSchedule;
        IRepository<OTP> repoOtp;
        DataDbContext dbContext;
        IEmailSender emailSender;
        IOTPHelper otphelper;

        public UserController(IRepository<Appointment> repo, IRepository<Schedule> repoSchedule, IEmailSender emailSender, IOTPHelper otphelper, DataDbContext dbContext, IRepository<OTP> repoOtp, IRepository<Dentist> repoDentist)
        {
            this.repo = repo;
            this.repoSchedule = repoSchedule;
            this.emailSender = emailSender;
            this.otphelper = otphelper;
            this.dbContext = dbContext;
            this.repoOtp = repoOtp;
            this.repoDentist = repoDentist;
        }

        private List<SelectListItem> GenerateSlots(
        TimeSpan start,
        TimeSpan end,
        int slotMinutes = 40)
        {
            var slots = new List<SelectListItem>();
            var current = start;

            while (current.Add(TimeSpan.FromMinutes(slotMinutes)) <= end)
            {
                var label = $"{current:hh\\:mm} - {current.Add(TimeSpan.FromMinutes(slotMinutes)):hh\\:mm}";
                slots.Add(new SelectListItem
                {
                    Text = label,
                    Value = current.ToString(@"hh\:mm")
                });

                current = current.Add(TimeSpan.FromMinutes(slotMinutes));
            }

            return slots;
        }

        // GET: UserController
        public ActionResult Index()
        {
            var data = new AppointmentViewModel
            {
                SubjectList = new List<SelectListItem>
                {
                    new SelectListItem { Text = "-- Chose --", Value = "-- Chose --" },
                    new SelectListItem { Text = "Dental Check-up", Value = "Dental Check-up" },
                    new SelectListItem { Text = "Teeth Cleaning", Value = "Teeth Cleaning" },
                    new SelectListItem { Text = "Tooth Filling", Value = "Tooth Filling" },
                    new SelectListItem { Text = "Tooth Extraction", Value = "Tooth Extraction" },
                    new SelectListItem { Text = "Root Canal Treatment", Value = "Root Canal" },
                    new SelectListItem { Text = "Dental Pain", Value = "Dental Pain" },
                    new SelectListItem { Text = "Consultation", Value = "Consultation" },
                    new SelectListItem { Text = "Cosmetic Dentistry", Value = "Cosmetic" },
                    new SelectListItem { Text = "Follow-up Appointment", Value = "Follow-up" },
                    new SelectListItem { Text = "Other", Value = "Other" }
                },
                AvailableSlots = new List<SelectListItem>(),
            };

            var dentistData = repoDentist.GetAll().FirstOrDefault().ToViewModel();

            var pageData = new UserViewModels
            {
                AppointmentView = data,
                DentistView = dentistData,
            };

            return View(pageData);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(UserViewModels collection)
        {
            try
            {
                var isTaken = repo.GetAll().Any(x=>x.ScheduleId == collection.AppointmentView.ScheduleId && x.SlotStartTime.ToString() == collection.AppointmentView.SlotStartTimeString + ":00");
                if (isTaken) {
                    TempData["ErrorMessage"] = "This time slot had already been taken, please select another.";
                    return RedirectToAction(nameof(Index));
                }
                if (ModelState.IsValid)
                {
                    repo.Add(collection.AppointmentView.ToModel());
                    TempData["SuccessMessage"] = "Your appointment has been booked successfully.";
                    var schedule = repoSchedule.Get(collection.AppointmentView.ScheduleId);
                    string subject = "Appointment Confirmed";
                    string body = $"Hello {collection.AppointmentView.PatientName}\n\n"+
                        "this an email to confirm you have booked an appointment with the following\n"+
                        $"subject: {collection.AppointmentView.Subject}\n"+
                        $"date: {schedule.Date}\n"+
                        $"time: {collection.AppointmentView.SlotStartTimeString}\n\n"+
                        "Thank you for choosing our clinic.";
                    emailSender.SendEmailAsync(collection.AppointmentView.Email, subject, body);
                    return RedirectToAction(nameof(Index));
                }
                collection.AppointmentView.SubjectList = new List<SelectListItem>
                    {
                        new SelectListItem { Text = "-- Chose --", Value = "-- Chose --" },
                        new SelectListItem { Text = "Dental Check-up", Value = "Dental Check-up" },
                        new SelectListItem { Text = "Teeth Cleaning", Value = "Teeth Cleaning" },
                        new SelectListItem { Text = "Tooth Filling", Value = "Tooth Filling" },
                        new SelectListItem { Text = "Tooth Extraction", Value = "Tooth Extraction" },
                        new SelectListItem { Text = "Root Canal Treatment", Value = "Root Canal" },
                        new SelectListItem { Text = "Dental Pain", Value = "Dental Pain" },
                        new SelectListItem { Text = "Consultation", Value = "Consultation" },
                        new SelectListItem { Text = "Cosmetic Dentistry", Value = "Cosmetic" },
                        new SelectListItem { Text = "Follow-up Appointment", Value = "Follow-up" },
                        new SelectListItem { Text = "Other", Value = "Other" }
                    };

                return View("Index", collection);
            }
            catch
            {
                TempData["ErrorMessage"] = "An error occurred while booking the appointment.";
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpGet]
        public IActionResult GetSlots(int scheduleId)
        {
            var schedule = repoSchedule.Get(scheduleId);
            if (schedule == null)
                return NotFound();

            var booked = repo.GetAll().Where(x=>x.ScheduleId == scheduleId).Select(x=>x.SlotStartTime).ToList();

            var slots = GenerateSlots(schedule.StartTime, schedule.EndTime).Where(x=> !booked.Contains(TimeSpan.Parse(x.Value))).ToList();

            return Json(slots);
        }
        [HttpPost]
        public async Task <IActionResult> RequestOTP([FromBody] OTPRequestViewModel model)
        {
            if (String.IsNullOrEmpty(model.Email))
            {
                return BadRequest("Email is required");
            }
            try
            {
                var exsistingCode = repoOtp.GetAll().FirstOrDefault(x => x.Email == model.Email && x.expiry > DateTime.Now);

                if(exsistingCode != null)
                {
                    return Conflict(new { message = "A valid code has already been sent. Please check your inbox or wait for it to expire." });
                }

                var OtpCode = await otphelper.GenerateOTP(model.Email);
                string subject = "Access Code";
                string body = $"Your verification code is: {OtpCode} it will stay vaild for 5 minutes do not share it with anyone";
                
                await emailSender.SendEmailAsync(model.Email, subject, body);
                return Ok(new { message = "A verification code has been sent to your email." });
            }
            catch (Exception ex) { 
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> VerifyOTP([FromBody] OTPVerifyViewModel model)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            if (String.IsNullOrEmpty(model.Code)) { 
                return BadRequest("Invalid code");
            }
            try {
                var valid = await otphelper.VerifyOTP(model.Email, model.Code);
                if (!valid) { 
                    return BadRequest("Wrong code please try again");
                }
                else
                {
                    if (String.IsNullOrEmpty(model.Email))
                        return BadRequest("Email and phone number are required");
                    var Appointments = dbContext.Appointment.Include(x=>x.Schedule).Where(x=>!x.IsDeleted && x.Email ==  model.Email && x.Schedule.Date >= today).ToList();
                    if (Appointments.Count == 0)
                        return BadRequest("No appointments found");
                    List<AppointmentViewModel> AppointmentList = new List<AppointmentViewModel>();
                    foreach (var item in Appointments)
                    {
                        var obj = new AppointmentViewModel
                        {
                            AppointmentId = item.Id,
                            Subject = item.Subject,
                            PatientName = item.PatientName,
                            Email = item.Email,
                            PhoneNumber = item.PhoneNumber,
                            StartTime = repoSchedule.Get(item.ScheduleId).Date.ToDateTime(TimeOnly.FromTimeSpan(item.SlotStartTime)),
                            EndTime = repoSchedule.Get(item.ScheduleId).Date.ToDateTime(TimeOnly.FromTimeSpan(item.SlotStartTime).AddMinutes(15)),
                        };
                        AppointmentList.Add(obj);
                    }
                    return Json(AppointmentList);
                }
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetDates()
        {
            var dates = await dbContext.Schedule.Select(x => x.Date.ToString("yyyy-MM-dd")).ToListAsync();
            return Json(dates);
        }

        [HttpGet]
        public async Task<IActionResult> GetNextAvailableDates()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            var dates = await dbContext.Schedule.Where(x => x.Date >= today && x.Status).OrderBy(x => x.Date).Take(3).Select(x=> new
            {
                Id = x.Id,
                Date = x.Date.ToString("yyyy-MM-dd"),
                StartTime = x.StartTime.ToString(@"hh\:mm"),
                EndTime = x.EndTime.ToString(@"hh\:mm"),
            }).ToListAsync();

            return Json(dates);
        }

        [HttpGet]
        public async Task<IActionResult> FindDates(DateOnly Date)
        {
            var dates = await dbContext.Schedule.Where(x => x.Date == Date).Select(x => new
            {
                Id = x.Id,
                Date = x.Date,
                StartTime = x.StartTime.ToString(@"hh\:mm"),
                EndTime = x.EndTime.ToString(@"hh\:mm"),
                Status = x.Status,

            }).FirstOrDefaultAsync();
            return Json(dates);
        }
        [HttpPost]
        public async Task<IActionResult> EditApt([FromBody] UserViewModels viewModel)
        {
            if (ModelState.IsValid)
            {

                var data = viewModel.AppointmentView.ToModel();
                var oldModel = repo.Get(viewModel.AppointmentView.AppointmentId);
                data.Schedule = repoSchedule.Get(data.ScheduleId);
                oldModel.Schedule = repoSchedule.Get(oldModel.ScheduleId);
                string subject = "Appointment changed";
                string body = $"Hello {data.PatientName} your appointment for: {oldModel.Subject} on {oldModel.Schedule.Date} at {oldModel.SlotStartTime}\n has been changed for: {data.Subject} on {data.Schedule.Date} at {data.SlotStartTime}";
                repo.Edit(data);
                await emailSender.SendEmailAsync(data.Email, subject, body);
                return Ok();
            }
            else
                return BadRequest();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteApt(int id)
        {
            if (ModelState.IsValid)
            {
                var data = repo.Get(id);
                data.Schedule = repoSchedule.Get(data.ScheduleId);
                string subject = "Appointment Removed";
                string body = $"Hello {data.PatientName} your appointment for: {data.Subject} on {data.Schedule.Date} at {data.SlotStartTime} has been remove please contact the Dental Center for more info";
                await emailSender.SendEmailAsync(data.Email, subject, body);
                repo.Delete(id);
                return Ok();
            }
            return BadRequest();
        }
    }
}
