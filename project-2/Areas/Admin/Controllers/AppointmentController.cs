using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using project_2.Areas.Admin.ViewModels;
using project_2.Data;
using project_2.Extenstions;
using project_2.Helpers.Email;
using project_2.Helpers.ImageHelpers;
using project_2.Models;
using project_2.Models.Repositories;
using project_2.ViewModels;

namespace project_2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class AppointmentController : Controller
    {
        IRepository<Appointment> repo;
        IRepository<Schedule> schedulerepo;
        IRepository<Dentist> dentistrepo;
        DataDbContext dbContext;
        IEmailSender emailSender;
        IImageHelper imageHelper;

        public AppointmentController(IRepository<Appointment> repo, DataDbContext dbContext, IRepository<Schedule> schedulerepo, IEmailSender emailSender, IRepository<Dentist> dentistrepo, IImageHelper imageHelper)
        {
            this.repo = repo;
            this.dbContext = dbContext;
            this.schedulerepo = schedulerepo;
            this.emailSender = emailSender;
            this.dentistrepo = dentistrepo;
            this.imageHelper = imageHelper;
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
        public IActionResult ViewAppointment()
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
            var Scheduler = schedulerepo.GetAll().Where(s => s.Status).Select(s => new
            {
                id = s.Id,
                date = s.Date.ToString("yyyy-MM-dd")
            }).ToList();
            ViewBag.ScheduleDates = Scheduler;
            return View(data);
        }
        public IActionResult SetSchedule()
        {
            ScheduleViewModel data = new ScheduleViewModel
            {
                StatusList = new List<SelectListItem>
                {
                    new SelectListItem {Text = "Active", Value="True"},
                    new SelectListItem {Text = "Disable", Value="False"},
                }
            };
            data.Status = true;
            return View(data);
        }
        public IActionResult EditSchedule()
        {
            ScheduleViewModel data = new ScheduleViewModel
            {
                StatusList = new List<SelectListItem>
                {
                    new SelectListItem {Text = "Active", Value="True"},
                    new SelectListItem {Text = "Disable", Value="False"},
                }
            };
            data.Status = true;
            return View(data);
        }
        public IActionResult InfoPage()
        {
            var data = dentistrepo.GetAll().FirstOrDefault();
            return View(data.ToViewModel());

        }
        [HttpPost]
        public IActionResult InfoPage(DentistViewModel viewmodel)
        {
            if (ModelState.IsValid)
            {
                var existingInfo = dentistrepo.GetAll().FirstOrDefault();
                viewmodel.DentistImage = imageHelper.SaveImages(viewmodel.File, viewmodel.DentistImage, "Images");
                if (existingInfo == null)
                    dentistrepo.Add(viewmodel.ToModel());
                else
                {
                    existingInfo.Name = viewmodel.Name;
                    existingInfo.PhoneNumber = viewmodel.PhoneNumber;
                    existingInfo.Experience = viewmodel.Experience;
                    existingInfo.Collage = viewmodel.Collage;
                    existingInfo.DentistImage = viewmodel.DentistImage;
                    existingInfo.DentistImage = imageHelper.SaveImages(viewmodel.File, viewmodel.DentistImage, "Images");
                    dentistrepo.Edit(existingInfo);
                }
                return RedirectToAction("InfoPage");
            }
            return View("InfoPage", viewmodel);
        }
        [HttpPost]
        public IActionResult Create(ScheduleViewModel ViewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewModel.StatusList = new List<SelectListItem> {
                    new SelectListItem {Text = "Active", Value="True"},
                    new SelectListItem {Text = "Disable", Value="False"},
                };
                return View("SetSchedule", ViewModel);
            }

            Schedule data = ViewModel.ToModel();
            schedulerepo.Add(data);
            return RedirectToAction("SetSchedule");
        }
        [HttpPost]
        public IActionResult Edit(ScheduleViewModel ViewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewModel.StatusList = new List<SelectListItem> {
                    new SelectListItem {Text = "Active", Value="True"},
                    new SelectListItem {Text = "Disable", Value="False"},
                };
                return View("EditSchedule", ViewModel);
            }
            Schedule data = ViewModel.ToModel();
            schedulerepo.Edit(data);
            return RedirectToAction("EditSchedule");
        }
        [HttpPost]
        public async Task<IActionResult> AddApt([FromBody] AddAppointmentViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var appointment = new Appointment
                {
                    PatientName = viewModel.PatientName,
                    Email = "N/A",
                    PhoneNumber = viewModel.PhoneNumber,
                    Subject = viewModel.Subject,
                    ScheduleId = viewModel.ScheduleId,
                    SlotStartTime = TimeSpan.Parse(viewModel.SlotStartTimeString),
                };
                repo.Add(appointment);
                return Ok();
            }
            else
                return BadRequest();
        }
        [HttpPost]
        public async Task<IActionResult> EditApt([FromBody] AppointmentViewModel viewModel)
        {
            if (ModelState.IsValid)
            {

                var data = viewModel.ToModel();
                var oldModel = repo.Get(viewModel.AppointmentId);
                data.Schedule = schedulerepo.Get(data.ScheduleId);
                oldModel.Schedule = schedulerepo.Get(oldModel.ScheduleId);
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
                data.Schedule = schedulerepo.Get(data.ScheduleId);
                string subject = "Appointment Removed";
                string body = $"Hello {data.PatientName} your appointment for: {data.Subject} on {data.Schedule.Date} at {data.SlotStartTime} has been remove please contact the Dental Center for more info";
                await emailSender.SendEmailAsync(data.Email, subject, body);
                repo.Delete(id);
                return Ok();
            }
            return BadRequest();
        } 
        [HttpGet]
        public async Task<IActionResult> GetDates()
        {
            var dates = await dbContext.Schedule.Select(x => x.Date.ToString("yyyy-MM-dd")).ToListAsync();
            return Json(dates);
        }
        [HttpGet]
        public async Task<IActionResult> GetSchedule(int scheduleId)
        {
            var data = await dbContext.Schedule.Where(x => x.Status && x.Id == scheduleId).Select(x => x.Date.ToString("yyyy-MM-dd")).ToListAsync();
            return Json(data);
        }
        [HttpGet]
        public async Task<IActionResult> FindDates(DateOnly Date)
        {
            var dates = await dbContext.Schedule.Where(x=>x.Date == Date).Select(x => new
            {
                Id = x.Id,
                Date = x.Date,
                StartTime = x.StartTime.ToString(@"hh\:mm"),
                EndTime = x.EndTime.ToString(@"hh\:mm"),
                Status = x.Status,
                
            }).FirstOrDefaultAsync();
            return Json(dates);
        }
        [HttpGet]
        public async Task<IActionResult> GetAppointment(DateOnly? Date)
        {
            var data = dbContext.Appointment.Include(x => x.Schedule).Where(x => !x.IsDeleted);
            var today = DateOnly.FromDateTime(DateTime.Today);
            if (Date.HasValue)
                data = data.Where(x => x.Schedule.Date == Date);
            else
                data = data.Where(x => x.Schedule.Date >= today);
            var responce = await data.Select(x=> new 
            {
                Id = x.Id,
                scheduleId = x.ScheduleId,
                PatientName = x.PatientName,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                Subject = x.Subject,
                SlotStartTime = x.SlotStartTime.ToString(@"hh\:mm"),
                Date = x.Schedule.Date.ToString("yyyy-MM-dd")
            }).ToListAsync();
                return Json(responce);
        }
        [HttpGet]
        public async Task<IActionResult> GetCanceled()
        {
            var date = DateTime.Today;
            var firstDay = new DateOnly(date.Year, date.Month, 1);
            var lastDay = new DateOnly(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
            var data = dbContext.Appointment.Include(x => x.Schedule).Where(x => x.IsDeleted && x.Schedule.Date >= firstDay && x.Schedule.Date <= lastDay);
            var responce = await data.Select(x => new
            {
                Id = x.Id,
                PatientName = x.PatientName,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                Subject = x.Subject,
                SlotStartTime = x.SlotStartTime.ToString(@"hh\:mm"),
                Date = x.Schedule.Date.ToString("yyyy-MM-dd"),
                DeletedAt = x.DeleteDate.HasValue ? x.DeleteDate.Value.ToString("yyyy-MM-dd HH:mm") : "NA"
            }).ToListAsync();
            return Json(responce);
        }
        [HttpGet]
        public IActionResult GetSlots(int scheduleId)
        {
            var schedule = schedulerepo.Get(scheduleId);
            if (schedule == null)
                return NotFound();

            var booked = repo.GetAll().Where(x => x.ScheduleId == scheduleId).Select(x => x.SlotStartTime).ToList();

            var slots = GenerateSlots(schedule.StartTime, schedule.EndTime).Where(x => !booked.Contains(TimeSpan.Parse(x.Value))).ToList();

            return Json(slots);
        }

        [HttpGet]
        public async Task<IActionResult> GetStats()
        {
            var date = DateTime.Today;
            var today = DateOnly.FromDateTime(DateTime.Now);
            var firstDay = new DateOnly(date.Year, date.Month, 1);
            var lastDay = new DateOnly(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));

            var totalToday = await dbContext.Appointment.Include(x => x.Schedule).Where(x => x.Schedule.Date == today && x.Schedule.Date <= lastDay).CountAsync();

            var activeToday = await dbContext.Appointment.Include(x => x.Schedule).Where(x => !x.IsDeleted && x.Schedule.Date == today && x.Schedule.Date <= lastDay).CountAsync();

            var totalTodaycanceld = await dbContext.Appointment.Include(x => x.Schedule).Where(x => x.IsDeleted && x.Schedule.Date == today && x.Schedule.Date <= lastDay).CountAsync();

            var total = await dbContext.Appointment.Include(x => x.Schedule).Where(x => x.Schedule.Date >= firstDay && x.Schedule.Date <= lastDay).CountAsync();

            var active = await dbContext.Appointment.Include(x => x.Schedule).Where(x => !x.IsDeleted && x.Schedule.Date >= firstDay && x.Schedule.Date <= lastDay).CountAsync();

            var percent = (total == 0) ? 0 : (int)((double) active / total * 100);
            var percentToday = (totalToday == 0)? 0 : (int)((double)activeToday / totalToday * 100);

            return Json(new
            {
                Total = total,
                TotalToday = totalToday,
                TotalTodaycanceld = totalTodaycanceld,
                Active = active,
                ActiveToday = activeToday,
                Percent = percent,
                percentToday = percentToday,
                Month = today.ToString("MMMM")
            });
        }
    }
}
