using project_2.Models;
using project_2.ViewModels;

namespace project_2.Extenstions
{
    public static class AppointmentMapping
    {
        public static AppointmentViewModel ToViewModel(this Appointment model)
        {
            return new AppointmentViewModel
            {
                AppointmentId = model.Id,
                ScheduleId = model.ScheduleId,
                Subject = model.Subject,
                PatientName = model.PatientName,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                SlotStartTime = model.SlotStartTime,
                StartTime = model.Schedule.Date.ToDateTime(TimeOnly.FromTimeSpan(model.SlotStartTime)),
                EndTime = model.Schedule.Date.ToDateTime(TimeOnly.FromTimeSpan(model.SlotStartTime)).AddMinutes(40),
                ReminderFlag = model.ReminderFlag,
            };
        }
        public static Appointment ToModel(this AppointmentViewModel viewmodel)
        {
            return new Appointment
            {
                Id = viewmodel.AppointmentId,
                ScheduleId = viewmodel.ScheduleId,
                Subject = viewmodel.Subject,
                PatientName = viewmodel.PatientName,
                PhoneNumber = viewmodel.PhoneNumber,
                Email = viewmodel.Email,
                SlotStartTime = TimeSpan.Parse(viewmodel.SlotStartTimeString),
                ReminderFlag = viewmodel.ReminderFlag,
            };
        }
        public static List<AppointmentViewModel> ToViewModelList(this List<Appointment> models)
        {
            return models.Select(x => x.ToViewModel()).ToList();
        }
        public static List<Appointment> ToModelList(this List<AppointmentViewModel> models)
        {
            return models.Select(x => x.ToModel()).ToList();
        }
    }
}
